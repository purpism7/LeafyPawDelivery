using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using DG.Tweening;

using Game;
using GameSystem;
using UI.Component;

namespace UI
{
    public interface ITop
    {
        void SetCurrency();
        void SetDropLetterCnt(int cnt, out int currCnt);
        void SetDropAnimalCurrencyCnt(int cnt);

        bool CheckMaxDropLetterCnt { get; }
        bool CheckMaxDropAnimalCurrencyCnt { get; }
    }

    public interface ITopAnim
    {
        void ActivateRight(System.Action completeAction);
        void DeactivateRight(System.Action completeAction);
    }

    public class Top : Common<Top.Data>, ITop, ITopAnim
    {
        public class Data : BaseData
        {
            
        }

        private const float InitPos = 400f;

        [SerializeField]
        private RectTransform topRootRectTm = null;
        [SerializeField]
        private RectTransform rightRootRectTm = null;

        //[SerializeField] private TextMeshProUGUI lvTMP = null;
        [SerializeField] private TextMeshProUGUI animalCurrencyTMP = null;
        [SerializeField] private TextMeshProUGUI objectCurrencyTMP = null;
        [SerializeField] private TextMeshProUGUI cashTMP = null;
        [SerializeField] private TextMeshProUGUI dropAnimalCurrencyCntTMP = null;
        [SerializeField] private TextMeshProUGUI dropLetterCntTMP = null;

        [SerializeField] private Image animalCurrencyImg = null;
        [SerializeField] private Image objectCurrencyImg = null;

        [SerializeField] private RectTransform collectCurrencyRootRectTm = null;
        [SerializeField] private RectTransform addCurrencyRootRectTm = null;
        
        [SerializeField] private RectTransform objectCurrencyRectTm = null;
        [SerializeField] private RectTransform animalCurrencyRectTm = null;
        [SerializeField] private RectTransform cashCurrencyRectTm = null;

        private List<CollectCurrency> _collectCurrencyList = new();
        private List<AddCurrency> _addCurrencyList = new();

        private Dictionary<int, int> _dropLetterCntDic = new();
        private Dictionary<int, int> _dropAnimalCurrencyCntDic = new();

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            PlaceManager.Event?.RemoveListener(OnChangedPlace);
            PlaceManager.Event?.AddListener(OnChangedPlace);

            _collectCurrencyList?.Clear();
            _addCurrencyList?.Clear();
            _dropLetterCntDic?.Clear();
            _dropAnimalCurrencyCntDic?.Clear();

            Initialize();
        }

        private void Initialize()
        {
            SetCurrencyImg();
            SetCurrency();
            SetDropLetterCnt(0);
            SetDropAnimalCurrencyCnt(0);
        }

        private void SetCurrency()
        {
            var userInfo = Info.UserManager.Instance?.User;
            if (userInfo == null)
                return;

            var currency = userInfo.GetCurrency(GameUtils.ActivityPlaceId);
            if (currency != null)
            {
                animalCurrencyTMP?.SetText(currency.Animal + "");
                objectCurrencyTMP?.SetText(currency.Object + "");
            }

            cashTMP?.SetText(userInfo.Cash + "");
        }

        private void SetCurrencyImg()
        {
            var placeData = Game.Data.Const.GetPlaceData(GameUtils.ActivityPlaceId);
            if (placeData == null)
                return;

            SetCurrencyImg(animalCurrencyImg, placeData.AnimalSpriteName);
            SetCurrencyImg(objectCurrencyImg, placeData.ObjectSpriteName);
        }

        private void SetCurrencyImg(Image img, string currency)
        {
            if (img == null)
                return;

            var atlasLoader = GameSystem.ResourceManager.Instance?.AtalsLoader;
            if (atlasLoader == null)
                return;

            img.sprite = atlasLoader.GetCurrencySprite(currency);
        }

        #region Animal Currency Count
        private void SetDropAnimalCurrencyCnt(int cnt)
        {
            int activityPlaceId = GameUtils.ActivityPlaceId;
            if (_dropAnimalCurrencyCntDic.ContainsKey(activityPlaceId))
            {
                _dropAnimalCurrencyCntDic[activityPlaceId] += cnt;
            }
            else
            {
                _dropAnimalCurrencyCntDic.Add(activityPlaceId, cnt);
            }

            SetDropAnimalCurrencyCntTMP();
        }

        private void SetDropAnimalCurrencyCntTMP()
        {
            dropAnimalCurrencyCntTMP?.SetText(string.Format("{0}/{1}", DropAnimalCurrencyCnt, Game.Data.Const.MaxDropAnimalCurrencyCount));
        }

        private int DropAnimalCurrencyCnt
        {
            get
            {
                int cnt = 0;

                if (_dropAnimalCurrencyCntDic == null)
                    return cnt;

                if (!_dropAnimalCurrencyCntDic.TryGetValue(GameUtils.ActivityPlaceId, out cnt))
                    return cnt;

                return cnt;
            }
        }
        #endregion

        #region Letter Count
        private void SetDropLetterCnt(int cnt)
        {
            int activityPlaceId = GameUtils.ActivityPlaceId;
            if (_dropLetterCntDic.ContainsKey(activityPlaceId))
            {
                _dropLetterCntDic[activityPlaceId] += cnt;
            }
            else
            {
                _dropLetterCntDic?.Add(activityPlaceId, cnt);
            }

            SetDropLetterCntTMP();
        }

        private void SetDropLetterCntTMP()
        {
            dropLetterCntTMP?.SetText(string.Format("{0}/{1}", DropLetterCnt, Game.Data.Const.MaxDropLetterCount));
        }

        private int DropLetterCnt
        {
            get
            {
                int cnt = 0;

                if (_dropLetterCntDic == null)
                    return cnt;

                if (!_dropLetterCntDic.TryGetValue(GameUtils.ActivityPlaceId, out cnt))
                    return cnt;

                return cnt;
            }
        }
        #endregion

        #region Anim Activate & Deactivate
        public void ActivateAnim(System.Action completeAction)
        {
            ActivateAnimTop(completeAction);
            ActivateAnimRight(completeAction);
        }

        public void DeactivateAnim(System.Action completeAction)
        {
            DeactivateAnimTop(completeAction);
            DeactivateAnimRight(completeAction);
        }

        private void ActivateAnimTop(System.Action completeAction)
        {
            if (!topRootRectTm)
                return;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(topRootRectTm.DOAnchorPosY(0, 0.3f).SetEase(Ease.OutBack))
                .OnComplete(() =>
                {
                    completeAction?.Invoke();
                });
            sequence.Restart();
        }

        private void DeactivateAnimTop(System.Action completeAction)
        {
            if (!topRootRectTm)
                return;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(topRootRectTm.DOAnchorPosY(InitPos, 0.3f).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    completeAction?.Invoke();
                });
            sequence.Restart();
        }

        private void ActivateAnimRight(System.Action completeAction)
        {
            if (!rightRootRectTm)
                return;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(rightRootRectTm.DOAnchorPosX(0, 0.3f).SetEase(Ease.OutBack))
                .OnComplete(() =>
                {
                    completeAction?.Invoke();
                });
            sequence.Restart();
        }

        private void DeactivateAnimRight(System.Action completeAction)
        {
            if (!rightRootRectTm)
                return;

            Sequence sequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(rightRootRectTm.DOAnchorPosX(InitPos, 0.3f).SetEase(Ease.InBack))
                .OnComplete(() =>
                {
                    completeAction?.Invoke();
                });
            sequence.Restart();
        }
        #endregion

        #region Collect Currency (Move Action Currnecy)
        public void CollectCashCurrency(Vector3 startPos, int currency)
        {
            if (_collectCurrencyList == null)
                return;

            var data = new CollectCurrency.Data()
            {
                StartPos = startPos,
                EndPos = cashCurrencyRectTm.position,
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.CurrencyCashSprite,
                CollectEndAction =
                    () =>
                    {
                        AddCashCurrency(currency);
                    },
            };

            CollectCurrency(data);

            // save currency value.
            Info.UserManager.Instance?.User?.SetCash(currency);
        }

        public void CollectCurrency(Vector3 startPos, Type.EElement eElement, int currency)
        {
            if (_collectCurrencyList == null)
                return;

            var placeData = Game.Data.Const.GetPlaceData(GameUtils.ActivityPlaceId);
            if (placeData == null)
                return;

            var currencyName = placeData.Animal.ToString();
            if(eElement == Type.EElement.Object)
            {
                currencyName = placeData.Object.ToString();
            }

            var data = new CollectCurrency.Data()
            {
                StartPos = startPos,
                EndPos = eElement == Type.EElement.Object ? objectCurrencyRectTm.position : animalCurrencyRectTm.position,
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(currencyName.ToLower()),
                CollectEndAction =
                    () =>
                    {
                        AddCurrency(eElement, currency);
                    },
            };

            CollectCurrency(data);

            // save currency value.
            Info.UserManager.Instance?.User?.SetCurrency(eElement, currency);
            MainGameManager.Instance?.AddAcquireCurrency(eElement, Game.Type.EAcquireAction.Obtain, 1);
        }

        private void CollectCurrency(CollectCurrency.Data data)
        {
            if (data == null)
                return;

            var collectCurrency = _collectCurrencyList.Find(collectCurrency => !collectCurrency.IsActivate);
            if (collectCurrency != null)
            {
                collectCurrency.Initialize(data);

                return;
            }

            var component = new ComponentCreator<CollectCurrency, CollectCurrency.Data>()
                .SetData(data)
                .SetRootRectTm(collectCurrencyRootRectTm)
                .Create();

            _collectCurrencyList.Add(component);
        }
        #endregion

        #region Add Currency (Action Currency Text)
        private void AddCurrency(Type.EElement eElement, int currency)
        {
            if (_addCurrencyList == null)
                return;

            var startPos = objectCurrencyTMP.transform.position;
            if(eElement == Type.EElement.Animal)
            {
                startPos = animalCurrencyTMP.transform.position;
            }
           
            var data = new AddCurrency.Data()
            {
                StartPos = startPos,
                EElement = eElement,
                Currency = currency,
            };

            AddCurrency(data);
        }

        private void AddCashCurrency(int currency)
        {
            if (_addCurrencyList == null)
                return;

            if (cashTMP == null)
                return;

            var startPos = cashTMP.transform.position;

            var data = new AddCurrency.Data()
            {
                StartPos = startPos,
                Currency = currency,
            };

            AddCurrency(data);
        }

        private void AddCurrency(AddCurrency.Data data)
        {
            if (data == null)
                return;

            data.StartPos.x -= 5f;
            data.StartPos.y += 20f;

            SetCurrency();

            var addCurrency = _addCurrencyList.Find(addCurrency => !addCurrency.IsActivate);
            if (addCurrency != null)
            {
                addCurrency.Initialize(data);
                addCurrency.transform.SetAsLastSibling();

                return;
            }

            var component = new ComponentCreator<AddCurrency, AddCurrency.Data>()
                .SetData(data)
                .SetRootRectTm(addCurrencyRootRectTm)
                .Create();

            _addCurrencyList.Add(component);
        }
        #endregion

        #region ITop
        void ITop.SetCurrency()
        {
            SetCurrency();
        }

        void ITop.SetDropLetterCnt(int cnt, out int currCnt)
        {
            SetDropLetterCnt(cnt);

            currCnt = DropLetterCnt;
        }

        void ITop.SetDropAnimalCurrencyCnt(int cnt)
        {
            SetDropAnimalCurrencyCnt(cnt);
        }

        bool ITop.CheckMaxDropLetterCnt
        {
            get
            {
                return DropLetterCnt >= Game.Data.Const.MaxDropLetterCount;
            }
        }

        bool ITop.CheckMaxDropAnimalCurrencyCnt
        {
            get
            {
                return DropAnimalCurrencyCnt >= Game.Data.Const.MaxDropAnimalCurrencyCount;
            }
        }
        #endregion

        #region ITopAnim
        void ITopAnim.ActivateRight(System.Action completeAction)
        {
            ActivateAnimRight(completeAction);
        }

        void ITopAnim.DeactivateRight(System.Action completeAction)
        {
            DeactivateAnimRight(completeAction);
        }
        #endregion

        private void OnChangedPlace(int placeId)
        {
            Initialize();
        }

        //public void OnClickCurrency()
        //{
        //    var popup = new GameSystem.PopupCreator<Shop, Shop.Data_>()
        //        .SetCoInit(true)
        //        .Create();

            
        //}

        public void OnClickSetting()
        {
            var popup = new GameSystem.PopupCreator<Setting, UI.BaseData>()
               .Create();

            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);
        }

        public void OnClickScreenshot()
        {
            EffectPlayer.Get?.Play(EffectPlayer.AudioClipData.EType.TouchButton);

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            var gameState = mainGameMgr.GameState;
            if (gameState == null)
                return;
            
            if (gameState.CheckState<Game.State.Edit>())
                return;

            mainGameMgr.SetGameState<Game.State.Screenshot>();
        }
    }
}

