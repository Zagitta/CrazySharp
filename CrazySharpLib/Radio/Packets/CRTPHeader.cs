namespace CrazySharpLib.Radio.Packets
{
    public struct CRTPHeader
    {
        public CRTPHeader(byte channel, byte port)
        {
            Channel = channel;
            Port = port;

            //http://wiki.bitcraze.se/projects:crazyflie:firmware:comm_protocol#packet_structure
            int val = ((Port & 0xF) << 4) | (0x3 << 2) | (Channel & 0x3);
            Header = (byte) val;
        }

        public CRTPHeader(byte header)
        {
            Channel = (byte)(header & 0x3);
            Port = (byte)(header >> 4);

            Header = header;
        }


        public readonly byte Header;
        public readonly byte Channel;
        public readonly byte Port;
    }
}