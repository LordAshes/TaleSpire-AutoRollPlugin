using BepInEx;
using HarmonyLib;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

namespace LordAshes
{
    public partial class AutoRollPlugin : BaseUnityPlugin
    {
        public static Action<int> callbackRollReady = null;
        public static Action callbackRollResult = null;

        private static System.Random random = new System.Random();

        [HarmonyPatch(typeof(UIDiceTray), "SetDiceUrl")]
        public static class Patches
        {
            public static bool Prefix(DiceRollDescriptor rollDescriptor)
            {
                return true;
            }

            public static void Postfix(DiceRollDescriptor rollDescriptor)
            {
                DiceManager dm = GameObject.FindObjectOfType<DiceManager>();
                foreach (DiceGroupDescriptor dgd in rollDescriptor.Groups)
                {
                    if (dgd.Name != "")
                    {
                        Debug.Log("Auto Roll Plugin: Spawning Dice");
                        foreach (DiceDescriptor dd in dgd.Dice)
                        {
                            UIDiceTray dt = GameObject.FindObjectOfType<UIDiceTray>();
                            bool saveSetting = (bool)PatchAssistant.GetField(dt, "_buttonHeld");
                            PatchAssistant.SetField(dt, "_buttonHeld", true);
                            dt.SpawnDice();
                            PatchAssistant.SetField(dt, "_buttonHeld", saveSetting);
                        }
                    }
                    else
                    {
                        Debug.Log("Auto Roll Plugin: Ignoring Manually Added Dice");
                    }
                }
            }
        }

        [HarmonyPatch(typeof(DiceManager), "CreateLocalRoll")]
        public static class PatchCreateLocalRoll
        {
            public static bool Prefix(DiceRollDescriptor rollDescriptor, bool isGmRoll)
            {
                return true;
            }

            public static void Postfix(DiceRollDescriptor rollDescriptor, bool isGmRoll, ref int __result)
            {
                string name = "";
                if(rollDescriptor.Groups!=null)
                {
                    if(rollDescriptor.Groups[0].Name!=null)
                    {
                        name = Convert.ToString(rollDescriptor.Groups[0].Name);
                    }
                }
                if (name != "")
                {
                    Debug.Log("Auto Roll Plugin: Dice Set '" + name + "' Ready ");
                    if (callbackRollResult != null) { callbackRollReady(__result); }
                }
                else
                {
                    Debug.Log("Auto Roll Plugin: Manual Dice Set Is Ready");
                }
            }
        }

        [HarmonyPatch(typeof(DiceManager), "RPC_DiceResult")]
        public static class PatchDiceResults
        {
            public static bool Prefix(bool isGmOnly, byte[] diceListData, PhotonMessageInfo msgInfo)
            {
                return true;
            }

            public static void Postfix(bool isGmOnly, byte[] diceListData, PhotonMessageInfo msgInfo)
            {
                if (callbackRollResult != null) { callbackRollResult(); }
            }
        }

        [HarmonyPatch(typeof(Die), "Spawn")]
        public static class PatchDieSpawn
        {
            public static bool Prefix(string resource, float3 pos, quaternion rot, int rollId, byte groupId, bool gmOnlyDie)
            {
                return false;
            }

            public static void Postfix(string resource, float3 pos, quaternion rot, int rollId, byte groupId, bool gmOnlyDie, ref Die __result)
            {
                object[] data = new object[]
                {
                    rollId,
                    groupId,
                    gmOnlyDie
                };
                Die component = PhotonNetwork.Instantiate(resource, pos, rot, 0, data).GetComponent<Die>();
                PatchAssistant.UseMethod(component, "Init", new object[] { rollId, groupId, gmOnlyDie }); // component.Init(rollId, groupId, gmOnlyDie);
                Vector3 orientation = new Vector3(random.Next(0, 180), random.Next(0, 180), random.Next(0, 180));
                Debug.Log("Auto Roll Plugin: Randomizing Die Starting Orientation ("+orientation.ToString()+")");
                component.transform.rotation = Quaternion.Euler(orientation);
                __result = component;
            }
        }

        public static class PatchAssistant
        {
            public static object GetProperty(object instance, string propertyName)
            {
                Type type = instance.GetType();
                foreach (PropertyInfo modifier in type.GetRuntimeProperties())
                {
                    if (modifier.Name.Contains(propertyName))
                    {
                        return modifier.GetValue(instance);
                    }
                }
                foreach (PropertyInfo modifier in type.GetProperties())
                {
                    if (modifier.Name.Contains(propertyName))
                    {
                        return modifier.GetValue(instance);
                    }
                }
                return null;
            }

            public static void SetProperty(object instance, string propertyName, object value)
            {
                Type type = instance.GetType();
                foreach (PropertyInfo modifier in type.GetRuntimeProperties())
                {
                    if (modifier.Name.Contains(propertyName))
                    {
                        modifier.SetValue(instance, value);
                        return;
                    }
                }
                foreach (PropertyInfo modifier in type.GetProperties())
                {
                    if (modifier.Name.Contains(propertyName))
                    {
                        modifier.SetValue(instance, value);
                        return;
                    }
                }
            }
            public static object GetField(object instance, string fieldName)
            {
                Type type = instance.GetType();
                foreach (FieldInfo modifier in type.GetRuntimeFields())
                {
                    if (modifier.Name.Contains(fieldName))
                    {
                        return modifier.GetValue(instance);
                    }
                }
                foreach (FieldInfo modifier in type.GetFields())
                {
                    if (modifier.Name.Contains(fieldName))
                    {
                        return modifier.GetValue(instance);
                    }
                }
                return null;
            }

            public static void SetField(object instance, string fieldName, object value)
            {
                Type type = instance.GetType();
                foreach (FieldInfo modifier in type.GetRuntimeFields())
                {
                    if (modifier.Name.Contains(fieldName))
                    {
                        modifier.SetValue(instance, value);
                        return;
                    }
                }
                foreach (FieldInfo modifier in type.GetFields())
                {
                    if (modifier.Name.Contains(fieldName))
                    {
                        modifier.SetValue(instance, value);
                        return;
                    }
                }
            }

            public static object UseMethod(object instance, string methodName, object[] parameters)
            {
                Type type = instance.GetType();
                foreach (MethodInfo modifier in type.GetRuntimeMethods())
                {
                    if (modifier.Name.Contains(methodName))
                    {
                        return modifier.Invoke(instance, parameters);
                    }
                }
                foreach (MethodInfo modifier in type.GetMethods())
                {
                    if (modifier.Name.Contains(methodName))
                    {
                        return modifier.Invoke(instance, parameters);
                    }
                }
                return null;
            }
        }
    }
}
