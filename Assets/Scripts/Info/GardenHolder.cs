using System.IO;
using Info.Holder;
using UnityEngine;

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

        private const string _secretKey = "hANkyulgARDeN";

        public override void LoadInfo()
        {
            
        }
    }
}

