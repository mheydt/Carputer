using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;
using Serilog.Core;

namespace Carputer.UWP.Devices.GPS.NMEA
{
    public class GlobalPositioningSystemFixData : NmeaGpsMessageBase
    {

        public static string Identifier { get; private set; } = "$GPGGA";
        public LocalTime Time { get; private set; }
        public string Latitude { get; private set; }
        public string LatitudeDirection { get; private set; }
        public string Longitude { get; private set; }
        public string LongitudeDirection { get; private set; }
        public int FixQuality { get; private set; }
        public int NumberOfSatellites { get; private set; }

        public double Altitude { get; private set; }
        public string AltitudeUnit { get; private set; }

        public double HorizontalDilutionOfPrecision { get; private set; }
        public double GeoidalSeparation { get; private set; }
        public string UnitsOfGeoidalSeparation { get; private set; }
        public string AgesOfDifferentialGPSData { get; private set; }
        public string DifferentialReferenceStationID { get; private set; }
        public string Checksum { get; private set; }

        public double LatitudeDegrees { get; private set; }
        public double LongitudeDegrees { get; private set; }

        public GlobalPositioningSystemFixData(string message, Logger logger)
        {
            if (!message.StartsWith("$GPGGA,")) throw new Exception("Expected $GPGGA, got " + message);
            var parts = message.Split(new[] { ',', '*' });

            logger.Information(string.Join(",", parts));

            Identifier = parts[0];

            Time = new LocalTime(
                Int32.Parse(parts[1].Substring(0, 2)),
                Int32.Parse(parts[1].Substring(2, 2)),
                Int32.Parse(parts[1].Substring(4, 2)),
                Int32.Parse(parts[1].Substring(7, 3)));

            Latitude = parts[2];
            LatitudeDirection = parts[3];
            Longitude = parts[4];
            LongitudeDirection = parts[5];

            logger.Information($"{Time}, {Latitude}, {LatitudeDirection}, {Longitude}, {LongitudeDirection}");

            LatitudeDegrees = (double.Parse(Latitude.Substring(0, 2)) +
                               double.Parse(Latitude.Substring(2, 2)) / 60 +
                               (double.Parse(Latitude.Substring(5, Latitude.Length - 5)) / 10000.0) / 60.0)
                              * (LatitudeDirection == "N" ? 1 : -1);

            LongitudeDegrees = (double.Parse(Longitude.Substring(0, 3)) +
                                double.Parse(Longitude.Substring(3, 2)) / 60 +
                                (double.Parse(Longitude.Substring(6, Longitude.Length - 6)) / 10000.0) / 60.0)
                               * (LongitudeDirection == "W" ? -1 : 1);

            FixQuality = int.Parse(parts[6]);

            NumberOfSatellites = int.Parse(parts[7]);
            HorizontalDilutionOfPrecision = double.Parse(parts[8]);

            Altitude = double.Parse(parts[9]);
            AltitudeUnit = parts[10];

            GeoidalSeparation = double.Parse(parts[11]);
            UnitsOfGeoidalSeparation = parts[12];
            AgesOfDifferentialGPSData = parts[13];
            DifferentialReferenceStationID = parts[14];

            Checksum = parts[15];
        }

        public override string ToString()
        {
            return $"{Time} {Latitude} {Longitude} {Altitude}";
        }
    }

}
