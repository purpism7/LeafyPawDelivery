using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Data
{
    public class Base
    {
        [JsonProperty("id")]
        public int Id = 0;

        public virtual void Initialize()
        {

        }
    }
}

