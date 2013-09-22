using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrazySharpLib.Radio.Packets
{
    public struct CRTPStatus
    {
        public CRTPStatus(byte status)
        {
            Status = status;

            AcknowledgeRecieved = (Status & 0x01) == 0x01;
            PowerDetected = (Status & 0x02) == 0x02;
            Retransmissions = Status >> 4;
        }

        public readonly byte Status;
        public readonly bool AcknowledgeRecieved;
        public readonly bool PowerDetected;
        public readonly int Retransmissions;
    }
}
