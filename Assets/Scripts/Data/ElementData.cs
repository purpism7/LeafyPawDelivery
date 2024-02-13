using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ElementData : Data.Base
{
    public int PlaceId = 0;

    public abstract int Currency { get; }
    public abstract Game.Type.EElement EElement { get; }
}
