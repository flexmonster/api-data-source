using NetCoreServer.Models.DataModels;
using System.Threading.Tasks;

namespace NetCoreServer.DataStorages
{
    public interface IDataStorage
    {
        public Task<IDataStructure> GetOrAddAsync(string cacheKey);
    }
}