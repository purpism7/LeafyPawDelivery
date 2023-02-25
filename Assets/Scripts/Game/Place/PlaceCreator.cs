using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UI;
using Game;
using Creature;
using Data;

namespace GameSystem
{
    public class PlaceCreator : BaseCreator<Place>
    {         
        private int _placeId = 0;
        private Transform _rootTm = null;
        private System.Action<int> _placeActivityAnimalAction = null;

        public PlaceCreator SetPlaceId(int id)
        {
            _placeId = id;

            return this;
        }

        public PlaceCreator SetRoot(Transform rootTm)
        {
            _rootTm = rootTm;

            return this;
        }

        public PlaceCreator SetPlaceActivityAnimalAction(System.Action<int> action)
        {
            _placeActivityAnimalAction = action;

            return this;
        }

        public override Place Create()
        {
            var place = ResourceManager.Instance?.Instantiate<Place>(_placeId, _rootTm);
            if (place == null)
            {
                return null;
            }

            place.Init(_placeActivityAnimalAction);

            return place;
        }
    }
}
