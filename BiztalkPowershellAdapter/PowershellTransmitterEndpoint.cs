using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Samples.BizTalk.Adapter.Common;
using System.Management.Automation.Runspaces;


namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    internal class PowershellTransmitterEndpoint : AsyncTransmitterEndpoint
    {
        private readonly AsyncTransmitter asyncTransmitter = null;
        private string propertyNamespace;

        public PowershellTransmitterEndpoint(AsyncTransmitter asyncTransmitter) : base(asyncTransmitter)
        {
            this.asyncTransmitter = asyncTransmitter;
        }

        public override void Open(EndpointParameters endpointParameters, IPropertyBag handlerPropertyBag, string propertyNamespace)
        {
            this.propertyNamespace = propertyNamespace;
        }

        /// <summary>
        /// Implementation for AsyncTransmitterEndpoint::ProcessMessage
        /// Transmit the message and optionally return the response message (for Request-Response support)
        /// </summary>
        public override IBaseMessage ProcessMessage(IBaseMessage message)
        {
            PowershellAdapterMessage psMsg = new PowershellAdapterMessage(message);
            
            // build url
            PowershellTransmitterProperties props = new PowershellTransmitterProperties(message, propertyNamespace);

            string script = props.Script;
            string host = props.Host;
            string user = props.User;
            string password = props.Password;

            bool bSuccess = true;
            string error = "";

            string scriptToRun = "";
            
            if(string.IsNullOrEmpty(host) && string.IsNullOrEmpty(user))
            {
                scriptToRun = script;
            }
            else if(!string.IsNullOrEmpty(host) && string.IsNullOrEmpty(user))
            {
                scriptToRun = @"$xmlmessage = $message.GetBody() 
                    Invoke-Command -ComputerName "+host+" -ScriptBlock { "+script+ " } -ArgumentList $xmlmessage";
            }
            else if (!string.IsNullOrEmpty(host) && !string.IsNullOrEmpty(user))
            {
                scriptToRun = @"$username = '"+user+"' \n" +
                    "$password = '"+password+"' \n" +
                    "$pass = ConvertTo-SecureString -AsPlainText $password -Force \n" +
                    "$cred = New-Object System.Management.Automation.PSCredential -ArgumentList $username,$pass \n" +
                    "$xmlmessage = $message.GetBody() \n" +
                    "Invoke-Command -ComputerName " +host+" -credential $cred -ScriptBlock { "+script+ " } -ArgumentList $xmlmessage";
            }

            Runspace runspace = RunspaceFactory.CreateRunspace();

            runspace.Open();
            runspace.SessionStateProxy.SetVariable("message", psMsg);

            Pipeline pipeline = runspace.CreatePipeline();
            pipeline.Commands.AddScript(scriptToRun);
            pipeline.Commands.Add("Out-String");
            
            pipeline.Invoke();
            if (pipeline.HadErrors)
            {
                var errors = pipeline.Error.ReadToEnd();
                error = errors[0].ToString();
                
                bSuccess = false;
            }

            runspace.Close();

            if (!bSuccess)
                throw new PowershellAdapterException(error);
                        
            return null;
        }
    }
}
