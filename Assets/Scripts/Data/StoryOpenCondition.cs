using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryOpenCondition : Data.Base
{
    [SerializeField]
    private int placeId = 0;
    public int[] ReqAnimalIds = null;
    public int[] ReqObjectIds = null;

    public int PlaceId { get { return placeId; } }

    public override void Initialize()
    {
        base.Initialize();
    }
}
