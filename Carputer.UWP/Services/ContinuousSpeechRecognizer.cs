using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Carputer.UWP.Interfaces;

namespace Carputer.UWP.Services
{
    public interface IContinuousSpeechRecognizer
    {
        
    }

    public class ContinuousSpeechRecognizer : IContinuousSpeechRecognizer, IService
    {
        private SpeechRecognizer _speechRecognizer;

        public async Task StartAsync()
        {
            try
            {
                _speechRecognizer = new SpeechRecognizer();
                _speechRecognizer.Timeouts.EndSilenceTimeout = TimeSpan.MaxValue;
                var compilationResult = await _speechRecognizer.CompileConstraintsAsync();
                _speechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
                _speechRecognizer.ContinuousRecognitionSession.Completed += ContinuousRecognitionSession_Completed;
                _speechRecognizer.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.MaxValue;
                //await _speechRecognizer.ContinuousRecognitionSession.StartAsync(SpeechContinuousRecognitionMode.Default);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            await Task.CompletedTask;
        }

        private void ContinuousRecognitionSession_Completed(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionCompletedEventArgs args)
        {
            Debug.WriteLine($"{args.Status}");
            _speechRecognizer.ContinuousRecognitionSession.StartAsync(SpeechContinuousRecognitionMode.Default);
        }

        private void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            Debug.WriteLine($"{args.Result.Text}");
        }

        public async Task StopAsync()
        {
            await _speechRecognizer.ContinuousRecognitionSession.StopAsync();
        }
    }
}
