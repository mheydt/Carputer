using System;
using System.Collections.Generic;
using System.Composition;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Carputer.UWP.Interfaces;

namespace Carputer.UWP.Services
{
    public interface ISettingsService
    {
        Task<T> GetAsync<T>(string key, T defaultValue = default(T));
        Task PutAsync<T>(string key, T value);
    }

    [Export(typeof(IService))]
    public class SettingsService : ISettingsService, IService
    {
        private ICacheService _cacheService;

        public SettingsService(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<T> GetAsync<T>(string key, T defaultValue = default(T))
        {
            var r = await _cacheService.GetAsync<T>(key, defaultValue);
            return r;
        }

        public async Task PutAsync<T>(string key, T value)
        {
            await _cacheService.PutAsync<T>(key, value);
        }

        public async Task StartAsync()
        {
        }

        public async Task StopAsync()
        {
        }
    }
}
