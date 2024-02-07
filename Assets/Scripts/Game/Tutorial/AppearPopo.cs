using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Tutorial
{
    public class AppearPopo : Step
    {
        private Game.Popo _popo = null;

        public override void Begin()
        {
            CreatePopo();
        }

        public override void End()
        {
            
        }

        public override void ChainUpdate()
        {
            
        }

        private void CreatePopo()
        {
            _popo = GameSystem.ResourceManager.Instance?.InstantiateGame<Game.Popo>(transform);
            if (_popo == null)
                return;

            var posY = -200f;
            _popo.Initialize(new Popo.Data()
            {
                startPos = new Vector3(-Screen.width + 300f, posY, 0),
            });

            _popo.MoveToTarget(new Vector3(0, posY, 0));
        }
    }
}

