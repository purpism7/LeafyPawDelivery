using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Arrange : EditCommand
    {
        private Game.BaseElement _gameBaseElement = null;
        private Vector3 _pos = Vector3.zero;

        public static void Execute(Game.BaseElement gameBaseElement, Vector3 pos)
        {
            new Arrange(gameBaseElement, pos)?.Execute();
        }

        public Arrange(Game.BaseElement gameBaseElement, Vector3 pos)
        {
            var elementData = gameBaseElement?.ElementData;
            if (elementData == null)
                return;

            _gameBaseElement = gameBaseElement;
            _pos = pos;

            gameBaseElement.EndEdit();
        }

        public override void Execute()
        {
            MainGameManager.Instance?.Arrange(_gameBaseElement, _pos);
        }
    }
}

