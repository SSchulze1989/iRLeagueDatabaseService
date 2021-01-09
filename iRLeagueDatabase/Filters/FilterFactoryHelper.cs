using iRLeagueManager.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public class FilterFactoryHelper
    {
        public static readonly Dictionary<string, Type> FilterTypeDictionary = new Dictionary<string, Type>();

        static FilterFactoryHelper()
        {
            RegisterFilterType(typeof(ColumnPropertyFilter));
            RegisterFilterType(typeof(MemberListFilter));
        }

        public static IResultsFilter GetFilter(string filterTypeName)
        {
            IResultsFilter filter;

            if (FilterTypeDictionary.ContainsKey(filterTypeName) == false)
            {
                throw new ArgumentException($"Invalid filter type name - filter type {filterTypeName} does not exist");
            }

            var filterType = FilterTypeDictionary[filterTypeName];
            filter = (IResultsFilter)filterType.GetConstructor(new Type[0]).Invoke(new object[0]);

            return filter;
        }

        public static IResultsFilter GetFilter(string filterTypeName, string columnPropertyName, bool exclude, bool onlyPoints, ComparatorTypeEnum comparator)
        {
            var filter = GetFilter(filterTypeName);
            filter.SetFilterOptions(columnPropertyName, comparator, exclude, onlyPoints);
            return filter;
        }

        public static void RegisterFilterType(Type type)
        {
            var name = type.Name;
            if (FilterTypeDictionary.ContainsKey(name) == false)
            {
                FilterTypeDictionary.Add(name, type);
            }
        }
    }
}
