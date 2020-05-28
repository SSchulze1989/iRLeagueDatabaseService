using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace iRLeagueDatabase.DataTransfer.Messages
{
    [MessageContract]
    public class ResponseMessage
    {
        [MessageHeader]
        public string Username;
        [MessageHeader]
        public string databaseName;
        [MessageHeader]
        public string status;
    }
}
