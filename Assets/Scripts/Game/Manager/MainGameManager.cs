using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

using GameSystem;

public interface IEvent
{
    void Starter(System.Action endAction);
}

public class MainGameManager : Singleton<MainGameManager>, Game.TutorialManager.IListener
{
    #region Inspector
    [SerializeField]
    private Game.PlaceManager placeMgr = null;
    [SerializeField]
    private Game.BoostManager boostMgr = null;
    public Transform tutorialRootTm = null;
    #endregion

    public IGameCameraCtr IGameCameraCtr { get; private set; } = null;
    public Game.RecordContainer RecordContainer { get; private set; } = null;

    private System.Action<Game.Base> _startEditAction = null;
    
    private IGrid _iGrid = null;
    private bool _endInitialize = false;

    private List<IUpdater> _iUpdaterList = new();
    private List<IFixedUpdater> _iFixedUpdaterList = new();

    private Dictionary<Game.Type.EGameState, Game.State.Base> _gameStateDic = new();

    private static Dictionary<Type, MonoBehaviour> _managerDic = new();

    public float GamePlayTimeSec { get; private set; } = 0;
    public Game.State.Base GameState { get; private set; } = null;
    public Game.Type.EGameState EGameState { get; private set; } = Game.Type.EGameState.None;

    public Game.TutorialManager TutorialMgr { get; private set; } = null;
    public bool IsTutorial { get; private set; } = true;

    public static T Get<T>()
    {
        if (_managerDic == null)
            return default(T);

        if(_managerDic.TryGetValue(typeof(T), out MonoBehaviour manager))
        {
            if (manager == null)
                return default(T);

            return manager.GetComponent<T>();
        }

        return default(T);
    }

    protected override void Initialize()
    {
        _endInitialize = false;

        _managerDic.Clear();

        AddManager(typeof(Game.AnimalManager), gameObject.GetOrAddComponent<Game.AnimalManager>()?.Initialize());
        AddManager(typeof(Game.ObjectManager), gameObject.GetOrAddComponent<Game.ObjectManager>()?.Initialize());
        AddManager(typeof(Game.PlaceManager), placeMgr?.Initialize());
        AddManager(typeof(Game.StoryManager), gameObject.GetOrAddComponent<Game.StoryManager>()?.Initialize());
        AddManager(typeof(Game.Manager.Guide), gameObject.GetOrAddComponent<Game.Manager.Guide>()?.Initialize());
        AddManager(typeof(Game.Manager.Acquire), gameObject.GetOrAddComponent<Game.Manager.Acquire>()?.Initialize());
        
        AddManager(typeof(Game.BoostManager), boostMgr?.Initialize());

        _gameStateDic.Clear();
    }

    public override IEnumerator CoInit(GameSystem.IPreprocessingProvider iProvider)
    {
        yield return StartCoroutine(base.CoInit(iProvider));

        var activityPlaceId = PlayerPrefs.GetInt(Game.Data.PlayPrefsKeyLastPlaceKey, Game.Data.Const.StartPlaceId);//placeMgr.ActivityPlaceId; 

        var inputMgr = iProvider.Get<InputManager>();
        IGameCameraCtr = inputMgr?.GameCameraCtr;

        _iGrid = inputMgr?.grid;

        InitializeIUpdateList(inputMgr);
        
        RecordContainer = new();
        RecordContainer?.Initialize();

        var acquireMgr = Get<Game.Manager.Acquire>();
        yield return StartCoroutine(acquireMgr?.CoInitialize(null));

        // 진입 연출 전 Deactivate Top, Bottom 
        //Game.UIManager.Instance?.DeactivateAnim();

        SetGameStateAsync(Game.Type.EGameState.Enter).Forget();

        yield return StartCoroutine(CoInitializeManager(activityPlaceId));
        yield return StartCoroutine(Get<Game.StoryManager>().CoInitialize(null));
        yield return StartCoroutine(boostMgr?.CoInitialize(
            new Game.BoostManager.Data
            {
                boostRootRectTm = Game.UIManager.Instance?.Top?.boostRootRectTm,
            }));

        _iGrid?.Overlap();

        Info.Connector.Create(transform);
        Game.Notification.Create(transform);
        Game.Timer.Create(transform);
        AdMob.Create();

        IsTutorial = CheckIsTutorial;
        
        _endInitialize = true;

        yield return null;
    }

    private void InitializeIUpdateList(InputManager inputMgr)
    {
        _iUpdaterList?.Clear();
        _iUpdaterList?.Add(inputMgr);
        _iUpdaterList?.Add(placeMgr);
        _iUpdaterList?.Add(boostMgr);
        _iUpdaterList?.Add(Game.UIManager.Instance);

        _iFixedUpdaterList?.Clear();
        _iFixedUpdaterList?.Add(IGameCameraCtr as IFixedUpdater);

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
    }

    public async UniTask EndLoadAsync(bool initialize)
    {
        await AnimEnterPlaceAsync();

        await UniTask.Yield();

        if(!IsTutorial)
        {
            Starter();
        }

        Game.Notification.Get?.AllNotify();
    }

    private async UniTask AnimEnterPlaceAsync()
    {
        bool endEnterPlace = false;

        float gameCameraOrthographicSize = IGameCameraCtr.DefaultOrthographicSize;
        if(IsTutorial)
        {
            gameCameraOrthographicSize = IGameCameraCtr.OrthographicSizeForTutorial;
        }

        IGameCameraCtr?.SetStopUpdate(true);

        Sequencer.EnqueueTask(
            () =>
            {
                var enterPlace = new PopupCreator<UI.EnterPlace, UI.EnterPlace.Data>()
                    .SetReInitialize(true)
                    .SetShowBackground(false)
                    .SetData(new UI.EnterPlace.Data()
                    {
                        gameCameraOrthographicSize = gameCameraOrthographicSize
                    })
                    .Create();

                enterPlace?.PlayAnim(IGameCameraCtr,
                    () =>
                    {
                        endEnterPlace = true;
                    });

                return enterPlace;
            });

        await UniTask.WaitUntil(() => endEnterPlace);

        if(IsTutorial)
        {
            InitializeTutorialManager();

            return;
        }

        await SetGameStateAsync(Game.Type.EGameState.Game);
        GameState?.End();

        IGameCameraCtr?.SetStopUpdate(false);
    }

    private void InitializeTutorialManager()
    {
        TutorialMgr = gameObject.GetOrAddComponent<Game.TutorialManager>();
        TutorialMgr?.Initialize(tutorialRootTm);

        TutorialMgr?.AddListener(this);
        TutorialMgr?.AddListener(Get<Game.StoryManager>());
    }

    private void DestroyTutorialManager()
    {
        if (TutorialMgr == null)
            return;

        DestroyImmediate(TutorialMgr);
        TutorialMgr = null;
    }

    private void Starter()
    {
        foreach (var manager in _managerDic.Values)
        {
            var iEvent = manager as IEvent;
            if (iEvent == null)
                continue;

            iEvent.Starter(null);
        }
    }

    private bool CheckIsTutorial
    {
        get
        {
            var animalMgr = Get<Game.AnimalManager>();
            if(animalMgr != null)
            {
                if(!animalMgr.CheckGetStarter)
                    return true;
            }

            var objectMgr = Get<Game.ObjectManager>();
            if (objectMgr != null)
            {
                if (!objectMgr.CheckGetStarter)
                    return true;
            }

            var connector = Info.Connector.Get;
            if(connector != null)
            {
                if (Auth.EGameType_ == Auth.EGameType.Continue)
                {
                    connector.SetCompleteTutorial(true);
                }

                return connector.IsCompleteTutorial == false;
            }

            return false;
        }
    }

    private void Update()
    {
        if (_iUpdaterList != null)
        {
            foreach (var iUpdater in _iUpdaterList)
            {
                if (iUpdater == null)
                    continue;

                iUpdater.ChainUpdate();
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
                if (iFixedUpdater == null)
                    continue;

                iFixedUpdater.ChainFixedUpdate();
            }
        }
    }

    #region GameState
    public async UniTask SetGameStateAsync(Game.Type.EGameState eGameState)
    {
        if (EGameState == eGameState)
            return;

        if(EGameState != Game.Type.EGameState.Game)
        {
            _gameStateDic?.GetValueOrDefault(EGameState)?.End();
        }
        
        Game.State.Base gameState = null;
        if (!_gameStateDic.TryGetValue(eGameState, out gameState))
        {
            var typeStr = typeof(Game.State.Base).Namespace + "." + eGameState.ToString();
            var type = System.Type.GetType(typeStr);
            
            gameState = System.Activator.CreateInstance(type) as Game.State.Base;
            _gameStateDic?.TryAdd(eGameState, gameState);
        }

        if(gameState != null)
        {
            EGameState = eGameState;
            GameState = gameState;

            gameState.Initialize(this);
            await gameState.InitializeAsync(this);
        }

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
        MovePlaceAsync(placeId, endMoveAction).Forget();
    }

    private async UniTask MovePlaceAsync(int placeId, System.Action endMoveAction)
    {
        await LoadAssetAsync(placeId);
        await CoInitializeManager(placeId);

        SetGameStateAsync(Game.Type.EGameState.Enter).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(UnityEngine.Random.Range(0.5f, 1f)));

        _iGrid?.Overlap();

        endMoveAction?.Invoke();

        EndLoadAsync(false).Forget();

        PlayerPrefs.SetInt(Game.Data.PlayPrefsKeyLastPlaceKey, placeId);
    }

    private async UniTask LoadAssetAsync(int placeId)
    {
        var addressableAssetLoader = ResourceManager.Instance?.AddressableAssetLoader;
        if (addressableAssetLoader == null)
            return;

        await addressableAssetLoader.LoadAnimalAssetAsync(placeId);

        var typeKey = string.Format("{0}_{1}", addressableAssetLoader.AssetLabelObject, placeId);
        await addressableAssetLoader.LoaGameAssetByIdAsync(typeKey);
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
        if (IGameCameraCtr != null)
        {
            pos = IGameCameraCtr.Center;
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
        if (IGameCameraCtr != null)
        {
            pos = IGameCameraCtr.Center;
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
        if (IGameCameraCtr != null)
        {
            pos = IGameCameraCtr.Center;
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
                    var obj = gameBaseElement as Game.Object;
                    if (obj == null)
                        return;

                    var objectMgr = Get<Game.ObjectManager>();
                    objectMgr?.ArrangeObject(obj, placeId);

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

    #region Game.TutorialManager.IListener
    void Game.TutorialManager.IListener.State(Game.Type.ETutorialStep step)
    {
        switch(step)
        {
            case Game.Type.ETutorialStep.EditAnimal:
                {
                    (IGameCameraCtr as GameSystem.GameCameraController)?.SetStopUpdate(false);

                    break;
                }

            case Game.Type.ETutorialStep.DescMap:
                {
                   

                    break;
                }

            case Game.Type.ETutorialStep.HappyLeafyPawDelivery:
                {
                    IsTutorial = false;

                    SetGameStateAsync(Game.Type.EGameState.Game).Forget();
                    GameState?.End();

                    break;
                }

            case Game.Type.ETutorialStep.ReturnGame:
                {
                    IGameCameraCtr.SetOrthographicSize(IGameCameraCtr.DefaultOrthographicSize);
                    Game.UIManager.Instance?.SetInteractable(true);

                    DestroyTutorialManager();

                    break;
                }
        }
    }
    #endregion
}

