using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities.Results;

namespace iRLeagueDatabase.Filters
{
    public interface IResultsFilter : IResultsFilterDescription
    {   
        List<FilterValueBaseEntity> FilterValues { get; set; }
        IEnumerable<ResultRowEntity> GetFilteredRows(IEnumerable<ResultRowEntity> resultRows, bool exclude = false);
    }
}
