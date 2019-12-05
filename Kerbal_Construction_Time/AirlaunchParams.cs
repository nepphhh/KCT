﻿using System;

namespace KerbalConstructionTime
{
    public class AirlaunchParams
    {
        public Guid VesselId { get; set; }
        public double Altitude { get; set; }
        public double KscAzimuth { get; set; }
        public double KscDistance { get; set; }
        public double LaunchAzimuth { get; set; }
        public double Velocity { get; set; }

        public bool Validate(out string errorMsg)
        {
            AirlaunchTechLevel lvl = AirlaunchTechLevel.GetCurrentLevel();
            if (lvl == null)
            {
                errorMsg = "No valid airlaunch configuration found";
                return false;
            }

            double minKscDist = 0;

            if (KscAzimuth >= 360 || KscAzimuth < 0)
            {
                errorMsg = "Invalid KSC azimuth";
                return false;
            }

            if (LaunchAzimuth >= 360 || LaunchAzimuth < 0)
            {
                errorMsg = "Invalid KSC azimuth";
                return false;
            }

            if (Altitude > lvl.MaxAltitude || Altitude < lvl.MinAltitude)
            {
                errorMsg = $"Altitude needs to be between {lvl.MinAltitude} and {lvl.MaxAltitude} m";
                return false;
            }

            if (Velocity > lvl.MaxVelocity || Velocity < lvl.MinVelocity)
            {
                errorMsg = $"Velocity needs to be between {lvl.MinVelocity} and {lvl.MaxVelocity} m/s";
                return false;
            }

            if (KscDistance > lvl.MaxKscDistance || KscDistance < minKscDist)
            {
                errorMsg = $"Distance from Space Center needs to be between {minKscDist / 1000:0.#} and {lvl.MaxKscDistance / 1000:0.#} km";
                return false;
            }

            errorMsg = null;
            return true;
        }
    }
}