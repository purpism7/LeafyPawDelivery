using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public partial class DropItem : Game.Base<DropItem.Data>
    {
        public class CurrencyData : Data
        {
            public Game.Type.EElement EElement = Type.EElement.None;

            public CurrencyData() : base(Type.EItem.Currency)
            {

            }
        }

    }
}
