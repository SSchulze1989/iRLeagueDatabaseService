using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Mapper
{
    public abstract class TypeMap
    {
        public abstract Type SourceType { get; }
        public abstract Type TargetType { get; }

        public abstract object Get(object source);
        public abstract object MapTo(object source, object target);
        public abstract bool Compare(object source, object target);
    }

    public class TypeCastException : Exception
    {
        public TypeCastException(Type type1) : base("Error casting object to " + type1.Name + ".")
        {
            Data.Add("Type1", type1);
        }

        public TypeCastException(Type type1, string message) : base(message)
        {
            Data.Add("Type1", type1);
        }

        public TypeCastException(Type type1, Exception innerException) : base("Error casting object to ", innerException)
        {
            Data.Add("Type1", type1);
        }

        public TypeCastException(Type type1, string message, Exception innerException) : base(message, innerException)
        {
            Data.Add("Type1", type1);
        }
    }

    public class TypeMapException<TSource, TTarget> : Exception
    {
        public Type SourceType { get; }
        public Type TargetType { get; }

        public TypeMapException() : this(null)
        {
        }

        public TypeMapException(Exception innerException) : this("Error mapping types " + typeof(TSource).Name + " to " + typeof(TTarget).Name + ".", innerException)
        {
        }

        public TypeMapException(string message, Exception innerException) : base(message, innerException)
        {
            SourceType = typeof(TSource);
            TargetType = typeof(TTarget);
            Data.Add("Source Type", typeof(TSource));
            Data.Add("Target Type", typeof(TTarget));
        }
    }

    public class TypeMap<TSource, TTarget> : TypeMap
    {
        public override Type SourceType => typeof(TSource);

        public override Type TargetType => typeof(TTarget);

        private Func<TSource, TTarget> GetFunc { get; }
        private Func<TSource, TTarget, TTarget> MapFunc { get; }
        private Func<TSource, TTarget, bool> CompFunc { get; }
        private List<TypeMap> IncludeDerived { get; } = new List<TypeMap>();

        public TypeMap(Func<TSource, TTarget> getFunc, Func<TSource, TTarget, TTarget> mapFunc, Func<TSource, TTarget, bool> compareFunc)
        {
            GetFunc = getFunc;
            MapFunc = mapFunc;
            CompFunc = compareFunc;
        }

        public TypeMap(Func<TSource, TTarget> getFunc, Func<TSource, TTarget, TTarget> mapFunc, Func<TSource, TTarget, bool> compareFunc, IEnumerable<TypeMap> includeDerived) : this(getFunc, mapFunc, compareFunc)
        {
            IncludeDerived = includeDerived.ToList();
        }

        private T CastTo<T>(object item)
        {
            if (item is T castItem)
                return castItem;

            throw new TypeCastException(typeof(T));
        }

        public override object Get(object source)
        {
            return GetFunc(CastTo<TSource>(source));
        }

        public override object MapTo(object source, object target)
        {
            try
            {
                return MapFunc(CastTo<TSource>(source), CastTo<TTarget>(target));
            }
            catch (Exception e)
            {
                throw new TypeMapException<TSource, TTarget>(innerException: e);
            }
        }

        public override bool Compare(object source, object target)
        {
            try
            {
                return CompFunc(CastTo<TSource>(source), CastTo<TTarget>(target));
            }
            catch (Exception e)
            {
                throw new TypeMapException<TSource, TTarget>(innerException: e);
            }
        }
    }
}
