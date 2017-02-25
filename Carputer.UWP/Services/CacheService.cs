using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carputer.UWP.Interfaces;

using System.Reactive.Linq;
using Windows.ApplicationModel.Contacts;
using Akavache;

namespace Carputer.UWP.Services
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key, T defaultValue = default(T));
        Task PutAsync<T>(string key, T value);
    }

    public class CacheService : ICacheService, IService
    {
        public static string ApplicationName { get; set; }

        public async Task<T> GetAsync<T>(string key, T defaultValue = default(T))
        {
            T retval = defaultValue;

            try
            {
                retval = await BlobCache.UserAccount.GetObject<T>(key)
                    .Catch(Observable.Return(defaultValue));
            }
            catch (KeyNotFoundException ex)
            {
            }
            /*
            var result = BlobCache.UserAccount.GetObject<T>(key)
                .Catch(Observable.Return(defaultValue));
                */
                /*
            var obs = BlobCache.UserAccount.GetObject<T>(key)
                .Subscribe(
                    x => retval = x,
                    ex => Console.WriteLine(ex.Message));
                    */
                    /*
            var r = BlobCache.UserAccount.GetObject<T>(key)
                .Catch(Observable.Return(defaultValue));
//                    .Subscribe(x => retval = x);

            await Task.Delay(1000);
            */
            return retval;
        }

        public async Task PutAsync<T>(string key, T value)
        {
            await BlobCache.UserAccount.InsertObject(key, value);
        }

        public async Task StartAsync()
        {
            BlobCache.ApplicationName = ApplicationName;
            await Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            await BlobCache.Shutdown();
        }
    }
}
