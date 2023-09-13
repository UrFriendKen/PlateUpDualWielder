using HarmonyLib;
using Kitchen;
using System;
using System.Reflection;
using Unity.Entities;

namespace KitchenDualWielder.Patches
{
    [HarmonyPatch]
    static class PruneByEquippableTool_Patch
    {
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.FirstInner(typeof(PruneByEquippableTool), t => t.Name.Contains($"c__DisplayClass_OnUpdate_LambdaJob0"));
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OriginalLambdaBody"));
        }

        [HarmonyPrefix]
        static bool OriginalLambdaBody_Prefix(ref PruneByEquippableTool ___hostInstance, Entity e, ref CItemTransferAccept accept)
        {
            if (accept.Status != ItemAcceptStatus.Pruned &&
                PatchController.StaticRequire(accept.Proposal, out CItemTransferProposal comp) &&
                PatchController.StaticRequire(comp.Destination, out CToolUser toolUser) &&
                PatchController.StaticRequire(comp.Destination, out CToolUserSecondHand toolUser2))
            {
                CEquippableTool equippableTool1;
                bool hasEquippableTool1 = PatchController.StaticRequire(toolUser.CurrentTool, out equippableTool1);
                CEquippableTool equippableTool2;
                bool hasEquippableTool2 = PatchController.StaticRequire(toolUser2.CurrentTool, out equippableTool2);
                CEquippableTool targetEquippable;
                bool targetIsEquippable = PatchController.StaticRequire(comp.Item, out targetEquippable);

                if (targetIsEquippable &&
                    ((PatchController.StaticRequire(comp.Destination, out CAttemptingInteraction comp5) && comp5.IsHeld) ||
                    ((accept.Flags & TransferFlags.ToolSlot) == 0)))
                {
                    return true;
                }
                if (!targetIsEquippable ||
                    !hasEquippableTool1 ||
                    (!hasEquippableTool2 && !ItemHelpers.IsSingleHandedTool(___hostInstance.EntityManager, comp.Item)))
                {
                    return true;
                }

                if (hasEquippableTool2)
                {
                    accept.Status = ItemAcceptStatus.Pruned;
                }
                if (accept.Status == ItemAcceptStatus.Pruned)
                {
                    accept.PrunedBy = ___hostInstance;
                    return false;
                }
            }
            return true;
        }
    }
}
