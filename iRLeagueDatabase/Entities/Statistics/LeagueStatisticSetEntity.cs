using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Statistics
{
    public class LeagueStatisticSetEntity : StatisticSetEntity
    {
        public override void Calculate(LeagueDbContext dbContext)
        {
            throw new NotImplementedException();
        }

        public override Task LoadRequiredDataAsync(LeagueDbContext dbContext)
        {
            throw new NotImplementedException();
        }
    }
}
