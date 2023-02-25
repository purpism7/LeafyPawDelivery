using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Info
{
    public class UserManager : Singleton<UserManager>
    {
        public User User { get; private set; } = new();

        public override IEnumerator CoInit()
        {
            yield return null;
        }
    }
}


