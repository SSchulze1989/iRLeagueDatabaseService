using iRLeagueDatabase.DataTransfer.Convenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Data
{
    public interface ISeasonDataProvider
    {
        SeasonConvenieneDTO GetSeason(long seasonId);
        SeasonConvenieneDTO[] GetSeasons(params long[] seasonIds);
    }
}