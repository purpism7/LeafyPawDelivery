using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class BaseComponent<T> : Base<T> where T : BaseData
    {
        public void SetParent(Transform rootTm)
        {
            if (!rootTm)
                return;

            if (!transform)
                return;

            transform.SetParent(rootTm);
        }

        // public void SetActive(bool active)
        // {
        //     if (!transform)
        //         return;
        //
        //     transform.SetActive(active);
        // }
    }
}
