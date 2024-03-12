using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class QuitGame : UI.BasePopup<BaseData>
    {
        public void Initialize()
        {
           
        }

        public override void Activate()
        {
            base.Activate();

            //Time.timeScale = 0;
        }

        public override void Deactivate()
        {
            base.Deactivate();

            //_endTask = true;
        }

        public void OnClickCancel()
        {
            //Time.timeScale = 1;
            Deactivate();
        }

        public void OnClickOK()
        {
            Application.Quit();
        }
    }
}

