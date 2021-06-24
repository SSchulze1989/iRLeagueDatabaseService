using iRLeagueDatabase.DataTransfer.Statistics.Convenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider
{
    public interface IStatsDataProvider
    {
        StatisticConvenienceDTO GetStatistics(long seasonId, bool includeSeason, bool includeLeague);
    }
}
