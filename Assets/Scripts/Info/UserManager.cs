using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class UserManager : Singleton<UserManager>
    {
        readonly private string UserInfoJsonFilePath = "Assets/Info/User.json";

        public User User { get; private set; } = null;

        public PlaceHolder PlaceHolder { get; private set; } = null;
        public ObjectHolder ObjectjHolder { get; private set; } = new();

        public override IEnumerator CoInit()
        {
            LoadInfo();

            yield return null;
        }

        private void LoadInfo()
        {
            if(!System.IO.File.Exists(UserInfoJsonFilePath))
            {
                return;
            }

            var jsonString = System.IO.File.ReadAllText(UserInfoJsonFilePath);

            User = JsonUtility.FromJson<User>(jsonString);
        }

        public void SaveInfo()
        {
            var json = JsonUtility.ToJson(User);
            System.IO.File.WriteAllText(UserInfoJsonFilePath, json);
        }
    }
}


