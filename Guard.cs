using UnityEngine;
using System.Collections;
using System.Linq;

namespace MscLib {
    /// <summary>
    /// Dancehall guard
    /// 
    /// To trigger the guard to chase and throw the player, 
    /// set Reputation to a value over 2 and set PlayerInHall to true
    /// </summary>
    public class Guard {
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Player reputation. Anything above 2 is a bad reputation
        /// </summary>
        public int Reputation {
            get { return reactFsm.FsmVariables.FindFsmInt("Reputation").Value; }
            set { reactFsm.FsmVariables.FindFsmInt("Reputation").Value = System.Math.Max(0, value); }
        }

        /// <summary>
        /// Does the guard consider the player to be inside the dance hall?
        /// </summary>
        public bool PlayerInHall {
            get { return reactFsm.FsmVariables.FindFsmBool("InHall").Value; }
            set { reactFsm.FsmVariables.FindFsmBool("InHall").Value = value; }
        }

        private PlayMakerFSM reactFsm;
      
        public Guard() : this(GameObject.Instantiate(GetSourceGameObject())) { }

        public Guard(GameObject g) {
            GameObject = g;

            GameObject guardObj = g.transform.Find("Guard").gameObject;
            //for some reason this object starts at a weird offset, might be an oversight
            //but we need to reset it here or he's going to be walking in the sky
            guardObj.transform.position = Vector3.zero;

            reactFsm = PlayMakerFSM.FindFsmOnGameObject(guardObj, "React");         
        }

        public static Guard DancehallGuard() {
            return new Guard(GetSourceGameObject());
        }        

        private static GameObject GetSourceGameObject() {
            var dancehall = GameObject.Find("DANCEHALL");
            //all of this stuff could be disabled so go thru the transforms
            return dancehall.transform.Cast<Transform>().FirstOrDefault(t => t.name == "Functions").transform.Cast<Transform>().FirstOrDefault(t => t.name == "GUARD").gameObject;
        }
    }
}