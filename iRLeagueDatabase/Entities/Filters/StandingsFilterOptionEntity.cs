using iRLeagueDatabase.Entities.Results;
using iRLeagueManager.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Filters
{
    public class StandingsFilterOptionEntity : Revision
    {
        [Key, Column(Order = 0)]
        public long ResultsFilterId { get; set; }

        [ForeignKey(nameof(ScoringTable)), Column(Order = 1)]
        public long ScoringTableId { get; set; }

        public virtual ScoringTableEntity ScoringTable { get; set; }
        public string ResultsFilterType { get; set; }
        public string ColumnPropertyName { get; set; }
        public ComparatorTypeEnum Comparator { get; set; }
        public bool Exclude { get; set; }
        public string FilterValues { get; set; }

        public override object MappingId => ResultsFilterId;
    }
}
