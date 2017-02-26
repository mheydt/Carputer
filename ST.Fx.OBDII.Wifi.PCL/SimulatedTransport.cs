using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public class SimulatedTransport : IOBD2Transport
    {
        public async Task<bool> ConnectAsync()
        {
            return true;
        }

        public async Task<bool> DisconnectAsync()
        {
            return true;
        }

        public async Task<string> ReadAsync()
        {
            return "";
        }

        public async Task<bool> WriteAsync(string data)
        {
            return true;
        }
    }
}
