using UnityEngine;
using System.Collections;
using System.Linq;

namespace MscLib {
    public class Gifu : MonoBehaviour, DrivableVehicle {
        private static Gifu _instance;

        public float FuelLevel {
            get => fuelTankFSM.FsmVariables.FindFsmFloat("FuelLevel").Value;
            set => fuelTankFSM.FsmVariables.FindFsmFloat("FuelLevel").Value = Mathf.Clamp(value, 0, MaxFuelLevel);
        }

        public float MaxFuelLevel => fuelCapFSM.FsmVariables.FindFsmFloat("MaxCapacity").Value;

        public GameObject VehicleObject { get; private set; }

        private PlayMakerFSM fuelTankFSM;
        private PlayMakerFSM fuelCapFSM;

        private Gifu() {
            VehicleObject = GameObject.Find("GIFU(750/450psi)");

            fuelTankFSM = PlayMakerFSM.FindFsmOnGameObject(VehicleObject.transform.Find("FuelTank").gameObject, "Data");

            //gifu does not have max capacity in the fuel tank's data, instead it seems to be here
            fuelCapFSM = GameObject.Find("GIFU(750/450psi)/LOD/FuelFiller/OpenCap").transform.Cast<Transform>().First(t => t.name == "CapTrigger_FuelGifu").GetComponents<PlayMakerFSM>().First(fsm => fsm.FsmName == "Trigger");
        }

        public static Gifu Instance {
            get {
                if (_instance == null) {
                    var go = new GameObject("MscLib_Gifu");
                    _instance = go.AddComponent<Gifu>();
                }

                return _instance;
            }
        }

        void OnDestroy() {
            _instance = null;
        }
    }
}