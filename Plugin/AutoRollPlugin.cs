using BepInEx;
using BepInEx.Configuration;
using Bounce.Singletons;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace LordAshes
{
    [BepInPlugin(Guid, Name, Version)]
    public partial class AutoRollPlugin : BaseUnityPlugin
    {
        // Plugin info
        public const string Name = "Auto Roll Plug-In";
        public const string Guid = "org.lordashes.plugins.autoroll";
        public const string Version = "1.0.3.0";

        private List<int> rollIds = new List<int>();

        /// <summary>
        /// Function for initializing plugin
        /// This function is called once by TaleSpire
        /// </summary>
        void Awake()
        {
            UnityEngine.Debug.Log("Auto Roll Plugin: Dice added via talespire://dice protocol will be auto added and rolled.");
            UnityEngine.Debug.Log("Auto Roll Plugin: For example:");
            UnityEngine.Debug.Log("Auto Roll Plugin: talespire://dice/Investigation:1D20+5");
            UnityEngine.Debug.Log("Auto Roll Plugin: talespire://dice/Attack:1D20+5/2D6+3");

            callbackRollReady = AutoRoll;
            callbackRollResult = AutoHide;
            var harmony = new Harmony(Guid);
            harmony.PatchAll();

            Utility.PostOnMainPage(this.GetType());
        }

        /// <summary>
        /// Function for determining if view mode has been toggled and, if so, activating or deactivating Character View mode.
        /// This function is called periodically by TaleSpire.
        /// </summary>
        void Update()
        {
        }

        public void AutoRoll(int rollId)
        {
            rollIds.Add(rollId);
            StartCoroutine((IEnumerator)DelayedDiceSetRoll(rollId));
        }

        public void AutoHide()
        {
            float hideDelay = Config.Bind("Setting", "Hide Delay In Seconds", 3f).Value;
            if (hideDelay >= 0)
            {
                StartCoroutine((IEnumerator)DelayedDiceSetHide(hideDelay));
            }
        }

        private IEnumerator DelayedDiceSetRoll(int rollId)
        {
            yield return new WaitForSeconds(0.500f);
            DiceManager dm = GameObject.FindObjectOfType<DiceManager>();
            Debug.Log("Auto Roll Plugin: Rolling Dice");
            float extraBounceHeight = Config.Bind("Setting", "Extra Bounce Height", 0.0f).Value;
            if (extraBounceHeight > 0.0f) { dm.GatherDice(SimpleSingletonBehaviour<CameraController>.Instance.transform.position + (extraBounceHeight * Vector3.up), rollId); }
            dm.ThrowDice(rollId);
        }

        private IEnumerator DelayedDiceSetHide(float delayHide)
        {
            yield return new WaitForSeconds(delayHide);
            DiceManager dm = GameObject.FindObjectOfType<DiceManager>();
            Debug.Log("Auto Roll Plugin: Clearing Dice");
            dm.ClearAllDice(rollIds.ElementAt(0));
            rollIds.RemoveAt(0);
        }
    }
}
