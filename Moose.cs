using UnityEngine;
using System.Collections;
using System.Linq;
using MSCLoader;

namespace MscLib {
    /// <summary>
    /// Spawn a moose
    /// 
    /// Gotcha: If you want to set the moose's starting location, use SetRoute
    /// The moose has some logic in it that may cause it to disappear. SetRoute handles this.
    /// </summary>
    public class Moose {
        public GameObject GameObject { get; private set; }

        private RouteSetter routeSetter;

        /// <summary>
        /// The ragdoll object that gets spawned and enabled when the moose collides with something
        /// </summary>
        public GameObject Ragdoll { get; private set; }

        public Moose() {
            //animals moose is sometimes completely disabled. since it has no parent, we need to just go through *everything* for it
            var mooseParent = Resources.FindObjectsOfTypeAll<Transform>().FirstOrDefault(t => t.name.Contains("AnimalsMoose"));
            var sourceMoose = mooseParent.Find("Moose").gameObject;
            
            GameObject = GameObject.Instantiate(sourceMoose);
            routeSetter = GameObject.AddComponent<RouteSetter>();
            Ragdoll = GameObject.transform.Find("Offset").Cast<Transform>().FirstOrDefault(t => t.name == "MooseRagdoll").gameObject;

            GameObject.SetActive(true);
        }

        /// <summary>
        /// Use this to set the moose's start location and target point
        /// </summary>
        /// <param name="startingPoint"></param>
        /// <param name="target"></param>
        public void SetRoute(Vector3 startingPoint, GameObject target) {
            routeSetter.SetRoute(startingPoint, target);
        }

        /// <summary>
        /// This helps set the start and end targets for the moose.
        /// When activated, the moose seems to immediately go into a FSM state
        /// where it tries to randomize its route. So in order to have the moose
        /// start and run to a target, we may have to wait a moment until we set the values
        /// </summary>
        class RouteSetter : MonoBehaviour {
            private PlayMakerFSM moveFsm;

            public RouteSetter() {
                moveFsm = PlayMakerFSM.FindFsmOnGameObject(gameObject, "Move");
            }

            public void SetRoute(Vector3 startingPoint, GameObject target) {
                StartCoroutine(SetTargetRoutine(startingPoint, target));
            }

            private IEnumerator SetTargetRoutine(Vector3 startingPoint, GameObject target) {
                if (moveFsm.ActiveStateName == "Randomize route") {
                    //if the moose is randomizing, we need to wait a moment
                    //so that it'll actually be on some route, or it'll just disappear
                    //this wait must happen before setting the moose's position *and* its target object
                    yield return new WaitForSeconds(0.1f);
                }

                transform.position = startingPoint;

                //this makes the moose chase the specific target object
                var routeEnd = moveFsm.FsmVariables.GetFsmGameObject("RouteEnd");
                routeEnd.Value = target;
            }
        }
    }
}
