using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Statistics
{
    public class DriverStatisticEntity
    {
        [Key]
        public long DriverStatisticsId { get; set; }

        [InverseProperty(nameof(DriverStatisticRowEntity.DriverStatistic))]
        public virtual List<DriverStatisticRowEntity> StatisticRows { get; set; }

        public DriverStatisticEntity()
        {
            StatisticRows = new List<DriverStatisticRowEntity>();
        }
    }
}
