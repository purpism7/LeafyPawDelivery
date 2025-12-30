using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using Info;
using System;
using Unity.VisualScripting;

namespace Game
{
    public interface IGardenManager
    {
        
    }

    public interface IPlotListener
    {
        bool OnPlotCreated(string objectUniqueID);
        bool OnPlotRemoved(string objectUniqueID);
    }

    public interface IPlotDataProvider
    {
        DateTime? GetGrowthEndTime(string objectUniqueID);
        int GetCropID(string objectUniqueID);

        bool IsBloomed(string objectUniqueID);
        bool IsGrowing(string objectUniqueID);
    }

    public class GardenManager : IGardenManager, IPlotListener, IPlotDataProvider
    {
        private GardenHolder _gardenHolder = new();
        
        public GardenManager Initialize()
        {
            _gardenHolder?.LoadInfo();
            
            return this;
        }

        public bool IsBloomed(string objectUniqueID)
        {
            var plotInfo = _gardenHolder.GetPlotInfo(objectUniqueID);
            if (plotInfo == null)
                return false;

            return plotInfo.growthEndTime.HasValue && DateTime.UtcNow >= plotInfo.growthEndTime.Value;
        }

        public bool IsGrowing(string objectUniqueID)
        {
            var plotInfo = _gardenHolder.GetPlotInfo(objectUniqueID);
            if (plotInfo == null)
                return false;

            return plotInfo.growthEndTime.HasValue && DateTime.UtcNow < plotInfo.growthEndTime.Value;
        }

        public bool IsEmpty(string objectUniqueID)
        {
            var plotInfo = _gardenHolder.GetPlotInfo(objectUniqueID);
            if (plotInfo == null)
                return false;

            return plotInfo.cropID <= 0 || !plotInfo.growthEndTime.HasValue;
        }


        #region IPlotListener

        bool IPlotListener.OnPlotCreated(string objectUniqueID)
        {
            if (_gardenHolder == null)
                return false;

            var cropDataContaeiner = CropDataContainer.Instance;
            if (cropDataContaeiner == null ||
                cropDataContaeiner.Datas == null)
                return false;

            var randomCropID = UnityEngine.Random.Range(1, cropDataContaeiner.Datas.Length + 1);
            var cropData = cropDataContaeiner.GetData(randomCropID);
            if (cropData == null)
                return false;

            return _gardenHolder.TryAddPlotInfo(objectUniqueID, cropData.Id, cropData.GrowthTimeSeconds);
        }

        bool IPlotListener.OnPlotRemoved(string objectUniqueID)
        {
            if (_gardenHolder == null)
                return false;

            return _gardenHolder.TryRemovePlotInfo(objectUniqueID);
        }

        #endregion

        #region IPlotDataProvider
        int IPlotDataProvider.GetCropID(string objectUniqueID)
        {
            return _gardenHolder?.GetPlotInfo(objectUniqueID)?.cropID ?? 0;
        }

        DateTime? IPlotDataProvider.GetGrowthEndTime(string objectUniqueID)
        {
            var plotInfo = _gardenHolder.GetPlotInfo(objectUniqueID);
            if (plotInfo == null)
                return null;

            return plotInfo.growthEndTime;
        }
        #endregion
    }
}

