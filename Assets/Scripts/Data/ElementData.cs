using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ElementData : Data.Base
{
    public int PlaceId = 0;
    public string ShortIconImgName = string.Empty;
    public string LargeIconImgName = string.Empty;

    public abstract int GetCurrency { get; }
    public abstract Type.EElement EElement { get; }
}
