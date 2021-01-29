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
    public class MemberListFilter : MemberListFilterDescription, IResultsFilter
    {
        public List<long> FilterValues { get; set; } = new List<long>();
        public bool Exclude { get; set; }
        public bool FilterPointsOnly { get; set; }

        public IEnumerable<T> GetFilteredRows<T>(IEnumerable<T> resultRows) where T : IResultRow
        {
            var memberIdList = FilterValues;

            return resultRows.Where(x => memberIdList.Contains(x.MemberId) != Exclude);
        }

        public IEnumerable<string> GetFilterValues()
        {
            return FilterValues.Select(x => x.ToString());
        }

        public void SetFilterOptions(string ColumnPropertyName, ComparatorTypeEnum comparator, bool exclude, bool onlyPoints)
        {
            throw new NotImplementedException();
        }

        public void SetFilterValueStrings(params string[] filterValues)
        {
            try
            {
                var memberIdList = filterValues.Select(x => long.Parse(x));
                FilterValues = memberIdList.ToList();
            }
            catch (Exception e)
            {
                throw new InvalidFilterValueException("Setting filter values failed. Please make sure all values are valid long values", e);
            }
        }
    }
}
