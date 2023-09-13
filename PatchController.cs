using Kitchen;
using KitchenMods;
using Unity.Entities;

namespace KitchenDualWielder
{
    public class PatchController : GenericSystemBase, IModSystem
    {
        static PatchController _instance;

        protected override void Initialise()
        {
            base.Initialise();
            _instance = this;
        }

        protected override void OnUpdate()
        {
        }

        internal static bool StaticHas<T>(bool errorVal = false) where T : struct, IComponentData
        {
            return _instance?.Has<T>() ?? errorVal;
        }

        internal static bool StaticHas<T>(Entity e, bool errorVal = false) where T : struct, IComponentData
        {
            return _instance?.Has<T>(e) ?? errorVal;
        }

        internal static bool StaticRequire<T>(out T comp, bool errorVal = false) where T : struct, IComponentData
        {
            comp = default;
            return _instance?.Require(out comp) ?? errorVal;
        }

        internal static bool StaticRequire<T>(Entity e, out T comp, bool errorVal = false) where T : struct, IComponentData
        {
            comp = default;
            return _instance?.Require<T>(e, out comp) ?? errorVal;
        }
    }
}
