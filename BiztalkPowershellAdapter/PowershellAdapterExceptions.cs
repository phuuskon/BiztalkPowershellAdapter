using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    internal class PowershellAdapterException : ApplicationException
    {
        public static string UnhandledTransmit_Error = "The PowershellAdapter encountered and error transmitting message.";

        public PowershellAdapterException() { }

        public PowershellAdapterException(string msg) : base(msg) { }

        public PowershellAdapterException(Exception inner) : base(String.Empty, inner) { }

        public PowershellAdapterException(string msg, Exception e) : base(msg, e) { }

        protected PowershellAdapterException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
