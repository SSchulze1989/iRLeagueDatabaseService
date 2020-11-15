using iRLeagueDatabase.Entities.Members;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Statistics
{
    public abstract class StatisticSetEntity : Revision
    {
        [Key]
        public long Id { get; set; }

        [InverseProperty(nameof(DriverStatisticRowEntity.StatisticSet))]
        public virtual List<DriverStatisticRowEntity> DriverStatistic { get; set; }

        public StatisticSetEntity()
        {
            DriverStatistic = new List<DriverStatisticRowEntity>();
        }

        public abstract Task LoadRequiredDataAsync(LeagueDbContext dbContext);
        public abstract void Calculate(LeagueDbContext dbContext);
    }
}
