using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

using iRLeagueDatabase.DataTransfer.Members;

namespace iRLeagueDatabase.User
{
    [DataContract]
    public class AuthenticationResult
    {
        [DataMember]
        public bool IsAuthenticated { get; internal set; }
        [DataMember]
        public string Status { get; internal set; }
        [DataMember]
        public LeagueUserDTO AuthenticatedUser { get; internal set; }
    }
}
