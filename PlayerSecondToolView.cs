using Kitchen;
using KitchenData;
using KitchenMods;
using MessagePack;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace KitchenDualWielder
{
    public class PlayerSecondToolView : UpdatableObjectView<PlayerSecondToolView.ViewData>
    {
        public class UpdateView : IncrementalViewSystemBase<ViewData>, IModSystem
        {
            EntityQuery Views;

            protected override void Initialise()
            {
                base.Initialise();
                Views = GetEntityQuery(typeof(CPlayer), typeof(CToolUserSecondHand), typeof(CLinkedView));
            }

            protected override void OnUpdate()
            {
                using NativeArray<CToolUserSecondHand> secondHands = Views.ToComponentDataArray<CToolUserSecondHand>(Allocator.Temp);
                using NativeArray<CLinkedView> views = Views.ToComponentDataArray<CLinkedView>(Allocator.Temp);

                for (int i = 0; i < views.Length; i++)
                {
                    CLinkedView view = views[i];
                    CToolUserSecondHand secondHand = secondHands[i];

                    int toolID = 0;
                    if (secondHand.CurrentTool != default && Require<CItem>(secondHand.CurrentTool, out CItem item))
                        toolID = item.ID;
                    SendUpdate(view, new ViewData()
                    {
                        UsingToolID = toolID
                    });
                }
            }
        }

        [MessagePackObject(false)]
        public struct ViewData : ISpecificViewData, IViewData, IViewResponseData, IViewData.ICheckForChanges<ViewData>
        {
            [Key(0)]
            public int UsingToolID;

            public bool IsChangedFrom(ViewData check)
            {
                return UsingToolID != check.UsingToolID;
            }

            public IUpdatableObject GetRelevantSubview(IObjectView view) => view.GetSubView<PlayerSecondToolView>();
        }

        public Transform ToolContainer;

        public Transform ToolContainerFlat;

        private ViewData Data;

        public GameObject CurrentTool;

        protected override void UpdateData(ViewData data)
        {
            if (data.UsingToolID != Data.UsingToolID)
            {
                if (CurrentTool != null)
                    GameObject.DestroyImmediate(CurrentTool);

                if (data.UsingToolID != 0 && GameData.Main.TryGet(data.UsingToolID, out Item item))
                {
                    CurrentTool = GameObject.Instantiate(item.Prefab);
                    Transform toolTransform = CurrentTool.transform;
                    if (item.HoldPose == ToolAttachPoint.HandFlat && ToolContainerFlat != null)
                    {
                        toolTransform.SetParent(ToolContainerFlat.transform);
                    }
                    else if (ToolContainer != null)
                    {
                        toolTransform.SetParent(ToolContainer.transform);
                    }
                    toolTransform.localPosition = Vector3.zero;
                    toolTransform.localRotation = Quaternion.identity;
                    toolTransform.localScale = Vector3.one;
                    CurrentTool.SetActive(true);
                }
            }
            Data = data;
        }
    }
}