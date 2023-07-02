using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;

using GameSystem.Load;
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
            
            StartCoroutine(CoInit());
        }

        private IEnumerator CoInit()
        {
            yield return StartCoroutine(FirebaseManager.Instance.CoInit());

            btn.interactable = true;
        }

        public void OnClick()
        {
            SceneLoader.LoadWithLoading(loadData);
        }
    }
}