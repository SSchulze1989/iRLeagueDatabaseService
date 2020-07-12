using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iRLeagueDatabase.DataTransfer;

namespace iRLeagueRESTService.Data
{
    public interface IModelDataProvider<TDataTransferObject, TModelId> : IDisposable
    {
        TDataTransferObject Get(Type requestType, TModelId requestId);
        TDataTransferObject[] GetArray(Type requestType, TModelId[] requestIds);
        TDataTransferObject Post(Type requestType, TDataTransferObject data);
        TDataTransferObject[] PostArray(Type requestType, TDataTransferObject[] data);
        TDataTransferObject Put(Type requestType, TDataTransferObject data);
        TDataTransferObject[] PutArray(Type requestType, TDataTransferObject[] data);
        bool Delete(Type requestType, TModelId requestId);
        bool DeleteArray(Type requestType, TModelId[] requestIds);
    }

    public interface IModelDataProvider : IModelDataProvider<MappableDTO, long[]>
    {

    }
}
