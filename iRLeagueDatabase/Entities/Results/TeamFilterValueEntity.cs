using iRLeagueDatabase.Entities.Members;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Results
{
    public class TeamFilterValueEntity : FilterValueBaseEntity
    {
        [ForeignKey(nameof(TeamValue)), Column("TeamIdValue")]
        public long TeamId { get; set; }
        public TeamEntity TeamValue { get; set; }
        public override object Value { get => TeamValue; set => TeamValue = (TeamEntity)value; }
        public override Type GetValueType()
        {
            return typeof(TeamEntity);
        }
    }
}
