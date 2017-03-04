using ST.Fx.OBDII;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using ST.Fx.Debug.Tracer;

namespace Carputer.Phone.UWP
{
    public class SocketTransport : IOBD2Transport
    {
        private Windows.Networking.Sockets.StreamSocket _socket;

        private string _address;
        private int _port;
        private StreamWriter _outstream;
        private StreamReader _instream;

        public SocketTransport(string address, int port)
        {
            _address = address;
            _port = port;
        }

        public async Task<bool> ConnectAsync(CancellationToken cts = default(CancellationToken))
        {
            try
            {
                _socket = new Windows.Networking.Sockets.StreamSocket();
                var serverHost = new Windows.Networking.HostName(_address);
                await _socket.ConnectAsync(serverHost, _port.ToString());
                _outstream = new StreamWriter(_socket.OutputStream.AsStreamForWrite());
                _instream = new StreamReader(_socket.InputStream.AsStreamForRead());
            }
            catch (Exception ex)
            {

            }
            return true;
        }

        public Task<bool> DisconnectAsync(CancellationToken cts = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public async Task<string> ReadAsync(CancellationToken cts = default(CancellationToken))
        {
            var buffer = new char[1000];
            var count = await _instream.ReadAsync(buffer, 0, buffer.Length);
            Tracer.writeLine("Read: " + buffer.ToString());
            return buffer.ToString();
        }

        public async Task<bool> WriteAsync(string data, CancellationToken cts = default(CancellationToken))
        {
            await _outstream.WriteAsync(data);
            await _outstream.FlushAsync();
            Tracer.writeLine("Wrote: " + data);
            return true;
        }
    }
}
