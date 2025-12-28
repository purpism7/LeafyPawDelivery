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
        }
        
        private void SaveInfo()
        {
            if(_plotInfos == null)
                return;

            var jsonString = JsonConvert.SerializeObject(_plotInfos);
            // var jsonStr = JsonHelper.ToJson(_plotInfos.ToArray());
            var encodeStr = jsonString.Encrypt(SecretKey);

            System.IO.File.WriteAllText(JsonFilePath, encodeStr);
        }

        public void CreatePlotInfo(string objectUniqueID)
        {
            if (_plotInfos == null)
                return;

            if (_plotInfos.Find(info => info.objectUniqueID == objectUniqueID) != null)
                return;
            
            var plotInfo = new PlotInfo
            {
                objectUniqueID = objectUniqueID,
            };
            
            _plotInfos?.Add(plotInfo);

            SaveInfo();
        }
    }
}

