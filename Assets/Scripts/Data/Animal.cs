using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Animal : ElementData
{
    [SerializeField]
    private int currency = 0;

    public override int Currency => currency;
    public override Game.Type.EElement EElement => Game.Type.EElement.Animal;

    public override void Initialize()
    {
        base.Initialize();

    }
}

