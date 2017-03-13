using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public class SimulatedTransport : IOBD2Transport
    {
        public async Task<bool> ConnectAsync(CancellationToken cts = default(CancellationToken))
        {
            return true;
        }

        public async Task<bool> DisconnectAsync(CancellationToken cts = default(CancellationToken))
        {
            return true;
        }

        public async Task<string> ReadAsync(CancellationToken cts = default(CancellationToken))
        {
            return "";
        }

        public async Task<bool> WriteAsync(string data, CancellationToken cts = default(CancellationToken))
        {
            return true;
        }
    }
}
