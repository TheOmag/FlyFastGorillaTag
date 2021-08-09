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



    [BepInPlugin("org.Crafterbot.monkeytag.Fly", "FlyFast", "1.0")]
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

public class class1
{
    static bool fly = false;
    static bool stop = false;
    static bool zeroG = false;
    static void Postfix(GorillaLocomotion.Player __instance)
    {
        if (!PhotonNetwork.CurrentRoom.IsVisible || !PhotonNetwork.InRoom)
        {
 
  

            List<InputDevice> list = new List<InputDevice>();
            InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
            list[0].TryGetFeatureValue(CommonUsages.primaryButton, out fly);
            InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
            list[0].TryGetFeatureValue(CommonUsages.secondaryButton, out stop);

            float speed = 1500;
           if(stop)
            {
                __instance.GetComponent<Rigidbody>().velocity = (__instance.headCollider.transform.forward * Time.deltaTime) * 0;
            }




            if (fly)
            {
                
                __instance.GetComponent<Rigidbody>().velocity = (__instance.headCollider.transform.forward * Time.deltaTime) * speed;
                __instance.bodyCollider.attachedRigidbody.useConeFriction = false;
                __instance.bodyCollider.attachedRigidbody.useGravity = false;
            } else
            {
                __instance.bodyCollider.attachedRigidbody.useConeFriction = true;
                __instance.bodyCollider.attachedRigidbody.useGravity = true;
            }
        }
    }
}