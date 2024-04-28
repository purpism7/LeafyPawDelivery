using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Event
{
    public class PlaceData : BaseData
    {
        public int Id { get; private set; } = 0;

        public PlaceData(int id)
        {
            Id = id;
        }
    }

    public class OpenPlaceData : PlaceData
    {
        public OpenPlaceData(int id) : base(id)
        {
            
        }
    }
}

