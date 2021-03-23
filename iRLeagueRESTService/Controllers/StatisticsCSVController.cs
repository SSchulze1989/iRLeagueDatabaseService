using iRLeagueDatabase.DataTransfer.Statistics;
using iRLeagueDatabase.DataTransfer.Statistics.Special;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Statistics;
using iRLeagueDatabase.Extensions;
using iRLeagueDatabase.Mapper;
using iRLeagueRESTService.Data;
using iRLeagueRESTService.Filters;
using log4net;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;

namespace iRLeagueRESTService.Controllers
{
    public class StatisticsCSVController : LeagueApiController
    {
        private ILog logger = log4net.LogManager.GetLogger(typeof(StatisticsCSVController));

        [HttpGet]
        [Authorize(Roles = LeagueRoles.UserOrAdmin)]
        public HttpResponseMessage Get([FromUri] string leagueName, [FromUri] string culture = null)
        {
            try
            {
                logger.Info($"Get alltime CSV for league: {leagueName}");
                CheckLeagueRole(User, leagueName);

                CultureInfo cultureInfo = null;
                if (string.IsNullOrEmpty(culture) == false)
                {
                    cultureInfo = CultureInfo.GetCultureInfo(culture);
                }
                if (culture == null)
                {
                    cultureInfo = CultureInfo.InvariantCulture;
                }

                var stream = new MemoryStream();
                var exportHelper = new CSVExportHelper()
                {
                    Delimiter = '\t',
                    UseAttributeNames = true,
                    Culture = cultureInfo
                };

                // get statistics data from data provider
                var databaseName = GetDatabaseNameFromLeagueName(leagueName);
                var csvStatisticRows = new List<StatisticRowCSV>();
                using (var dbContext = CreateDbContext(databaseName))
                {
                    // quick and dirty db access | hardcoded ids!
                    var leagueSetId = 3;
                    var heSetId = 7;
                    var mapper = new DTOMapper(dbContext);

                    dbContext.Configuration.LazyLoadingEnabled = false;

                    // preload data from db
                    var leagueSet = dbContext.Set<LeagueStatisticSetEntity>()
                        .Where(x => x.Id == leagueSetId)
                        .Include(x => x.StatisticSets)
                        .FirstOrDefault();
                    var heSet = dbContext.Set<LeagueStatisticSetEntity>()
                        .Where(x => x.Id == heSetId)
                        .Include(x => x.StatisticSets)
                        .FirstOrDefault();

                    var statisticSetIds = dbContext.Set<StatisticSetEntity>().Local.Select(x => x.Id);
                    // driver statistic rows
                    dbContext.Set<DriverStatisticRowEntity>()
                        .Where(x => statisticSetIds.Contains(x.StatisticSetId))
                        .Include(x => x.Member.Team)
                        .Load();

                    var seasonIds = leagueSet.StatisticSets
                        .Concat(heSet.StatisticSets)
                        .OfType<SeasonStatisticSetEntity>()
                        .Select(x => x.SeasonId);

                    // seasons
                    dbContext.Set<SeasonEntity>()
                        .Where(x => seasonIds.Contains(x.SeasonId))
                        .Load();

                    dbContext.ChangeTracker.DetectChanges();

                    foreach (var row in leagueSet.DriverStatistic)
                    {
                        var csvRow = new StatisticRowCSV();
                        mapper.MapToDriverStatisticRowDTO(row, csvRow);
                        var heRow = heSet.DriverStatistic.SingleOrDefault(x => x.MemberId == row.MemberId);
                        if (heRow != null)
                        {
                            csvRow.HeTitles = heRow.Titles;
                        }
                        csvRow.Name = row.Member.Fullname;
                        csvRow.IRacingId = row.Member.IRacingId;
                        csvRow.TeamName = row.Member.Team?.Name;
                        csvStatisticRows.Add(csvRow);
                    }

                    // check for last champions
                    var lastChamp = leagueSet.StatisticSets
                        .OfType<SeasonStatisticSetEntity>()
                        .Where(x => x.IsSeasonFinished)
                        .OrderBy(x => x.Season.SeasonEnd)
                        .LastOrDefault()?.DriverStatistic
                        .Where(x => x.CurrentSeasonPosition > 0)
                        .OrderBy(x => x.CurrentSeasonPosition)
                        .FirstOrDefault()?.Member;
                    var lastHeChamp = heSet.StatisticSets
                        .OfType<SeasonStatisticSetEntity>()
                        .Where(x => x.IsSeasonFinished)
                        .OrderBy(x => x.Season.SeasonEnd)
                        .LastOrDefault()?.DriverStatistic
                        .Where(x => x.CurrentSeasonPosition > 0)
                        .OrderBy(x => x.CurrentSeasonPosition)
                        .FirstOrDefault()?.Member;

                    if (csvStatisticRows.Any(x => x.MemberId == lastChamp.MemberId))
                    {
                        csvStatisticRows.Single(x => x.MemberId == lastChamp.MemberId).IsCurrentChamp = true;
                    }
                    if (csvStatisticRows.Any(x => x.MemberId == lastHeChamp.MemberId))
                    {
                        csvStatisticRows.Single(x => x.MemberId == lastHeChamp.MemberId).IsCurrentHeChamp = true;
                    }
                }

                foreach (var row in csvStatisticRows)
                {
                    // calculate driver rating
                    var rating = CalcDriverRank(row);
                    row.DriverRank = rating.ToString();
                    row.RankValue = (int)rating.GetValueOrDefault();

                    // calculate fair play rating
                    row.FairPlayRating = CalculateFairplayRating(row);                        
                }

                exportHelper.WriteToStream(stream, csvStatisticRows);

                string filename = "AllTimeStats.csv";
                var result = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                stream.Position = 0;
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = filename;
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentLength = stream.Length;

                logger.Info($"Send data - ${filename}");
                return result;
            }
            catch (Exception e)
            {
                logger.Error("Error in get Statistic CSV", e);
                throw e;
            }
        }

        private DriverRank? CalcDriverRank(StatisticRowCSV row)
        {
            if (row.Races == 0)
                return null;

            DriverRank? rank = null;

            if (row.IsCurrentChamp)
            {
                rank = DriverRank.Meister;
            }
            else if (row.IsCurrentHeChamp)
            {
                rank = DriverRank.EisenMeister;
            }
            else if (row.RacesCompleted >= 30 && row.Titles >= 1)
            {
                rank = DriverRank.Platin;
            }
            else if (row.HeTitles >= 1)
            {
                rank = DriverRank.Eisen;
            }
            else if (row.RacesCompleted >= 30 && row.Wins >= 5)
            {
                rank = DriverRank.GoldPlus;
            }
            else if (row.RacesCompleted >= 20 && row.Wins >= 1)
            {
                rank = DriverRank.Gold;
            }
            else if (row.RacesCompleted >= 20 && row.Top3 >= 5)
            {
                rank = DriverRank.SilberPlus;
            }
            else if (row.RacesCompleted >= 10 && (row.Top3 >= 1 || row.Top10 >= 10))
            {
                rank = DriverRank.Silber;
            }
            else if (row.RacesCompleted >= 10 && row.Top10 >= 5)
            {
                rank = DriverRank.BronzePlus;
            }
            else if (row.RacesCompleted >= 5 && (row.Top10 >= 1 || row.RacesCompleted >= 25))
            {
                rank = DriverRank.Bronze;
            }

            return rank;
        }

        private double CalculateFairplayRating(StatisticRowCSV row)
        {
            return (row.RacesCompleted > 6) ? (double)Math.Round(((row.PenaltyPoints + (double)row.Incidents / 15) / row.RacesCompleted), 4) : 100;
        }

        private enum DriverRank
        {
            [Description("")]
            None = 0,
            Bronze = 1,
            BronzePlus = 2,
            Silber = 3,
            SilberPlus = 4,
            Gold = 5,
            GoldPlus = 6,
            Eisen = 7,
            Platin = 9,
            EisenMeister = 8,
            Meister = 10
        }
    }
}