using KitchenMods;
using Unity.Entities;

namespace KitchenDualWielder
{
    public struct CToolUserSecondHand : IComponentData, IModComponent
    {
        public Entity CurrentTool;
    }
}
