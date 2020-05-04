using Microsoft.BizTalk.Adapter.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Phuuskon.BizTalk.Adapters.PowershellManagement
{
    public class PowershellAdapterManagement : IAdapterConfig, IStaticAdapterConfig, IAdapterConfigValidation
    {
        public string GetConfigSchema(ConfigType configType)
        {
            switch (configType)
            {
                case ConfigType.TransmitHandler:
                    return GetResource("Phuuskon.BizTalk.Adapters.PowershellManagement.TransmitHandler.xsd");

                case ConfigType.TransmitLocation:
                    return GetResource("Phuuskon.BizTalk.Adapters.PowershellManagement.TransmitLocation.xsd");

                default:
                    return null;
            }
        }

        public Result GetSchema(string uri, string namespaceName, out string fileLocation)
        {
            fileLocation = null;
            return Result.Continue;
        }

        public string[] GetServiceDescription(string[] wsdlReferences)
        {
            string[] result = new string[1];
            result[0] = GetResource("Phuuskon.BizTalk.Adapters.PowershellManagement.service1.wsdl");
            return result;
        }

        public string GetServiceOrganization(Microsoft.BizTalk.Component.Interop.IPropertyBag endpointConfiguration, string nodeIdentifier)
        {
            string result = GetResource("Phuuskon.BizTalk.Adapters.PowershellManagement.CategorySchema.xml");
            return result;
        }

        public string ValidateConfiguration(ConfigType configType, string configuration)
        {
            string validXml = String.Empty;

            switch (configType)
            {
                case ConfigType.TransmitHandler:
                    validXml = configuration;
                    break;

                case ConfigType.TransmitLocation:
                    validXml = ValidateTransmitLocation(configuration);
                    break;
            }

            return validXml;
        }

        private string GetResource(string resource)
        {
            string value = null;
            if (null != resource)
            {
                Assembly assem = this.GetType().Assembly;
                Stream stream = assem.GetManifestResourceStream(resource);
                StreamReader reader = null;

                using (reader = new StreamReader(stream))
                {
                    value = reader.ReadToEnd();
                }
            }

            return value;
        }

        private string ValidateTransmitLocation(string xmlInstance)
        {
            // Load up document
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlInstance);

            // Build up inner text
            StringBuilder builder = new StringBuilder();

            XmlNode script = document.SelectSingleNode("Config/script");
            if (null != script && 0 < script.InnerText.Length)
            {
                builder.Append(script.InnerText + @"\");
            }
            
            XmlNode uri = document.SelectSingleNode("Config/uri");
            if (null == uri)
            {
                uri = document.CreateElement("uri");
                document.DocumentElement.AppendChild(uri);
            }
            uri.InnerText = "Powershell://"+builder.ToString();

            return document.OuterXml;
        }
    }
}
