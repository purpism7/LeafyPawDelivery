using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class ContinueGame : Game.Common
    {
        public interface IListener
        {
            void New();
            void Continue();
        }

        private IListener _iListener = null;

        public void Initialize(IListener iListener)
        {
            _iListener = iListener;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            _endTask = true;
        }

        public void OnClickNew()
        {
            _iListener?.New();

            Deactivate();
        }

        public void OnClickContinue()
        {
            _iListener?.Continue();

            Deactivate();
        }

        public override void Begin()
        {
            base.Begin();

            _endTask = false;
        }
    }
}

