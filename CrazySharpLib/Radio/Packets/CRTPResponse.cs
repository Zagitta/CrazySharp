using System;
using System.Collections.ObjectModel;

namespace CrazySharpLib.Radio.Packets
{
    public struct CRTPResponse
    {
        public CRTPResponse(byte[] data)
        {
            //First byte is status part
            Status = new CRTPStatus(data[0]);

            if(data.Length < 2)
            {
                Header = new CRTPHeader(0,15);
                Data = new byte[0];
                return;
            }

            //Second byte is an CRTPHeader
            Header = new CRTPHeader(data[1]);

            //data is rest of the array
            Data = new byte[data.Length - 2];

            Array.Copy(data, 2, Data, 0, Data.Length);
        }

        public readonly CRTPStatus Status;
        public readonly CRTPHeader Header;
        public readonly byte[] Data;

    }
}