using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Results
{
    public class DoubleFilterValueEntity : FilterValueBaseEntity
    {
        [Column("DoubleValue")]
        public double DoubleValue { get; set; }
        public override object Value { get => DoubleValue; set => DoubleValue = (double)value; }
        public override Type GetValueType()
        {
            return typeof(double);
        }
    }
}
