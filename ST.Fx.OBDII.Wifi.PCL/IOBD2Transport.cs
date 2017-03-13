using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public interface IOBD2Transport
    {
        Task<bool> ConnectAsync(CancellationToken cts = default(CancellationToken));
        Task<bool> WriteAsync(string data, CancellationToken cts = default(CancellationToken));
        Task<string> ReadAsync(CancellationToken cts = default(CancellationToken));
        Task<bool> DisconnectAsync(CancellationToken cts = default(CancellationToken));
    }
}