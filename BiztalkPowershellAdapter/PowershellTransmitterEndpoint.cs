using Microsoft.BizTalk.Component.Interop;
using Microsoft.BizTalk.Message.Interop;
using Microsoft.Samples.BizTalk.Adapter.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;


namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    internal class PowershellTransmitterEndpoint : AsyncTransmitterEndpoint
    {
        private AsyncTransmitter asyncTransmitter = null;
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
            //Stream source = message.BodyPart.Data;
            PowershellAdapterMessage psMsg = new PowershellAdapterMessage(message);
            
            // build url
            PowershellTransmitterProperties props = new PowershellTransmitterProperties(message, propertyNamespace);

            string script = props.Script;
            //string arguments = props.Arguments;
            string host = props.Host;
            string user = props.User;
            string password = props.Password;

            bool bSuccess = true;
            string error = "";

            string scriptToRun = "";
            
            if(String.IsNullOrEmpty(host) && String.IsNullOrEmpty(user))
            {
                scriptToRun = script;
            }
            else if(!String.IsNullOrEmpty(host) && String.IsNullOrEmpty(user))
            {
                scriptToRun = @"$xmlmessage = $message.GetBody() \n" +
                    "Invoke-Command -ComputerName "+host+" -ScriptBlock { "+script+ " } -ArgumentList $xmlmessage";
            }
            else if (!String.IsNullOrEmpty(host) && !String.IsNullOrEmpty(user))
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
            
            Collection<PSObject> results = pipeline.Invoke();
            if (pipeline.HadErrors)
            {
                var errors = pipeline.Error.ReadToEnd();
                error = errors[0].ToString();
                
                bSuccess = false;
            }

            runspace.Close();

            if (!bSuccess)
                throw new PowershellAdapterException(error);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                stringBuilder.AppendLine(obj.ToString());
            }
            string scriptRet = stringBuilder.ToString();

            /*
            var psCommmand = script + " ";
            psCommmand = psCommmand + arguments;
            var psCommandBytes = System.Text.Encoding.Unicode.GetBytes(psCommmand);
            var psCommandBase64 = Convert.ToBase64String(psCommandBytes);

            var startInfo = new ProcessStartInfo()
            {
                FileName = "powershell.exe",
                Arguments = $"-NoProfile -ExecutionPolicy unrestricted -EncodedCommand {psCommandBase64}",
                UseShellExecute = false
            };
            Process.Start(startInfo);
            */
            return null;
        }
    }
}
