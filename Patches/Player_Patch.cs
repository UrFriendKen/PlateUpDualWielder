using HarmonyLib;
using Kitchen;
using Unity.Entities;

namespace KitchenDualWielder.Patches
{
    [HarmonyPatch]
    static class Player_Patch
    {
        [HarmonyPatch(typeof(Player), "CompleteJoining")]
        [HarmonyPostfix]
        static void CompleteJoining_Postfix(ref World ___World, ref Entity ___Entity)
        {
            ___World.EntityManager.AddComponent<CToolUserSecondHand>(___Entity);
        }
    }
}
