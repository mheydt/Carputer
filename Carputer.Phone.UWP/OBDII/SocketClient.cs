using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;

namespace Carputer.Phone.UWP.OBDII
{
    public class SocketClient : IOBDIITransport
    {
        private StreamSocket _socket;
        private Stream _istream;
        private Stream _ostream;
        private StreamWriter _writer;
        private StreamReader _reader;

        private bool _connected;
        public bool Connected { get { return _connected; } }

        public SocketClient()
        {
            _connected = false;
        }

        public async Task<bool> InitAsync(CancellationToken cancellation = default(CancellationToken))
        {
            try
            {
                _socket = new StreamSocket();
                await _socket.ConnectAsync(new HostName("192.168.0.10"), "35000");
                _istream = _socket.InputStream.AsStreamForRead();
                _ostream = _socket.OutputStream.AsStreamForWrite();
                _writer = new StreamWriter(_ostream);
                _reader = new StreamReader(_istream);

                _connected = true;

                return true;
            }
            catch (Exception ex)
            {
                
            }

            return false;
        }

        public async Task ShutdownAsync(CancellationToken cancellation = default(CancellationToken))
        {
            _writer?.Dispose();
            _reader?.Dispose();
            _istream?.Dispose();
            _ostream?.Dispose();
            _socket?.Dispose();

            _writer = null;
            _reader = null;
            _istream = null;
            _ostream = null;
            _socket = null;

            _connected = false;

            await Task.FromResult(true);
        }

        public async Task WriteAsync(string data, CancellationToken cancellation = default(CancellationToken))
        {
            await _writer.WriteAsync(data + "\r");
            await _writer.FlushAsync();

            Debug.WriteLine($"Wrote: {data}");
        }

        public async Task<string> ReadAsync(CancellationToken cancellation = default(CancellationToken))
        {
            Debug.WriteLine("ReadAsync in");
            var buffer = new byte[1024];
            var count = await _istream.ReadAsync(buffer, 0, buffer.Length, cancellation);
            var data = Encoding.ASCII.GetString(buffer, 0, count);
            Debug.WriteLine($"{count} {data}");
            return data;
        }

        public async Task<string> ExecuteCommand(string command, string terminator = ">", CancellationToken cancellation = default(CancellationToken))
        {
            Debug.WriteLine($"ExecuteCommand: {command}");

            await WriteAsync(command, cancellation);
            var response = await listenForResponse(terminator, cancellation);

            Debug.WriteLine($"ExecuteCommand out: {response}");

            return response;
        }

        private async Task<string> listenForResponse(string terminator, CancellationToken cancellation = default(CancellationToken))
        {
            Debug.WriteLine("listenForResponse in");

            var response = "";

            while (true)
            {
                Debug.WriteLine("Waiting for more data");
                var r = await ReadAsync(cancellation);
                Debug.WriteLine($"listenForResponse: Read: {r}");

                r = r.Replace("SEARCHING...", "").Replace("\r\n", " ").Replace("\r", " ");

                response = response + r;
                response = response.Trim();

                if (r.EndsWith(terminator)) break;
            }

            Debug.WriteLine("listenForResponse out: " + response);

            return response;
        }
    }
}
