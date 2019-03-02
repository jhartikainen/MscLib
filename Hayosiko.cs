using UnityEngine;
using System.Collections;
using MSCLoader;
using System.Linq;

namespace MscLib {
    public class Hayosiko : MonoBehaviour, DrivableVehicle {
        private static Hayosiko _instance;

        public float FuelLevel {
            get => fuelTankFSM.FsmVariables.FindFsmFloat("FuelLevel").Value;
            set => fuelTankFSM.FsmVariables.FindFsmFloat("FuelLevel").Value = Mathf.Clamp(value, 0, MaxFuelLevel);
        }

        public float MaxFuelLevel => fuelTankFSM.FsmVariables.FindFsmFloat("MaxCapacity").Value;

        public GameObject VehicleObject { get; private set; }

        private PlayMakerFSM fuelTankFSM;
        
        private Hayosiko() {
            VehicleObject = GameObject.Find("HAYOSIKO(1500kg, 250)");

            fuelTankFSM = PlayMakerFSM.FindFsmOnGameObject(VehicleObject.transform.Find("FuelTank").gameObject, "Data");            
        }

        public static Hayosiko Instance {
            get {
                if (_instance == null) {
                    var go = new GameObject("MscLib_Hayosiko");
                    _instance = go.AddComponent<Hayosiko>();
                }

                return _instance;
            }
        }

        void OnDestroy() {
            _instance = null;
        }
    }
}
 