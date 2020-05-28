using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace iRLeagueDatabase.DataTransfer.Messages
{
    [MessageContract]
    public class GETItemsRequestMessage
    {
        [MessageHeader]
        public string userName;
        [MessageHeader]
        public string password;
        [MessageHeader]
        public string databaseName;
        [MessageHeader]
        public bool requestResponse;
        [MessageHeader]
        public string requestItemType;
        [MessageBodyMember]
        public long[][] requestItemIds;
    }
}
