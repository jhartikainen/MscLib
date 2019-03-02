using UnityEngine;
using System.Collections;
using HutongGames.PlayMaker;
using System;
using MSCLoader;
using System.Linq;

namespace MscLib {
    public class Player {
        private static Player instance;

        private GameObject gameObject;
        private CharacterController characterController;
        private PlayMakerFSM swearFSM;
        private GameObject camera;
        private PlayMakerFSM cameraFSM;
        private PlayMakerFSM pissFSM;
        private PlayMakerFSM drinkFSM;
        private PlayMakerFSM hitboxFsm;

        public static Transform Transform => instance.gameObject.transform;
        public static GameObject GameObject => instance.gameObject;

        /// <summary>
        /// Is player in driver or passenger mode in a vehicle?
        /// </summary>
        public static bool IsDrivingOrPassenger => Transform.parent != null;

        public static DrivableVehicle CurrentVehicle => Vehicles.Find(Transform.root.gameObject);

        /// <summary>
        /// Attaching things into this will keep them aligned with what the player is looking at
        /// </summary>
        public static GameObject Camera => instance.camera;

        /// <summary>
        /// Drunkenness level. Not sure of the scale, but 3.5f is fairly drunk and by 5f you start to black out
        /// </summary>
        public static float DrunkLevel {
            get {
                return instance.FsmFloat("PlayerDrunk");
            }
            set {
                //these values can't be negative
                instance.FsmFloat("PlayerDrunk", Mathf.Max(0, value));
            }
        }

        /// <summary>
        /// 100 is max white, before meter turns red.
        /// 200 should be max before death but setting this to 201 will not kill player
        /// </summary>
        public static float UrineLevel {
            get {
                return instance.FsmFloat("PlayerUrine");
            }
            set {
                instance.FsmFloat("PlayerUrine", Mathf.Max(0, value));
            }
        }

        /// <summary>
        /// Different item types that can be triggered with a drinking animation and drinking effects
        /// </summary>
        public enum DrinkableItem { BEER, SPIRITS, MILK, VIINA };

        private Player() {
            gameObject = GameObject.Find("PLAYER");
            characterController = gameObject.GetComponent<CharacterController>();
            swearFSM = GameObject.Find("PLAYER/Pivot/Camera/FPSCamera/SpeakDatabase").GetComponent<PlayMakerFSM>();
            camera = GameObject.Find("PLAYER/Pivot/Camera/FPSCamera");
            cameraFSM = camera.GetComponent<PlayMakerFSM>();
            pissFSM = GameObject.Find("PLAYER/Pivot/Camera/FPSCamera/Piss").GetComponent<PlayMakerFSM>();
            drinkFSM = GameObject.Find("PLAYER/Pivot/Camera/FPSCamera/FPSCamera/Drink").GetComponent<PlayMakerFSM>();

            var fightTrigger = gameObject.transform.Find("Pivot/Camera/FPSCamera").Cast<Transform>().FirstOrDefault(t => t.name == "FightTrigger").gameObject;
            hitboxFsm = fightTrigger.GetComponent<PlayMakerFSM>();
        }

        /// <summary>
        /// Call me in your mod's OnLoad or this isn't going to work
        /// </summary>
        public static void Initialize() {
            //this stuff might remain instanciated if the player exists to menu
            //and loads back into the game. Seems best way to check it is
            //to just compare if the gameobjects are what they should be
            if(instance == null || instance.gameObject != GameObject.Find("PLAYER")) {
                instance = new Player();
            }            
        }

        private float FsmFloat(string name) {
            return FsmVariables.GlobalVariables.FindFsmFloat(name).Value;
        }

        private void FsmFloat(string name, float value) {
            FsmVariables.GlobalVariables.FindFsmFloat(name).Value = value;
        }

        public static void Swear() {            
            instance.swearFSM.SendEvent("SWEARING");
        }
    
        /// <summary>
        /// You may want to set UrineLevel before using this
        /// if you want to ensure the player will have something to pee from
        /// </summary>
        public static void Piss() {            
            //for some reason we need to send both of these events
            //in order for this to work
            instance.pissFSM.SendEvent("WAKEUP");
            instance.pissFSM.SendEvent("FINISHED");
        }

        public static void Punch() {
            instance.cameraFSM.SendEvent("FIST");
        }

        public static void Middlefinger() {
            instance.cameraFSM.SendEvent("MIDDLEFINGER");
        }

        /// <summary>
        /// Triggers drinking animation + any effects from drinking
        /// </summary>
        /// <param name="drink"></param>
        public static void Drink(DrinkableItem drink) {
            instance.drinkFSM.SendEvent("DRINK" + drink.ToString());
        }


        public static void Move(Vector3 movement) {            
            instance.characterController.Move(movement);
        }

        /// <summary>
        /// If you spawn another dancehall fighter using the Fighter class, you can use
        /// this to set that fighter as the opponent. If this is not done, the punches from
        /// the new fighter will not register with the player.
        /// </summary>
        /// <param name="f"></param>
        public static void SetFightOpponent(Fighter f) {
            //without setting these, the fighter's punches won't register unless it's the default fighter            
            instance.hitboxFsm.FsmVariables.FindFsmGameObject("Fighter").Value = f.GameObject;
            instance.hitboxFsm.FsmVariables.FindFsmGameObject("FighterFist").Value = f.Fist;
            instance.hitboxFsm.FsmVariables.FindFsmGameObject("Fist").Value = f.Fist;
        }
    }
}
