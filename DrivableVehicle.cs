using UnityEngine;
using System.Collections;

namespace MscLib {
    public interface DrivableVehicle {
        float FuelLevel { get; set; }
        float MaxFuelLevel { get; }
        GameObject VehicleObject { get; }
    }
}