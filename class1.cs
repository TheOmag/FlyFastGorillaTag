using System;
using System.Collections.Generic;
using HarmonyLib;
using BepInEx;
using UnityEngine;
using System.Reflection;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.UI;
using System.IO;
using System.Net;
using Photon.Realtime;
using UnityEngine.Rendering;

namespace FlyFast
{

    [BepInPlugin("org.Crafterbot.monkeytag.Flyfast", "Fly fast", "1.6.1")]
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

        public static bool FlyFastMod = true;
        private static bool primaryButton1 = false;
        private static bool secondaryButton1 = false;
        private static bool grip1 = false;
        private static bool trigger1 = false;
        private static bool active = false;
        private static bool hitOneOff = false;
        private static Vector3 savePos = new Vector3(0f, 0f, 0f);
        private static GameObject dot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        static void Postfix(GorillaLocomotion.Player __instance)
        {
            if (!PhotonNetwork.CurrentRoom.IsVisible || !PhotonNetwork.InRoom)
            {
                List<InputDevice> list = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.HeldInHand | UnityEngine.XR.InputDeviceCharacteristics.Right | UnityEngine.XR.InputDeviceCharacteristics.Controller, list);
                list[0].TryGetFeatureValue(CommonUsages.primaryButton, out primaryButton1);
                list[0].TryGetFeatureValue(CommonUsages.secondaryButton, out secondaryButton1);
                list[0].TryGetFeatureValue(CommonUsages.gripButton, out grip1);
                list[0].TryGetFeatureValue(CommonUsages.triggerButton, out trigger1);

                //config stuff
                string[] line = File.ReadAllLines("BepInEx\\plugins\\FlyFast\\config.txt");
                string Line2 = line[0];
                string Line3 = line[1];
                string Line4 = line[2];
                string Line9 = line[9];
                string Line12 = line[12];

                float gripMode1 = int.Parse(Line2);
                float gripMode2 = int.Parse(Line3);
                float gripMode3 = int.Parse(Line4);

                if (gripMode1 <= 8000 && gripMode2 <= 8000 && gripMode3 <= 8000)
                {
                    active = true;
                }
                //flying stuff
                if (active)
                {
                    float speed = 1500;
                    if (primaryButton1 & !grip1)
                    {
                        speed = gripMode1;
                    }
                    //put photon stuff back in
                    if (grip1 & primaryButton1)
                    {
                        speed = gripMode2;
                    }
                    if (trigger1 & grip1 & primaryButton1)
                    {
                        speed = gripMode3;
                    }



                    //stop

                    if (secondaryButton1)
                    {
                        primaryButton1 = false;
                        __instance.bodyCollider.attachedRigidbody.velocity = new Vector3(0f, 0.149f, 0f);
                    }

                    //fly thingy

                    if (grip1 && !primaryButton1 && Line12 == "true")
                    {
                        Ray FlyPoint = new Ray(__instance.rightHandTransform.position, __instance.rightHandTransform.forward);
                        Physics.Raycast(FlyPoint, 1000);
                        RaycastHit hit;

                        dot.AddComponent<MeshRenderer>();
                        dot.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);

                        if (Physics.Raycast(FlyPoint, out hit))
                        {
                            //point
                            if (!hitOneOff)
                            {
                                dot.transform.position = hit.point;
                                dot.GetComponent<Renderer>().material.SetColor("_color", Color.white);
                                savePos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                            }
                            else
                            {
                                dot.transform.position = savePos;
                                dot.GetComponent<Renderer>().material.SetColor("_color", Color.red);
                                dot.AddComponent<MeshRenderer>();
                            }

                            if (trigger1)
                            {
                                hitOneOff = true;
                            }
                        }
                        else
                        {
                            if (!hitOneOff)
                            {
                                GameObject.Destroy(dot.GetComponent<MeshRenderer>());
                            }
                        }
                        if (trigger1 && hitOneOff)
                        {
                            dot.AddComponent<MeshRenderer>();
                            __instance.bodyCollider.attachedRigidbody.velocity = (savePos - __instance.bodyCollider.transform.position) * Time.deltaTime * 200f;
                            if (__instance.bodyCollider.transform.position == savePos)
                            {
                                grip1 = false;
                            }
                        }
                        if (hitOneOff && !trigger1)
                        {
                            __instance.bodyCollider.attachedRigidbody.velocity = new Vector3(0f, 0.149f, 0f);
                        }

                    }
                    else
                    {
                        GameObject.Destroy(dot.GetComponent<MeshRenderer>());
                        GameObject.Destroy(dot.GetComponent<Collider>());
                        hitOneOff = false;
                    }

                    if (primaryButton1)
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

