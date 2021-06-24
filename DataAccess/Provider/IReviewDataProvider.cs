using iRLeagueDatabase.DataTransfer.Reviews;
using iRLeagueDatabase.DataTransfer.Reviews.Convenience;
using iRLeagueDatabase.Entities.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace iRLeagueDatabase.DataAccess.Provider
{
    public interface IReviewDataProvider
    {
        SessionReviewsDTO GetReviewsFromSession(long sessionId, SessionBaseEntity preLoadedSession = null, IncidentReviewDataDTO[] preLoadedReviews = null);
        SeasonReviewsDTO GetReviewsFromSeason(long seasonId);
    }
}
