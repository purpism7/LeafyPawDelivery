using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

namespace UI.Component
{
    public class CollectCurrency : Base<CollectCurrency.Data>
    {
        public class Data : BaseData
        {
            public Vector3 StartPos = Vector3.zero;
            public Vector3 EndPos = Vector3.zero;
        }
        
        [SerializeField] private BezierCurves bezierCurves = null;
        
        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            Deactivate();

            bezierCurves?.Initialize(data.StartPos, data.EndPos);
        }
    }
}

