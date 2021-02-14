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

using iRLeagueDatabase.DataTransfer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Data
{
    public static class SelectFieldsHelper
    {
        public static dynamic GetSelectedFieldObject(BaseDTO obj)
        {
            if (obj.SerializableProperties == null || obj.SerializableProperties.Count() == 0)
            {
                return obj;
            }
            var result = new ExpandoObject() as IDictionary<string, object>;

            foreach (var property in obj.SerializableProperties)
            {
                var child = property.Value.GetValue(obj);
                if (child?.GetType().IsArray == true)
                {
                    var array = (child as IEnumerable).OfType<object>();
                    var resultArray = new List<object>();
                    foreach(var item in array)
                    {
                        if (item is BaseDTO dto)
                        {
                            resultArray.Add(SelectFieldsHelper.GetSelectedFieldObject(dto));
                        }
                        else
                        {
                            resultArray.Add(property.Value.GetValue(obj));
                        }
                    }
                    result.Add(property.Key, resultArray);
                }
                else if (child is BaseDTO dto)
                {
                    result.Add(property.Key, SelectFieldsHelper.GetSelectedFieldObject(dto));
                }
                else
                {
                    result.Add(property.Key, property.Value.GetValue(obj));
                }
            }
            return result;
        }
    }
}