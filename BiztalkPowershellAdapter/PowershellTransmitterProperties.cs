using Microsoft.BizTalk.Message.Interop;
using Microsoft.Samples.BizTalk.Adapter.Common;
using System;
using System.Xml;

namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    class PowershellTransmitterProperties : ConfigProperties
    {
        private static int handlerSendBatchSize = 20;
        public static int BatchSize { get { return handlerSendBatchSize; } }

        private readonly string script;
        private readonly string host;
        private readonly string user;
        private readonly string password;
        
        public string Script { get { return script; } }
        public string Host { get { return host; } }
        public string User { get { return user; } }
        public string Password { get { return password; } }

        public string Uri { get; set; }
        

        public PowershellTransmitterProperties(IBaseMessage message, string propertyNamespace)
        {
            XmlDocument locationConfigDom = null;

            //  get the adapter configuration off the message
            IBaseMessageContext context = message.Context;
            string config = (string)context.Read("AdapterConfig", propertyNamespace);

            //  the config can be null all that means is that we are doing a dynamic send
            if (null != config)
            {
                locationConfigDom = new XmlDocument();
                locationConfigDom.LoadXml(config);

                Uri = Extract(locationConfigDom, "/Config/uri", string.Empty);

                script = Extract(locationConfigDom, "/Config/script", string.Empty);
                host = IfExistsExtract(locationConfigDom, "Config/host", string.Empty);
                user = IfExistsExtract(locationConfigDom, "Config/user", string.Empty);
                password = IfExistsExtract(locationConfigDom, "Config/password", string.Empty);
            }
        }
                
        public void UpdateUriForDynamicSend()
        {
            if (!string.IsNullOrEmpty(Uri))
            {
                // Strip off the adapters alias
                const string adapterAlias = "Powershell://";
                if (Uri.StartsWith(adapterAlias, StringComparison.OrdinalIgnoreCase))
                {
                    Uri = Uri.Substring(adapterAlias.Length);
                }
            }
        }
    }
}
