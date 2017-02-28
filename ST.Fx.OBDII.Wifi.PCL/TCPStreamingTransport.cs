using Sockets.Plugin;
using ST.Fx.Debug.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Fx.OBDII.Wifi.PCL
{
    public class TCPStreamingTransport : IOBD2Transport
    {
        private TcpSocketClient _client;
        private string _ipaddress;
        private int _port;
        private byte[] _buffer = new byte[1024];

        public TCPStreamingTransport(string ipaddress, int port)
        {
            _ipaddress = ipaddress;
            _port = port;
        }

        public async Task<bool> ConnectAsync(CancellationToken token = default(CancellationToken))
        {
            if (_client != null) return true;

            _client = new TcpSocketClient();
            try
            {
                await _client.ConnectAsync(_ipaddress, _port, false, token);
                return true;
            }
            catch (Exception ex)
            {
                Tracer.writeLine($"Error connecting: {ex.Message}");
                _client = null;
            }

            return false;
        }

        public async Task<bool> DisconnectAsync(CancellationToken token = default(CancellationToken))
        {
            if (_client == null) return true;

            await _client.DisconnectAsync();
            _client = null;

            Tracer.writeLine("Disconnected");

            return true;
        }

        public async Task<string> ReadAsync(CancellationToken token = default(CancellationToken))
        {
            var bytes = await _client.ReadStream.ReadAsync(_buffer, 0, _buffer.Length, token);
            var str = Encoding.UTF8.GetString(_buffer, 0, bytes);
            Tracer.writeLine($"ReadAsync: [{str.Length}] " + str);
            return str;
        }

        public async Task<bool> WriteAsync(string data, CancellationToken token = default(CancellationToken))
        {
            Tracer.writeLine("Writing: " + data);
            try
            {
                var buffer = Encoding.UTF8.GetBytes(data);
                await _client.WriteStream.WriteAsync(buffer, 0, buffer.Length, token);
                _client.WriteStream.Flush();
                Tracer.writeLine("Wrote: " + data);
                return true;
            }
            catch (Exception ex)
            {
                Tracer.writeLine($"Exception writing: {ex.Message}");
            }

            return false;
        }
    }
}
