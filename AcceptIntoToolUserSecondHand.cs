using Kitchen;
using KitchenMods;
using Unity.Collections;
using Unity.Entities;

namespace KitchenDualWielder
{
    public class AcceptIntoToolUserSecondHand : TransferAcceptSystem, IModSystem
    {
        EntityQuery Proposals;

        protected override void Initialise()
        {
            base.Initialise();
            Proposals = GetEntityQuery(typeof(CItemTransferProposal));
        }

        public override void AcceptTransfer(Entity proposal_entity, Entity acceptance, EntityContext ctx, out Entity return_item)
        {
            return_item = default;
            if (Require(proposal_entity, out CItemTransferProposal comp))
            {
                ctx.Set(comp.Destination, new CToolUserSecondHand
                {
                    CurrentTool = comp.Item
                });
                ctx.Set(comp.Item, new CToolInUse
                {
                    User = comp.Destination
                });
                ctx.Set(comp.Item, default(CHeldBy));
            }
        }

        protected override void OnUpdate()
        {
            using NativeArray<Entity> entities = Proposals.ToEntityArray(Allocator.Temp);
            using NativeArray<CItemTransferProposal> proposals = Proposals.ToComponentDataArray<CItemTransferProposal>(Allocator.Temp);
            EntityContext ctx = new EntityContext(EntityManager);
            for (int i = 0; i < proposals.Length; i++)
            {
                Entity e = entities[i];
                CItemTransferProposal proposal = proposals[i];
                if (proposal.Status == ItemTransferStatus.Pruned ||
                    (proposal.Flags & TransferFlags.RequireMerge) != 0)
                {
                    continue;
                }
                if (!ItemHelpers.IsSingleHandedTool(ctx, proposal.Item))
                    continue;
                if (!Require(proposal.Destination, out CToolUser toolUser) ||
                    toolUser.CurrentTool == default)
                {
                    continue;
                }
                if (!ItemHelpers.IsSingleHandedTool(ctx, toolUser.CurrentTool))
                    continue;
                if (!Require(proposal.Destination, out CToolUserSecondHand toolUserSecondHand) ||
                    toolUserSecondHand.CurrentTool != default)
                {
                    continue;
                }
                Accept(e, TransferFlags.ToolSlot);
            }
        }
    }
}
