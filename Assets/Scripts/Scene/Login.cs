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

            StartCoroutine(CoInit());
        }

        private IEnumerator CoInit()
        {
            var firebaseMgr = FirebaseManager.Instance;

            yield return StartCoroutine(firebaseMgr.CoInit());

            _iListener?.EndLoad();

            if(firebaseMgr.Auth.IsValid)
            {
                yield return new WaitForSeconds(1f);

                SceneLoader.LoadWithLoading(loadData);

                yield break;
            }

            btn.interactable = true;
        }

        public void OnClick()
        {
            SceneLoader.LoadWithLoading(loadData);
        }
    }
}