using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Reviews
{
    public class VoteCategoryEntity : LeagueMappableEntity
    {
        [Key]
        public long CatId { get; set; }

        [ForeignKey(nameof(League))]
        public long LeagueId { get; set; }
        public virtual LeagueEntity League { get; set; }

        public string Text { get; set; }
        public int Index { get; set; }
        public int DefaultPenalty { get; set; }

        public override object MappingId => CatId;

        public override long GetLeagueId()
        {
            return LeagueId;
        }
    }
}
