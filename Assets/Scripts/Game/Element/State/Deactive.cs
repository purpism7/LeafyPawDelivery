using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Element.State
{
    public class DeActive : Base
    {
        public override Base Initialize(GameSystem.GameCameraController gameCameraCtr = null, GameSystem.IGrid iGrid = null)
        {
            base.Initialize(gameCameraCtr, iGrid);

            return this;
        }
    }
}
