using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Results
{
    public abstract class FilterValueBaseEntity
    {
        [Key, ForeignKey(nameof(ResultsFilterOption)), Column(Order = 1)]
        public long ScoringId { get; set; }
        [Key, ForeignKey(nameof(ResultsFilterOption)), Column(Order = 2)]
        public long ResultsFilterOptionId { get; set; }
        [Key, Column(Order = 3)]
        public long FilterValueId { get; set; }
        public virtual ResultsFilterOptionEntity ResultsFilterOption { get; set; }
        public abstract object Value { get; set; }

        public abstract Type GetValueType();
    }
}
