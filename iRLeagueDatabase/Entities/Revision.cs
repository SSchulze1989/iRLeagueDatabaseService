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

        [ForeignKey(nameof(CreatedBy))]
        public long? CreatedByUserId { get; set; }
        public LeagueUserEntity CreatedBy { get; set; }
        [ForeignKey(nameof(LastModifiedBy))]
        public long? LastModifiedByUserId { get; set; }
        public LeagueUserEntity LastModifiedBy { get; set; }
    } 
}