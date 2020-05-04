using Microsoft.BizTalk.Message.Interop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Phuuskon.BizTalk.Adapters.PowershellTransmitter
{
    public class PowershellAdapterMessage
    {
        private Stream msgStream { get; set; }
        public string BodyPartName { get; private set; }
        public Guid MessageID { get; private set; }

        public PowershellAdapterMessage(IBaseMessage pMsg)
        {
            msgStream = pMsg.BodyPart.Data;
            BodyPartName = pMsg.BodyPartName;
            MessageID = pMsg.MessageID;
        }

        public XmlDocument GetBody()
        {
            XmlDocument xmlDoc = new XmlDocument();
            using (msgStream)
            {
                using (XmlReader reader = XmlReader.Create(msgStream))
                {
                    xmlDoc.Load(reader);
                }
            }
            return xmlDoc;
        }
    }
}
