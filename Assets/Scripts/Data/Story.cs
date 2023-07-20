using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Story : Data.Base
{
    public int PlaceId = 0;
    public string Name = string.Empty;
    public int Order = 0;
    public string PrefabName = string.Empty;
}
