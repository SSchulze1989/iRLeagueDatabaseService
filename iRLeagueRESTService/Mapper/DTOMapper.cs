using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueManager;
using iRLeagueDatabase.Entities;
using iRLeagueDatabase.Entities.Members;
using iRLeagueDatabase.Entities.Sessions;
using iRLeagueDatabase.DataTransfer;
using iRLeagueDatabase.DataTransfer.Members;
using iRLeagueDatabase.DataTransfer.Sessions;
using iRLeagueManager.Enums;

using iRLeagueUserDatabase;

namespace iRLeagueDatabase.Mapper
{
    public partial class DTOMapper
    {
        private UsersDbContext UserDbContext { get; }

        private LeagueDbContext LeagueDbContext { get; }

        private IList<TypeMap> TypeMaps { get; } = new List<TypeMap>();

        public IEnumerable<TypeMap> GetTypeMaps() => TypeMaps;

        public DTOMapper(LeagueDbContext leagueDbContext)
        {
            LeagueDbContext = leagueDbContext;
            RegisterTypeMaps();
        }

        public void RegisterTypeMaps()
        {
            RegisterBaseTypeMaps();
            RegisterMemberTypeMaps();
            RegisterResultsTypeMaps();
            RegisterReviewsTypeMaps();
            RegisterSessionsTypeMaps();
            RegisterFiltersTypeMaps();
            RegisterStatisticsTypeMaps();
        }

        public TTarget MapTo<TTarget>(MappableEntity source) where TTarget : MappableDTO, new()
        {
            return MapTo(source, targetType: typeof(TTarget)) as TTarget;
        }
        public TTarget MapTo<TTarget, TSource>(TSource source, TTarget target) where TTarget : MappableDTO where TSource : MappableEntity
        {
            if (source == null)
                throw new ArgumentNullException(nameof(target), "Error mapping " + typeof(TSource).Name + " to " + typeof(TTarget).Name + ". Source was NULL.");
            if (target == null)
                throw new ArgumentNullException(nameof(target), "Error mapping " + typeof(TSource).Name + " to " + typeof(TTarget).Name + ". Target was NULL.");

            var targetType = target.GetType();
            var sourceType = source.GetType();

            return MapTo(source, target, sourceType, targetType) as TTarget;
        }

        private TypeMap GetTypeMap(Type sourceType, Type targetType)
        {
            if (sourceType == null || targetType == null)
                throw new Exception("No typemap found. Type was null.");

            var typeMap = TypeMaps.SingleOrDefault(x => x.SourceType.Equals(sourceType) && x.TargetType.Equals(targetType));

            if (typeMap == null)
                typeMap = TypeMaps.SingleOrDefault(x => x.SourceType.Equals(sourceType.BaseType) && x.TargetType.Equals(targetType));

            if (typeMap == null)
                throw new Exception("No typemap found. SourceType: " + sourceType.Name + " - TargetType: " + targetType.Name);

            return typeMap;
        }

        private TTarget DefaultGet<TSource, TTarget>(TSource source = null) where TSource : class where TTarget : MappableDTO, new()
        {
            if (source == null)
                return null;
            TTarget target = new TTarget();

            return target;
        }

        private bool DefaultCompare<TSource, TTarget>(TSource source, TTarget target) where TSource : MappableEntity where TTarget : MappableDTO
        {
            return source.MappingId.Equals(target.MappingId);
        }

        public void RegisterTypeMap<TSource, TTarget>(Func<TSource, TTarget, TTarget> mapFunc) where TSource : MappableEntity where TTarget : MappableDTO, new()
        {
            var typeMap = new TypeMap<TSource, TTarget>(DefaultGet<TSource, TTarget>, mapFunc, DefaultCompare);
            RegisterTypeMap(typeMap);
        }

        public void RegisterTypeMap<TSource, TTarget>(Func<TSource, TTarget> getFunc, Func<TSource, TTarget, TTarget> mapFunc, Func<TSource, TTarget, bool> compareFunc)
        {
            RegisterTypeMap(new TypeMap<TSource, TTarget>(getFunc, mapFunc, compareFunc));
        }

        public void RegisterTypeMap<TSource, TTarget>(TypeMap<TSource, TTarget> typeMap)
        {
            if (TypeMaps.Any(x => x.SourceType.Equals(typeMap.SourceType) && x.TargetType.Equals(typeMap.TargetType)))
                throw new InvalidOperationException("Can not add typemap. Already defined a typemap configuration for the given Types\nType1: " + typeMap.SourceType.Name + " - Type2: " + typeMap.TargetType.Name + ".");

            TypeMaps.Add(typeMap);
        }

        public object MapTo(object source, Type targetType)
        {
            return MapTo(source, null, source?.GetType(), targetType);
        }

        public object MapTo(object source, object target, Type sourceType, Type targetType)
        {
            if (source == null)
            {
                return null;
            }

            try
            {
                var typeMap = GetTypeMap(sourceType, targetType);

                if (target == null)
                    target = typeMap.Get(source) as MappableDTO;

                typeMap.MapTo(source, target);
            }
            catch
            {
                throw;
            }

            return target;
        }
    }
}
