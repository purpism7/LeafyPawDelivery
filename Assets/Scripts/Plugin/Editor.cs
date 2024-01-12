using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plugin
{
    public class Editor : Base
    {
        public override void Initialize()
        {
            
        }

        public override void SetString(string key, string value)
        {
            var jsonFilePath = string.Format(_userInfoJsonFilePath, key);

            System.IO.File.WriteAllText(jsonFilePath, value);
        }

        public override string GetString(string key)
        {
            var jsonFilePath = string.Format(_userInfoJsonFilePath, key);
            if (!System.IO.File.Exists(jsonFilePath))
                return string.Empty;

            return System.IO.File.ReadAllText(jsonFilePath);
        }
    }
}

