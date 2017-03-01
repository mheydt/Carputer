using Sockets.Plugin;
using ST.Fx.Debug.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carputer.Phone.UWP
{
    public class SimWifiObdII
    {
        private int _port;
        private TcpSocketClient _client;

        public SimWifiObdII(int port)
        {
            _port = port;
        }

        public async Task InitializeAsync()
        {
            _client = new TcpSocketClient();
            await _client.ConnectAsync("127.0.0.1", _port);

            Task.Run(async () => readStream());
        }

        private async Task readStream()
        {
            var buffer = new byte[20 * 1024];

            while (true)
            {
                var count = _client.ReadStream.Read(buffer, 0, buffer.Length);
                var data = Encoding.UTF8.GetString(buffer);

                Tracer.writeLine(data);

                var response = "";
                switch (data)
                {
                    case "ATZ\r":
                    case "ATE0\r":
                    case "ATL1\r":
                    case "ATSP00\r":
                        response = "OK>";
                        break;
                }

                Tracer.writeLine("Response: " + response);

                var bytes = Encoding.UTF8.GetBytes(response);
                _client.WriteStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
