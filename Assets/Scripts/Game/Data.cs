using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Data
    {
        public static Const Const = null;

        public static int PlayerPrefsVersion = 23;
        public static string PlayPrefsKeyLastPlaceKey = "KeyLastPlaceId_" + PlayerPrefsVersion;
        public static string PlayPrefsKeyNickName = "KeyNickName_" + PlayerPrefsVersion;
    }
}