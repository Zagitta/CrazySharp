using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CrazySharpLib.Misc;
using CrazySharpLib.Radio;
using CrazySharpLib.Radio.Packets;

namespace CrazySharpLib
{
    public class CrazyLog
    {
        private readonly ICrazyRadio _radio;
        private readonly DefaultDispatcher _dispatcher;
        private readonly ConcurrentQueue<InfoReply> _infoReplies = new ConcurrentQueue<InfoReply>();
        private readonly ConcurrentQueue<ToCItem> _itemReplies = new ConcurrentQueue<ToCItem>(); 

        public CrazyLog(ICrazyRadio radio, DefaultDispatcher dispatcher)
        {
            _radio = radio;
            _dispatcher = dispatcher;
            _dispatcher.LogEvent += LogEvent;
        }

        private void LogEvent(object sender, EventArgs<CRTPResponse> eventArgs)
        {
            var response = eventArgs.Data;

            var channel = (LoggingChannels) response.Header.Channel;

            switch (channel)
            {
                case LoggingChannels.TableOfContent:
                    ToCEvent(response);
                    break;
                case LoggingChannels.Settings:
                    
                    break;
                case LoggingChannels.Logdata:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        private void ToCEvent(CRTPResponse response)
        {
            LoggingCmd cmd = (LoggingCmd) response.Data[0];

            if(cmd == LoggingCmd.ToCItem)
            {
                _itemReplies.Enqueue(new ToCItem(response.Data));
            }
            else if(cmd == LoggingCmd.ToCInfo)
            {
                _infoReplies.Enqueue(new InfoReply(response.Data));
            }
        }


        private void Reset()
        {
            _radio.EnqueueSend(CRTPLog.ResetLog);
        }


        internal class InfoReply
        {
            internal InfoReply(byte[] data)
            {
                VariableCount = data[1];
                CRC = BitConverter.ToUInt32(data, 2);
                MaxBlocks = data[6];
                MaxVariables = data[7];
            }

            public readonly byte VariableCount;
            public readonly uint CRC;
            public readonly byte MaxBlocks;
            public readonly byte MaxVariables;
        }

        private InfoReply GetInfo()
        {
            Reset();

            _radio.EnqueueSend(CRTPLog.GetTableOfContent);

            InfoReply reply;

            //busy wait for response
            while (_infoReplies.TryDequeue(out reply) == false) {}

            return reply;
        }

//        private ToCItem GetItem()
//        {
//            _radio.EnqueueSend(CRTPLog.GetItem);
//
//            InfoReply reply;
//
//            //busy wait for response
//            while (_infoReplies.TryDequeue(out reply) == false) ;
//
//            return reply;
//        }

        private IEnumerable<ToCItem> BulkGetItem(byte start, byte end)
        {
            for (byte i = start; i < end; i++)
            {
                _radio.EnqueueSend(CRTPLog.GetItem(i));
            }

            for (byte i = start; i < end; i++)
            {
                ToCItem reply;
                
                //busy wait for response
                while (_itemReplies.TryDequeue(out reply) == false) ;

                yield return reply;
            }
        } 


        public List<ToCItem> GetTableOfContent()
        {
            Reset();

            var info = GetInfo();

            return new List<ToCItem>(BulkGetItem(0, info.VariableCount));
        }

    }

    public enum LogDataType : byte
    {
        UInt8 = 1,
        UInt16 = 2,
        UInt32 = 3,
        Int8 = 4,
        Int16 = 5,
        Int32 = 6,
        Float = 7,
        Double = 8
    }
}
