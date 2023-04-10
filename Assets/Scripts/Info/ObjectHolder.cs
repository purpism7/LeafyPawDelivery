using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

namespace Info
{
    public class ObjectHolder : MonoBehaviour
    {
        private readonly string ObjectInfoJsonFilePath = "Assets/Info/Object.json";

        private List<Info.Object> _obectInfoList = new();

        public ObjectHolder()
        {
            _obectInfoList.Clear();
        }

        public void Init()
        {
        }

        private void LoadInfo()
        {
            var filePath = ObjectInfoJsonFilePath;
            if (!System.IO.File.Exists(filePath))
            {
                return;
            }

            var jsonString = System.IO.File.ReadAllText(filePath);
            var obj = JsonUtility.FromJson<Info.Object>(jsonString);
        }

        private void SaveInfo()
        {
            
        }
    }
}

