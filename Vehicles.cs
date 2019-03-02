using UnityEngine;
using System.Collections;

namespace MscLib {
    public static class Vehicles {
        public static DrivableVehicle Hayosiko => MscLib.Hayosiko.Instance;
        public static DrivableVehicle Gifu => MscLib.Gifu.Instance;
        public static DrivableVehicle Satsuma => MscLib.Satsuma.Instance;


        public static DrivableVehicle Find(GameObject g) {
            if (g == Hayosiko.VehicleObject) {
                return Hayosiko;
            }

            if (g == Gifu.VehicleObject) {
                return Gifu;
            }

            if (g == Satsuma.VehicleObject) {
                return Satsuma;
            }

            return null;
        }
    }
}
