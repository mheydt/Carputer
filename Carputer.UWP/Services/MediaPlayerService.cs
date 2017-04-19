using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Carputer.UWP.Interfaces;

namespace Carputer.UWP.Services
{
    public interface IMediaPlayerService
    {
        MediaElement Player { get; }

        Task PlayMp3Async(StorageFile file);
    }

    public class MediaPlayerService : IMediaPlayerService, IService
    {
        public MediaElement Player { get; private set; }


        public MediaPlayerService()
        {
            Player = new MediaElement() { AudioCategory = AudioCategory.Media };
        }

        public async Task StartAsync()
        {
        }

        public async Task StopAsync()
        {
        }

        public async Task PlayMp3Async(StorageFile file)
        {
            Player.SetSource(await file.OpenAsync(FileAccessMode.Read), file.ContentType);
            Player.Play();
        }
    }
}
