using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.State
{
    public class Edit : Base
    {
        private BaseElement _gameBaseElement = null;

        public override void Initialize(MainGameManager mainGameMgr)
        {
            UI.ITopAnim iTopAnim = UIManager.Instance?.Top;
            iTopAnim?.DeactivateRight(null);
            iTopAnim?.DeactivateLeft(null);

            var activityPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            if (activityPlace == null)
                return;

            SetWaterUIDeactivate(activityPlace);

            activityPlace.Bust();
        }

        public override void End()
        {
            _gameBaseElement = null;
        }

        private void SetWaterUIDeactivate(IPlace place)
        {
            var objectList = place?.ObjectList;
            if (objectList == null)
                return;
            
            foreach (var obj in objectList)
            {
                if(obj == null)
                    continue;
                
                if(!obj.IsActivate)
                    continue;

                if (obj is IGardenPlot gardenPlot)
                {
                    gardenPlot.SetWaterUIActivate(false);
                }
            }
        }

        public void SetEditElement(BaseElement gameBaseElement)
        {
            _gameBaseElement?.EnableCollision(false);

            gameBaseElement?.AddRigidBody2D();
            gameBaseElement?.EnableCollision(true);

            _gameBaseElement = gameBaseElement;
        }

        public bool CheckIsEditElement(BaseElement gameBaseElement)
        {
            if (_gameBaseElement == null)
                return false;

            if (gameBaseElement == null)
                return true;

            if (gameBaseElement.Id != _gameBaseElement.Id)
                return true;

            if (gameBaseElement.UId != _gameBaseElement.UId)
                return true;

            return false;
        }
    }
}

