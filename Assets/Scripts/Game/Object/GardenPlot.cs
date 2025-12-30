using UnityEngine;
using System;
using System.Threading;
using Cysharp.Threading.Tasks;

using GameSystem;
using Info;
using UI.WorldUI;

namespace Game
{
    public interface IGardenPlot
    {
        void SetWaterUIActivate(bool activate);
        void SetPlotListener(IPlotListener plotListener);
        void SetPlotDataProvider(IPlotDataProvider plotDataProvider);
    }
    
    public class GardenPlot : Object, IGardenPlot,
        WaterWorldUI.IListener
    {
        [SerializeField] private SpriteRenderer corpSpriteRenderer = null;
        
        private WaterWorldUI _waterWorldUI = null;
        private ISowSeeds _sowSeeds = null;

        private IPlotListener _plotListener = null;
        private IPlotDataProvider _plotDataProvider = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            Debug.Log(data.ObjectUniqueID);
        }

        public override void Activate()
        {
            base.Activate();

            if (_plotDataProvider.IsGrowing(_data?.ObjectUniqueID))
            {
                EnsureSowSeeds();
                UpdateRemainingGrowthTimeAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
        }

        public override void Deactivate()
        {
            base.Deactivate();
            
            WaterWorldUIDeactivate();
            SowSeedsDeactivate();
        }

        protected override void Remove(bool refresh)
        {
            base.Remove(refresh);

            _plotListener?.OnPlotRemoved(_data?.ObjectUniqueID);
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
            ObjectPooler.Instance?.Return(_sowSeeds.Poolable);
            _sowSeeds = null;
        }

        private void EnsureWaterWorldUI()
        {
            if (_plotDataProvider == null)
                return;

            if (_plotDataProvider.IsGrowing(_data?.ObjectUniqueID))
                return;

            if (_waterWorldUI != null)
                return;

            var data = new UI.WorldUI.WaterWorldUI.Data();

            data.WithListener(this)
                .WithTargetTm(transform)
                .WithOffset(new Vector2(0, 120f));

            _waterWorldUI = new GameSystem.ComponentCreator<UI.WorldUI.WaterWorldUI, UI.WorldUI.WaterWorldUI.Data>()
                .SetRootRectTm(UIManager.Instance?.WorldUIRootRectTr)
                .SetData(data)
                .Create();
        }

        private void EnsureSowSeeds()
        {
            if (_sowSeeds != null)
                return;

            var data = new UI.WorldUI.SowSeeds.Data()
                    .WithBloomTimeSeconds(0);

            data.WithTargetTm(transform)
                .WithOffset(new Vector2(0, 90f));

            _sowSeeds = new GameSystem.ComponentCreator<UI.WorldUI.SowSeeds, UI.WorldUI.SowSeeds.Data>()
                .SetRootRectTm(UIManager.Instance?.WorldUIRootRectTr)
                .SetData(data)
                .Create();
            _sowSeeds?.Activate();
        }

        private async UniTask UpdateRemainingGrowthTimeAsync(CancellationToken ct)
        {
            if (ct == null)
                return;

            var growthEndTime = _plotDataProvider?.GetGrowthEndTime(_data?.ObjectUniqueID);
            if (growthEndTime == null)
                return;

            while (!ct.IsCancellationRequested && IsActivate)
            {
                var remaining = growthEndTime.Value - DateTime.UtcNow;

                _sowSeeds?.UpdateTimerText((float)remaining.TotalSeconds);

                if (remaining.TotalSeconds <= 0) 
                    break;

                await UniTask.Delay(500, cancellationToken: ct);
            }
        }

        #region IGardenPlot
        void IGardenPlot.SetWaterUIActivate(bool activate)
        {
            if (_data.ObjectType != Type.ObjectType.Garden)
                return;

            EnsureWaterWorldUI();

            if (activate)
                _waterWorldUI?.Activate();
            else
                _waterWorldUI?.Deactivate();
        }

        public void SetPlotListener(IPlotListener plotListener)
        {
            _plotListener = plotListener;
        }

        public void SetPlotDataProvider(IPlotDataProvider plotDataProvider)
        {
            _plotDataProvider = plotDataProvider;
        }

        #endregion

        #region WaterWorldUI.IListener

        void WaterWorldUI.IListener.OnClick()
        {
            if (_plotListener == null)
                return;

            if (_plotListener.OnPlotCreated(_data?.ObjectUniqueID))
            {
                EnsureSowSeeds();
                UpdateRemainingGrowthTimeAsync(this.GetCancellationTokenOnDestroy()).Forget();
            }
        }
        #endregion
    }
}

