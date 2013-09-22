using System;

namespace CrazySharpLib.Radio.Packets
{
    public class CRTPCommand : CRTPPacket
    {
        public CRTPCommand() : base(0, PortAllocation.Commander)
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
                Header.Header,
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