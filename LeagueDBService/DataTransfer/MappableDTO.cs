using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace iRLeagueDatabase.DataTransfer
{
    [DataContract]
    public abstract class MappableDTO
    {
        public virtual object MappingId { get; } = null;
        public abstract object[] Keys { get; }
    }
}
