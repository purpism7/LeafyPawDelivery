using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

using GameSystem.Load;

namespace Scene
{
    public class Begin : Base
    {
        [SerializeField] private LoadData loadData = null;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(3f);

            Init(null);
        }

        public override void Init(IListener iListener)
        {
            base.Init(iListener);
            
            SceneLoader.LoadWithoutLoading(loadData);
        }
    }
}

