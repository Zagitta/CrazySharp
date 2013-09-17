using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibUsbDotNet;
using LibUsbDotNet.Main;


namespace CrazySharpLib
{
    enum CrazyRequest : byte
    {
        SetRadioChannel   = 0x01,
        SetRadioAddress   = 0x02,
        SetDataRate       = 0x03,
        SetRadioPower     = 0x04,
        SetRadioARD       = 0x05,
        SetRadioARC       = 0x06,
        EnableAck         = 0x10,
        SetContCarrier    = 0x20,
        StartScanChannels = 0x21,
        /*GetScanChannels = 0x21*/
        LaunchBootloader  = 0xFF
    }


    public class CrazyFlieDongle
    {
        private readonly UsbRegistry _deviceReg;
        private UsbDevice _device;
        private byte _radioChannel;
        private byte[] _radioAdress;

        private CrazyFlieDongle(UsbRegistry deviceReg)
        {
            _deviceReg = deviceReg;
        }

        public void OpenDevice()
        {
            if (_deviceReg.Open(out _device) == false)
            {
                throw new InvalidOperationException("Unable to open the dongle, was it removed?");
            }
        }

        public void CloseDevice()
        {
            if(_device == null || _device.IsOpen == false)
            {
                throw new InvalidOperationException("Unable to close the device because it has not yet been opened");
            }
            
            if(_device.Close() == false)
                throw new InvalidOperationException("Unable to close the device for unknown reasons, maybe it was removed");
        }


        public byte RadioChannel
        {
            get { return _radioChannel; }
            set
            {
                if(value > 125)
                    throw new ArgumentOutOfRangeException("Channel must be between 0 and 125");

                _radioChannel = value;
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
            }
        }

        public byte DataRate { get; set; }



        private void SendVendorSetup(byte request, byte value, int index = 0, byte[] data = null)
        {
            


        }



        #region Static methods

        /// <summary>
        /// Get all crazyflie dongles connected to the computer.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<CrazyFlieDongle> GetDongles()
        {
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
