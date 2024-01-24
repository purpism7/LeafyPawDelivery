using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Command
{
    public class Remove : EditCommand
    {
        private Type.EElement _eElement = Type.EElement.None;
        private int _id = 0;
        private int _uId = 0;

        public static void Execute(Game.BaseElement gameBaseElement)
        {
            new Remove(gameBaseElement)?.Execute();
        }

        public Remove(Game.BaseElement gameBaseElement)
        {
            if (gameBaseElement == null)
                return;

            if (gameBaseElement.ElementData == null)
                return;

            _eElement = gameBaseElement.ElementData.EElement;
            _id = gameBaseElement.Id;
            _uId = gameBaseElement.UId;

            gameBaseElement?.ElementCollision?.Reset();
        }

        public override void Execute()
        {
            MainGameManager.Instance?.Remove(_eElement, _id, _uId);
        }
    }
}