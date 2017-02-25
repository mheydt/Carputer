using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NLog;
using Serilog;
using Serilog.Sinks.Network;

namespace ST.Spikes.Log2Net.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Udp(IPAddress.Loopback, 9998)
                .CreateLogger();

            var client = new UdpClient(9998);
            var ep = new IPEndPoint(IPAddress.Any, 0);

            client.BeginReceive(new AsyncCallback(recv), client);

            Task.Delay(1000).Wait();
            log.Information("HI");

            Console.ReadLine();
        }

        private static void recv(IAsyncResult res)
        {
            var client = res.AsyncState as UdpClient;
            var rep = new IPEndPoint(IPAddress.Any, 0);
            var bytes = client.EndReceive(res, ref rep);
            Console.WriteLine(Encoding.ASCII.GetString(bytes));
            client.BeginReceive(new AsyncCallback(recv), client);
        }
    }
}
