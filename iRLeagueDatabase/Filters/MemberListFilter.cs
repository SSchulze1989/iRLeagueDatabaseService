using iRLeagueDatabase.Entities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public class MemberListFilter : IResultsFilter
    {
        public string FilterName => "Member List";

        public string FilterDescription => "Include/Exclude members based on a static list";

        public int NrOfFilterValues => -1;

        public IEnumerable<string> FilterValueDescriptions => new string[] { "Select member" };

        public List<FilterValueBaseEntity> FilterValues { get; set; } = new List<FilterValueBaseEntity>();

        public IEnumerable<ResultRowEntity> GetFilteredRows(IEnumerable<ResultRowEntity> resultRows, bool exclude)
        {
            var memberIdList = FilterValues.OfType<MemberFilterValueEntity>().Select(x => x.MemberId);

            return resultRows.Where(x => memberIdList.Contains(x.MemberId) != exclude);
        }
    }
}
