using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.Entities.Statistics
{
    /// <summary>
    /// Statistic set with pre-calculated statistic data. This should be used to import data from other league scoring pages, such as DanLisa,
    /// to keep the data of previous for use in the current statistic calculation.
    /// </summary>
    public class ImportedStatisticSetEntity : StatisticSetEntity
    {
        /// <summary>
        /// Import source of the underlying statistic data
        /// </summary>
        public string ImportSource { get; set; }
        /// <summary>
        /// Description if necessary
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The first date that is counted in the pre-calculated statistic set
        /// </summary>
        public DateTime? FirstDate { get; set; }
        /// <summary>
        /// The last date that is counted in the pre-calculated statistic set
        /// </summary>
        public DateTime? LastDate { get; set; }

        /// <summary>
        /// Calculate statistic data based on the current data set.
        /// <para>Without function on <see cref="ImportedStatisticSetEntity"/></para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework</param>
        public override void Calculate(LeagueDbContext dbContext)
        {
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        /// <summary>
        /// Perform a check if recalculation is required based on the current data set.
        /// <para>Calling this function will also set the value of <see cref="StatisticSetEntity.RequiresRecalculation"/></para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework</param>
        /// <returns>always returns <see langword="false"/> on <see cref="ImportedStatisticSetEntity"/></returns>
        public override async Task<bool> CheckRequireRecalculationAsync(LeagueDbContext dbContext)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            return false;
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        /// <summary>
        /// <para>Load all data required for calculating the statistics from the database.</para>
        /// <para>Without function when used on <see cref="ImportedStatisticSetEntity"/></para>
        /// </summary>
        /// <param name="dbContext">Database context from EntityFramework.</param>
        /// <param name="force">Force loading data again even if IsDataLoaded is true.</param>
        public override async Task LoadRequiredDataAsync(LeagueDbContext dbContext, bool force = false)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            
        }
    }
}
