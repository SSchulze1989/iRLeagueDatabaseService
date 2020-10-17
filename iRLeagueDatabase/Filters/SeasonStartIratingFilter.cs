using iRLeagueDatabase.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public class SeasonStartIRatingFilter : SeasonStartIRatingFilterDescription, IResultsFilter
    {
        public List<FilterValueBaseEntity> FilterValues { get; set; } = new List<FilterValueBaseEntity>();

        public IEnumerable<ResultRowEntity> GetFilteredRows(IEnumerable<ResultRowEntity> resultRows, bool exclude = false)
        {
            int cutoffIrating;
            try
            {
                cutoffIrating = FilterValues.OfType<IntFilterValueEntity>().FirstOrDefault().IntValue;
            }
            catch (Exception e)
            {
                throw new InvalidFilterValueException("Filter is null or has invalid type", innerException: e);
            }
            
            // Return rows with irating >= cutoff or exclude them
            return resultRows.Where(x => (x.SeasonStartIRating >= cutoffIrating) != exclude);
        }
    }
}
