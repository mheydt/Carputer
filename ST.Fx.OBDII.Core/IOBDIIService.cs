using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Fx.OBDII.Core
{
    public interface IOBDIIService
    {
        Task<bool> InitAsync(bool simulatormode = false);
        Task DisconnectAsync();

    }
}
