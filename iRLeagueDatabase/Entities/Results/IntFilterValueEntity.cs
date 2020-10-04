using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Results
{
    public class IntFilterValueEntity : FilterValueBaseEntity
    {
        [Column("IntValue")]
        public int IntValue { get; set; }
        public override object Value { get => IntValue; set => IntValue = (int)value; }
        public override Type GetValueType()
        {
            return typeof(int);
        }
    }
}
