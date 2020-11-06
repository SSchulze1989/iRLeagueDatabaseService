using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Filters
{
    public class StringFilterValueEntity : FilterValueBaseEntity
    {
        [Column("StringValue")]
        public string StringValue { get; set; }
        public override object Value { get => StringValue; set => StringValue = (string)value; }
        public override Type GetValueType()
        {
            return typeof(string);
        }
    }
}
