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
    public class Top : Common<Top.Data>
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

        [SerializeField] private Image animalCurrencyImg = null;
        [SerializeField] private Image objectCurrencyImg = null;

        [SerializeField] private RectTransform collectCurrencyRootRectTm = null;
        [SerializeField] private RectTransform addCurrencyRootRectTm = null;
        
        [SerializeField] private RectTransform objectCurrencyRectTm = null;
        [SerializeField] private RectTransform animalCurrencyRectTm = null;
        [SerializeField] private RectTransform cashCurrencyRectTm = null;

        private List<CollectCurrency> _collectCurrencyList = new();
        private List<AddCurrency> _addCurrencyList = new();

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            PlaceManager.Event?.RemoveListener(OnChangedPlace);
            PlaceManager.Event?.AddListener(OnChangedPlace);

            _collectCurrencyList?.Clear();
            _addCurrencyList?.Clear();
            
            Initialize();
        }

        private void Initialize()
        {
            SetCurrencyImg();
            SetCurrency();
        }

        public void SetCurrency()
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
            MainGameManager.Instance?.AddAcquire(eElement, Game.Type.EAcquireAction.Obtain, 1); 
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

        private void OnChangedPlace(int placeId)
        {
            Debug.Log("Top OnChangedPlace = " + placeId);
            Initialize();
        }

        public void OnClickCurrency()
        {
            var popup = new GameSystem.PopupCreator<Shop, Shop.Data_>()
                .SetCoInit(true)
                .Create();
        }

        public void OnClickSetting()
        {
            var popup = new GameSystem.PopupCreator<Setting, UI.BaseData>()
               .Create();
        }

        public void OnClickScreenshot()
        {
            MainGameManager.Instance.SetGameState<Game.State.Screenshot>();
        }
    }
}

