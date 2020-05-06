using Microsoft.BizTalk.Message.Interop;
using System.IO;
using System.Xml;

namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    public static class IBaseMessageExtensions
    {
        public static XmlDocument GetBody(this IBaseMessage msg)
        {
            XmlDocument xmlDoc = new XmlDocument();
            using (Stream stream = msg.BodyPart.Data)
            {
                XmlReader reader = XmlReader.Create(stream);
                xmlDoc.Load(reader);
            }
            return xmlDoc;
        }       
    }
}
