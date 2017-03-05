using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Carputer.Phone.UWP.OBDII
{
    public interface IOBDIIServer
    {
        Task InitAsync(CancellationToken cancellation = default(CancellationToken));
        Task ShutdownAsync(CancellationToken cancellation = default(CancellationToken));
        Task<string> ExecuteCommand(string command, CancellationToken cancellation = default(CancellationToken));
    }
}
