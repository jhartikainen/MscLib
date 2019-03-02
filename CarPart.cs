using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MSCLoader;

namespace MscLib {
    public class CarPart {
        private PlayMakerFSM dbFsm;
        private PlayMakerFSM tightnessFsm;
        private IRemovalMethod removalMethod;

        public string Name => PartsDbEntry.name;
        public bool IsAttached => dbFsm.FsmVariables.GetFsmBool("Installed").Value;
        public float Tightness {
            get => tightnessFsm == null ? 0 : tightnessFsm.FsmVariables.GetFsmFloat("Tightness").Value;
            set => tightnessFsm.FsmVariables.GetFsmFloat("Tightness").Value = value;
        }
        public GameObject PartsDbEntry { get; private set; }

        private CarPart() { }

        public static CarPart TryCreate(GameObject partsDbEntry) {
            var dbFsm = PlayMakerFSM.FindFsmOnGameObject(partsDbEntry, "Data");
            if (dbFsm == null) {
                ModConsole.Print("No DBData");
                return null;
            }

            //the actual useful logic is linked in another gameobject, which is
            //referred by either one of these
            var thisPart = dbFsm.FsmVariables.GetFsmGameObject("ThisPart").Value;
            var activateThis = dbFsm.FsmVariables.GetFsmGameObject("ActivateThis").Value;
            var activateThis1 = dbFsm.FsmVariables.GetFsmGameObject("ActivateThis1").Value;
            var activateThis2 = dbFsm.FsmVariables.GetFsmGameObject("ActivateThis2").Value;

            //we could have multiple of these so we'll just stick 'em all into a list
            //and filter out everything that's null and that should work
            var logicObjects = (new List<GameObject>() { thisPart, activateThis, activateThis1, activateThis2 })
                .Where(v => v != null)
                .ToList();

            if (logicObjects.Count == 0) {
                ModConsole.Print("NO LOGIC OBJECTS");
                return null;
            }


            //If the part is not attached to anything, the FSM containing this data
            //is sometimes disabled. By using GetComponents, we can get disabled components too.
            var fsms = logicObjects.SelectMany(l => l.GetComponents<PlayMakerFSM>());

            //this might or might not, depending on if this part uses bolts or not
            //(some bolted parts use some weird ass method for tracking bolts that doesn't use this)
            var tightnessFsm = fsms.FirstOrDefault(f => f.FsmVariables.GetFsmFloat("Tightness") != null);

            var o = new CarPart();
            o.dbFsm = dbFsm;
            o.PartsDbEntry = partsDbEntry;
            o.tightnessFsm = tightnessFsm;

            //majority of parts seem to use this FSM based removal
            //sometimes these aren't on the logic object itself, but
            //rather in one of its children so look in children as well
            var removalFsms = fsms
                .Concat(logicObjects.SelectMany(l => l.transform.Cast<Transform>().SelectMany(t => t.gameObject.GetComponents<PlayMakerFSM>())))
                .Where(f => f.FsmName == "Removal");
            if (removalFsms.Count() > 0) {
                o.removalMethod = new FsmRemoval(removalFsms);
            }
            else {
                ModConsole.Print("Using bolt method for " + partsDbEntry.name);
                //but not all, so try to use bolt removal method instead...
                //in this case we should not have more than 1 logic object (hopefully)
                var bolts = logicObjects[0].transform.Cast<Transform>().Where(t => t.name == "BoltPM").Select(t => t.gameObject).ToList();
                o.removalMethod = new BoltsRemoval(bolts, partsDbEntry);
            }

            return o;
        }

        public void Remove() {
            removalMethod.Remove();
        }

        interface IRemovalMethod {
            void Remove();
        }

        class FsmRemoval : IRemovalMethod {
            private IEnumerable<PlayMakerFSM> fsms;

            public FsmRemoval(IEnumerable<PlayMakerFSM> fsms) {
                this.fsms = fsms;
            }

            public void Remove() {
                foreach (var fsm in fsms) {
                    fsm.SendEvent("REMOVE");
                }
            }
        }

        class BoltsRemoval : IRemovalMethod {
            private List<PlayMakerFSM> screwFsms;

            public BoltsRemoval(List<GameObject> bolts, GameObject g) {
                if (bolts.Count == 0) {
                    ModConsole.Print("No bolts? " + g.name);
                }

                screwFsms = bolts
                    .Select(b => b.GetComponents<PlayMakerFSM>().FirstOrDefault(f => f.FsmName == "Screw"))
                    .Where(f => f != null)
                    .ToList();
            }

            public void Remove() {
                //untightening all bolts by one should effectively mean it'll pop off at some point
                screwFsms.ForEach(s => s.SendEvent("UNTIGHTEN"));
            }
        }
    }
}