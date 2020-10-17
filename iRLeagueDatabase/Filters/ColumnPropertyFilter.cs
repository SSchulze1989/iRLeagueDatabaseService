using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Filters
{
    public class ColumnPropertyFilter : ColumnPropertyFilterDescription, IResultsFilter
    {
        public List<FilterValueBaseEntity> FilterValues { get; set; } = new List<FilterValueBaseEntity>();

        public IEnumerable<ResultRowEntity> GetFilteredRows(IEnumerable<ResultRowEntity> resultRows, bool exclude = false)
        {
            //string columnPropertyName;
            ComparatorTypeEnum comparator;
            //IComparable value;

            // check for correct filter value types
            if (FilterValues.ElementAt(0).Value is string columnPropertyName) { }
            else
            {
                throw new InvalidFilterValueException("Filter value 0 does not have the correct type string");
            }

            if (FilterValues.ElementAt(1).Value is int comparatorInt) 
            {
                try
                {
                    comparator = (ComparatorTypeEnum)comparatorInt;
                }
                catch (Exception e)
                {
                    throw new InvalidFilterValueException("Failed to cast value 1 to type ComparatorTypeEnum, see inner exception for details", e);
                }
            }
            else
            {
                throw new InvalidFilterValueException("Filter value 1 does not have the correct type int");
            }

            if (FilterValues.ElementAt(2).Value is IComparable compValue) { }
            else
            {
                throw new InvalidFilterValueException("Filter value 2 dose not have the correct type IComparable");
            }

            // get property by columnPropertyName
            var columnProperty = typeof(ResultRowEntity).GetProperty(columnPropertyName);
            if (columnProperty == null)
            {
                throw new InvalidFilterValueException($"Column property witht the name {columnPropertyName} not found");
            }

            Func<IComparable, IComparable, bool> compare;

            switch (comparator)
            {
                case ComparatorTypeEnum.IsBigger:
                    compare = (x, y) => { var c = x.CompareTo(y); return c == 1; };
                    break;
                case ComparatorTypeEnum.IsBiggerOrEqual:
                    compare = (x, y) => { var c = x.CompareTo(y); return c == 1 || c == 0; };
                    break;
                case ComparatorTypeEnum.IsEqual:
                    compare = (x, y) => { var c = x.CompareTo(y); return c == 0; };
                    break;
                case ComparatorTypeEnum.IsSmallerOrEqual:
                    compare = (x, y) => { var c = x.CompareTo(y); return c == -1 || c == 0; };
                    break;
                case ComparatorTypeEnum.IsSmaller:
                    compare = (x, y) => { var c = x.CompareTo(y); return c == -1; };
                    break;
                case ComparatorTypeEnum.NotEqual:
                    compare = (x, y) => { var c = x.CompareTo(y); return c == 1 || c == -1; };
                    break;
                default:
                    compare = (x, y) => true;
                    break;
            }

            return resultRows.Where(x => compare(((IComparable)columnProperty.GetValue(x)), compValue));
        }
    }
}
