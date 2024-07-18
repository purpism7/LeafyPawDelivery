using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Game.State
{
    public class Conversation : Base
    {
        private Vector3 _initCameraPos = Vector3.zero;
        
        public override void Initialize(MainGameManager mainGameMgr)
        {
            // MainGameManager.Instance?.IGameCameraCtr?.ZoomIn(transform.position);

            IPlace iPlace = MainGameManager.Get<PlaceManager>()?.ActivityPlace;
            if (iPlace == null ||
                iPlace.AnimalList == null)
                return;


            var id = MainGameManager.Get<AnimalManager>()?.SelectIdForConversation;
            
            foreach (var animal in iPlace.AnimalList)
            {
                if(animal == null)
                    continue;

                if (animal.Id == id)
                {
                    
                }
            }
            
            
            
            // MainGameManager.Get<PlaceManager>()?.Get.
            // MainGameManager.Instance.IGameCameraCtr.ZoomIn(transform.position);

            // _selectAnimalPos.y -= 70f;

            // mainGameMgr.IGameCameraCtr?.ZoomIn(_selectAnimalPos);
        }
        
        public override void End()
        {
            
        }

        public Conversation SelectAnimalPos(Creature.Animal animal)
        {
            if (animal == null)
                return null;

            // _selectAnimalPos = animal.transform.position;

            return this;
        }
    }
}


