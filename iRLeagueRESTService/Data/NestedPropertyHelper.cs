using iRLeagueDatabase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace iRLeagueRESTService.Data
{
    public static class NestedPropertyHelper
    {
        public static IEnumerable<PropertyInfo> OrderNestedProperties(IEnumerable<PropertyInfo> properties)
        {
            return properties
                .OrderBy(x => x.Name)
                .ThenBy(x => x is NestedPropertyInfo nested ? nested.GetPropertyTree().Count() : 0);
        }
    }
}