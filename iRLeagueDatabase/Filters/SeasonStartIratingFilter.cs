using iRLeagueDatabase.Entities.Filters;
using iRLeagueDatabase.Entities.Results;
using iRLeagueManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public class SeasonStartIRatingFilter : SeasonStartIRatingFilterDescription, IResultsFilter
    {
        public List<int> FilterValues { get; set; } = new List<int>();
        public bool Exclude { get; set; }

        public IEnumerable<ResultRowEntity> GetFilteredRows(IEnumerable<ResultRowEntity> resultRows)
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
            return resultRows.Where(x => (x.SeasonStartIRating >= cutoffIrating) != Exclude);
        }

        public IEnumerable<string> GetFilterValues()
        {
            throw new NotImplementedException();
        }

        public void SetFilterOptions(string ColumnPropertyName, ComparatorTypeEnum comparator, bool exclude)
        {
            throw new NotImplementedException();
        }

        public void SetFilterValueStrings(params string[] filterValues)
        {
            throw new NotImplementedException();
        }
    }
}
