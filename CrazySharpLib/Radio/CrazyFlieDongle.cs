using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CrazySharpLib.Exceptions;
using CrazySharpLib.Radio;
using CrazySharpLib.Radio.Packets;
using LibUsbDotNet;
using LibUsbDotNet.Main;


namespace CrazySharpLib
{
    /// <summary>
    /// A raw interface to the crazy flie dongle
    /// </summary>
    public class CrazyFlieDongle
    {
        private readonly UsbRegistry _deviceReg;
        private UsbDevice _device;
        private IUsbDevice _iDevice;
        private UsbEndpointWriter _writer;
        private UsbEndpointReader _reader;

        public CrazyFlieDongle(UsbRegistry deviceReg)
        {
            _deviceReg = deviceReg;
        }

        public void Open()
        {
            if (_deviceReg.Open(out _device) == false)
            {
                throw new InvalidOperationException("Unable to open the dongle, was it removed?");
            }

            _writer = _device.OpenEndpointWriter(WriteEndpointID.Ep01, EndpointType.Bulk);
            _reader = _device.OpenEndpointReader(ReadEndpointID.Ep01, 32, EndpointType.Bulk);

            _iDevice = _device as IUsbDevice;

            if (_iDevice != null)
            {
                _iDevice.SetConfiguration(1);
                _iDevice.ClaimInterface(0);
            }

            Setup();
        }

        public void Close()
        {
            if(_device == null || _device.IsOpen == false)
            {
                throw new InvalidOperationException("Unable to close the device because it has not yet been opened");
            }
                        
            if(_device.Close() == false)
                throw new InvalidOperationException("Unable to close the device for unknown reasons, maybe it was removed");
        }
        
        #region Properties

        private byte _radioChannel;
        private byte[] _radioAdress;
        private DataRate _radioDataRate;
        private bool _continousCarrier;
        private RadioPower _radioPower;
        private byte _autoRetryDelay;
        private byte _autoRetryCount;
        private bool _autoAck;
        private DataRate _dataRate;

        public bool ContinousCarrier
        {
            get { return _continousCarrier; }
            set
            {
                _continousCarrier = value;
                SendVendorSetup(CrazyRequest.SetContCarrier, value ? (byte)1 : (byte)0);
            }
        }

        public byte RadioChannel
        {
            get { return _radioChannel; }
            set
            {
                if(value > 125)
                    throw new ArgumentOutOfRangeException("value", "Channel must be between 0 and 125");

                _radioChannel = value;
                SendVendorSetup(CrazyRequest.SetRadioChannel, value);
            }
        }

        public DataRate RadioDataRate
        {
            get { return _radioDataRate; }
            set
            {
                _radioDataRate = value;
                SendVendorSetup(CrazyRequest.SetDataRate, (byte) value);
            }
        }

        public RadioPower RadioPower
        {
            get { return _radioPower; }
            set
            {
                _radioPower = value;
                SendVendorSetup(CrazyRequest.SetRadioPower, (byte) value);
            }
        }

        public byte AutoRetryDelay
        {
            get { return _autoRetryDelay; }
            set
            {
                _autoRetryDelay = value;
                SendVendorSetup(CrazyRequest.SetRadioARD, (byte) (0x80 | value));
            }
        }

        public byte AutoRetryCount
        {
            get { return _autoRetryCount; }
            set
            {
                _autoRetryCount = value;
                SendVendorSetup(CrazyRequest.SetRadioARC, value);
            }
        }

        public bool AutoAck
        {
            get { return _autoAck; }
            set
            {
                _autoAck = value;
                SendVendorSetup(CrazyRequest.EnableAck, (byte)(value ? 1 : 0));
            }
        }

        public byte[] RadioAdress
        {
            get{ return _radioAdress; }
            set
            {
                if(value.Length != 5)
                    throw new ArgumentOutOfRangeException("value", "Address must be 5 bytes long");

                _radioAdress = value;
                SendVendorSetup(CrazyRequest.SetRadioAddress, 0, value);
            }
        }

        public DataRate DataRate
        {
            get { return _dataRate; }
            set
            {
                _dataRate = value;
                SendVendorSetup(CrazyRequest.SetDataRate, (byte)value);
            }
        }


        #endregion


        //TODO: fix me
        public byte[] ScanChannels(int start = 0, int stop = 125)
        {
            var oldChannel = RadioChannel;

            
            int len = stop - start + 1;

            var setup = new UsbSetupPacket(0x40, (byte)CrazyRequest.StartScanChannels, start, stop, len);

            byte[] test = new byte[len];


            if (_device.ControlTransfer(ref setup, test, len, out len) == false)
            {

                throw new Exception("Unable to do control transfer");
            }

            byte[] data = new byte[64];

            SendVendorSetup(0xC0, CrazyRequest.GetScanChannels, 0, data);

            //restore old channel after scanning
            RadioChannel = oldChannel;

            return data;
        }

        /// <summary>
        /// Method that sends a packet on the current channel
        /// </summary>
        /// <param name="packet">The packet to send, for example a CRTPCommand</param>
        /// <returns>Returns whatever data the crazy flie decided to send back in its ack payload</returns>
        public CRTPResponse SendPacket(CRTPPacket packet)
        {
            //transform packet to byte array
            var send = packet.CreatePacket();

            int len;
            //write packet to radio
            ErrorCode e = _writer.Write(send, 1000, out len);

            //TODO: Check if write ever returns OK instead
            if (e != ErrorCode.None)
            {
                throw new SendPacketException(e, "Error in writing to endpoint");
            }

            var buffer = new byte[64];
            
            e = _reader.Read(buffer, 1000, out len);

            if (e != ErrorCode.None)
            {
                throw new SendPacketException(e, "Error in reading from endpoint");
            }

            Array.Resize(ref buffer, len);

            var response = new CRTPResponse(buffer);

            return response;
        }

        private void Setup()
        {
            DataRate = DataRate.DR2Mbps;
            RadioChannel = 10;
            ContinousCarrier = false;
            RadioAdress = new byte[] { 0xE7, 0xE7, 0xE7, 0xE7, 0xE7 };
            RadioPower = RadioPower.RP_0dbm;
            AutoRetryCount = 3;
            AutoRetryDelay = 32;
            AutoAck = true;
        }
        
        private void SendVendorSetup(CrazyRequest request, byte value, byte[] data = null, short index = 0)
        {
            SendVendorSetup(0x40, request, value, data, index);
        }

        private void SendVendorSetup(byte requestType, CrazyRequest request, byte value, byte[] data = null, short index = 0)
        {
            short dataLen = data != null ? (short) data.Length : (short)0;

            var setup = new UsbSetupPacket(requestType, (byte) request, value, index, dataLen);

            int len;

            if (data == null)
            {
                if (_device.ControlTransfer(ref setup, IntPtr.Zero, 0, out len) == false)
                {
                    throw new Exception("Unable to do control transfer");
                }
            }
            else
            {
                if (_device.ControlTransfer(ref setup, data, dataLen, out len) == false)
                {
                    throw new Exception("Unable to do control transfer");
                }
            }

        }


        enum CrazyRequest : byte
        {
            SetRadioChannel = 0x01,
            SetRadioAddress = 0x02,
            SetDataRate = 0x03,
            SetRadioPower = 0x04,
            SetRadioARD = 0x05,
            SetRadioARC = 0x06,
            EnableAck = 0x10,
            SetContCarrier = 0x20,
            StartScanChannels = 0x21,
            GetScanChannels = 0x21,
            LaunchBootloader = 0xFF
        }


        #region Static methods

        /// <summary>
        /// Get all crazyflie dongles connected to the computer.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CrazyFlieDongle> GetDongles()
        {
            //UsbDevice.ForceLibUsbWinBack = true;
            
            foreach (UsbRegistry dev in UsbDevice.AllDevices)
            {
                if(DonglePredicate(dev))
                {
                    yield return new CrazyFlieDongle(dev);
                }
            }
        }

        /// <summary>
        /// Matches the VID and PID according to: http://wiki.bitcraze.se/projects:crazyradio:protocol?s[]=pid#usb_protocol
        /// </summary>
        private static bool DonglePredicate(UsbRegistry reg)
        {
            return reg.Pid == 0x7777 && reg.Vid == 0x1915;
        }

        #endregion
    }
}
