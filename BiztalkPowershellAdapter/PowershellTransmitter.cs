using Microsoft.BizTalk.Component.Interop;
using Microsoft.Samples.BizTalk.Adapter.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    public class PowershellTransmitAdapter : AsyncTransmitter
    {
        internal static string POWERSHELL_NAMESPACE = "http://schemas.microsoft.com/BizTalk/2003/Messaging/Transmit/powershell-properties";

        public PowershellTransmitAdapter() : base(
			"Powershell Transmit Adapter",
			"1.0",
			"Runs powershell scripts",
			"Powershell script",
			new Guid("49f50364-0b81-4ed9-8c67-ad7ba5328dfe"),
			POWERSHELL_NAMESPACE,
			typeof(PowershellTransmitterEndpoint),
			PowershellTransmitterProperties.BatchSize)
		{
        }

        protected override void HandlerPropertyBagLoaded()
        {
            IPropertyBag config = this.HandlerPropertyBag;
            if (null != config)
            {
                XmlDocument handlerConfigDom = ConfigProperties.IfExistsExtractConfigDom(config);
                if (null != handlerConfigDom)
                {
                    PowershellTransmitterProperties.ReadTransmitHandlerConfiguration(handlerConfigDom);
                }
            }
        }
    }
}
