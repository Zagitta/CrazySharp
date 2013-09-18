using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrazySharpLib
{
    /// <summary>
    /// The port used to describe the target on the crazyflie
    /// </summary>
    enum PortAllocation : byte
    {
        /// <summary>
        /// Read console text that is printed to the console on the Crazyflie using consoleprintf 
        /// </summary>
        Console = 0x0,
        /// <summary>
        /// Get/set parameters from the Crazyflie. Parameters are defined using a macro in the Crazyflie source-code
        /// </summary>
        Parameters = 0x2,
        /// <summary>
        /// Sending control set-points for the roll/pitch/yaw/thrust regulators
        /// </summary>
        Commander = 0x3,
        /// <summary>
        /// Set up log blocks with variables that will be sent back to the Crazyflie at a specified period. Log variables are defined using a macro in the Crazyflie source-code
        /// </summary>
        Log = 0x5,
        /// <summary>
        /// Used to control and query the communication link
        /// </summary>
        LinkLayer = 0x15
    }

    public abstract class CRTPPacket
    {
        private readonly byte _channel;
        private readonly byte _port;

        internal CRTPPacket(byte channel, PortAllocation port)
        {
            _channel = channel;
            _port = (byte)port;
        }

        /// <summary>
        /// http://wiki.bitcraze.se/projects:crazyflie:firmware:comm_protocol#packet_structure
        /// </summary>
        /// <returns>A byte combiantion of the port and channel</returns>
        protected byte GetHeaderByte()
        {
            int val = ((_port & 0xF) << 4) | (0x3 << 2) | (_channel & 0x3);
            return (byte) val;
        }

        public abstract byte[] CreatePacket();
    }


    public class CRTPCommand : CRTPPacket
    {
        public CRTPCommand(byte channel) : base(channel, PortAllocation.Commander)
        {
        }

        public float Roll { get; set; }
        public float Pitch { get; set; }
        public float Yaw { get; set; }
        public UInt16 Thrust { get; set; }

        public override byte[] CreatePacket()
        {
            var rollBytes = BitConverter.GetBytes(Roll);
            var pitchBytes = BitConverter.GetBytes(Pitch);
            var yawBytes = BitConverter.GetBytes(Yaw);
            var thrustBytes = BitConverter.GetBytes(Thrust);

            return new[]
            {
                GetHeaderByte(),
                rollBytes[0],
                rollBytes[1],
                rollBytes[2],
                rollBytes[3],
                pitchBytes[0],
                pitchBytes[1],
                pitchBytes[2],
                pitchBytes[3],
                yawBytes[0],
                yawBytes[1],
                yawBytes[2],
                yawBytes[3],
                thrustBytes[0],
                thrustBytes[1]
            };
        }
    }


}
