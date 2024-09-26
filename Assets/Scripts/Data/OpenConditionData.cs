
using UnityEngine;

[System.Serializable]
public class OpenConditionData : Data.Base
{
    public enum EType
    {
        None,

        Starter,
        Buy,
        Hidden,
        Bonus,
        
        Special,
    };

    [SerializeField]
    private string Type = string.Empty;
    public int AnimalCurrency = 0;
    public int ObjectCurrency = 0;
    public int Cash = 0;
    public bool Advertising = false;
    public int[] ReqAnimalIds = null;
    public int[] ReqObjectIds = null;
    [SerializeField]
    private int reqStoryId = 0; // 숨겨진 오브젝트를 배치를 위한 조건.

    public EType eType = EType.None;

    public int ReqStoryId { get { return reqStoryId; } }
    
    public override void Initialize()
    {
        base.Initialize();

        System.Enum.TryParse(Type, out eType);
    }
}
