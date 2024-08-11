using HarmonyLib;
using Kitchen;
using UnityEngine;

namespace KitchenDualWielder.Patches
{
    [HarmonyPatch]
    static class LocalViewRouter_Patch
    {
        [HarmonyPatch(typeof(LocalViewRouter), "GetPrefab")]
        [HarmonyPostfix]
        static void GetPrefab_Postfix(ViewType view_type, ref GameObject __result)
        {
            if (view_type == ViewType.Player && __result != null && __result.GetComponentInChildren<PlayerSecondToolView>() == null)
            {
                Transform transform = __result.transform;
                PlayerSecondToolView secondToolView = transform.Find("Holding")?.gameObject.AddComponent<PlayerSecondToolView>();
                
                if (secondToolView != null)
                {
                    Transform upperSpine = __result.transform
                        .Find("MorphmanPlus")?
                        .Find("Armature")?
                        .Find("LowerSpine")?
                        .Find("Spine")?
                        .Find("UpperSpine");
                    
                    if (upperSpine != null)
                    {
                        Transform handR = upperSpine
                            .Find("Shoulder.R")?
                            .Find("UpperArm.R")?
                            .Find("LowerArm.R")?
                            .Find("Hand.R");
                        Transform handHoldPointR = handR?.Find("HandHoldPoint");
                        Transform handHoldPointFlatR = handR?.Find("HandHoldPoint Flat");



                        Transform handL = upperSpine
                            .Find("Shoulder.L")?
                            .Find("UpperArm.L")?
                            .Find("LowerArm.L")?
                            .Find("Hand.L");

                        if (handL != null)
                        {
                            if (handHoldPointR != null)
                            {
                                Transform handHoldPointL = GameObject.Instantiate(handHoldPointR.gameObject).transform;
                                handHoldPointL.name = handHoldPointR.name;
                                handHoldPointL.SetParent(handL);
                                handHoldPointL.localPosition = new Vector3(-handHoldPointR.localPosition.x, handHoldPointR.localPosition.y, -handHoldPointR.localPosition.z);
                                Vector3 rotationEuler = handHoldPointR.localRotation.eulerAngles;
                                handHoldPointL.localRotation = Quaternion.Euler(rotationEuler.x, rotationEuler.y + 180f, rotationEuler.z);
                                handHoldPointL.localScale = handHoldPointR.localScale;
                                secondToolView.ToolContainer = handHoldPointL;
                            }

                            if (handHoldPointFlatR != null)
                            {
                                Transform handHoldPointFlatL = GameObject.Instantiate(handHoldPointFlatR).transform;
                                handHoldPointFlatL.name = handHoldPointFlatR.name;
                                handHoldPointFlatL.SetParent(handL);
                                handHoldPointFlatL.localPosition = new Vector3(-handHoldPointFlatR.localPosition.x, handHoldPointFlatR.localPosition.y, -handHoldPointFlatR.localPosition.z);
                                Vector3 rotationEuler = handHoldPointFlatR.localRotation.eulerAngles;
                                handHoldPointFlatL.localRotation = Quaternion.Euler(rotationEuler.x, rotationEuler.y + 180f, rotationEuler.z);
                                handHoldPointFlatL.localScale = handHoldPointFlatR.localScale;
                                secondToolView.ToolContainerFlat = handHoldPointFlatL;
                            }
                        }
                    }
                }
                
            }
        }
    }
}
