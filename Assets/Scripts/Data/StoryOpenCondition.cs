using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryOpenCondition : Data.Base
{
    public int StoryId = 0;
    public int[] ReqAnimalIds = null;
    public int[] ReqObjectIds = null;

    public override void Initialize()
    {
        base.Initialize();

        Debug.Log("StoryId = " + StoryId);
    }
}
