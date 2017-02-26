using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public interface IOBD2Transport
    {
        Task<bool> ConnectAsync();
        Task<bool> WriteAsync(string data);
        Task<string> ReadAsync();
        Task<bool> DisconnectAsync();
    }
}