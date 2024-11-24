using System.Collections;
using System.Collections.Generic;
using Game.Creature;
using GameSystem;
using UnityEngine;

namespace Game.Element.State
{
    public class Interaction : BaseState
    {
        public override BaseState Initialize()
        {
            base.Initialize();
            
            Type = Game.Type.EElementState.Interaction;
            
            return this;
        }

        public override void Apply(BaseElement gameBaseElement)
        {
            base.Apply(gameBaseElement);

            var animal = gameBaseElement as Creature.Animal;
            if (animal == null)
                return;
            
            int interactionObjectId = AnimalContainer.Instance.GetInteractionObjectId(gameBaseElement.Id);
            if (interactionObjectId <= 0)
                return;

            IPlace iPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            if (iPlace == null)
                return;
            
            var obj = iPlace.ObjectList.Find(obj => obj.Id == interactionObjectId);
            var iObject = obj as IObject;
            if (iObject != null)
            {
                animal.ActionCtr?.MoveToTarget(iObject.LocalPos,
                    () =>
                    {
                        // animal.Deactivate();
                        // End();
                        obj?.ObjectActCtr?.PlaySpecial(
                            () =>
                            {
                                
                            }
                        );
                    });
            }
        }

        public override void End()
        {
            base.End();
            
            
        }
    }
}

