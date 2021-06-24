using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider
{
    public interface ILeagueActionProvider<TModelId> : IDisposable
    {
        void CalculateScoredResult(TModelId sessionId);
        void CalculateScoredResultArray(TModelId[] sessionId);
    }

    public interface ILeagueActionProvider : ILeagueActionProvider<long> { }
}
