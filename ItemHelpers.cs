using Kitchen;
using KitchenData;
using Unity.Entities;

namespace KitchenDualWielder
{
    internal static class ItemHelpers
    {
        public static bool IsSingleHandedTool(EntityContext ctx, Entity entity)
        {
            if (!ctx.Require(entity, out CEquippableTool equippableTool) || !equippableTool.CanHoldItems)
                return false;
            if (!ctx.Require(entity, out CItem item) || !GameData.Main.TryGet(item.ID, out Item itemGDO) || (itemGDO.HoldPose != ToolAttachPoint.Hand && itemGDO.HoldPose != ToolAttachPoint.HandFlat))
                return false;
            return true;
        }
    }
}
