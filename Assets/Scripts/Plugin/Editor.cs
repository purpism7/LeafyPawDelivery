using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plugin
{
    public class Editor : Base
    {
#if UNITY_EDITOR
        private const string _userInfoJsonFilePath = "Assets/Info/User.json";
#else
        readonly private string _userInfoJsonFilePath = Application.persistentDataPath + "/Info/User.json";
#endif

        public override void Initialize()
        {
            
        }

        public override void SetString(string key, string value)
        {
            System.IO.File.WriteAllText(_userInfoJsonFilePath, value);
        }

        public override string GetString(string key)
        {
            if (!System.IO.File.Exists(_userInfoJsonFilePath))
                return string.Empty;

            return System.IO.File.ReadAllText(_userInfoJsonFilePath);
        }
    }
}

