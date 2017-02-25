using ST.Fx.Gps.NMEA.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ST.Fx.Gps.NMEA
{
    public interface IGpsService
    {
        IObservable<GlobalPositioningSystemFixData> Positions { get; }

        Task InitializeAsync();
        Task ShutdownAsync();
    }
}
