using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Newtonsoft.Json;

using Game;

public class Item : Data.Base
{
    [JsonProperty("type")] 
    public Type.EItemSub EItemSub { get; private set; } = Type.EItemSub.None;

    [JsonProperty("value")] 
    public int Value { get; private set; } = 0;

    [JsonProperty("price")] 
    public int Price { get; private set; } = 0;
    
    [JsonProperty("payment")] 
    public Type.EPayment EPayment { get; private set; } = Type.EPayment.None;

}

