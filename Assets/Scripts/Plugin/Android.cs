using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Plugin
{
    public class Android : Base
    {
        public override void Initialize()
        {
            //string userInfoJsonFilePath = Application.persistentDataPath + "/Info/User.json";
            //if (System.IO.File.Exists(userInfoJsonFilePath))
            //{
            //    System.IO.File.Delete(userInfoJsonFilePath);
            //}
        }

        public override void SetString(string key, string value)
        {
            
        }

        public override string GetString(string key)
        {
            return string.Empty;
        }
    }
}

