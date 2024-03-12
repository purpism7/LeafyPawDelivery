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
        }

        public override void Execute()
        {
            SetObjectPosZ();

            MainGameManager.Instance?.Arrange(_gameBaseElement, _pos);
        }

        private void SetObjectPosZ()
        {
            var elementData = _gameBaseElement?.ElementData;
            if (elementData == null)
                return;

            if (elementData.EElement == Game.Type.EElement.Object)
            {
                var activityPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
                if (activityPlace == null)
                    return;

                _pos.z = _gameBaseElement.LocalPos.y + activityPlace.ObjectPosZ;
                _gameBaseElement.SetLocalPosZ(_pos.z);
            }
        }
    }
}

