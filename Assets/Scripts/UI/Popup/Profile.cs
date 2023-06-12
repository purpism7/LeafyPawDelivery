using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Profile : BasePopup<Profile.Data>
    {
        public class Data : BaseData
        {
            
        }

        public override IEnumerator CoInitialize(Data data)
        {
            yield return StartCoroutine(base.CoInitialize(data));
            
            Debug.Log("Profile");
        }
    }
}

