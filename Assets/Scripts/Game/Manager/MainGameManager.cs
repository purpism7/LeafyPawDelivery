using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

using Cysharp.Threading.Tasks;

using GameSystem;

public interface IStarter
{
    void Check();
}

public class MainGameManager : Singleton<MainGameManager>
{
    #region Inspector
    [SerializeField]
    private Game.PlaceManager placeMgr = null;
    #endregion

    public GameCameraController GameCameraCtr { get; private set; } = null;
    public Game.State.Base GameState { get; private set; } = null;
    public Game.RecordContainer RecordContainer { get; private set; } = null;

    private System.Action<Game.Base> _startEditAction = null;
    private IUpdater _iUpdateInputMgr = null;
    private IUpdater _iUpdateGameCameraCtr = null;
    private IUpdater _iUpdateGrid = null;
    private IGrid _iGrid = null;

    private static Dictionary<Type, MonoBehaviour> _managerDic = new();

    public static T Get<T>()
    {
        if (_managerDic == null)
            return default(T);

        var manager = _managerDic[typeof(T)];
        if (manager == null)
            return default(T);

        return manager.GetComponent<T>();
    }

    protected override void Initialize()
    {
        _managerDic.Clear();

        AddManager(typeof(Game.AnimalManager), gameObject.GetOrAddComponent<Game.AnimalManager>());
        AddManager(typeof(Game.ObjectManager), gameObject.GetOrAddComponent<Game.ObjectManager>());

        AddManager(typeof(Game.PlaceManager), placeMgr);
        AddManager(typeof(Game.StoryManager), gameObject.GetOrAddComponent<Game.StoryManager>());
        AddManager(typeof(Game.Manager.Guide), gameObject.GetOrAddComponent<Game.Manager.Guide>());
        AddManager(typeof(Game.Manager.Acquire), gameObject.GetOrAddComponent<Game.Manager.Acquire>());
    }

    public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
    {
        yield return StartCoroutine(base.CoInit(iProvider));

        var activityPlaceId = Game.Data.Const.StartPlaceId;//placeMgr.ActivityPlaceId; 

        var inputMgr = iProvider.Get<InputManager>();
        GameCameraCtr = inputMgr?.GameCameraCtr;

        _iGrid = inputMgr?.grid;

        _iUpdateInputMgr = inputMgr;
        _iUpdateGameCameraCtr = GameCameraCtr;
        _iUpdateGrid = inputMgr?.grid;

        RecordContainer = new();

        var acquireMgr = Get<Game.Manager.Acquire>();
        yield return StartCoroutine(acquireMgr?.CoInitialize(null));

        yield return StartCoroutine(CoInitializeManager(activityPlaceId));
        yield return StartCoroutine(Get<Game.StoryManager>().CoInitialize(null));

        yield return null;

        Game.Manager.Guide.Create();

        EndLoad(true);
    }

    private void AddManager(Type type, MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null)
            return;

        _managerDic.TryAdd(type, monoBehaviour);
    }

    private IEnumerator CoInitializeManager(int placeId)
    {
        var setting = new Info.Setting();
        if (placeMgr != null)
        {
            yield return StartCoroutine(placeMgr.CoInitialize(
                new Game.PlaceManager.Data()
                {
                    placeId = placeId,
                    setting = setting,
                }));
        }

        var objectMgr = Get<Game.ObjectManager>();
        yield return StartCoroutine(objectMgr?.CoInitialize(
            new Game.ObjectManager.Data
            {
                PlaceId = placeId,
            }));

        var animalMgr = Get<Game.AnimalManager>();
        yield return StartCoroutine(animalMgr?.CoInitialize(
            new Game.AnimalManager.Data
            {
                PlaceId = placeId,
            }));

        placeMgr?.ActivityPlace?.Activate();

        yield return null;

        var iGridCell = _iGrid as IGridCell;
        if(iGridCell != null)
        {
            Game.Carrier.Create(iGridCell);
        }

        SetGameState<Game.State.Game>();

        yield return null;

        _iGrid?.Overlap();
    }

    private void EndLoad(bool initialize)
    {
        Starter();
    }

    private void Starter()
    {
        Debug.Log("Starter");

        foreach(var manager in _managerDic.Values)
        {
            var iStarter = manager as IStarter;
            if (iStarter == null)
                continue;

            iStarter.Check();
        }
    }

    private void Update()
    {
        _iUpdateInputMgr?.ChainUpdate();
        _iUpdateGameCameraCtr?.ChainUpdate();

        placeMgr?.ChainUpdate();

        _iUpdateGrid?.ChainUpdate();
    }

    #region GameState
    public void SetGameState<T>() where T : Game.State.Base
    {
        if (GameState != null &&
            GameState.Type.Equals(typeof(T)))
            return;

        GameState = System.Activator.CreateInstance<T>();
        GameState?.Initialize(this);
    }
    #endregion

    public void SetStartEditAction(System.Action<Game.Base> action)
    {
        _startEditAction = action;
    }

    public void Add(Game.Type.EElement EElement, int id)
    {
        if (EElement == Game.Type.EElement.Animal)
        {
            Get<Game.AnimalManager>()?.Add(id);
        }
        else if (EElement == Game.Type.EElement.Object)
        {
            Get<Game.ObjectManager>()?.Add(id);
        }
    }

    public bool CheckExist(Game.Type.EElement EElement, int id)
    {
        if (EElement == Game.Type.EElement.Animal)
        {
            return Get<Game.AnimalManager>().CheckExist(id);
        }
        else if (EElement == Game.Type.EElement.Object)
        {
            return Get<Game.ObjectManager>().CheckExist(id);
        }

        return false;
    }

    #region Place
    public void MovePlace(int placeId, System.Action endMoveAction)
    {
        StartCoroutine(CoMovePlace(placeId, endMoveAction));

        //AsyncMovePlace(placeId, endMoveAction).Forget();
    }

    private IEnumerator CoMovePlace(int placeId, System.Action endMoveAction)
    {
        yield return null;

        yield return StartCoroutine(CoInitializeManager(placeId));

        yield return new WaitForSeconds(UnityEngine.Random.Range(2f, 3f));

        endMoveAction?.Invoke();

        yield return null;

        EndLoad(false);
    }

    //private async UniTask AsyncMovePlace(int placeId, System.Action endMoveAction)
    //{
    //    //yield return StartCoroutine(CoInitializeManager(placeId));

    //    await UniTask.Delay(2100);

    //    endMoveAction?.Invoke();

    //    await UniTask.Yield();

    //    Starter();
    //}
    #endregion

    #region Animal
    public void AddAnimalToPlace(int id)
    {
        var animalInfo = Get<Game.AnimalManager>()?.GetAnimalInfo(id);
        if (animalInfo == null)
            return;

        var activityPlace = placeMgr?.ActivityPlace;
        if (activityPlace == null)
            return;

        Vector3 pos = Vector3.zero;
        if (GameCameraCtr != null)
        {
            pos = GameCameraCtr.Center;
        }

        var animal = activityPlace.AddAnimal(id, animalInfo.SkinId, pos);
        if (animal == null)
            return;

        _startEditAction?.Invoke(animal);
    }

    public void ChangeAnimalSkinToPlace(int id, int skinId)
    {
        var animalMgr = Get<Game.AnimalManager>();
        if (animalMgr == null)
            return;

        var activityPlace = placeMgr?.ActivityPlace;
        if (activityPlace == null)
            return;

        Vector3 pos = Vector3.zero;
        if (GameCameraCtr != null)
        {
            pos = GameCameraCtr.Center;
        }

        int currSkinId = animalMgr.GetCurrenctSkinId(id);

        if(activityPlace.ChangeAnimalSkin(id, skinId, pos, currSkinId))
        {
            animalMgr?.ApplySkin(id, skinId);
        }
    }
    #endregion

    #region Object
    public void AddObjectToPlace(int id)
    {
        var activityPlace = placeMgr?.ActivityPlace;
        if (activityPlace == null)
            return;

        var editObject = Get<Game.ObjectManager>()?.GetAddEditObject(id);
        if (editObject == null)
            return;

        Vector3 pos = Vector3.zero;
        if (GameCameraCtr != null)
        {
            pos = GameCameraCtr.Center;
        }

        var obj = activityPlace.AddObject(id, pos, editObject.UId);
        if (obj == null)
            return;

        _startEditAction?.Invoke(obj);
    }
    #endregion

    #region Edit
    public void Remove(Game.Type.EElement EElement, int id, int uId)
    {
        switch (EElement)
        {
            case Game.Type.EElement.Animal:
                {
                    placeMgr?.ActivityPlace?.RemoveAnimal(id);
                    Get<Game.AnimalManager>()?.Remove(id);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshAnimalList();

                    break;
                }

            case Game.Type.EElement.Object:
                {
                    var objectMgr = Get<Game.ObjectManager>();

                    placeMgr?.ActivityPlace?.RemoveObject(id, uId);
                    objectMgr?.Remove(id, uId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList(objectMgr);

                    break;
                }
        }

        _iGrid?.Overlap();
    }

    public void Arrange(Game.BaseElement gameBaseElement, Vector3 pos)
    {
        if (gameBaseElement == null ||
            gameBaseElement.ElementData == null)
            return;

        int placeId = 0;
        if (placeMgr != null)
        {
            placeId = placeMgr.ActivityPlace.Id;
        }

        switch (gameBaseElement.ElementData.EElement)
        {
            case Game.Type.EElement.Animal:
                {
                    Get<Game.AnimalManager>()?.ArrangeAnimal(gameBaseElement.Id, pos);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshAnimalList();

                    break;
                }

            case Game.Type.EElement.Object:
                {
                    var objectMgr = Get<Game.ObjectManager>();

                    objectMgr?.ArrangeObject(gameBaseElement.Id, gameBaseElement.UId, pos, placeId);

                    Game.UIManager.Instance?.Bottom?.EditList?.RefreshObjectList(objectMgr);

                    break;
                }
        }

        _iGrid?.Overlap();
    }
    #endregion

    #region Acquire
    public void AddAcquire(Game.Type.EElement eElement, Game.Type.EAcquireAction eAcquireAction, int value)
    {
        var eAcquire = eElement == Game.Type.EElement.Animal ? Game.Type.EAcquire.AnimalCurrency : Game.Type.EAcquire.ObjectCurrency;

        Get<Game.Manager.Acquire>()?.Add(eAcquire, eAcquireAction, value);
        RecordContainer?.Add(eAcquire, eAcquireAction, value);
    }
    #endregion
}

