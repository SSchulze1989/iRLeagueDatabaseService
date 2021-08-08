using iRLeagueDatabase.DataTransfer.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider.Generic
{
    public class GenericScoredResultsDataProvider : GenericDataProviderBase<ScoredResultDataDTO, long[]>
    {
        public GenericScoredResultsDataProvider(IProviderContext<LeagueDbContext> context) : base(context)
        {
        }

        public override ScoredResultDataDTO GetData(long[] key)
        {
            return GetData(new long[][] { key }).FirstOrDefault();
        }

        public override IEnumerable<ScoredResultDataDTO> GetData(IEnumerable<long[]> keys)
        {
            var resultsDataProvider = new ResultsDataProvider(ProviderContext);
            
            if (keys.Any(x => x.Length != 2))
            {
                throw new ArgumentException("Wrong number of primary keys", nameof(keys));
            }

            var ids = keys.Select(x => new KeyValuePair<long, long>(x[0], x[1]));
            var data = resultsDataProvider.GetScoredResults(ids);
            return data;
        }
    }
}
