using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.Entities;
using iRLeagueDatabase.DataTransfer;
using System.Runtime.Serialization;

namespace iRLeagueDatabase.Mapper
{
    public partial class EntityMapper
    {
        private LeagueDbContext DbContext { get; }
        public string UserId { get; set; }
        public string UserName { get; set; }

        public bool ForceOldVersion { get; set; } = false;

        private IList<TypeMap> TypeMaps { get; } = new List<TypeMap>();

        public EntityMapper(LeagueDbContext dbContext)
        {
            DbContext = dbContext;
            RegisterTypeMaps();
        }

        private void RegisterTypeMaps()
        {
            RegisterBaseTypeMaps();
            RegisterMemberTypeMaps();
            RegisterResultsTypeMaps();
            RegisterReviewsTypeMaps();
            RegisterSessionsTypeMaps();
        }

        public TypeMap GetTypeMap(Type sourceType, Type targetType)
        {
            if (sourceType == null || targetType == null)
                throw new Exception("No typemap found. Mapping types are null!");

            var typeMap = TypeMaps.SingleOrDefault(x => x.SourceType.Equals(sourceType) && x.TargetType.Equals(targetType));

            if(typeMap == null)
                typeMap = TypeMaps.SingleOrDefault(x => x.SourceType.Equals(sourceType) && x.TargetType.Equals(targetType.BaseType));

            if (typeMap == null)
                throw new Exception($"No typemap found. Source: {sourceType} | Target: {targetType}");

            return typeMap;
        }

        public object MapTo(object source, Type targetType)
        {
            return MapTo(source, null, source.GetType(), targetType);
        }

        public object MapTo(object source, object target)
        {
            return MapTo(source, target, source.GetType(), target.GetType());
        }

        public TTarget MapTo<TTarget>(object source) where TTarget : MappableEntity
        {
            if (source == null)
                return null;
            TTarget target = null;
            target = MapTo(source, target, source.GetType(), typeof(TTarget)) as TTarget;

            return target;
        }

        /// <summary>
        /// Map a Source DTO to a database Entity
        /// </summary>
        /// <typeparam name="TSource">DTO type of Mapping Source</typeparam>
        /// <typeparam name="TTarget">Entity type of Mapping Target</typeparam>
        /// <param name="source">Mapping Source</param>
        /// <param name="target">Mapping Target - if Target is null the entity is retrieved from the database through the mapping Id</param>
        /// <returns></returns>
        public TTarget MapTo<TSource, TTarget>(TSource source, TTarget target) where TSource : MappableDTO where TTarget : MappableEntity
        {
            return MapTo(source, target, typeof(TSource), typeof(TTarget)) as TTarget;
        }

        /// <summary>
        /// Map a Source DTO to a database Entity
        /// </summary>
        /// <param name="source">Mapping Source</param>
        /// <param name="target">Mapping Target - if Target is null the entity is retrieved from the database through the mapping Id</param>
        /// <param name="sourceType">DTO type of Mapping Source</param>
        /// <param name="targetType">Entity type of Mapping Target</param>
        /// <returns>Mapped database Entity</returns>
        public object MapTo(object source, object target, Type sourceType, Type targetType)
        {
            if (source == null)
                return null;

            try
            {
                var typeMap = GetTypeMap(sourceType, targetType);

                if (target == null)
                    target = typeMap.Get(source) as MappableEntity;

                typeMap.MapTo(source, target);
            }
            catch
            {
                throw;
            }

            return target;
        }

        private TTarget DefaultGet<TSource, TTarget>(TSource source, ref TTarget target) where TSource : MappableDTO where TTarget : MappableEntity, new()
        {
            return target = DefaultGet<TSource, TTarget>(source);
        }

        /// <summary>
        /// Default method for getting an entity from the database as a mapping target or source.
        /// If mapping Id on the source is null, a new entity is created.
        /// </summary>
        /// <typeparam name="TSource">Source type for mapping</typeparam>
        /// <typeparam name="TTarget">Target type for mapping</typeparam>
        /// <param name="source">Mapping source that contains the Id of the mapping target</param>
        /// <returns></returns>
        private TTarget DefaultGet<TSource, TTarget>(TSource source) where TSource : MappableDTO where TTarget : MappableEntity, new()
        {
            if (source == null)
                return null;
            TTarget target;

            if (source.MappingId == null)
            {
                target = DbContext.Set<TTarget>().Create();
            }
            else
                target = DbContext.Set<TTarget>().Find(source.Keys);

            return target;
        }

        private TTarget DefaultGet<TTarget>(object[] keys) where TTarget : MappableEntity, new()
        {
            return DbContext.Set<TTarget>().Find(keys);
        }

        private bool DefaultCompare<TSource, TTarget>(TSource source, TTarget target) where TSource : MappableDTO where TTarget : MappableEntity
        {
            return source.MappingId.Equals(target.MappingId);
        }

        public void RegisterTypeMap<TSource, TTarget>(Func<TSource, TTarget, TTarget> mapFunc) where TSource : MappableDTO where TTarget : MappableEntity, new()
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

        public ICollection<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceCollection, ICollection<TTarget> targetCollection, 
            Func<TSource, TTarget, TTarget> map, Func<TSource, object> key, bool removeFromCollection = false, bool removeFromDatabase = false, bool autoAddMissing = false) 
            where TSource : MappableDTO where TTarget : MappableEntity
        {
            return MapCollection(sourceCollection, targetCollection, map, x => new object[] { key(x) }, 
                removeFromCollection: removeFromCollection, removeFromDatabase: removeFromDatabase, autoAddMissing: autoAddMissing);
        }

        public ICollection<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceCollection, ICollection<TTarget> targetCollection, 
            Func<TSource, TTarget> get, Func<TSource, object> key, bool removeFromCollection = false, bool removeFromDatabase = false, bool autoAddMissing = false) 
            where TSource : MappableDTO where TTarget : MappableEntity
        {
            return MapCollection(sourceCollection, targetCollection, (src, trg) => get(src), key, 
                removeFromCollection: removeFromCollection, removeFromDatabase: removeFromDatabase, autoAddMissing: autoAddMissing);
        }

        public ICollection<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceCollection, ICollection<TTarget> targetCollection, 
            Func<TSource, TTarget> get, Func<TSource, object[]> keys, bool removeFromCollection = false, bool removeFromDatabase = false, bool autoAddMissing = false) 
            where TSource : MappableDTO where TTarget : MappableEntity
        {
            return MapCollection(sourceCollection, targetCollection, (src, trg) => get(src), keys, 
                removeFromCollection: removeFromCollection, removeFromDatabase: removeFromDatabase, autoAddMissing: autoAddMissing);
        }
        public ICollection<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceCollection, ICollection<TTarget> targetCollection, Func<TSource, TTarget, TTarget> map, Func<TSource, object[]> keys, bool removeFromCollection = false, bool removeFromDatabase = false, bool autoAddMissing = false) where TSource : MappableDTO where TTarget : MappableEntity
        {
            //if (sourceCollection == null)
            //{
            //    targetCollection = null;
            //    return targetCollection;
            //}

            if (targetCollection == null)
                targetCollection = new List<TTarget>();

            var newTargetsList = new List<TSource>();
            var removeEntitiesList = targetCollection.ToList();

            foreach(var source in sourceCollection)
            {
                var target = targetCollection.SingleOrDefault(x => x.MappingId.Equals(source.MappingId));
                
                if (target == null)
                    newTargetsList.Add(source);
                else
                {
                    removeEntitiesList.Remove(target);
                    target = map(source, target);
                }
            }

            foreach(var source in newTargetsList)
            {
                var dbSet = DbContext.Set(typeof(TTarget));
                var target = dbSet.Find(source.Keys) as TTarget;
                
                if (target == null)
                {
                    if (autoAddMissing)
                        target = dbSet.Create() as TTarget;
                    else
                        continue;
                }

                target = map(source, target);
                targetCollection.Add(target);
            }

            if (removeFromCollection)

            {
                foreach (var target in removeEntitiesList)
                {
                    targetCollection.Remove(target);

                    if (removeFromDatabase)
                        target.Delete(DbContext);
                }
            }

            return targetCollection;
        }

        public class EntityNotFoundException : Exception
        {
            public EntityNotFoundException(string entityName, params object[] keys)
            {
                Data.Add("EntityName", entityName);
                Data.Add("Keys", keys);
            }

            public EntityNotFoundException(string entityName, string message, params object[] keys) : base(message)
            {
                Data.Add("EntityName", entityName);
                Data.Add("Keys", keys);
            }

            public EntityNotFoundException(string entityName, string message, Exception innerException, params object[] keys) : base(message, innerException)
            {
                Data.Add("EntityName", entityName);
                Data.Add("Keys", keys);
            }
        }
    }
}
