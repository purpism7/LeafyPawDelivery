using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public abstract class BaseComponent<TData> : Base<TData> where TData : BaseData
    {
        public override void Initialize(TData data)
        {
            base.Initialize(data);
        }

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
