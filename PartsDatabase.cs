using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;

namespace MscLib {
    /// <summary>
    /// Access most of the car parts that exist ingame
    /// 
    /// This is a monobehaviour so we can automatically clean this up
    /// between the player going into the menu and back into the game
    /// Otherwise the data in this object would go stale between loads
    /// </summary>
    public class PartsDatabase : MonoBehaviour {
        private static PartsDatabase _instance;

        /// <summary>
        /// these are known parts that exist in the parts DB that aren't actually parts at all,
        /// or they might be parts but not something that actually can be messed with for useful purposes
        /// FuelLine: unknown, seems to be a part of something, starts attached, but does not exist as a separate part?
        /// WarningTriangle: it's only attaching into the tractor, so it doesn't really make any difference
        /// Block: Does not seem to really attach to anything
        /// </summary>
        private static List<string> ignoredParts = new List<string>() { "Gears", "Body", "RegPlateFront", "RegPlateRear", "Windows black wrap", "Wristwatch", "Holiday Present", "FuelLine", "FireworksBag", "WarningTriangle", "Ratchet Set", "Roll Cage", "Block" };

        private List<CarPart> bodyParts;
        private List<CarPart> mechanicsParts;
        private List<CarPart> engineParts;
        private List<CarPart> orderParts;
        private IEnumerable<CarPart> everything;

        //these are exposed as IEnumerable because you're not really supposed to be changing these lists via the public api
        public static IEnumerable<CarPart> Body => Instance.bodyParts;
        public static IEnumerable<CarPart> Mechanics => Instance.mechanicsParts;
        public static IEnumerable<CarPart> Engine => Instance.engineParts;
        public static IEnumerable<CarPart> Order => Instance.orderParts;

        private static PartsDatabase Instance {
            get {
                if (_instance == null) {
                    var go = new GameObject("MscLib_PartsDatabase");
                    _instance = go.AddComponent<PartsDatabase>();
                }

                return _instance;
            }
        }

        private PartsDatabase() {
            bodyParts = MkParts(GameObject.Find("Database/DatabaseBody").transform.Cast<Transform>().Select(t => t.gameObject).Where(p => !ignoredParts.Contains(p.name)));
            mechanicsParts = MkParts(GameObject.Find("Database/DatabaseMechanics").transform.Cast<Transform>().Select(t => t.gameObject).Where(p => !ignoredParts.Contains(p.name)));
            engineParts = MkParts(GameObject.Find("Database/DatabaseMotor").transform.Cast<Transform>().Select(t => t.gameObject).Where(p => !ignoredParts.Contains(p.name)));
            orderParts = MkParts(GameObject.Find("Database/DatabaseOrders").transform.Cast<Transform>().Select(t => t.gameObject).Where(p => !ignoredParts.Contains(p.name)));

            everything = bodyParts.Concat(mechanicsParts).Concat(engineParts).Concat(orderParts);
        }

        void OnDestroy() {
            _instance = null;
        }

        List<CarPart> MkParts(IEnumerable<GameObject> allParts) {
            var ps = new List<CarPart>();
            foreach (var part in allParts) {
                var p = CarPart.TryCreate(part);

                if (p == null) {
                    ModConsole.Print("Can't make part: " + part.name);
                    continue;
                }

                ps.Add(p);
            }

            return ps;
        }

        public static CarPart FindAttached(string name) {
            return Instance.everything.Where(part => part.IsAttached).FirstOrDefault(part => part.Name.ToLower().Contains(name.ToLower()));
        }
    }
}