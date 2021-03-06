﻿// MIT License

// Copyright (c) 2020 Simon Schulze

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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

        public override bool RequiresRecalculation { get => false; set => base.RequiresRecalculation = value; }

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
            RequiresRecalculation = false;
            return RequiresRecalculation;
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
            if (IsDataLoaded && force == false)
            {
                return;
            }
            await base.LoadRequiredDataAsync(dbContext, force);
        }
    }
}
