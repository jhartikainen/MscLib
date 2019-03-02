using UnityEngine;
using System.Collections;
using System.Linq;

namespace MscLib {
    public class Dancer {
        public GameObject GameObject { get; private set; }

        public enum DancerType { DANCER_1 = 0, DANCER_2 = 1, DANCER_COUPLE_1 = 2, DANCER_COUPLE_2 = 3 };

        public Dancer(DancerType dancerType) {
            var dancehall = GameObject.Find("DANCEHALL");
            var dancers = dancehall.transform.Cast<Transform>()
                .FirstOrDefault(t => t.name == "Functions")
                .transform.Cast<Transform>()
                .Where(t => t.name == "Dancer" || t.name == "DanceCouple")
                .Select(t => t.gameObject);

            //the enum is arranged by index
            GameObject = GameObject.Instantiate(dancers.ElementAt((int)dancerType));
        }
    }
}