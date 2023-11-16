using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shop : Data.Base
{
    [SerializeField]
    private string Category = string.Empty;
    [SerializeField]
    private string Payment = string.Empty;
    [SerializeField]
    private int PaymentValue = 0;
    [SerializeField]
    private int Value = 0;

    public Game.Type.ECategory ECategory { get; private set; } = Game.Type.ECategory.None;
    public Game.Type.EPayment EPayment { get; private set; } = Game.Type.EPayment.None;

    public override void Initialize()
    {
        base.Initialize();

        if(System.Enum.TryParse(Category, out Game.Type.ECategory eCategory))
        {
            ECategory = eCategory;
        }

        if (System.Enum.TryParse(Payment, out Game.Type.EPayment ePayment))
        {
            EPayment = ePayment;
        }
    }
}
