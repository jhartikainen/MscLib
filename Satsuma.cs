using UnityEngine;
using System.Collections;
using System.Linq;

namespace MscLib {
    public class Satsuma : MonoBehaviour, DrivableVehicle {
        private static Satsuma _instance;

        public float FuelLevel {
            get => fuelTankFSM.FsmVariables.FindFsmFloat("FuelLevel").Value;
            set => fuelTankFSM.FsmVariables.FindFsmFloat("FuelLevel").Value = Mathf.Clamp(value, 0, MaxFuelLevel);
        }

        public float MaxFuelLevel => fuelTankFSM.FsmVariables.FindFsmFloat("MaxCapacity").Value;

        public GameObject VehicleObject { get; private set; }

        private PlayMakerFSM fuelTankFSM;

        private Satsuma() {
            VehicleObject = GameObject.Find("SATSUMA(557kg, 248)");

            //on the satsuma this is actually stored in the parts db
            fuelTankFSM = PlayMakerFSM.FindFsmOnGameObject(PartsDatabase.Mechanics.First(t => t.Name == "FuelTank").PartsDbEntry, "Data");
        }

        public static Satsuma Instance {
            get {
                if (_instance == null) {
                    var go = new GameObject("MscLib_Satsuma");
                    _instance = go.AddComponent<Satsuma>();
                }

                return _instance;
            }
        }

        void OnDestroy() {
            _instance = null;
        }
    }
}