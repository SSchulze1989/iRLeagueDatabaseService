using iRLeagueDatabase.DataTransfer;
using System;
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
            if (obj.serializableProperties == null || obj.serializableProperties.Count() == 0)
            {
                return obj;
            }
            var result = new ExpandoObject() as IDictionary<string, object>;

            foreach (var property in obj.serializableProperties)
            {
                var child = property.GetValue(obj);
                if (child is BaseDTO dto)
                {
                    result.Add(property.Name, SelectFieldsHelper.GetSelectedFieldObject(dto));
                }
                else
                {
                    result.Add(property.Name, property.GetValue(obj));
                }
            }
            return result;
        }
    }
}