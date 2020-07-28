using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueDatabase.Entities.Members;

namespace iRLeagueDatabase.Entities
{
    public class Revision : MappableEntity
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