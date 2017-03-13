using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public interface IOBDIITransport
    {
        Task<bool> InitAsync(CancellationToken cancellation = default(CancellationToken));
        Task ShutdownAsync(CancellationToken cancellation = default(CancellationToken));
        Task<string> ExecuteCommand(string command, string termiantor = ">", CancellationToken cancellation = default(CancellationToken));
    }
}
