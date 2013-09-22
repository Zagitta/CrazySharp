namespace CrazySharpLib.Radio.Packets
{
    public abstract class CRTPPacket
    {
        protected readonly CRTPHeader Header;

        internal CRTPPacket(byte channel, PortAllocation port)
        {
            Header = new CRTPHeader(channel, (byte)port);
        }

        public abstract byte[] CreatePacket();
    }

    class CRTPIdlePacket : CRTPPacket
    {
        public CRTPIdlePacket() : base(3, PortAllocation.LinkLayer)
        {
        }

        public override byte[] CreatePacket()
        {
            return new[]
            {
                Header.Header
            };
        }
    }
}
