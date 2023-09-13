using HarmonyLib;
using Kitchen;
using System;
using System.Reflection;

namespace KitchenDualWielder.Patches
{
    [HarmonyPatch]
    static class AcceptIntoIntoToolUser_Patch
    {
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.FirstInner(typeof(AcceptIntoIntoToolUser), t => t.Name.Contains($"c__DisplayClass_OnUpdate_LambdaJob0"));
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OriginalLambdaBody"));
        }

        [HarmonyPrefix]
        static bool OriginalLambdaBody_Prefix(ref AcceptIntoIntoToolUser ___hostInstance, ref CItemTransferProposal proposal)
        {
            if (proposal.Status != ItemTransferStatus.Pruned &&
                PatchController.StaticRequire(proposal.Destination, out CToolUserSecondHand toolUserSecondHand) &&
                toolUserSecondHand.CurrentTool != default)
            {
                EntityContext ctx = new EntityContext(___hostInstance.EntityManager);
                return ItemHelpers.IsSingleHandedTool(ctx, proposal.Item);
            }
            return true;
        }
    }
}
