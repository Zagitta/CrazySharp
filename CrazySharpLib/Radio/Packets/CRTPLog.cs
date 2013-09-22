using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrazySharpLib.Radio.Packets
{
    public enum LoggingChannels : byte
    {
        TableOfContent = 0,
        Settings = 1,
        Logdata = 2
    }

    public enum LoggingCmd : byte
    {
        CreateBlock = 0,
        AppendBlock = 1,
        DeleteBlock = 2,
        StartLogging = 3,
        StopLogging = 4,
        ResetLogging = 5,

        ToCItem = 0,
        ToCInfo = 1
    }

    class CRTPLog : CRTPPacket
    {
        internal CRTPLog(byte channel) : base(channel, PortAllocation.Log)
        {
            
        }

        internal CRTPLog(LoggingChannels channel, LoggingCmd command, byte index = 0) : this((byte)channel)
        {
            Index = index;
            Command = (byte) command;
        }

        public byte Command { get; set; }
        public byte Index { get; set; }


        public override byte[] CreatePacket()
        {
            return new[]
            {
                Header.Header,
                Command,
                Index
            };
        }

        public static CRTPLog GetItem(byte index)
        {
            return new CRTPLog(LoggingChannels.TableOfContent, LoggingCmd.ToCItem, index: index);
        }

        public static CRTPLog GetTableOfContent
        {
            get { return new CRTPLog(LoggingChannels.TableOfContent, LoggingCmd.ToCInfo); }
        }

        public static CRTPLog ResetLog
        {
            get { return new CRTPLog(LoggingChannels.Settings, LoggingCmd.ResetLogging); }
        }
    }


}
