using System;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using System.Reflection;
using UnityEngine.XR;
using Photon.Pun;
using System.IO;
using System.Net;
using Photon.Realtime;
using UnityEngine.Rendering;

namespace FlyFast
{

    [BepInPlugin("org.Crafterbot.monkeytag.Flyfast", "Fly fast", "1.5.0")]
    public class MyPatcher : BaseUnityPlugin
    {
        public void Awake()
        {
            var harmony = new Harmony("com.Crafterbot.monkeytag.FlyFast");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }

    [HarmonyPatch(typeof(GorillaLocomotion.Player))]
    [HarmonyPatch("Update", MethodType.Normal)]

    public class code
    {
        static bool fly = false;
        static bool stop = false;
        static bool grip1 = false;
        static bool trigger1 = false;
        static bool active = false;
        static void Postfix(GorillaLocomotion.Player __instance)
        {
            if (!PhotonNetwork.CurrentRoom.IsVisible || !PhotonNetwork.InRoom)
            {
                List<InputDevice> list = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
                list[0].TryGetFeatureValue(CommonUsages.primaryButton, out fly);
                list[0].TryGetFeatureValue(CommonUsages.secondaryButton, out stop);
                list[0].TryGetFeatureValue(CommonUsages.gripButton, out grip1);
                list[0].TryGetFeatureValue(CommonUsages.triggerButton, out trigger1);
                //ghost mode idea hit left secondary for move through walls
                //config stuff
                string[] array = File.ReadAllLines("BepInEx\\plugins\\FlyFast\\config.txt");
                string Line2 = array[0];
                string Line3 = array[1];
                string Line4 = array[2];
                string Line9 = array[9];

                float gripMode1 = int.Parse(Line2);
                float gripMode2 = int.Parse(Line3);
                float gripMode3 = int.Parse(Line4);

                if (gripMode1 < 8001 && gripMode2 < 8001 && gripMode3 < 8001)
                {
                    active = true;
                }
                //flying stuff
                if (active)
                {
                    float speed = 1500;
                    if (fly & !grip1)
                    {
                        speed = gripMode1;
                    }
                    //put photon stuff back in
                    if (grip1 & fly)
                    {
                        speed = gripMode2;
                    }
                    if (trigger1 & grip1 & fly)
                    {
                        speed = gripMode3;
                    }



                    //stop
                    if (stop)
                    {
                        fly = false;
                        __instance.bodyCollider.attachedRigidbody.velocity = __instance.headCollider.transform.forward * Time.deltaTime;
                    }
                    //fly thingy
                    if (fly)
                    {
                        if (Line9 == "false")
                        {
                            __instance.bodyCollider.attachedRigidbody.velocity = __instance.headCollider.transform.forward * Time.deltaTime * speed;
                        }
                        else
                        {





                            __instance.bodyCollider.attachedRigidbody.velocity = __instance.rightHandTransform.transform.forward * Time.deltaTime * speed;
                        }
                    }
                }
            }
        }
    }
}


