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
    /// <summary>
    /// Base class for statistic set implementation. Any class that is used to calculate statistics should inherit from this class.
    /// All derived classes will be mapped on one table in the database. This table will share all possible column values for all derived classes
    /// but each class can have additional dependencies to foreign keys and mapping tables.
    /// </summary>
    public abstract class StatisticSetEntity : Revision
    {
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// List of <see cref="DriverStatisticRowEntity"/> holding the calculated statistic data for each driver that had an appearance in the related data.
        /// <para>Can be empty if the implemented statistic set does not have any driver statistics.</para>
        /// </summary>
        [InverseProperty(nameof(DriverStatisticRowEntity.StatisticSet))]
        public virtual List<DriverStatisticRowEntity> DriverStatistic { get; set; }

        /// <summary>
        /// Time between two database updates for the statistics set
        /// </summary>
        public long UpdateInterval;
        /// <summary>
        /// DateTime for the first/next update of the statistic set. 
        /// <para>The next update can be calculated from: <see cref="UpdateTime"/> + x * <see cref="UpdateInterval"/>.</para>
        /// </summary>
        public DateTime? UpdateTime;

        /// <summary>
        /// If true, statistics will be recalculated at the next update time. If false, calculation will be skipped.
        /// </summary>
        public bool RequiresRecalculation { get; set; }

        /// <summary>
        /// True if LoadRequiredDataAsync has been performed earlier on this object.
        /// </summary>
        [NotMapped]
        public bool IsDataLoaded { get; protected set; } = false;

        public StatisticSetEntity()
        {
            DriverStatistic = new List<DriverStatisticRowEntity>();
        }

        /// <summary>
        /// <para>Load all data required for calculating the statistics from the database.</para>
        /// <para>Must be called prior to <see cref="Calculate"/> if lazy loading is disabled!</para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework.</param>
        /// <param name="force">Force loading data again even if IsDataLoaded is true.</param>
        public abstract Task LoadRequiredDataAsync(LeagueDbContext dbContext, bool force = false);
        /// <summary>
        /// Calculate statistic data based on the current data set.
        /// <para>Make sure either lazy-loading is enabled on the context or run <see cref="LoadRequiredDataAsync(LeagueDbContext, bool)"/> before execution.</para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework</param>
        public abstract void Calculate(LeagueDbContext dbContext);
        /// <summary>
        /// Perform a check if recalculation is required based on the current data set.
        /// <para>Calling this function will also set the value of <see cref="StatisticSetEntity.RequiresRecalculation"/></para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework</param>
        /// <returns><see langword="true"/> if calculation is required</returns>
        public abstract Task<bool> CheckRequireRecalculationAsync(LeagueDbContext dbContext);
    }
}
