using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Filters;
using iRLeagueManager.Enums;

namespace iRLeagueDatabase.Entities.Filters
{
    public class ResultsFilterOptionEntity : LeagueRevision
    {
        [Key, Column(Order = 0)]
        public long ResultsFilterId { get; set; }

        [ForeignKey(nameof(Scoring)), Column(Order = 1)]
        public long ScoringId { get; set; }
        public virtual ScoringEntity Scoring { get; set; }
        public string ResultsFilterType { get; set; }
        public string ColumnPropertyName { get; set; }
        public ComparatorTypeEnum Comparator { get; set; }
        public bool Exclude { get; set; }
        public bool FilterPointsOnly { get; set; }

        //[InverseProperty(nameof(FilterValueBaseEntity.ResultsFilterOption))]
        //public virtual List<FilterValueBaseEntity> FilterValues { get; set; }
        public string FilterValues { get; set; }

        public override object MappingId => ResultsFilterId;

        public override void Delete(LeagueDbContext dbContext)
        {
            if (Scoring != null)
            {
                var results = Scoring.GetAllSessions().Where(x => x.SessionResult != null).Select(x => x.SessionResult);
                foreach(var result in results)
                {
                    result.RequiresRecalculation = true;
                }
            }
            base.Delete(dbContext);
        }

        public override long GetLeagueId()
        {
            return Scoring.GetLeagueId();
        }
    }
}
