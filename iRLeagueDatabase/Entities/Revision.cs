using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities
{
    public abstract class Revision : MappableEntity
    {
        public DateTime? CreatedOn { get; set; } = null;
        public DateTime? LastModifiedOn { get; set; } = null;

        public int Version { get; set; }

        public string CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserId { get; set; }
        public string LastModifiedByUserName { get; set; }
    }
}
