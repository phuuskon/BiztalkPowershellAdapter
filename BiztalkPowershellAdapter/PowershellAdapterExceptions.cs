using System;
using System.Runtime.Serialization;

namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    public class PowershellAdapterException : ApplicationException
    {
        public static readonly string UnhandledTransmit_Error = "The PowershellAdapter encountered and error transmitting message.";

        public PowershellAdapterException() { }

        public PowershellAdapterException(string msg) : base(msg) { }

        public PowershellAdapterException(Exception inner) : base(string.Empty, inner) { }

        public PowershellAdapterException(string msg, Exception e) : base(msg, e) { }

        protected PowershellAdapterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
