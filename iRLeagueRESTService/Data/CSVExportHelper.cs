using iRLeagueDatabase.DataTransfer;
using System;
using System.Collections.Generic;
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
                var columnData = columns.Select(x => x.Value.GetValue(item)?.ToString() ?? "");
                stringBuilder.AppendLine(columnData.Aggregate((x, y) => x + Delimiter + y));
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
                    if (dataMemberAttribute != null && dataMemberAttribute.IsNameSetExplicitly)
                    {
                        columns.Add(new KeyValuePair<string, PropertyInfo>(dataMemberAttribute.Name, property));
                    }
                    else
                    {
                        columns.Add(new KeyValuePair<string, PropertyInfo>(property.Name, property));
                    }
                }
                return columns;
            }
        }
    }
}