using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Sessions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
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

        [ForeignKey(nameof(League))]
        public long LeagueId { get; set; }
        public virtual LeagueEntity League { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// List of <see cref="DriverStatisticRowEntity"/> holding the calculated statistic data for each driver that had an appearance in the related data.
        /// <para>Can be empty if the implemented statistic set does not have any driver statistics.</para>
        /// </summary>
        [InverseProperty(nameof(DriverStatisticRowEntity.StatisticSet))]
        public virtual List<DriverStatisticRowEntity> DriverStatistic { get; set; }

        /// <summary>
        /// Time between two database updates for the statistics set
        /// </summary>
        public long UpdateInterval { get; set; }
        /// <summary>
        /// DateTime for the first/next update of the statistic set. 
        /// <para>The next update can be calculated from: <see cref="UpdateTime"/> + x * <see cref="UpdateInterval"/>.</para>
        /// </summary>
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// If true, statistics will be recalculated at the next update time. If false, calculation will be skipped.
        /// </summary>
        public virtual bool RequiresRecalculation { get; set; }

        public override object MappingId => Id;

        /// <summary>
        /// True if LoadRequiredDataAsync has been performed earlier on this object.
        /// </summary>
        [NotMapped]
        public bool IsDataLoaded { get; protected set; } = false;

        [NotMapped]
        public DateTime? StartDate => DriverStatistic.Min(x => x.FirstRaceDate);
        [NotMapped]
        public DateTime? EndDate => DriverStatistic.Max(x => x.LastRaceDate);

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
        public virtual async Task LoadRequiredDataAsync(LeagueDbContext dbContext, bool force = false)
        {
            if (DriverStatistic == null || DriverStatistic.Count == 0)
            {
                await dbContext.Entry(this)
                    .Collection(x => x.DriverStatistic)
                    .LoadAsync();

                var memberIds = DriverStatistic.Select(x => x.MemberId);
                var sessionIds = DriverStatistic
                    .SelectMany(x => new long?[] { x.FirstSessionId, x.LastSessionId, x.FirstRaceId, x.LastRaceId })
                    .Where(x => x != null)
                    .Select(x => x.Value);
                var resultRowIds = DriverStatistic
                    .SelectMany(x => new long?[] { x.FirstResultRowId, x.LastResultRowId })
                    .Where(x => x != null)
                    .Select(x => x.Value);

                await dbContext.Set<LeagueMemberEntity>()
                    .Where(x => memberIds.Contains(x.MemberId))
                    .LoadAsync();
                await dbContext.Set<SessionBaseEntity>()
                    .Where(x => sessionIds.Contains(x.SessionId))
                    .LoadAsync();
                await dbContext.Set<ResultRowEntity>()
                    .Where(x => resultRowIds.Contains(x.ResultRowId))
                    .LoadAsync();

                dbContext.ChangeTracker.DetectChanges();
            }
        }
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
#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public virtual async Task<bool> CheckRequireRecalculationAsync(LeagueDbContext dbContext)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            return RequiresRecalculation;
        }

        public override long GetLeagueId()
        {
            return LeagueId;
        }
    }
}
