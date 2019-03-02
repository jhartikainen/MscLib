using UnityEngine;
using System.Collections;
using System.Linq;

namespace MscLib {
    /// <summary>
    /// Abstraction of the Dancehall Fighter guy
    /// 
    /// Note: Because of how the combat is implemented, you can't have multiple of these
    /// attacking the player at the same time. Hits will register from only one at a time
    /// and you must use MscLib.Player.SetFightOpponent(someFighterInstance) to choose which one.
    /// </summary>
    public class Fighter {
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Used to set values in Player for hitbox hit checking
        /// </summary>
        public GameObject Fist { get; private set; }

        private GameObject pelvis;
        private GameObject head;

        /// <summary>
        /// Set the hitpoints for the fighter
        /// 
        /// The fighter has two hitpoint values: in pelvis and in head.
        /// It seems only the value in head matters, not 100% sure so when setting we set them both
        /// </summary>
        public int Hitpoints {
            get { return head.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmInt("RequiredHits").Value; }
            set {
                pelvis.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmInt("RequiredHits").Value = value;
                head.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmInt("RequiredHits").Value = value;
            }
        }

        /// <summary>
        /// Construct a new fighter based on the one at the dancehall, since that works ok'ish        
        /// </summary>
        public Fighter() {
            var newFighter = GameObject.Instantiate(GetSourceGameObject());
            Setup(newFighter);
        }

        public Fighter(GameObject g) {
            Setup(g);
        }

        private void Setup(GameObject g) {
            GameObject = g;

            Fist = g.transform.Find("Fighter/Pivot/Char/skeleton/pelvis/spine_middle/spine_upper/collar_right/shoulder_right/arm_right/hand_right").Cast<Transform>().FirstOrDefault(t => t.name == "FighterFist").gameObject;

            pelvis = g.transform.Find("Fighter/Pivot/Char/skeleton/pelvis").gameObject;
            head = g.transform.Find("Fighter/Pivot/Char/skeleton/pelvis/spine_middle/spine_upper/PhysHead").gameObject;            
        }

        /// <summary>
        /// Get a Fighter object based on the default dancehall fighter guy
        /// </summary>
        /// <returns></returns>
        public static Fighter DancehallFighter() {
            return new Fighter(GetSourceGameObject());
        }

        private static GameObject GetSourceGameObject() {
            var dancehall = GameObject.Find("DANCEHALL");
            //all of this stuff could be disabled so go thru the transforms
            return dancehall.transform.Cast<Transform>().FirstOrDefault(t => t.name == "Functions").transform.Cast<Transform>().FirstOrDefault(t => t.name == "FIGHTER").gameObject;
        }
    }

}