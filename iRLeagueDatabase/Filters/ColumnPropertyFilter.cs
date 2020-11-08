using iRLeagueDatabase.Entities.Results;
using iRLeagueManager.Enums;
using iRLeagueDatabase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public class ColumnPropertyFilter : ColumnPropertyFilterDescription, IResultsFilter
    {
        public string ColumnPropertyName { get; set; }
        public ComparatorTypeEnum Comparator { get; set; }
        public List<string> FilterValues { get; set; } = new List<string>();
        public bool Exclude { get; set; }

        public IEnumerable<ResultRowEntity> GetFilteredRows(IEnumerable<ResultRowEntity> resultRows)
        {
            // get property by columnPropertyName
            var nestedColumnProperty = typeof(ResultRowEntity).GetNestedPropertyInfo(ColumnPropertyName);
            if (nestedColumnProperty == null)
            {
                throw new InvalidFilterValueException($"Column property witht the name {ColumnPropertyName} not found");
            }

            Type propertyType = nestedColumnProperty.PropertyType;
            if (typeof(IComparable).IsAssignableFrom(propertyType) == false)
            {
                throw new InvalidFilterValueException($"Column {ColumnPropertyName} does not have a comparable type");
            }

            // Convert string to column property type
            IEnumerable<IComparable> comparableFilterValues;
            if (propertyType.Equals(typeof(TimeSpan)))
            {
                comparableFilterValues = FilterValues.Select(x => TimeSpan.Parse(x)).Cast<IComparable>();
            }
            else
            { 
                comparableFilterValues = FilterValues.Select(x => Convert.ChangeType(x, propertyType)).Cast<IComparable>(); 
            }

            Func<IComparable, IEnumerable<IComparable>, bool> compare;

            switch (Comparator)
            {
                case ComparatorTypeEnum.IsBigger:
                    compare = (x, y) => { var c = x.CompareTo(y.FirstOrDefault()); return c == 1; };
                    break;
                case ComparatorTypeEnum.IsBiggerOrEqual:
                    compare = (x, y) => { var c = x.CompareTo(y.FirstOrDefault()); return c == 1 || c == 0; };
                    break;
                case ComparatorTypeEnum.IsEqual:
                    compare = (x, y) => { var c = x.CompareTo(y.FirstOrDefault()); return c == 0; };
                    break;
                case ComparatorTypeEnum.IsSmallerOrEqual:
                    compare = (x, y) => { var c = x.CompareTo(y.FirstOrDefault()); return c == -1 || c == 0; };
                    break;
                case ComparatorTypeEnum.IsSmaller:
                    compare = (x, y) => { var c = x.CompareTo(y.FirstOrDefault()); return c == -1; };
                    break;
                case ComparatorTypeEnum.NotEqual:
                    compare = (x, y) => { var c = x.CompareTo(y.FirstOrDefault()); return c == 1 || c == -1; };
                    break;
                case ComparatorTypeEnum.InList:
                    compare = (x, y) => { var c = y.Any(z => x.CompareTo(z) == 0); return c; };
                    break;
                default:
                    compare = (x, y) => true;
                    break;
            }

            return resultRows.Where(x => { var value = (IComparable)nestedColumnProperty.GetValue(x); return (value != null && compare(value, comparableFilterValues)) != Exclude; });
        }

        public IEnumerable<string> GetFilterValues()
        {
            return FilterValues;
        }

        public void SetFilterOptions(string columnPropertyName, ComparatorTypeEnum comparator, bool exclude)
        {
            ColumnPropertyName = columnPropertyName;
            Comparator = comparator;
            Exclude = exclude;
        }

        public void SetFilterValueStrings(params string[] filterValues)
        {
            FilterValues = filterValues.ToList();
        }
    }
}
