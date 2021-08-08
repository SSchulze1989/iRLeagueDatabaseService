using iRLeagueDatabase.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider.Generic
{
    public abstract class GenericDataProviderBase<TData, TKey> : DataProviderBase, IDataProvider<TData, TKey>
    {
        protected GenericDataProviderBase(IProviderContext<LeagueDbContext> context) : base(context)
        {
        }

        public abstract TData GetData(TKey key);

        public abstract IEnumerable<TData> GetData(IEnumerable<TKey> keys);

        object IDataProvider<TKey>.GetData(TKey key)
        {
            return GetData(key);
        }

        IEnumerable<object> IDataProvider<TKey>.GetData(IEnumerable<TKey> keys)
        {
            return GetData(keys).Cast<object>();
        }
    }
}
