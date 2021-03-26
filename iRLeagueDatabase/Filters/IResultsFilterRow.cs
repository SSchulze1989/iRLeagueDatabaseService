using iRLeagueDatabase.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public interface IResultsFilterEntry<T>
    {
        bool ExcludeFromResults { get; set; }
        bool ExcludeFromPoints { get; set; }
        T Entry { get; set; }
    }
}
