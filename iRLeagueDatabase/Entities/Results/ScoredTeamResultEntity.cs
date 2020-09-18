using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Results
{
    public class ScoredTeamResultEntity : ScoredResultEntity
    {
        [InverseProperty(nameof(ScoredTeamResultRowEntity.ScoredTeamResult))]
        public virtual List<ScoredTeamResultRowEntity> TeamResults { get; set; }

        public override void Delete(LeagueDbContext dbContext)
        {
            TeamResults?.ToList().ForEach(x => x.Delete(dbContext));
            base.Delete(dbContext);
        }
    }
}
