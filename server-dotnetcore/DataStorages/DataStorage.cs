using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NetCoreServer.DataLoaders;
using NetCoreServer.Models.DataModels;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NetCoreServer.DataStorages
{
    public class DataStorage : IDataStorage
    {
        private readonly IMemoryCache _memoryCache;
        private readonly DatasourceOptions _datasourceOptions;
        private readonly DataStorageOptions _dataStorageOptions;
        private readonly ConcurrentDictionary<object, SemaphoreSlim> _locks = new ConcurrentDictionary<object, SemaphoreSlim>();

        public DataStorage(IOptions<DatasourceOptions> dataSourceOptions, IOptions<DataStorageOptions> dataStorageOptions)
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _datasourceOptions = dataSourceOptions.Value;
            _dataStorageOptions = dataStorageOptions.Value;
        }
        /// <summary>
        /// Get data from cache or add it if key doesn't present
        /// </summary>
        /// <param name="cacheKey">key</param>
        /// <returns>Loaded data</returns>
        public async Task<IDataStructure> GetOrAddAsync(string cacheKey)
        {
            if (!_memoryCache.TryGetValue(cacheKey, out IDataStructure dataStructure))
            {
                SemaphoreSlim certLock = _locks.GetOrAdd(cacheKey, k => new SemaphoreSlim(1, 1));
                await certLock.WaitAsync();

                try
                {
                    if (!_memoryCache.TryGetValue(cacheKey, out dataStructure))
                    {
                        using (var parserFactory = new ParserFactory(_datasourceOptions.Indexes[cacheKey]))
                        {
                            var parser = parserFactory.CreateParser(cacheKey);
                            var dataLoader = new DataLoader(parser);
                            dataStructure = dataLoader.Load();
                        }
                        if (_dataStorageOptions.DataRefreshTime != 0)
                        {
                            _memoryCache.Set(cacheKey, dataStructure, GetMemoryCacheEntryOptions(_dataStorageOptions.DataRefreshTime));
                        }
                        else
                        {
                            _memoryCache.Set(cacheKey, dataStructure);
                        }
                    }
                }
                finally
                {
                    certLock.Release();
                }
            }
            return dataStructure;
        }

        /// <summary>
        /// Set options for cache entry including PostEvictionCallback
        /// </summary>
        /// <param name="expireInMilliseconds">Time when cache entry will expire</param>
        /// <returns></returns>
        private MemoryCacheEntryOptions GetMemoryCacheEntryOptions(int expireInMinutes = 60)
        {
            var expirationTime = DateTime.Now.AddMinutes(expireInMinutes);
            var expirationToken = new CancellationChangeToken(
                new CancellationTokenSource(TimeSpan.FromMinutes(expireInMinutes + .01)).Token);

            var memoryCacheEntryOptions = new MemoryCacheEntryOptions();
            memoryCacheEntryOptions.SetAbsoluteExpiration(expirationTime);
            memoryCacheEntryOptions.AddExpirationToken(expirationToken);

            memoryCacheEntryOptions.PostEvictionCallbacks.Add(new PostEvictionCallbackRegistration()
            {
                EvictionCallback = (key, value, reason, state) =>
                {
                    if (reason == EvictionReason.TokenExpired || reason == EvictionReason.Expired)
                    {
                        IDataStructure dataStructure = null;
                        using (var parserFactory = new ParserFactory(_datasourceOptions.Indexes[key as string]))
                        {
                            var parser = parserFactory.CreateParser(key as string);
                            var dataLoader = new DataLoader(parser);
                            dataStructure = dataLoader.Load();
                        }
                        if (dataStructure != null)
                        {
                            _memoryCache.Set(key, dataStructure, GetMemoryCacheEntryOptions(expireInMinutes));
                        }
                        else
                        {
                            _memoryCache.Set(key, value, GetMemoryCacheEntryOptions(expireInMinutes));
                        }
                    }
                }
            });

            return memoryCacheEntryOptions;
        }
    }
}