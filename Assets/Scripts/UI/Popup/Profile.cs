using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Profile : Base<Profile.Data>
    {
        public class Data : BaseData
        {

        }

        public override IEnumerator CoInit(Data data)
        {
            yield return StartCoroutine(base.CoInit(data));

            Debug.Log("Profile");
        }
    }
}

