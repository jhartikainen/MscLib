using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MscLib {
    /// <summary>
    /// This can spawn a variety of items, primarily those sold by Teimo with few others
    /// </summary>
    public class Spawner : MonoBehaviour {
        private static Spawner _instance;
        private GameObject spawnerObject;

        /// <summary>
        /// All spawnable item types
        /// </summary>
        public enum ItemType {
            BEER, SAUSAGE, MACARONBOX, PIZZA, CHIPS, JUICE, YEAST,
            SUGAR, MILK, MOSQUITOSPRAY, COOLANT, BRAKEFLUID, MOTOROIL, TWOSTROKE_FUEL,
            FIRE_EXTINGUISHER, NITROUS_BOTTLE, BATTERY, CIGARETTES, OILFILTER, SPARKPLUG_BOX,
            COFFEE, CHARCOAL, ALTERNATOR_BELT, SPARKPLUG
        };

        //This is just a convenience to help convert the ItemType enum names into strings
        //for cases where we can't just titlecase it easily
        private static Dictionary<ItemType, string> itemMapping = new Dictionary<ItemType, string> {
            { ItemType.SAUSAGE, "Sausages" },
            { ItemType.MACARONBOX, "MacaronBox" },
            { ItemType.MOSQUITOSPRAY, "MosquitoSpray" },
            { ItemType.BRAKEFLUID, "BrakeFluid" },
            { ItemType.MOTOROIL, "MotorOil" },
            { ItemType.TWOSTROKE_FUEL, "TwoStroke" },
            { ItemType.FIRE_EXTINGUISHER, "FireExtinguisher" },
            { ItemType.NITROUS_BOTTLE, "N2OBottle" },
            { ItemType.SPARKPLUG_BOX, "SparkplugBox" },
            { ItemType.ALTERNATOR_BELT, "Alternatorbelt" },
        };

        private Spawner() {
            spawnerObject = GameObject.Find("Spawner/CreateItems");
        }

        /// <summary>
        /// Spawn an item
        /// </summary>
        /// <param name="t">Item type</param>
        /// <returns>New item's GameObject</returns>
        public static GameObject SpawnItem(ItemType t) {
            string fsmName;
            if (itemMapping.ContainsKey(t)) {
                fsmName = itemMapping[t];
            }
            else {
                var itemString = t.ToString();
                fsmName = char.ToUpper(itemString[0]) + itemString.Substring(1).ToLower();
            }

            var fsm = PlayMakerFSM.FindFsmOnGameObject(Instance.spawnerObject, fsmName);

            //this triggers the item to spawn
            fsm.SendEvent("SPAWNITEM");

            //the spawner fsm stores a reference to the newly created object in this variable
            return fsm.FsmVariables.FindFsmGameObject("New").Value;
        }

        private static Spawner Instance {
            get {
                if (_instance == null) {
                    var go = new GameObject("MscLib_Spawner");
                    _instance = go.AddComponent<Spawner>();
                }

                return _instance;
            }
        }

        void OnDestroy() {
            _instance = null;
        }
    }
}
