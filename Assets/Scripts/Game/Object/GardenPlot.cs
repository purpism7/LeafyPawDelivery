using UnityEngine;
using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;

using GameSystem;
using Info;
using UI.WorldUI;
using UI.Common;

namespace Game
{
    public interface IGardenPlot
    {
        void SetPlotListener(IPlotListener plotListener);
        void SetPlotDataProvider(IPlotDataProvider plotDataProvider);

        bool IsBloomed { get; }

        void SetWorldUIActive(bool isActive);
    }
    
    public class GardenPlot : Object, IGardenPlot,
        WaterWorldUI.IListener
    {
        [SerializeField] private SpriteRenderer cropSpriteRenderer = null;
        
        private WaterWorldUI _waterWorldUI = null;
        private SowSeeds _sowSeeds = null;

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

            cropSpriteRenderer?.SetActive(false);
            spriteRenderer?.SetActive(true);

            if (IsBloomed)
            {
                if(BloomCrop())
                    spriteRenderer?.SetActive(false);
            }
            else if (IsGrowing)
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

            if (cropSpriteRenderer != null)
            {
                order -= 50;
                cropSpriteRenderer.sortingOrder = order;
            }
        }

        public override void SetColor(Color color)
        {
            base.SetColor(color);

            if (cropSpriteRenderer == null)
                return;

            cropSpriteRenderer.color = color;
        }

        protected override void SetOutline(float width)
        {
            base.SetOutline(width);

            if (cropSpriteRenderer == null)
                return;

            cropSpriteRenderer.material?.SetFloat("_Thickness", width);
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

        private bool BloomCrop()
        {
            if (cropSpriteRenderer == null)
                return false;
            
            int cropId = _plotDataProvider?.GetCropID(_data?.ObjectUniqueID) ?? 0;
            if (cropId <= 0)
                return false;

            var cropData = CropDataContainer.Instance?.GetData(cropId);
            if (cropData == null)
                return false;

            var sprite = ResourceManager.Instance?.AtalsLoader?.GetCropSprite(cropData.ImgName);
            if (sprite == null)
                return false;

            cropSpriteRenderer.sprite = sprite;
            cropSpriteRenderer?.SetActive(true);

            isWind = true;

            return true;
        }

        private void EnsureWaterWorldUI()
        {
            if (IsGrowing)
                return;

            if (IsBloomed)
                return;

            if (_waterWorldUI != null)
                return;

            var uiManager = UIManager.Instance;

            var data = new UI.WorldUI.WaterWorldUI.Data();
            data.WithListener(this)
                .WithTargetTm(transform)
                .WithOffset(new Vector2(0, 90f))
                .WithOrder(SortingOrder);

            _waterWorldUI = new GameSystem.ComponentCreator<UI.WorldUI.WaterWorldUI, UI.WorldUI.WaterWorldUI.Data>()
                .SetRootRectTm(uiManager?.WorldUIGameRootRectTr)
                .SetData(data)
                .Create();
            _waterWorldUI?.Activate();

            uiManager?.SortWorldUIDepth();
        }

        private void EnsureSowSeeds()
        {
            if (IsBloomed)
                return;

            if (_sowSeeds != null)
                return;

            var uiManager = UIManager.Instance;

            var data = new UI.WorldUI.SowSeeds.Data();
            data.WithTargetTm(transform)
                .WithOffset(new Vector2(0, 75f))
                .WithOrder(SortingOrder);

            _sowSeeds = new GameSystem.ComponentCreator<UI.WorldUI.SowSeeds, UI.WorldUI.SowSeeds.Data>()
                .SetRootRectTm(uiManager?.WorldUIGameRootRectTr)
                .SetData(data)
                .Create();
            _sowSeeds?.Activate();

            uiManager?.SortWorldUIDepth();
        }
        
        private async UniTask UpdateRemainingGrowthTimeAsync(CancellationToken ct)
        {
            var growthEndTime = _plotDataProvider?.GetGrowthEndTime(_data?.ObjectUniqueID);
            if (growthEndTime == null)
                return;

            while (!ct.IsCancellationRequested && IsActivate)
            {
                var remaining = growthEndTime.Value - DateTime.UtcNow;

                _sowSeeds?.UpdateTimerText((float)remaining.TotalSeconds);

                if (remaining.TotalSeconds <= 0)
                {
                    SowSeedsDeactivate();
                    BloomCrop();
                    break;
                }
               
                await UniTask.Delay(500, cancellationToken: ct);
            }
        }
        
        private bool IsGrowing
        {
            get
            {
                if (_plotDataProvider == null)
                    return false;

                return _plotDataProvider.IsGrowing(_data?.ObjectUniqueID);
            }
        }

        #region IGardenPlot
        public void SetPlotListener(IPlotListener plotListener)
        {
            _plotListener = plotListener;
        }

        public void SetPlotDataProvider(IPlotDataProvider plotDataProvider)
        {
            _plotDataProvider = plotDataProvider;
        }
        
        public bool IsBloomed
        {
            get
            {
                if (_plotDataProvider == null)
                    return false;

                return _plotDataProvider.IsBloomed(_data?.ObjectUniqueID);
            }
        }
        
        void IGardenPlot.SetWorldUIActive(bool activate)
        {
            if (_data.ObjectType != Type.ObjectType.Garden)
                return;

            EnsureWaterWorldUI();

            if (activate)
            {
                _waterWorldUI?.SetOrder(SortingOrder);
                _waterWorldUI?.Activate();

                _sowSeeds?.SetOrder(SortingOrder);
                _sowSeeds?.Activate();
            }
            else
            {
                _waterWorldUI?.Deactivate();
                _sowSeeds?.Deactivate();
            }
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

