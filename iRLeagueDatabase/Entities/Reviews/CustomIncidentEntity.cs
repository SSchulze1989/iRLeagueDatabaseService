using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iRLeagueDatabase.Entities.Reviews
{
    public class CustomIncidentEntity : MappableEntity
    {
        [Key]
        public long IncidentId { get; set; }
        public string Text { get; set; }
        public int Index { get; set; }

        public override object MappingId => IncidentId;
    }
}
