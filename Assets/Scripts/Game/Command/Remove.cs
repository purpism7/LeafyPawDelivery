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
        private bool _refresh = true;

        public static void Execute(Game.BaseElement gameBaseElement, bool refresh)
        {
            new Remove(gameBaseElement, refresh)?.Execute();
        }

        public Remove(Game.BaseElement gameBaseElement, bool refresh)
        {
            if (gameBaseElement == null)
                return;

            if (gameBaseElement.ElementData == null)
                return;

            _eElement = gameBaseElement.ElementData.EElement;
            _id = gameBaseElement.Id;
            _uId = gameBaseElement.UId;
            _refresh = refresh;

            gameBaseElement?.ElementCollision?.Reset();
        }

        public override void Execute()
        {
            MainGameManager.Instance?.Remove(_eElement, _id, _uId, _refresh);
        }
    }
}