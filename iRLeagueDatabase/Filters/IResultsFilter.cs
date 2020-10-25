using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iRLeagueDatabase.Entities.Filters;
using iRLeagueDatabase.Entities.Results;
using iRLeagueManager.Enums;

namespace iRLeagueDatabase.Filters
{
    public interface IResultsFilter : IResultsFilterDescription
    {
        void SetFilterOptions(string ColumnPropertyName, ComparatorTypeEnum comparator, bool exclude);
        IEnumerable<string> GetFilterValues();
        void SetFilterValueStrings(params string[] filterValues);
        IEnumerable<ResultRowEntity> GetFilteredRows(IEnumerable<ResultRowEntity> resultRows);
    }
}
