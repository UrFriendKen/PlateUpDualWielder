using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenDualWielder
{
    public class TakeFromToolUserSecondHand : TransferInteractionProposalSystem, IModSystem
    {
        protected override bool IsPossible(ref InteractionData data)
        {
            CreateProposal(data.Target, data.Interactor, is_drop: false, data.Interactor);
            CreateProposal(data.Interactor, data.Target, is_drop: true, data.Interactor);
            return false;
        }

        private void CreateProposal(Entity from, Entity to, bool is_drop, Entity interactor)
        {
            if (Require(from, out CToolUserSecondHand toolUserSecondHand) && Has<CEquippableTool>(toolUserSecondHand.CurrentTool))
            {
                CreateProposal(interactor, toolUserSecondHand.CurrentTool, from, to, TransferFlags.Interaction | TransferFlags.NoReturns | TransferFlags.ToolSlot | (is_drop ? TransferFlags.Drop : TransferFlags.Null));
            }
        }

        public override void SendTransfer(Entity transfer, Entity acceptance, EntityContext ctx)
        {
            if (Require<CItemTransferProposal>(transfer, out CItemTransferProposal comp) && Require(comp.Source, out CToolUserSecondHand comp2))
            {
                ctx.Set(comp.Source, default(CToolUserSecondHand));
                ctx.Set(comp2.CurrentTool, default(CToolInUse));
            }
        }

        public override void ReceiveResult(Entity result, Entity transfer, Entity acceptance, EntityContext ctx)
        {
            if (Require<CItemTransferProposal>(transfer, out CItemTransferProposal comp) && Require(comp.Source, out CToolUserSecondHand _))
            {
                ctx.Set(comp.Source, new CToolUserSecondHand
                {
                    CurrentTool = result
                });
                ctx.Set(result, new CToolInUse
                {
                    User = comp.Source
                });
            }
        }

        public override void Tidy(EntityContext ctx, CItemTransferProposal proposal)
        {
        }
    }
}
