using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

using GameSystem.Load;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Scene
{
    public class Login : Base
    {
        [SerializeField] private LoadData loadData = null;
        [SerializeField] private Button btn = null;

        public override void Init(IListener iListener)
        {
            base.Init(iListener);
            
            // SceneLoader.LoadWithLoading(loadData);

            _iListener?.EndLoad();
            
            StartCoroutine(CoInteractive());
        }

        private IEnumerator CoInteractive()
        {
            yield return new WaitForSeconds(3f);
            
            btn.interactable = true;
        }

        public void OnClick()
        {
            SceneLoader.LoadWithLoading(loadData);
        }
    }
}