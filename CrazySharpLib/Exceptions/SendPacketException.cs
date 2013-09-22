using System;
using System.Runtime.Serialization;
using LibUsbDotNet.Main;

namespace CrazySharpLib.Exceptions
{
    [Serializable]
    public class SendPacketException : Exception
    {
        public ErrorCode Error { get; private set; }
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public SendPacketException(ErrorCode error, string message) : base(message)
        {
            Error = error;
        }

        public SendPacketException(string message) : base(message)
        {
        }

        public SendPacketException(string message, Exception inner) : base(message, inner)
        {
        }

        protected SendPacketException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}