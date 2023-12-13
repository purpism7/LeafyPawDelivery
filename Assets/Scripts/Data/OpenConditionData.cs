
using UnityEngine;

[System.Serializable]
public class OpenConditionData : Data.Base
{
    public enum EType
    {
        None,

        Starter,
        Buy,
        Bonus,
    };

    [SerializeField]
    private string Type = string.Empty;
    public int AnimalCurrency = 0;
    public int ObjectCurrency = 0;
    public int Cash = 0;
    public bool Advertising = false;
    public int[] ReqAnimalIds = null;
    public int[] ReqObjectIds = null;

    public EType eType = EType.None;

    public override void Initialize()
    {
        base.Initialize();
       
        System.Enum.TryParse(Type, out eType);
    }
}
