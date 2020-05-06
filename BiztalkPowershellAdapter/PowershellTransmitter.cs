using Microsoft.Samples.BizTalk.Adapter.Common;
using System;

namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    public class PowershellTransmitAdapter : AsyncTransmitter
    {
        internal static readonly string POWERSHELL_NAMESPACE = "http://schemas.microsoft.com/BizTalk/2003/Messaging/Transmit/powershell-properties";

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
            // implementation not needed since adapter has no handler properties
        }
    }
}
