using GameSystem;
using UI.WorldUI;
using UnityEngine;

namespace Game
{
    public interface IGardenPlot
    {
        void SetWaterUIActivate(bool activate);
    }
    
    public class GardenPlot : Object, IGardenPlot,
        WaterWorldUI.IListener
    {
        [SerializeField] private SpriteRenderer corpSpriteRenderer = null;
        
        private WaterWorldUI _waterWorldUI = null;
        private SowSeeds _sowSeeds = null;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            Debug.Log(data.ObjectUniqueID);
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            WaterWorldUIDeactivate();
            SowSeedsDeactivate();
        }

        protected override void SetSortingOrder(int order)
        {
            base.SetSortingOrder(order);

            if (corpSpriteRenderer != null)
            {
                corpSpriteRenderer.sortingOrder = order;
            }
        }

        private void WaterWorldUIDeactivate()
        {
            if (_waterWorldUI == null)
                return;
            
            _waterWorldUI.Deactivate();
            ObjectPooler.Instance?.Return(_waterWorldUI);
            _waterWorldUI = null;
        }
        
        private void SowSeedsDeactivate()
        {
            if (_sowSeeds == null)
                return;
            
            _sowSeeds.Deactivate();
            ObjectPooler.Instance?.Return(_sowSeeds);
            _sowSeeds = null;
        }
        
        #region IGardenPlot
        void IGardenPlot.SetWaterUIActivate(bool activate)
        {
            if (_data.ObjectType != Type.ObjectType.Garden)
                return;

            if (_waterWorldUI == null)
            {
                var data = new UI.WorldUI.WaterWorldUI.Data();
                
                data.WithListener(this)
                    .WithTargetTm(transform)
                    .WithOffset(new Vector2(0, 120f));

                _waterWorldUI = new GameSystem.ComponentCreator<UI.WorldUI.WaterWorldUI, UI.WorldUI.WaterWorldUI.Data>()
                    .SetRootRectTm(UIManager.Instance?.WorldUIRootRectTr)
                    .SetData(data)
                    .Create();
            }
            
            if (activate)
                _waterWorldUI?.Activate();
            else
                _waterWorldUI?.Deactivate();
        }
        #endregion
        
        #region WaterWorldUI.IListener

        void WaterWorldUI.IListener.OnClick()
        {
            if (_sowSeeds == null)
            {
                var data = new UI.WorldUI.SowSeeds.Data
                {

                };

                data.WithTargetTm(transform)
                    .WithOffset(new Vector2(0, 90f));

                _sowSeeds = new GameSystem.ComponentCreator<UI.WorldUI.SowSeeds, UI.WorldUI.SowSeeds.Data>()
                    .SetRootRectTm(UIManager.Instance?.WorldUIRootRectTr)
                    .SetData(data)
                    .Create();
                _sowSeeds?.Activate();
            }
        }
        #endregion
    }
}

