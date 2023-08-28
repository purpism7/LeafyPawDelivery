using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Arrange : EditCommand
    {
        private Type.EElement _eElement = Type.EElement.None;
        private int _id = 0;
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

            _eElement = elementData.EElement;
            _id = _eElement == Type.EElement.Animal ? gameBaseElement.Id : gameBaseElement.UId;
            _pos = pos;

            gameBaseElement.EndEdit();
        }

        public override void Execute()
        {
            MainGameManager.Instance?.Arrange(_eElement, _id, _pos);
        }
    }
}

