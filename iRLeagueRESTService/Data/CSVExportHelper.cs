using iRLeagueDatabase.DataTransfer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace iRLeagueRESTService.Data
{
    public class CSVExportHelper
    {
        public char Delimiter { get; set; } = '\t';
        public bool UseAttributeNames { get; set; }
        public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

        private static Dictionary<Type, Func<object, string>> typeConversions;
        public static ReadOnlyDictionary<Type, Func<object, string>> TypeConversions => new ReadOnlyDictionary<Type, Func<object, string>>(typeConversions);

        static CSVExportHelper()
        {
            typeConversions = new Dictionary<Type, Func<object, string>>
            {
                { typeof(double), x => ((double?)x)?.ToString("0.00") },
                { typeof(DateTime), x => ((DateTime?)x)?.ToShortDateString() },
                { typeof(DateTime?), x => ((DateTime?)x)?.ToShortDateString() }
            };
        }

        private string ColumnValueToString(Type type, string columnName, object value)
        {
            if (type != null && TypeConversions.ContainsKey(type))
            {
                var conversion = TypeConversions[type];
                return conversion.Invoke(value);
            }
            else
            {
                return value?.ToString();
            }
        }

        private string GetRow(IEnumerable<KeyValuePair<string, PropertyInfo>> columns, object rowItem)
        {
            var columnsData = columns.Select(x => ColumnValueToString(x.Value.PropertyType, x.Key, x.Value.GetValue(rowItem)));
            return columnsData.Aggregate((x, y) => x + Delimiter + y);
        }

        public void WriteToStream(Stream stream, IEnumerable<object> data)
        {
            Thread.CurrentThread.CurrentCulture = Culture;

            var stringBuilder = new StringBuilder();

            // get column properties
            var columns = GetCSVColumns(data.First());

            // write headers
            stringBuilder.AppendLine(columns.Select(x => x.Key).Aggregate((x, y) => x + Delimiter + y));

            // write data rows
            foreach (var item in data)
            {
                stringBuilder.AppendLine(GetRow(columns, item));
            }

            // write data as binary to stream writer
            var writer = new BinaryWriter(stream);
            byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
            writer.Write(bytes, 0, bytes.Length);
        }

        private IEnumerable<KeyValuePair<string, PropertyInfo>> GetCSVColumns(object item)
        {
            if (item is BaseDTO baseDTO)
            {
                return baseDTO.SerializableProperties;
            }
            else
            {
                var properties = item.GetType()
                    .GetProperties();

                List<KeyValuePair<string, PropertyInfo>> columns = new List<KeyValuePair<string, PropertyInfo>>();
                foreach(var property in properties)
                {
                    var dataMemberAttribute = (DataMemberAttribute)property.GetCustomAttribute(typeof(DataMemberAttribute));
                    string columnName;
                    if (dataMemberAttribute != null && dataMemberAttribute.IsNameSetExplicitly)
                    {
                        columnName = dataMemberAttribute.Name;
                    }
                    else
                    {
                        columnName = property.Name;
                    }
                    columns.Add(new KeyValuePair<string, PropertyInfo>(columnName, property));
                }
                return columns;
            }
        }
    }
}