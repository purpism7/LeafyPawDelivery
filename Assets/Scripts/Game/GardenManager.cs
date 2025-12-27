using System.Collections.Generic;
using UnityEngine;

using Info;

namespace Game
{
    public interface IGardenManager
    {
    
    }

    public interface IPlotCreator
    {
        void CreatePlot(string objectUniqueID);
    }
    
    public class GardenManager : IGardenManager, IPlotCreator
    {
        // private Dictionary<int, List<PlotInfo>> _plotInfos = new Dictionary<int, List<PlotInfo>>();
        private GardenHolder _gardenHolder = new();
        
        public GardenManager Initialize()
        {
            _gardenHolder?.LoadInfo();
            
            return this;
        }
        
        void IPlotCreator.CreatePlot(string objectUniqueID)
        {
            var plotInfo = new PlotInfo();
        }
    }
}

