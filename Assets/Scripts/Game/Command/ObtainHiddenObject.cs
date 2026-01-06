using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameSystem;
using UI;

namespace Game.Command
{
    public class ObtainHiddenObject : EditCommand
    {
        private GameObject hiddenObjectGameObj = null;
        private Type.EElement _eElement = Type.EElement.None;
        private int _id = 0;
        private int _uId = 0;

        public static void Execute(Game.BaseElement gameBaseElement)
        {
            new ObtainHiddenObject(gameBaseElement)?.Execute();
        }

        public ObtainHiddenObject(Game.BaseElement gameBaseElement)
        {
            if (gameBaseElement == null)
                return;

            hiddenObjectGameObj = gameBaseElement.gameObject;

            if (gameBaseElement.ElementData == null)
                return;

            _eElement = gameBaseElement.ElementData.EElement;
            _id = gameBaseElement.Id;
            _uId = gameBaseElement.UId;
        }

        public override void Execute()
        {
            Sequencer.EnqueueTask(
                () =>
                {
                    var obtain = new PopupCreator<Obtain, Obtain.Data>()
                        .SetData(new Obtain.Data()
                        {
                            EElement = _eElement,
                            Id = _id,
                            ClickAction = () =>
                            {
                                GameObject.Destroy(hiddenObjectGameObj);
                            },
                        })
                        .SetCoInit(true)
                        .SetReInitialize(true)
                        .Create();

                    return obtain;
                });

            MainGameManager.Get<Game.ObjectManager>()?.Add(_id);
        }
    }
}
