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
        private readonly PortAllocation _port;

        internal CRTPPacket(byte channel, PortAllocation port)
        {
            _channel = channel;
            _port = port;
        }

        /// <summary>
        /// http://wiki.bitcraze.se/projects:crazyflie:firmware:comm_protocol#packet_structure
        /// </summary>
        /// <returns>A byte combiantion of the port and channel</returns>
        protected byte GetHeaderByte()
        {
            int val = (((int)_port) << 4) | (_channel & 0x3);
            return (byte) val;
        }

        public abstract byte[] CreatePacket();
    }


    public class CRTPCommand : CRTPPacket
    {
        internal CRTPCommand(byte channel) : base(channel, PortAllocation.Commander)
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


            var bytes = new byte[14];
            bytes[0] = GetHeaderByte();

            for (int i = 0; i < 4; i++)
            {
                //insert bytes in little endian format like done here: https://bitbucket.org/bitcraze/crazyflie-pc-client/src/2cf63fad1090d21bdd488756ae91954ea07fb576/lib/cflib/crazyflie/commander.py?at=default#cl-73
                bytes[i] = rollBytes[i];
                bytes[i + 4] = yawBytes[i];
                bytes[i + 8] = pitchBytes[i];
            }

            bytes[12] = thrustBytes[0];
            bytes[13] = thrustBytes[1];

            return bytes;
        }
    }


}
