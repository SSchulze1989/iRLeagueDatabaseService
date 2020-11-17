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

        public bool RequiresRecalculation { get; set; }

        [NotMapped]
        public bool IsDataLoaded { get; protected set; } = false;

        public StatisticSetEntity()
        {
            DriverStatistic = new List<DriverStatisticRowEntity>();
        }

        public abstract Task LoadRequiredDataAsync(LeagueDbContext dbContext, bool force = false);
        public abstract void Calculate(LeagueDbContext dbContext);
        public abstract Task<bool> CheckRequireRecalculationAsync(LeagueDbContext dbContext);
    }
}
