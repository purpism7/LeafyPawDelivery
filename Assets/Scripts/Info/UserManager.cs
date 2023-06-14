using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class UserManager : Singleton<UserManager>
    {
#if UNITY_EDITOR
        readonly private string _userInfoJsonFilePath = "Assets/Info/User.json";
#else
        readonly private string _userInfoJsonFilePath = Application.persistentDataPath + "/Info/User.json";
#endif
        
        public User User { get; private set; } = null;

        //private Dictionary<System.Type, Holder.Base> _holderDic = new();

        protected override void Initialize()
        {
            
        }
        
        public override IEnumerator CoInit()
        {
            yield return StartCoroutine(base.CoInit());

            LoadInfo();

            //InitHolder();

            yield return null;
        }

        //private void InitHolder()
        //{
        //    _holderDic.Clear();

        //    _holderDic.Add(typeof(PlaceHolder), new PlaceHolder());
        //    _holderDic.Add(typeof(ObjectHolder), new ObjectHolder());
        //}

        private void LoadInfo()
        {
            if(!System.IO.File.Exists(_userInfoJsonFilePath))
                return;

            var jsonString = System.IO.File.ReadAllText(_userInfoJsonFilePath);

            User = JsonUtility.FromJson<User>(jsonString);
        }

        //public void SaveInfo()
        //{
        //    var json = JsonUtility.ToJson(User);
        //    System.IO.File.WriteAllText(UserInfoJsonFilePath, json);
        //}

        //public void SaveInfo(int objectId, Vector2 pos)
        //{
        //    var objectHolder = GetHolder<ObjectHolder>();
        //    if(objectHolder == null)
        //    {
        //        return;
        //    }
        //}

        //public T GetHolder<T>() where T : Holder.Base
        //{
        //    if(_holderDic == null)
        //    {
        //        return default(T);
        //    }

        //    if(_holderDic.TryGetValue(typeof(T), out Holder.Base holder))
        //    {
        //        return holder as T;
        //    }

        //    return default(T);
        //}
    }
}


