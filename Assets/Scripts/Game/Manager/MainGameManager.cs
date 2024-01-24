using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;

public interface IEvent
{
    void Starter();
}

public class MainGameManager : Singleton<MainGameManager>
{
    #region Inspector
    [SerializeField]
    private Game.PlaceManager placeMgr = null;
    [SerializeField]
    private Game.BoostManager boostMgr = null;
    #endregion

    public IGameCameraCtrProvider IGameCameraCtrProvider { get; private set; } = null;
    public Game.State.Base GameState { get; private set; } = null;
    public Game.RecordContainer RecordContainer { get; private set; } = null;

    private System.Action<Game.Base> _startEditAction = null;
    
    private IGrid _iGrid = null;
    private bool _endInitialize = false;

    private List<IUpdater> _iUpdaterList = new();
    private List<IFixedUpdater> _iFixedUpdaterList = new();

    private static Dictionary<Type, MonoBehaviour> _managerDic = new();

    //public ServerTime ServerTime { get; private set; } = null;

    public float GamePlayTimeSec { get; private set; } = 0;

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
        _endInitialize = false;

        _managerDic.Clear();

        AddManager(typeof(Game.AnimalManager), gameObject.GetOrAddComponent<Game.AnimalManager>());
        AddManager(typeof(Game.ObjectManager), gameObject.GetOrAddComponent<Game.ObjectManager>());

        AddManager(typeof(Game.PlaceManager), placeMgr);
        AddManager(typeof(Game.StoryManager), gameObject.GetOrAddComponent<Game.StoryManager>());
        AddManager(typeof(Game.Manager.Guide), gameObject.GetOrAddComponent<Game.Manager.Guide>());
        AddManager(typeof(Game.Manager.Acquire), gameObject.GetOrAddComponent<Game.Manager.Acquire>());
        AddManager(typeof(Game.BoostManager), boostMgr);

        //ServerTime = gameObject.GetOrAddComponent<ServerTime>();
    }

    public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
    {
        yield return StartCoroutine(base.CoInit(iProvider));

        var activityPlaceId = PlayerPrefs.GetInt("LastPlaceId", Game.Data.Const.StartPlaceId);//placeMgr.ActivityPlaceId; 

        var inputMgr = iProvider.Get<InputManager>();
        IGameCameraCtrProvider = inputMgr?.GameCameraCtr;

        _iGrid = inputMgr?.grid;

        InitializeIUpdateList(inputMgr);

        RecordContainer = new();

        var acquireMgr = Get<Game.Manager.Acquire>();
        yield return StartCoroutine(acquireMgr?.CoInitialize(null));

        // 진입 연출 전 Deactivate Top, Bottom 
        //Game.UIManager.Instance?.DeactivateAnim();

        SetGameState<Game.State.Enter>();

        yield return StartCoroutine(CoInitializeManager(activityPlaceId));
        yield return StartCoroutine(Get<Game.StoryManager>().CoInitialize(null));
        yield return StartCoroutine(boostMgr?.CoInitialize(new Game.BoostManager.Data
        {
            boostRootRectTm = Game.UIManager.Instance?.Top?.boostRootRectTm,
        }));

        Info.Connector.Create(transform);
        Game.Notification.Create(transform);

        //yield return EndLoadAsync(true);

        Debug.Log("End MainGameMgr Initialize");
        _endInitialize = true;
    }

    private void InitializeIUpdateList(InputManager inputMgr)
    {
        _iUpdaterList?.Clear();
        _iUpdaterList?.Add(inputMgr);
        _iUpdaterList?.Add(placeMgr);
        _iUpdaterList?.Add(boostMgr);
        _iUpdaterList?.Add(Game.UIManager.Instance);

        _iFixedUpdaterList?.Clear();
        _iFixedUpdaterList?.Add(IGameCameraCtrProvider as IFixedUpdater);

        //_iUpdaterList?.Add(inputMgr?.grid);
    }

    private void AddManager(Type type, MonoBehaviour monoBehaviour)
    {
        if (monoBehaviour == null)
            return;

        _managerDic.TryAdd(type, monoBehaviour);
    }

    private IEnumerator CoInitializeManager(int placeId)
    {
        if (placeMgr != null)
        {
            yield return StartCoroutine(placeMgr.CoInitialize(
                new Game.PlaceManager.Data()
                {
                    placeId = placeId,
                    //setting = setting,
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

        yield return null;

        _iGrid?.Overlap();
    }

    public async UniTask EndLoadAsync(bool initialize)
    {
        AnimEnterPlace();

        //await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
        await UniTask.Yield();

        Starter();

        Game.Notification.Get?.AllNotify();
    }

    private void AnimEnterPlace()
    {
        Sequencer.EnqueueTask(
            () =>
            {
                var enterPlace = new PopupCreator<UI.EnterPlace, UI.EnterPlace.Data>()
                    .SetReInitialize(true)
                    .SetShowBackground(false)
                    .Create();

                enterPlace?.PlayAnim(IGameCameraCtrProvider,
                    () =>
                    {
                        SetGameState<Game.State.Game>();
                    });

                return enterPlace;
            });
    }

    private void Starter()
    {
        //Debug.Log("Starter");

        foreach (var manager in _managerDic.Values)
        {
            var iEvent = manager as IEvent;
            if (iEvent == null)
                continue;

            iEvent.Starter();
        }
    }

    private void Update()
    {
        if (_iUpdaterList != null)
        {
            foreach (var iUpdater in _iUpdaterList)
            {
                iUpdater?.ChainUpdate();
            }
        }

        if(_endInitialize)
        {
            GamePlayTimeSec += Time.deltaTime;
#if UNITY_EDITOR
            UI.ITop iTop = Game.UIManager.Instance?.Top;
            if (iTop != null)
            {
                iTop.GamePlatTimeTMP?.SetText(GamePlayTimeSec.ToString("F0"));
            }
#endif
        }
    }

    private void FixedUpdate()
    {
        if (_iFixedUpdaterList != null)
        {
            foreach (var iFixedUpdater in _iFixedUpdaterList)
            {
                iFixedUpdater?.ChainFixedUpdate();
            }
        }
    }

    #region GameState
    public void SetGameState<T>() where T : Game.State.Base
    {
        if (GameState != null &&
            GameState.Type.Equals(typeof(T)))
            return;

        GameState?.End();

        GameState = System.Activator.CreateInstance<T>();
        GameState?.Initialize(this);
    }
    #endregion

    public void SetStartEditAction(System.Action<Game.Base> action)
    {
        _startEditAction = action;
    }

    // 동물 / 꾸미기 획득.
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

        SetGameState<Game.State.Enter>();

        AsyncDelay(endMoveAction).Forget();

        PlayerPrefs.SetInt("LastPlaceId", placeId);
    }

    private async UniTask AsyncDelay(System.Action endMoveAction)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(2f, 3f)));

        endMoveAction?.Invoke();

        await UniTask.Yield();

        await EndLoadAsync(false);
    }
    #endregion

    #region Animal & Object
    public bool CheckIsAll
    {
        get
        {
            var animalMgr = Get<Game.AnimalManager>();
            if (animalMgr == null)
                return false;

            if (!animalMgr.CheckIsAll)
                return false;

            var objectMgr = Get<Game.ObjectManager>();
            if (objectMgr == null)
                return false;

            if (!objectMgr.CheckIsAll)
                return false;

            var user = Info.UserManager.Instance?.User;
            if (user == null)
                return false;

            Info.UserManager.Instance?.SaveLastPlaceId();
            
            Info.Connector.Get?.SetOpenPlace(user.LastPlaceId);

            return true;
        }
    }

    public void AddAnimalToPlace(int id)
    {
        var animalInfo = Get<Game.AnimalManager>()?.GetAnimalInfo(id);
        if (animalInfo == null)
            return;

        var activityPlace = placeMgr?.ActivityPlace;
        if (activityPlace == null)
            return;

        Vector3 pos = Vector3.zero;
        if (IGameCameraCtrProvider != null)
        {
            pos = IGameCameraCtrProvider.Center;
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
        if (IGameCameraCtrProvider != null)
        {
            pos = IGameCameraCtrProvider.Center;
        }

        int currSkinId = animalMgr.GetCurrenctSkinId(id);

        if(activityPlace.ChangeAnimalSkin(id, skinId, pos, currSkinId))
        {
            animalMgr?.ApplySkin(id, skinId);
        }
    }

    public void AddObjectToPlace(int id)
    {
        var activityPlace = placeMgr?.ActivityPlace;
        if (activityPlace == null)
            return;

        var editObject = Get<Game.ObjectManager>()?.GetAddEditObject(id);
        if (editObject == null)
            return;

        Vector3 pos = Vector3.zero;
        if (IGameCameraCtrProvider != null)
        {
            pos = IGameCameraCtrProvider.Center;
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

        var element = gameBaseElement.ElementData.EElement;

        int placeId = 0;
        if (placeMgr != null)
        {
            placeId = placeMgr.ActivityPlace.Id;
        }

        switch (element)
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

        AddAcquireArrange(element, Game.Type.EAcquireAction.Use, 1);            

        _iGrid?.Overlap();
    }
    #endregion

    #region Acquire
    public void AddAcquireCurrency(Game.Type.EElement eElement, Game.Type.EAcquireAction eAcquireAction, int value)
    {
        var eAcquire = eElement == Game.Type.EElement.Animal ? Game.Type.EAcquire.AnimalCurrency : Game.Type.EAcquire.ObjectCurrency;

        AddAcquire(eAcquire, eAcquireAction, value);
    }

    public void AddAcquireArrange(Game.Type.EElement eElement, Game.Type.EAcquireAction eAcquireAction, int value)
    {
        var eAcquire = eElement == Game.Type.EElement.Animal ? Game.Type.EAcquire.Animal : Game.Type.EAcquire.Object;

        AddAcquire(eAcquire, eAcquireAction, value);
    }

    public void AddAcquire(Game.Type.EAcquire eAcquire, Game.Type .EAcquireAction eAcquireAction, int value)
    {
        Get<Game.Manager.Acquire>()?.Add(eAcquire, eAcquireAction, value);
        RecordContainer?.Add(eAcquire, eAcquireAction, value);
    }
    #endregion
}

