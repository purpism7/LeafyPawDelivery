using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Games
{
    public class Data
    {
        public static Const Const = null;

        public static string SecretKey { get { return "LEafypawDEliverY"; }  }

        public static int PlayerPrefsVersion = 23;
        public static string PlayPrefsKeyLastPlaceKey = "KeyLastPlaceId_" + PlayerPrefsVersion;
        public static string PlayPrefsKeyNickName = "KeyNickName_" + PlayerPrefsVersion;
    }
}