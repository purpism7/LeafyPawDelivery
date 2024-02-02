using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class Screenshot : BasePopup<BaseData>
    {
        public override void Initialize(BaseData data)
        {
            base.Initialize(data);
        }

        public override void Activate()
        {
            base.Activate();

            Game.UIManager.Instance?.DeactivateAnim();
        }

        public override void Deactivate()
        {
            base.Deactivate();

            MainGameManager.Instance?.SetGameStateAsync(Game.Type.EGameState.Game);
        }

        public void OnClick()
        {
            Game.UIManager.Instance?.ActivateAnim(null);

            Deactivate();
        }
    }
}
