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
                throw new Exception("No typemap found.");

            var typeMap = TypeMaps.SingleOrDefault(x => x.SourceType.Equals(sourceType) && x.TargetType.Equals(targetType));

            if(typeMap == null)
                typeMap = TypeMaps.SingleOrDefault(x => x.SourceType.Equals(sourceType) && x.TargetType.Equals(targetType.BaseType));

            if (typeMap == null)
                throw new Exception("No typemap found.");

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

        public TTarget MapTo<TSource, TTarget>(TSource source, TTarget target) where TSource : MappableDTO where TTarget : MappableEntity
        {
            return MapTo(source, target, typeof(TSource), typeof(TTarget)) as TTarget;
        }

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

        private TTarget DefaultGet<TSource, TTarget>(TSource source) where TSource : MappableDTO where TTarget : MappableEntity, new()
        {
            if (source == null)
                return null;
            TTarget target;

            if (source.MappingId == null)
            {
                target = DbContext.Set<TTarget>().Create(); 
                //DbContext.SaveChanges();
            }
            else
                target = DbContext.Set<TTarget>().Find(source.Keys);

            //if (target == null)
            //    throw new EntityNotFoundException(nameof(TTarget), "Could not find Entity in Database.", source.Keys);

            return target;
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

        public ICollection<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceCollection, ICollection<TTarget> targetCollection, Func<TSource, TTarget, TTarget> map, Func<TSource, object> key, bool removeFromCollection = false, bool removeFromDatabase = false) where TSource : MappableDTO where TTarget : MappableEntity
        {
            return MapCollection(sourceCollection, targetCollection, map, x => new object[] { key(x) }, removeFromCollection, removeFromDatabase);
        }

        public ICollection<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceCollection, ICollection<TTarget> targetCollection, Func<TSource, TTarget> get, Func<TSource, object> key, bool removeFromCollection = false, bool removeFromDatabase = false) where TSource : MappableDTO where TTarget : MappableEntity
        {
            return MapCollection(sourceCollection, targetCollection, (src, trg) => get(src), key, removeFromCollection, removeFromDatabase);
        }

        public ICollection<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceCollection, ICollection<TTarget> targetCollection, Func<TSource, TTarget> get, Func<TSource, object[]> keys, bool removeFromCollection = false, bool removeFromDatabase = false) where TSource : MappableDTO where TTarget : MappableEntity
        {
            return MapCollection(sourceCollection, targetCollection, (src, trg) => get(src), keys, removeFromCollection, removeFromDatabase);
        }
        public ICollection<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> sourceCollection, ICollection<TTarget> targetCollection, Func<TSource, TTarget, TTarget> map, Func<TSource, object[]> keys, bool removeFromCollection = false, bool removeFromDatabase = false) where TSource : MappableDTO where TTarget : MappableEntity
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
                var target = map(source, null);
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
