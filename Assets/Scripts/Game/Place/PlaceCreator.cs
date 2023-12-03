using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using UI;
using Game;
using Game.Creature;
using Data;

namespace GameSystem
{
    public class PlaceCreator : BaseCreator<Game.Place>
    {         
        private Transform _rootTm = null;
        private Game.Place.Data _data = null;
        //private System.Action<int> _placeActivityAnimalAction = null;

        public PlaceCreator SetRoot(Transform rootTm)
        {
            _rootTm = rootTm;

            return this;
        }

        public PlaceCreator SetData(Game.Place.Data data)
        {
            _data = data;

            return this;
        }

        //public PlaceCreator SetPlaceActivityAnimalAction(System.Action<int> action)
        //{
        //    _placeActivityAnimalAction = action;

        //    return this;
        //}

        public override Game.Place Create()
        {
            if (_data == null)
                return null;

            int placeId = _data.Id;

            var place = ResourceManager.Instance?.Instantiate<Game.Place>(placeId, _rootTm);
            if (place == null)
                return null;

            place.Id = placeId;
            place.Initialize(_data);

            return place;
        }
    }
}
