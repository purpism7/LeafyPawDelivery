
using UnityEngine;

public class AcquireData : Data.Base
{
    [SerializeField]
    private string Type = string.Empty;
    [SerializeField]
    private string Action = string.Empty;
    public int Value = 0;

    public Game.Type.EAcquire EAcquireType = Game.Type.EAcquire.None;
    public Game.Type.EAcquireAction EAcquireActionType = Game.Type.EAcquireAction.None;

    public override void Initialize()
    {
        base.Initialize();

        System.Enum.TryParse(Type, out EAcquireType);
        System.Enum.TryParse(Action, out EAcquireActionType);
    }
}