using Microsoft.Extensions.Options;
using NetCoreServer.DataStorages;
using System.Threading.Tasks;

namespace NetCoreServer
{
    public class PrepopulatingCacheService : IPrepopulatingService
    {
        private IDataStorage _dataStorage;
        private DatasourceOptions _options;

        /// <summary>
        /// Prepopulate cache with data from DatasourceOptions
        /// </summary>
        /// <param name="dataStorage">Storage where data will be loaded</param>
        /// <param name="options">Data source options</param>
        public PrepopulatingCacheService(IDataStorage dataStorage, IOptions<DatasourceOptions> options)
        {
            _dataStorage = dataStorage;
            _options = options.Value;
        }

        public async Task Prepopulate()
        {
            foreach (var index in _options.Indexes.Keys)
            {
                await _dataStorage.GetOrAddAsync(index);
            }
        }
    }
}