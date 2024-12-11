using System.Collections;
using System.Collections.Generic;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

namespace Game.Element.State
{
    public class Interaction : Base
    {
        public interface IListener
        {
            void Finish();
        }

        private IListener _iListener = null;
        
        public override Base Initialize(GameSystem.GameCameraController gameCameraCtr = null, GameSystem.IGrid iGrid = null)
        {
            base.Initialize(gameCameraCtr, iGrid);
            
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
            
            var obj = iPlace.ObjectList?.Find(obj => obj.Id == interactionObjectId);
            var iObject = obj as IObject;
            if (iObject != null)
            {
                var targetPos = iObject.LocalPos;
                targetPos.y -= 40f;
                
                animal.ActionCtr?.MoveToTarget(targetPos,
                    () =>
                    {
                        UIManager.Instance.ActivateSreenSaver(Game.Type.EScreenSaverType.InteractionAnimal);
                        animal.Deactivate();
                        _iListener = animal;
                        
                        obj?.ObjectActCtr?.PlaySpecial(
                            () =>
                            {
                                _iListener?.Finish();
                                
                                UIManager.Instance?.DeactivateScreenSaver();
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

