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
        void SetFilterOptions(string ColumnPropertyName, ComparatorTypeEnum comparator, bool exclude, bool filterPointsOnly);
        IEnumerable<string> GetFilterValues();
        void SetFilterValueStrings(params string[] filterValues);
        bool FilterPointsOnly { get; set; }
        IEnumerable<T> GetFilteredRows<T>(IEnumerable<T> resultRows) where T : IResultRow;
    }
}
