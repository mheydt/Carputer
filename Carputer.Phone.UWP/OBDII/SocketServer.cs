using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace App1
{
    public class Server
    {
        private StreamSocketListener _listener;
        private List<CancellationTokenSource> _activeConnectionTokens = new List<CancellationTokenSource>();
        private List<Task> _tasks = new List<Task>();

        public async Task InitAsync()
        {
            _listener = new StreamSocketListener();
            _listener.ConnectionReceived += _listener_ConnectionReceived;
            await _listener.BindServiceNameAsync("35000");
            Debug.WriteLine("Server listening");
        }

        public async Task ShutdownAsync()
        {
            _activeConnectionTokens.ForEach(t => t.Cancel());
            Task.WaitAll(_tasks.ToArray());
        }

        private async void _listener_ConnectionReceived(StreamSocketListener sender, StreamSocketListenerConnectionReceivedEventArgs args)
        {
            Debug.WriteLine("Server got connection");
            var task = doListen(args);
            _tasks.Add(task);
        }

        private async Task doListen(StreamSocketListenerConnectionReceivedEventArgs args)
        {
            var istream = args.Socket.InputStream.AsStreamForRead();
            var ostream = args.Socket.OutputStream.AsStreamForWrite();

            var reader = new StreamReader(istream);
            var writer = new StreamWriter(ostream);

            var buffer = new byte[1024];

            var cancellation = new CancellationTokenSource();
            _activeConnectionTokens.Add(cancellation);

            while (!cancellation.IsCancellationRequested)
            {
                var bytes = await istream.ReadAsync(buffer, 0, buffer.Length);
                if (bytes == 0)
                {
                    Debug.WriteLine("Connection closed");
                    break;
                }

                var data = Encoding.ASCII.GetString(buffer, 0, bytes);

                Debug.WriteLine($"Read: {bytes} {data}");

                await process(data, writer);
            }

            istream.Dispose();
            ostream.Dispose();

            Debug.WriteLine("Exiting listener");
        }

        private async Task process(string data, StreamWriter writer)
        {
            switch (data)
            {
                case "ATZ\r":
                    await writer.WriteAsync("OK>\r");
                    await writer.FlushAsync();
                    break;

                case "ATE0\r":
                    await writer.WriteAsync("OK>\r");
                    await writer.FlushAsync();
                    break;

                case "ATSP00\r":
                    await writer.WriteAsync("OK>\r");
                    await writer.FlushAsync();
                    break;

                case "0902\r":
                    await writer.WriteAsync("SEARCHING...");
                    await writer.FlushAsync();
                    await Task.Delay(1000);
                    await writer.WriteAsync("OK>\r");
                    await writer.FlushAsync();
                    break;
            }
        }
    }
}
