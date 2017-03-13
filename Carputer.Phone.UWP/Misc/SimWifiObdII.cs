using Sockets.Plugin;
using ST.Fx.Debug.Tracer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sockets.Plugin.Abstractions;

namespace Carputer.Phone.UWP
{
    public class SimWifiObdII
    {
        private int _port;
        private TcpSocketListener _listener;

        public SimWifiObdII(int port)
        {
            _port = port;
        }

        public async Task InitializeAsync()
        {
            _listener = new TcpSocketListener();
            _listener.ConnectionReceived += _listener_ConnectionReceived;
            await _listener.StartListeningAsync(_port);
        }

        private async void _listener_ConnectionReceived(object sender, Sockets.Plugin.Abstractions.TcpSocketListenerConnectEventArgs args)
        {
            Tracer.writeLine($"Connect Received");

            var buffer = new byte[2048];
            var bytesRead = -1;
            while (bytesRead != 0)
            {
                bytesRead = await args.SocketClient.ReadStream.ReadAsync(buffer, 0, buffer.Length);

                Tracer.writeLine("Bytes read: " + bytesRead);

                if (bytesRead > 0)
                {
                    var data = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Tracer.writeLine("OBDII Sim Read: " + data);

                    var response = "";
                    switch (data)
                    {
                        case "ATZ\r":
                        case "ATE0\r":
                        case "ATL1\r":
                        case "ATSP00\r":
                            response = "OK>";
                            break;

                        case "0902\r":
                            processVinRequest(args);
                            return;

                        case "010C\r":
                            response = "01 0C 00 00";
                            break;
                    }

                    Tracer.writeLine("sending response");

                    await sendAsync(response, args);

                    Tracer.writeLine("sent response");
                }
            }

            Tracer.writeLine("listener out");
        }

        private async Task processVinRequest(TcpSocketListenerConnectEventArgs args)
        {
            sendAsync("SEARCHING...\r", args)
                .ContinueWith(_ => sendAsync("49 02 01 00 00 00 00 49 02 02 00 00 00 00 49 02 03 00 00 00 00 49 02 04 00 00 00 00 49 02 05 00 00 00 00\r>", args));
        }

        private async Task sendAsync(string msg, TcpSocketListenerConnectEventArgs args)
        {
            Tracer.writeLine("Responding with: " + msg);

            var bytes = Encoding.UTF8.GetBytes(msg);
            await args.SocketClient.WriteStream.WriteAsync(bytes, 0, bytes.Length);
            args.SocketClient.WriteStream.Flush();
            Tracer.writeLine("replied");
        }
    }
}
