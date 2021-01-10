// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataTransfer
{
    [DataContract]
    public abstract class BaseDTO
    {
        public List<PropertyInfo> serializableProperties { get; set; }

        public void SetSerializableProperties(string[] fields)
        {
            serializableProperties = new List<PropertyInfo>();
            if (fields != null && fields.Count() > 0)
            {
                var fieldGrouping = fields
                    .Select(x => x.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries))
                    .Where(x => x.Count() > 0)
                    .GroupBy(x => x[0], x => string.Join(".", x.Skip(1)));
                var type = this.GetType();

                foreach(var field in fieldGrouping)
                {
                    var key = field.Key;
                    var property = type.GetProperty(key);
                    if (property != null)
                    {
                        var obj = property.GetValue(this);
                        if (obj is IEnumerable enumerable )
                        {
                            var array = enumerable.OfType<object>();
                            foreach(var arrObj in array)
                            {
                                if (arrObj is BaseDTO dto)
                                {
                                    dto.SetSerializableProperties(field.ToArray());
                                }
                            }
                        }
                        else if (obj is BaseDTO dto)
                        {
                            dto.SetSerializableProperties(field.ToArray());
                        }
                        serializableProperties.Add(property);
                    }
                }
                return;
            }
        }
    }
}
