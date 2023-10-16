using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Animal : ElementData
{
    [SerializeField]
    private int Currency = 0;

    public override int GetCurrency => Currency;
    public override Game.Type.EElement EElement => Game.Type.EElement.Animal;
}

