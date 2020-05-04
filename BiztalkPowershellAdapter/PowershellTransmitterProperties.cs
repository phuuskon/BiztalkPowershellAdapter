using Microsoft.BizTalk.Message.Interop;
using Microsoft.Samples.BizTalk.Adapter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    class PowershellTransmitterProperties : ConfigProperties
    {
        private static int handlerSendBatchSize = 20;
        public static int BatchSize { get { return handlerSendBatchSize; } }

        private string script;
        //private string arguments;
        private string host;
        private string user;
        private string password;
        private string uri;

        public string Script { get { return script; } }
        //public string Arguments { get { return arguments; } }
        public string Host { get { return host; } }
        public string User { get { return user; } }
        public string Password { get { return password; } }

        public string Uri
        {
            get { return uri; }
            set { uri = value; }
        }

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

                this.ReadLocationConfiguration(locationConfigDom);
            }
        }

        public virtual void ReadLocationConfiguration(XmlDocument configDOM)
        {
            this.uri = Extract(configDOM, "/Config/uri", string.Empty);

            //  In the case of running under SSO the uri will be different
            //  because we add the userName from SSO into the uri
            //if (!uri.Equals(this.uri))
            //    throw new InconsistentConfigurationUri(this.uri, uri);

            this.script = Extract(configDOM, "/Config/script", string.Empty);
            //this.arguments = Extract(configDOM, "/Config/arguments", string.Empty);
            this.host = IfExistsExtract(configDOM, "Config/host", string.Empty);
            this.user = IfExistsExtract(configDOM, "Config/user", string.Empty);
            this.password = IfExistsExtract(configDOM, "Config/password", string.Empty);

            // If we needed to use SSO we will need this extra property
            //this.ssoAffiliateApplication = IfExistsExtract(configDOM, "/Config/ssoAffiliateApplication");
        }

        public static void ReadTransmitHandlerConfiguration(XmlDocument configDOM)
        {
            // Handler properties
            //handlerSendBatchSize = ExtractInt(configDOM, "/Config/sendBatchSize");
        }

        public void UpdateUriForDynamicSend()
        {
            if (!String.IsNullOrEmpty(this.Uri))
            {
                // Strip off the adapters alias
                const string adapterAlias = "Powershell://";
                if (this.Uri.StartsWith(adapterAlias, StringComparison.OrdinalIgnoreCase))
                {
                    this.Uri = this.Uri.Substring(adapterAlias.Length);
                }
            }
        }
    }
}
