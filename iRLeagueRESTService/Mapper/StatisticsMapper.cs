// MIT License

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

using iRLeagueDatabase.DataTransfer.Results;
using iRLeagueDatabase.DataTransfer.Statistics;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Results;
using iRLeagueDatabase.Entities.Statistics;
using iRLeagueDatabase.Extensions;
using iRLeagueManager.Timing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        public void RegisterStatisticsTypeMaps()
        {
            RegisterTypeMap<StatisticSetEntity, StatisticSetDTO>(MapToStatisticSetDTO);
            RegisterTypeMap<SeasonStatisticSetEntity, SeasonStatisticSetDTO>(MapToSeasonStatisticSetDTO);
            RegisterTypeMap<SeasonStatisticSetEntity, StatisticSetDTO>(src => new SeasonStatisticSetDTO(), (src, trg) => MapToSeasonStatisticSetDTO(src, (SeasonStatisticSetDTO)trg), DefaultCompare);
            RegisterTypeMap<LeagueStatisticSetEntity, LeagueStatisticSetDTO>(MapToLeagueStatisticSetDTO);
            RegisterTypeMap<LeagueStatisticSetEntity, StatisticSetDTO>(src => new LeagueStatisticSetDTO(), (src, trg) => MapToLeagueStatisticSetDTO(src, (LeagueStatisticSetDTO)trg), DefaultCompare);
            RegisterTypeMap<ImportedStatisticSetEntity, ImportedStatisticSetDTO>(MapToImportedStatisticSetDTO);
            RegisterTypeMap<ImportedStatisticSetEntity, StatisticSetDTO>(src => new ImportedStatisticSetDTO(), (src, trg) => MapToImportedStatisticSetDTO(src, (ImportedStatisticSetDTO)trg), DefaultCompare);
            RegisterTypeMap<StatisticSetEntity, DriverStatisticDTO>(MapToDriverStatisticDTO);
            RegisterTypeMap<SeasonStatisticSetEntity, DriverStatisticDTO>(MapToDriverStatisticDTO);
            RegisterTypeMap<LeagueStatisticSetEntity, DriverStatisticDTO>(MapToDriverStatisticDTO);
            RegisterTypeMap<ImportedStatisticSetEntity, DriverStatisticDTO>(MapToDriverStatisticDTO);
        }

        public StatisticSetDTO MapToStatisticSetDTO(StatisticSetEntity source, StatisticSetDTO target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new StatisticSetDTO();
            }

            MapToVersionDTO(source, target);
            target.Id = source.Id;
            target.Name = source.Name;
            target.UpdateInterval = TimeSpanConverter.Convert(source.UpdateInterval);
            target.UpdateTime = source.UpdateTime.GetValueOrDefault();

            return target;
        }

        public SeasonStatisticSetDTO MapToSeasonStatisticSetDTO(SeasonStatisticSetEntity source, SeasonStatisticSetDTO target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new SeasonStatisticSetDTO();
            }

            MapToStatisticSetDTO(source, target);

            target.ScoringTableId = source.ScoringTableId;
            target.SeasonId = source.SeasonId;

            return target;
        }

        public LeagueStatisticSetDTO MapToLeagueStatisticSetDTO(LeagueStatisticSetEntity source, LeagueStatisticSetDTO target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new LeagueStatisticSetDTO();
            }

            MapToStatisticSetDTO(source, target);

            target.SeasonStatisticSetIds = source.StatisticSets.Select(x => x.Id).ToArray();

            return target;
        }

        public ImportedStatisticSetDTO MapToImportedStatisticSetDTO(ImportedStatisticSetEntity source, ImportedStatisticSetDTO target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new ImportedStatisticSetDTO();
            }

            MapToStatisticSetDTO(source, target);
            target.Description = source.Description;
            target.ImportSource = source.ImportSource;
            target.FirstDate = source.FirstDate.GetValueOrDefault();
            target.LastDate = source.LastDate.GetValueOrDefault();

            return target;
        }

        public DriverStatisticDTO MapToDriverStatisticDTO(StatisticSetEntity source, DriverStatisticDTO target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new DriverStatisticDTO();
            }

            target.StatisticSetId = source.Id;
            target.DriverStatisticRows = source.DriverStatistic.Select(x => MapToDriverStatisticRowDTO(x)).ToArray();

            return target;
        }

        public DriverStatisticRowDTO MapToDriverStatisticRowDTO(DriverStatisticRowEntity source, DriverStatisticRowDTO target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = new DriverStatisticRowDTO();
            }

            target.AvgFinalPosition = source.AvgFinalPosition;
            target.AvgFinishPosition = source.AvgFinishPosition;
            target.AvgIncidentsPerKm = source.AvgIncidentsPerKm;
            target.AvgIncidentsPerLap = source.AvgIncidentsPerLap;
            target.AvgIncidentsPerRace = source.AvgIncidentsPerRace;
            target.AvgIRating = source.AvgIRating;
            target.AvgPenaltyPointsPerKm = source.AvgPenaltyPointsPerKm;
            target.AvgPenaltyPointsPerLap = source.AvgPenaltyPointsPerLap;
            target.AvgPenaltyPointsPerRace = source.AvgPenaltyPointsPerRace;
            target.AvgPointsPerRace = source.AvgPointsPerRace;
            target.AvgSRating = source.AvgSRating;
            target.AvgStartPosition = source.AvgStartPosition;
            target.RacesCompletedPct = ((double)source.RacesCompleted / source.Races).GetZeroWhenInvalid();
            target.BestFinalPosition = source.BestFinalPosition;
            target.BestFinishPosition = (int)source.BestFinishPosition;
            target.BestStartPosition = (int)source.BestStartPosition;
            target.BonusPoints = (int)source.BonusPoints;
            target.CompletedLaps = (int)source.CompletedLaps;
            target.CurrentSeasonPosition = source.CurrentSeasonPosition;
            target.DrivenKm = source.DrivenKm;
            target.EndIRating = source.EndIRating;
            target.EndSRating = source.EndSRating;
            target.FastestLaps = source.FastestLaps;
            target.FirstRaceFinalPosition = source.FirstRaceFinalPosition;
            target.FirstRaceFinishPosition = (int)source.FirstRaceFinishPosition;
            target.FirstRaceId = source.FirstRaceId;
            target.FirstRaceDate = source.FirstRaceDate;
            target.FirstRaceStartPosition = (int)source.FirstRaceStartPosition;
            target.FirstResultRowId = source.FirstResultRowId;
            target.FirstSessionId = source.FirstSessionId;
            target.FirstSessionDate = source.FirstSessionDate;
            target.Incidents = (int)source.Incidents;
            target.IncidentsUnderInvestigation = source.IncidentsUnderInvestigation;
            target.IncidentsWithPenalty = source.IncidentsWithPenalty;
            target.LastRaceFinalPosition = source.LastRaceFinalPosition;
            target.LastRaceFinishPosition = (int)source.LastRaceFinishPosition;
            target.LastRaceId = source.LastRaceId;
            target.LastRaceDate = source.LastRaceDate;
            target.LastRaceStartPosition = (int)source.LastRaceStartPosition;
            target.LastResultRowId = source.LastResultRowId;
            target.LastSessionId = source.LastSessionId;
            target.LastSessionDate = source.LastSessionDate;
            target.LeadingKm = source.LeadingKm;
            target.LeadingLaps = (int)source.LeadingLaps;
            target.MemberId = source.MemberId;
            target.PenaltyPoints = (int)source.PenaltyPoints;
            target.Poles = source.Poles;
            target.RacePoints = (int)source.RacePoints;
            target.Races = source.Races;
            target.RacesCompleted = source.RacesCompleted;
            target.RacesInPoints = source.RacesInPoints;
            target.StartIRating = source.StartIRating;
            target.StartSRating = source.StartSRating;
            target.StatisticSetId = source.StatisticSetId;
            target.Top10 = source.Top10;
            target.Top15 = source.Top15;
            target.Top20 = source.Top20;
            target.Top25 = source.Top25;
            target.Top3 = source.Top3;
            target.Top5 = source.Top5;
            target.TotalPoints = (int)source.TotalPoints;
            target.Wins = source.Wins;
            target.WorstFinalPosition = source.WorstFinalPosition;
            target.WorstFinishPosition = (int)source.WorstFinishPosition;
            target.WorstStartPosition = (int)source.WorstStartPosition;
            target.Titles = source.Titles;
            target.HardChargerAwards = source.HardChargerAwards;
            target.CleanestDriverAwards = source.CleanestDriverAwards;

            return target;
        }
    }


    public partial class EntityMapper
    {
        public void RegisterStatisticsTypeMaps()
        {
            //RegisterTypeMap<StatisticSetDTO, StatisticSetEntity>((src, trg) => MapToStatisticSetEntity(src, trg));
            RegisterTypeMap<StatisticSetDTO, StatisticSetEntity>(src =>
            {
                if (src is SeasonStatisticSetDTO)
                {
                    return DefaultGet<SeasonStatisticSetEntity>(src);
                }
                else if (src is LeagueStatisticSetDTO)
                {
                    return DefaultGet<LeagueStatisticSetEntity>(src);
                }
                else if (src is ImportedStatisticSetDTO)
                {
                    return DefaultGet<ImportedStatisticSetEntity>(src);
                }
                else
                {
                    return DefaultGet<StatisticSetEntity>(src);
                }
            }, (src, trg) =>
            {
                if (src is SeasonStatisticSetDTO seasonStatisticSet)
                {
                    return MapToSeasonStatisticSetEntity(seasonStatisticSet, (SeasonStatisticSetEntity)trg);
                }
                else if (src is LeagueStatisticSetDTO leagueStatisticSet)
                {
                    return MapToLeagueStatisticSetEntity(leagueStatisticSet, (LeagueStatisticSetEntity)trg);
                }
                else if (src is ImportedStatisticSetDTO importedStatisticSet)
                {
                    return MapToImportedStatisticSetEntity(importedStatisticSet, (ImportedStatisticSetEntity)trg);
                }
                else
                {
                    return MapToStatisticSetEntity(src, trg);
                }
            }, DefaultCompare);
            RegisterTypeMap<SeasonStatisticSetDTO, SeasonStatisticSetEntity>(MapToSeasonStatisticSetEntity);
            RegisterTypeMap<LeagueStatisticSetDTO, LeagueStatisticSetEntity>(MapToLeagueStatisticSetEntity);
            RegisterTypeMap<ImportedStatisticSetDTO, ImportedStatisticSetEntity>(MapToImportedStatisticSetEntity);
            RegisterTypeMap<DriverStatisticDTO, ImportedStatisticSetEntity>(MapToImportedStatisticSetEntity);
            RegisterTypeMap<DriverStatisticDTO, StatisticSetEntity>(src => DefaultGet<ImportedStatisticSetEntity>(src), (src, trg) => MapToImportedStatisticSetEntity(src, (ImportedStatisticSetEntity)trg), DefaultCompare);
        }

        public StatisticSetEntity MapToStatisticSetEntity(StatisticSetDTO source, StatisticSetEntity target = null, bool force = false)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (MapToRevision(source, target) == false && force == false)
            {
                return target;
            }

            target.Name = source.Name;
            target.UpdateInterval = TimeSpanConverter.Convert(source.UpdateInterval);
            target.UpdateTime = source.UpdateTime;
            target.RequiresRecalculation = true;

            return target;
        }

        public SeasonStatisticSetEntity MapToSeasonStatisticSetEntity(SeasonStatisticSetDTO source, SeasonStatisticSetEntity target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = DefaultGet<SeasonStatisticSetEntity>(source);
            }

            if (MapToRevision(source, target) == false)
            {
                return target;
            }

            MapToStatisticSetEntity(source, target, force: true);
            target.ScoringTable = DefaultGet<ScoringTableEntity>(new object[] { source.ScoringTableId });
            target.Season = DefaultGet<SeasonEntity>(new object[] { source.SeasonId });

            return target;
        }

        public LeagueStatisticSetEntity MapToLeagueStatisticSetEntity(LeagueStatisticSetDTO source, LeagueStatisticSetEntity target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = DefaultGet<LeagueStatisticSetEntity>(source);
            }

            if (MapToRevision(source, target) == false)
            {
                return target;
            }

            MapToStatisticSetEntity(source, target, force: true);
            MapCollection(source.SeasonStatisticSetIds.Select(x => new StatisticSetDTO() { Id = x }), target.StatisticSets, DefaultGet<StatisticSetDTO, StatisticSetEntity>, x => x.Keys,
                removeFromCollection: true);

            return target;
        }

        public ImportedStatisticSetEntity MapToImportedStatisticSetEntity(ImportedStatisticSetDTO source, ImportedStatisticSetEntity target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = DefaultGet<ImportedStatisticSetEntity>(source);
            }

            if (MapToRevision(source, target) == false)
            {
                return target;
            }

            MapToStatisticSetEntity(source, target, force: true);
            target.Description = source.Description;
            target.FirstDate = source.FirstDate;
            target.LastDate = source.LastDate;
            target.ImportSource = source.ImportSource;

            return target;
        }

        public ImportedStatisticSetEntity MapToImportedStatisticSetEntity(DriverStatisticDTO source, ImportedStatisticSetEntity target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = DefaultGet<ImportedStatisticSetEntity>(new object[] { source.StatisticSetId });
            }

            MapCollection(source.DriverStatisticRows, target.DriverStatistic, MapToDriverStatisticRowEntity, x => x.Keys, 
                removeFromCollection: true, removeFromDatabase: true, autoAddMissing: true);
            target.FirstDate = target.DriverStatistic.Min(x => x.FirstSessionDate);
            target.LastDate = target.DriverStatistic.Max(x => x.LastSessionDate);

            return target;
        }

        public DriverStatisticRowEntity MapToDriverStatisticRowEntity(DriverStatisticRowDTO source, DriverStatisticRowEntity target = null)
        {
            if (source == null)
            {
                return null;
            }
            if (target == null)
            {
                target = DefaultGet<DriverStatisticRowEntity>(source);
            }

            target.AvgFinalPosition = source.AvgFinalPosition;
            target.AvgFinishPosition = source.AvgFinishPosition;
            target.AvgIncidentsPerKm = source.AvgIncidentsPerKm;
            target.AvgIncidentsPerLap = source.AvgIncidentsPerLap;
            target.AvgIncidentsPerRace = source.AvgIncidentsPerRace;
            target.AvgIRating = source.AvgIRating;
            target.AvgPenaltyPointsPerKm = source.AvgPenaltyPointsPerKm;
            target.AvgPenaltyPointsPerLap = source.AvgPenaltyPointsPerLap;
            target.AvgPenaltyPointsPerRace = source.AvgPenaltyPointsPerRace;
            target.AvgPointsPerRace = source.AvgPointsPerRace;
            target.AvgSRating = source.AvgSRating;
            target.AvgStartPosition = source.AvgStartPosition;
            target.BestFinalPosition = source.BestFinalPosition;
            target.BestFinishPosition = source.BestFinishPosition;
            target.BestStartPosition = source.BestStartPosition;
            target.BonusPoints = source.BonusPoints;
            target.CompletedLaps = source.CompletedLaps;
            target.CurrentSeasonPosition = source.CurrentSeasonPosition;
            target.DrivenKm = source.DrivenKm;
            target.EndIRating = source.EndIRating;
            target.EndSRating = source.EndSRating;
            target.FastestLaps = source.FastestLaps;
            target.FirstRaceFinalPosition = source.FirstRaceFinalPosition;
            target.FirstRaceFinishPosition = source.FirstRaceFinishPosition;
            target.FirstRaceId = source.FirstRaceId;
            target.FirstRaceDate = source.FirstRaceDate;
            target.FirstRaceStartPosition = source.FirstRaceStartPosition;
            target.FirstResultRowId = source.FirstResultRowId;
            target.FirstSessionId = source.FirstSessionId;
            target.FirstSessionDate = source.FirstSessionDate;
            target.Incidents = source.Incidents;
            target.IncidentsUnderInvestigation = source.IncidentsUnderInvestigation;
            target.IncidentsWithPenalty = source.IncidentsWithPenalty;
            target.LastRaceFinalPosition = source.LastRaceFinalPosition;
            target.LastRaceFinishPosition = source.LastRaceFinishPosition;
            target.LastRaceId = source.LastRaceId;
            target.LastRaceDate = source.LastRaceDate;
            target.LastRaceStartPosition = source.LastRaceStartPosition;
            target.LastResultRowId = source.LastResultRowId;
            target.LastSessionId = source.LastSessionId;
            target.LastSessionDate = source.LastSessionDate;
            target.LeadingKm = source.LeadingKm;
            target.LeadingLaps = source.LeadingLaps;
            target.MemberId = source.MemberId;
            target.PenaltyPoints = source.PenaltyPoints;
            target.Poles = source.Poles;
            target.RacePoints = source.RacePoints;
            target.Races = source.Races;
            target.RacesCompleted = source.RacesCompleted;
            target.RacesInPoints = source.RacesInPoints;
            target.StartIRating = source.StartIRating;
            target.StartSRating = source.StartSRating;
            target.StatisticSetId = source.StatisticSetId;
            target.Top10 = source.Top10;
            target.Top15 = source.Top15;
            target.Top20 = source.Top20;
            target.Top25 = source.Top25;
            target.Top3 = source.Top3;
            target.Top5 = source.Top5;
            target.TotalPoints = source.TotalPoints;
            target.Wins = source.Wins;
            target.WorstFinalPosition = source.WorstFinalPosition;
            target.WorstFinishPosition = source.WorstFinishPosition;
            target.WorstStartPosition = source.WorstStartPosition;
            target.Titles = source.Titles;
            target.HardChargerAwards = source.HardChargerAwards;
            target.CleanestDriverAwards = source.CleanestDriverAwards;

            return target;
        }
    }
}
