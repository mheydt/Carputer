using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Caliburn.Micro;
using Carputer.UWP.Services;
using PropertyChanged;

namespace Carputer.UWP.ViewModels
{
    [ImplementPropertyChanged]
    public class MusicPlayerViewModel : Screen
    {
        private MediaElement _player = new MediaElement() {AudioCategory = AudioCategory.Media};
        private IMediaPlayerService _mediaPlayerService;

        public MusicPlayerViewModel(IMediaPlayerService mediaPlayerService)
        {
            _mediaPlayerService = mediaPlayerService;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            _mediaPlayerService.Player.MediaOpened += Player_MediaOpened;
        }

        private void Player_MediaOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            _mediaPlayerService.Player.MediaOpened -= Player_MediaOpened;
        }

        public async Task Play(object context)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            folder = await folder.GetFolderAsync("Media\\Music");
            var sf = await folder.GetFileAsync("07 Emotion Detector.mp3");

            await _mediaPlayerService.PlayMp3Async(sf);
        }

        private async Task onActivateAsync()
        { 
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            folder = await folder.GetFolderAsync("Media\\Music");
            var sf = await folder.GetFileAsync("07 Emotion Detector.mp3");

            _player.SetSource(await sf.OpenAsync(FileAccessMode.Read), sf.ContentType);
            _player.Play();
        }
    }
}
