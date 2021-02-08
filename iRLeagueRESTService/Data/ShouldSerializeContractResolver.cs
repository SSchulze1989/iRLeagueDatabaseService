using iRLeagueDatabase.DataTransfer;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Web;

namespace iRLeagueRESTService.Data
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {

        protected override JsonProperty CreateProperty(System.Reflection.MemberInfo member, Newtonsoft.Json.MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            if (property.DeclaringType == typeof(BaseDTO) || property.DeclaringType.BaseType == typeof(BaseDTO))
            {
                if (property.PropertyName == "serializableProperties")
                {
                    property.ShouldSerialize = instance => { return false; };
                }
                else
                {
                    var temp = property.ShouldSerialize;
                    property.ShouldSerialize = instance => 
                    {
                        var p = (BaseDTO)instance;
                        return p.serializableProperties.Any(x => x.Key == property.PropertyName);
                    };
                }
            }
            return property;
        }

        private bool ShouldSerialize(object instance)
        {
            var p = (BaseDTO)instance;
            //return p.serializableProperties.Contains(property.PropertyName);
            return false;
        }
    }
}