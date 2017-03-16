using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Gpio;
using Carputer.UWP.Interfaces;

namespace Carputer.UWP.Services
{
    public interface IPowerShutdownMonitorService
    {
        int PowerPinID { get; }
        int MillisecondsToShutdown { get; set; }
        GpioPinValue ShutdownPinState { get; }
        EventHandler DoShutdown { get; set; }
    }

    public class PowerShutdownMonitorService : IPowerShutdownMonitorService, IService
    {
        public int PowerPinID { get; private set; } = 23;
        private GpioPin _powerPin;
        private GpioPinValue _pinValue;

        private Task _shutdownTask;
        private GpioPinValue _pinState;

        public GpioPinValue ShutdownPinState { get; set; } = GpioPinValue.Low;
        public int MillisecondsToShutdown { get; set; } = 10000;
        public EventHandler DoShutdown { get; set; }

        private CancellationTokenSource _cts = null;

        public PowerShutdownMonitorService()
        {
        }

        private void _powerPin_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            _pinState = sender.Read();
            if (_pinState == ShutdownPinState && _shutdownTask == null)
            {
                _cts = new CancellationTokenSource();
                _shutdownTask = Task.Delay(MillisecondsToShutdown, _cts.Token)
                    .ContinueWith(_ => triggerShutdown(), TaskContinuationOptions.NotOnCanceled);

            }
            else
            {
                if (_shutdownTask != null)
                {
                    _cts.Cancel();
                    _cts = null;
                    _shutdownTask = null;
                }
            }
        }

        private void triggerShutdown()
        {
            _cts = null;
            _shutdownTask = null;

            DoShutdown?.Invoke(this, new EventArgs());
        }

        public async Task StartAsync()
        {
            var gpio = await GpioController.GetDefaultAsync();
            _powerPin = gpio.OpenPin(PowerPinID);
            _powerPin.SetDriveMode(GpioPinDriveMode.Input);
            _powerPin.ValueChanged += _powerPin_ValueChanged;
            _pinValue = _powerPin.Read();
        }

        public async Task StopAsync()
        {
            _powerPin.Dispose();
            await Task.CompletedTask;
        }
    }
}
