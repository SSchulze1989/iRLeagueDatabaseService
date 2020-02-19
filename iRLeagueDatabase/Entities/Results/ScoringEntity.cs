using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.ComponentModel;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using iRLeagueManager.Enums;
using iRLeagueDatabase.Entities.Sessions;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoringEntity : Revision
    {
        [Key]
        public long ScoringId { get; set; }
        public string Name { get; set; }
        public override object MappingId => ScoringId;
        public int DropWeeks { get; set; }
        public int AverageRaceNr { get; set; }
        public virtual List<SessionBaseEntity> Sessions { get; set; }
        [ForeignKey(nameof(Season))]
        public long SeasonId { get; set; }
        [Required]
        public virtual SeasonEntity Season { get; set; }
        public string BasePoints { get; set; }
        public string BonusPoints { get; set; }
        public string IncPenaltyPoints { get; set; }
        public string MultiScoringFactors { get; set; }
        public virtual List<ScoringEntity> MultiScoringResults { get; set; }

        //public ScoringRuleBase Rule { get; set; }

        public ScoringEntity() { }
    }
}
