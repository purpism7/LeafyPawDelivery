using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace UI
{
    public class Top : Common<Top.Data>
    {
        public class Data : BaseData
        {

        }

        public TextMeshProUGUI LvTMP;
        public TextMeshProUGUI LeafTMP;
        public TextMeshProUGUI BerryTMP;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            var userInfo = Info.UserManager.Instance?.User;
            if(userInfo != null)
            {
                LvTMP?.SetText(userInfo.Lv + "");
                LeafTMP?.SetText(userInfo.Leaf + "");
                BerryTMP?.SetText(userInfo.Berry + "");
            }
        }
    }
}

