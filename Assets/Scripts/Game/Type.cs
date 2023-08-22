using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Type
{
    public enum ETab
    {
        Animal,
        Object,
    }

    public enum EElement
    {
        None,
        
        Animal,
        Object,
    }
    
    public enum EScene
    {
        None,

        Loading,
        Logo,
        Login,
        Game,
    }

    public enum EOpen
    {
        None,
        
        Animal,
        Object,
        Story,
    }

    public enum EAnimalCurrency
    {
        None,

        Acorn,
    }

    public enum EObjectCurrency
    {
        None,

        Leaf,
    }

    public enum EObjectGrade
    {
        None,

        Unique,
        Epic,
        Rare,
        Normal,
    }
}
