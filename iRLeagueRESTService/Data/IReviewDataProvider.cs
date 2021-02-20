using iRLeagueDatabase.DataTransfer.Reviews.Convenience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace iRLeagueRESTService.Data
{
    public interface IReviewDataProvider
    {
        SessionReviewsDTO GetReviewsFromSession(long sessionId);
        SeasonReviewsDTO GetReviewsFromSeason(long seasonId);
    }
}
