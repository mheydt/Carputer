using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace ST.Fx.Gps.NMEA.UWP
{
    public class PiUartGpsDevice
    {
        private SerialDevice _device;
        private DataReader _reader;

        public IObservable<string> Readings { get; private set; }
        private Subject<string> _subject;

        public PiUartGpsDevice()
        {
            _subject = new Subject<string>();
            Readings = _subject;
        }

        public async Task initializeAsync()
        {
            try
            { 
                var aqs = SerialDevice.GetDeviceSelector();
                var dis = await DeviceInformation.FindAllAsync(aqs);
                var entry = dis.First();

                _device = await SerialDevice.FromIdAsync(entry.Id);
                if (_device != null)
                {
                    _device.WriteTimeout = TimeSpan.FromMilliseconds(1000);
                    _device.ReadTimeout = TimeSpan.FromMilliseconds(1000);
                    _device.BaudRate = 9600;
                    _device.Parity = SerialParity.None;
                    _device.StopBits = SerialStopBitCount.One;
                    _device.DataBits = 8;

                    startReading();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private Task _readTask = null;
        private CancellationTokenSource _cts;
        private uint _bufferLength = 1024;

        private StringBuilder _buffer = new StringBuilder();

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
