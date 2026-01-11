using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

using Newtonsoft.Json;

using Info.Holder;

namespace Info
{
    public class GardenHolder : Holder.Base
    {
        protected override string JsonFilePath
        {
            get
            {
                return Path.Combine(RootJsonFilePath, "Garden.txt");
            }
        }

        private const string SecretKey = "hANkyulgARDeN";
        
        private List<PlotInfo> _plotInfos = new();

        public override void LoadInfo()
        {
            RootJsonFilePath = Utility.GetInfoPath();
            
            if (System.IO.File.Exists(JsonFilePath))
            {
                var decodeStr = System.IO.File.ReadAllText(JsonFilePath);
                var jsonStr = decodeStr.Decrypt(SecretKey);

                _plotInfos = JsonConvert.DeserializeObject<List<PlotInfo>>(jsonStr);
            }

            var userPlotInfos = UserManager.Instance?.User?.PlotUniqueIDs?.ToList();
            if (userPlotInfos != null)
            {
                List<PlotInfo > plotInfos = new();
                
                for (int i = userPlotInfos.Count - 1; i >= 0; --i)
                {
                    var plotUniqueID = userPlotInfos[i];
                    if (string.IsNullOrEmpty(plotUniqueID))
                        continue;

                    if (_plotInfos?.Find(info => info.uniqueID == plotUniqueID) == null)
                        _plotInfos?.RemoveAt(i);
                }
            }
        }
        
        private void SaveInfo()
        {
            if(_plotInfos == null)
                return;

            var jsonString = JsonConvert.SerializeObject(_plotInfos);
            Debug.Log(jsonString);
            var encodeStr = jsonString.Encrypt(SecretKey);

            System.IO.File.WriteAllText(JsonFilePath, encodeStr);
        }

        public bool TryAddPlotInfo(string objectUniqueID, int cropID, int growthTimeSeconds)
        {
            var plotInfo = GetPlotInfo(objectUniqueID);
            if (plotInfo != null)
                return false;

            plotInfo = new PlotInfo
            {
                objectUniqueID = objectUniqueID,
                cropID = cropID,
                growthEndTime = System.DateTime.UtcNow.AddSeconds(growthTimeSeconds)
            };
            
            _plotInfos?.Add(plotInfo);

            SaveInfo();
            
            List<string> plotUniqueIDs = new();
            if (_plotInfos != null)
            {
                for (int i = 0; i < _plotInfos.Count; ++i)
                {
                    if(_plotInfos[i] == null)
                        continue;
                    
                    plotUniqueIDs.Add(_plotInfos[i].uniqueID);
                }
            }
            
            UserManager.Instance?.SetGardenPlotInfos(plotUniqueIDs);

            return true;
        }

        public bool TryRemovePlotInfo(string objectUniqueID)
        {
            var plotInfo = GetPlotInfo(objectUniqueID);
            if (plotInfo == null)
                return false;

            return _plotInfos.Remove(plotInfo);
        }

        public PlotInfo GetPlotInfo(string objectUniqueID)
        {
            return _plotInfos?.Find(info => info.objectUniqueID == objectUniqueID);
        }
    }
}

