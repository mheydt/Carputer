﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Fx.OBDII
{
    public interface IOBDIIService
    {
        Task InitAsync();
        Task ShutdownAsync();
    }
}
