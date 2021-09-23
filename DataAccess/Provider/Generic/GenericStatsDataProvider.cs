using iRLeagueDatabase.DataAccess.Mapper;
using iRLeagueDatabase.DataTransfer.Statistics;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider.Generic
{
    public class GenericStatsDataProvider : GenericDataProviderBase<StatisticSetDTO, long[]>, IDataProvider<StatisticSetDTO, long[]>
    {
        public GenericStatsDataProvider(IProviderContext<LeagueDbContext> context) : base(context)
        {
        }

        public override StatisticSetDTO GetData(long[] key)
        {
            return GetData(new long[][] { key }).FirstOrDefault();
        }

        public override IEnumerable<StatisticSetDTO> GetData(IEnumerable<long[]> requestIds)
        {
            List<StatisticSetEntity> entities = new List<StatisticSetEntity>();
            var mapper = new DTOMapper(DbContext);
            try
            {
                if (requestIds != null)
                {
                    var requestKeys = requestIds.Select(x => x.Cast<object>().ToArray()).ToArray();
                    foreach (var keys in requestKeys)
                    {
                        var entity = DbContext.Set<StatisticSetEntity>().Find(keys);
                        //if (entity == null)
                        //throw new Exception("Entity not found in Database - Type: " + rqEntityType.Name + " || keys: { " + keys.Select(x => x.ToString()).Aggregate((x, y) => ", ") + " }");
                        if (entity != null)
                            entities.Add(entity);
                    }
                }
                else
                {
                    // if requestIds is empty all entries belonging to this league should be returned
                    // this requires the league id to be checked additionally while executing the query
                    var set = DbContext.Set<StatisticSetEntity>();
                    entities = set
                        .Where(x => x.LeagueId == DbContext.CurrentLeagueId)
                        .ToList();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while getting data from Database.", e);
            }
            List<StatisticSetDTO> resultItems = new List<StatisticSetDTO>();
            try
            {
                foreach (var entity in entities)
                {
                    // Check for league id
                    if (entity is IHasLeagueId hasLeague && DbContext.CurrentLeagueId != 0)
                    {
                        if (CheckLeague(DbContext.CurrentLeagueId, hasLeague) == false)
                        {
                            continue;
                        }
                    }

                    var dto = mapper.MapTo<StatisticSetDTO>(entity);
                    resultItems.Add(dto);
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error while mapping data.", e);
            }

            return resultItems;
        }
    }
}
