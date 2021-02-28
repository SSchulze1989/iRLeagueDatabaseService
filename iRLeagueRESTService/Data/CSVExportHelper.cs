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
using System.Web;

namespace iRLeagueRESTService.Data
{
    public class CSVExportHelper
    {
        public char Delimiter { get; set; } = '\t';
        public bool UseAttributeNames { get; set; }

        public void WriteToFile<T>(StreamWriter writer, IEnumerable<T> data)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

            var stringBuilder = new StringBuilder();

            // get header row
            stringBuilder.AppendLine(string.Join(Delimiter.ToString(), GetCSVHeader(data.FirstOrDefault()).ToArray()));

            // get properties to export for each item
            foreach(var item in data)
            {
                PropertyInfo
                if (item is BaseDTO baseDTO)
                {
                    properties = baseDTO.SerializableProperties;
                }
                else
                {
                    properties = item.GetType().GetProperties();
                }
            }

            if (typeof(ICSVExport).IsAssignableFrom(typeof(T)))
            {
                var first = ((ICSVExport)entity.FirstOrDefault());
                if (first == null)
                    return;
                buffer += first.GetCSVHeader(Delimiter);
            }
            else
            {
                foreach (var field in typeof(T).GetProperties())
                {
                    buffer += field.Name + Delimiter;
                }
            }
            buffer += "\n";
            foreach (T Row in entity.GetRows())
            {
                if (Row is ICSVExport csvExport)
                {
                    buffer += csvExport.GetCSVString(Delimiter);
                }
                else
                {
                    foreach (var field in typeof(T).GetProperties())
                    {
                        if (field.GetValue(Row) == null)
                        {
                            buffer += "" + Delimiter;
                        }
                        else if (field.PropertyType.Equals(typeof(Enum)))
                        {
                            buffer += ((Enum)field.GetValue(Row)).ToCSVString() + Delimiter;
                        }
                        else
                        {
                            buffer += field.GetValue(Row).ToString() + Delimiter;
                        }
                    }
                }
                buffer += "\n";
            }
        }

        private IEnumerable<string> GetCSVHeader(object item)
        {
            if (item is BaseDTO baseDTO)
            {
                return baseDTO.SerializableProperties.Select(x => x.Key);
            }
            else
            {
                var properties = item.GetType()
                    .GetProperties();

                List<string> names = new List<string>();
                foreach(var property in properties)
                {
                    var dataMemberAttribute = (DataMemberAttribute)property.GetCustomAttribute(typeof(DataMemberAttribute));
                    if (dataMemberAttribute != null && dataMemberAttribute.IsNameSetExplicitly)
                    {
                        names.Add(dataMemberAttribute.Name);
                    }
                    else
                    {
                        names.Add(property.Name);
                    }
                }
                return names;
            }
        }
    }
}