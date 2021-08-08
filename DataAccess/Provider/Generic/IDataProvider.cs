using iRLeagueDatabase.DataTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iRLeagueDatabase.DataAccess.Provider.Generic
{
    public interface IDataProvider<TKey>
    {
        object GetData(TKey key);
        IEnumerable<object> GetData(IEnumerable<TKey> keys);
    }

    public interface IDataProvider<TData, TKey> : IDataProvider<TKey>
    {
        new TData GetData(TKey key);
        new IEnumerable<TData> GetData(IEnumerable<TKey> keys);
    }
}
