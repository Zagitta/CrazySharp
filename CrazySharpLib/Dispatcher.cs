using System;
using System.Runtime.Remoting.Proxies;
using System.Text;
using CrazySharpLib.Misc;
using CrazySharpLib.Radio;
using CrazySharpLib.Radio.Packets;

namespace CrazySharpLib
{
    public interface ICrazyDispatcher
    {
        void Callback(CRTPResponse response);
    }

    public class DefaultDispatcher : ICrazyDispatcher
    {
        public EventHandler<EventArgs<CrazyError>> ErrorEvent;
        public EventHandler<EventArgs<CRTPResponse>> LogEvent;
        public EventHandler<EventArgs<string>> ConsoleEvent;
        public EventHandler<EventArgs<CRTPStatus>> StatusEvent;


        public void Callback(CRTPResponse response)
        {
            if(StatusChecker(response.Status))
                return;

            var port = (PortAllocation) response.Header.Port;

            switch (port)
            {
                case PortAllocation.Log:
                    if (LogEvent != null)
                        LogEvent(this, new EventArgs<CRTPResponse>(response));
                    break;

                case PortAllocation.Console:
                    if (ConsoleEvent != null)
                    {
                        var s = Encoding.ASCII.GetString(response.Data, 0, response.Data.Length);

                        ConsoleEvent(this, new EventArgs<string>(s));
                    }
                    break;

//                default:
//                    throw new ArgumentOutOfRangeException("response", "the port was out of range");
            }
        }

        private bool StatusChecker(CRTPStatus status)
        {
            if (StatusEvent != null)
                StatusEvent(this, new EventArgs<CRTPStatus>(status));


            if (status.AcknowledgeRecieved) return false;

            if (ErrorEvent != null)
                ErrorEvent(this, new EventArgs<CrazyError>(CrazyError.AcknowledgeError));

            return true;
        }
    }

    public enum CrazyError
    {
        AcknowledgeError,

    }
}