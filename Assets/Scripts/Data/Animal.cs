using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Animal : ElementData
{
    public int Currency = 0;

    public override int GetCurrency => Currency;
    public override Type.EElement EElement => Type.EElement.Animal;
}

