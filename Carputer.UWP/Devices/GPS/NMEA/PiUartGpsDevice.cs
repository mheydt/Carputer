using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using Carputer.UWP.Util.Serilog.Udp;
using Serilog;
using Serilog.Core;

namespace Carputer.UWP.Devices.GPS.NMEA
{
    public class PiUartGpsDevice
    {
        private SerialDevice _device;
        private DataReader _reader;
        private Task _readTask = null;
        private CancellationTokenSource _cts;
        private uint _bufferLength = 1024;

        private StringBuilder _buffer = new StringBuilder();


        public ISubject<string> Readings { get; private set; }
        private Subject<string> _subject;

        public bool InitializedSuccessfully { get; set; } = false;

        private Logger _logger;

        public PiUartGpsDevice()
        {
            _subject = new Subject<string>();
            Readings = _subject;

            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Udp(IPAddress.Parse("192.168.0.26"), 9998)
                .CreateLogger();

            _logger.Information("HI from PiUartGpsDevice!");
        }

        public async Task InitializeAsync()
        {
            try
            {
                _logger.Information("InitializeAsync");

                InitializedSuccessfully = false;

                var aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);

                _logger.Information($"# of devices {dis.Count}");

                var entry = dis.First();

                _logger.Information("2");

                _device = await SerialDevice.FromIdAsync(entry.Id);
                if (_device != null)
                {
                    _logger.Information("3");

                    _device.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                    _device.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    _device.BaudRate = 9600;
                    _device.Parity = SerialParity.None;
                    _device.StopBits = SerialStopBitCount.One;
                    _device.DataBits = 8;

                    InitializedSuccessfully = true;

                    _logger.Information("Initialized");

                    startReading();
                }
                else
                {
                    _logger.Information("Null device");
                }
            }
            catch (Exception ex)
            {
                _logger.Information(ex.Message);
                _logger.Error(ex, "");
            }
        }

        public async Task ShutdownAsync()
        {
            if (InitializedSuccessfully)
            {
                if (_readTask != null)
                {
                    _cts.Cancel();
                    await _readTask;
                    _readTask = null;
                    _cts = null;
                    _reader.Dispose();
                    _reader = null;
                }
            }
        }

        private void startReading()
        {
            if (_device != null)
            {
                _reader = new DataReader(_device.InputStream);
                _reader.InputStreamOptions = InputStreamOptions.Partial;
                _cts = new CancellationTokenSource();

                try
                {
                    _readTask = Task.Run(async () =>
                    {
                        try
                        {
                            while (!_cts.Token.IsCancellationRequested)
                            {
                                if (_cts.IsCancellationRequested) _cts.Token.ThrowIfCancellationRequested();
                                var readTask = _reader.LoadAsync(_bufferLength).AsTask();
                                var bytesRead = await readTask;
                                if (bytesRead > 0)
                                {
                                    var data = _reader.ReadString(bytesRead);
                                    process(data);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            //Console.WriteLine(e);
                        }
                    }, _cts.Token);
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
        }

        private void process(string data)
        {
            _logger.Information($"Got GPS data: {data}");
            try
            {
                _buffer.Append(data);

                var content = _buffer.ToString();
                while (content.Contains("\r\n"))
                {
                    var index = content.IndexOf("\r\n");
                    var msg = content.Substring(0, index);
                    if (msg.StartsWith("$GP"))
                    {
                        // raise event
                        _subject.OnNext(msg);
                    }

                    content = content.Substring(index + 2, content.Length - index - 2);
                }

                _buffer.Clear();
                _buffer.Append(content);

            }
            catch (Exception e)
            {
                //Console.WriteLine(e);
            }
        }
    }
}
