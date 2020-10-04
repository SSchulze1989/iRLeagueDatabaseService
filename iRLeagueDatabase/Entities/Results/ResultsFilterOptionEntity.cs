using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Filters;

namespace iRLeagueDatabase.Entities.Results
{
    public class ResultsFilterOptionEntity : MappableEntity
    {
        [Key, Column(Order = 2)]
        public long ResultsFilterId { get; set; }

        [Key, ForeignKey(nameof(Scoring)), Column(Order = 1)]
        public long ScoringId { get; set; }
        public virtual ScoringEntity Scoring { get; set; }
        public string ResultsFilterType { get; set; }
        public bool Exclude { get; set; }

        [InverseProperty(nameof(FilterValueBaseEntity.ResultsFilterOption))]
        public virtual List<FilterValueBaseEntity> FilterValues { get; set; }

        public override object MappingId => new object[] { ScoringId, ResultsFilterId };
    }
}
