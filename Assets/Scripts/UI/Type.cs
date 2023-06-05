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
    
    public enum EScene
    {
        None,

        Loading,
        Logo,
        Login,
        Game,
    }

    public enum EOpenType
    {
        None,
        
        Animal,
        Object,
        Story,
    }
}
