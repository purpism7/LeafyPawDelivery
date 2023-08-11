using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace UI
{
    public class NickName : Game.Common
    {
        public interface IListener
        {
            void Confirm();
        }

        [SerializeField] private TMP_InputField inputFieldTMP = null;

        private IListener _iListener = null;

        public void Initialize(IListener iListener)
        {
            _iListener = iListener;
        }

        public void OnClick()
        {
            var nickName = inputFieldTMP?.text;
            if(string.IsNullOrEmpty(nickName) ||
               nickName.Length < 1)
            {
                Debug.Log("more 2 character!!");

                return;
            }
             
            PlayerPrefs.SetString(Game.Data.KeyNickName, nickName);

            _iListener?.Confirm();

            _endTask = true;
        }

        public override void Begin()
        {
            base.Begin();

            _endTask = false;
        }
    }
}

