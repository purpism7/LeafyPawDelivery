using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.Localization.Settings;

using UI.Component;
using GameSystem;
using Cysharp.Threading.Tasks;

namespace UI
{
    public class Profile : BasePopup<Profile.Data>, SkinCell.IListener, BuyCash.IListener
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public Game.Type.EElement EElement = Game.Type.EElement.None;
        }

        [Header("Animal")]
        [SerializeField]
        private RectTransform renderTextureRootRectTm = null;
        [SerializeField]
        private RectTransform animalRootRectTm = null;
        [SerializeField]
        private TextMeshProUGUI nameTMP = null;
        [SerializeField]
        private TextMeshProUGUI descTMP = null;
        [SerializeField]
        private OpenCondition animalGetCurrency = null;

        [Header("Skin")]
        [SerializeField]
        private RectTransform skinRootRectTm = null;
        [SerializeField]
        private ToggleGroup skinToggleGroup = null;

        [Header("Object")]
        [SerializeField]
        private RectTransform objectRootRectTm = null;
        [SerializeField]
        private Image iconImg = null;
        [SerializeField]
        private TextMeshProUGUI objectNameTMP = null;
        [SerializeField]
        private OpenCondition objectGetCurrency = null;

        private List<SkinCell> _skinCellList = new();
        private SkinCell _selectSkinCell = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            UIUtils.SetActive(iconImg?.gameObject, false);
            UIUtils.SetActive(renderTextureRootRectTm, false);

            if (_data == null)
                return;

            UIUtils.SetActive(animalRootRectTm, _data.EElement == Game.Type.EElement.Animal);
            UIUtils.SetActive(objectRootRectTm, _data.EElement == Game.Type.EElement.Object);

            SetDescTMP();
            SetGetCurrency();

            if (_data.EElement == Game.Type.EElement.Animal)
            {
                var animalInfo = MainGameManager.Get<Game.AnimalManager>()?.GetAnimalInfo(data.Id);
                if (animalInfo == null)
                    return;

                SetSelectSkinInfo(animalInfo.SkinId);
            }
            else if (_data.EElement == Game.Type.EElement.Object)
            {
                SetObjectNameTMP();
                SetIconImg();
            }
        }

        public override void Activate()
        {
            base.Activate();

            SetAnimalSkinList();
        }

        public override void Deactivate()
        {
            DeactivateSkinCellList();
            
            Game.RenderTextureElement.Destroy();

            _selectSkinCell = null;

            base.Deactivate();
        }

        private void SetAnimalNameTMP(int skinId)
        {
            if (_data == null)
                return;

            nameTMP?.SetText(GetAnimalName(skinId));
        }

        private string GetAnimalName(int skinId)
        {
            if (_data == null)
                return string.Empty;

            return GameUtils.GetName(_data.EElement, _data.Id, skinId);
        }

        private void SetObjectNameTMP()
        {
            if (_data == null)
                return;

            var localName = GameUtils.GetName(_data.EElement, _data.Id);

            objectNameTMP?.SetText(localName);
        }

        private void SetDescTMP()
        {
            var text = string.Empty;
            if (_data.EElement == Game.Type.EElement.Animal)
            {
                text = LocalizationSettings.StringDatabase.GetLocalizedString(_data.EElement.ToString(), "description_" + _data.Id, LocalizationSettings.SelectedLocale);
            }

            descTMP?.SetText(text);
        }

        private void SetSelectSkinInfo(int skinId)
        {
            SetAnimalNameTMP(skinId);
            SetRenderTexture(skinId);
            SetAnimalGetCurrency(skinId);
        }

        private void SetRenderTexture(int skinId)
        {
            if (_data == null)
                return;

            Game.RenderTextureElement.Destroy();

            Game.RenderTextureElement.Create(
                new Game.RenderTextureElement.Data()
                {
                    Id = _data.Id,
                    SkinId = skinId,
                    EElement = _data.EElement,
                });

            UIUtils.SetActive(renderTextureRootRectTm, true);
        }

        private void SetIconImg()
        {
            if (iconImg == null)
                return;

            var sprite = GameUtils.GetLargeIconSprite(_data.EElement, _data.Id);
            iconImg.sprite = sprite;

            UIUtils.SetActive(iconImg?.gameObject, true);
        }

        private void SetGetCurrency()
        {
            if (_data == null)
                return;

            var placeData = MainGameManager.Get<Game.PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return;

            if(_data.EElement == Game.Type.EElement.Object)
            {
                var objectData = ObjectContainer.Instance.GetData(_data.Id);
                if (objectData == null)
                    return;

                objectGetCurrency?.Initialize(new OpenCondition.Data()
                {
                    ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(placeData.ObjectSpriteName),
                    Text = objectData.Currency.ToString(),
                    PossibleFunc = () => true,
                });
                objectGetCurrency?.Activate();
            }
        }

        private void SetAnimalGetCurrency(int skinId)
        {
            if (_data == null)
                return;

            var placeData = MainGameManager.Get<Game.PlaceManager>()?.ActivityPlaceData;
            if (placeData == null)
                return;

            var animalData = AnimalContainer.Instance.GetData(_data.Id);
            if (animalData == null)
                return;

            string currencyText = animalData.Currency.ToString();
            var animalSkinData = AnimalSkinContainer.Instance?.GetData(skinId, animalData.Id);
            if (animalSkinData != null &&
                animalSkinData.Bonus > 0)
            {
                currencyText = string.Format("{0} (+{1})", animalData.Currency, animalSkinData.Bonus);
            }

            animalGetCurrency?.Initialize(new OpenCondition.Data()
            {
                ImgSprite = GameSystem.ResourceManager.Instance?.AtalsLoader?.GetCurrencySprite(placeData.AnimalSpriteName),
                Text = currencyText,
                PossibleFunc = () => true,
            });

            DelayActivateAsync().Forget();
        }

        private async UniTask DelayActivateAsync()
        {
            await UniTask.WaitForEndOfFrame(this);

            animalGetCurrency?.Activate();
        }

        private int SelectSkinId
        {
            get
            {
                return _selectSkinCell != null ? _selectSkinCell.SkinId : 0;
            }
        }

        #region Skin
        private void SetAnimalSkinList()
        {
            if (_data == null)
                return;

            var animalMgr = MainGameManager.Get<Game.AnimalManager>();
            if (animalMgr == null)
                return;

            if (_data.EElement != Game.Type.EElement.Animal)
                return;

            var skinList = AnimalSkinContainer.Instance.GetSkinList(_data.Id);
            if (skinList == null)
                return;

            int currenctSkinId = animalMgr.GetCurrenctSkinId(_data.Id);
            if(currenctSkinId <= 0)
            {
                currenctSkinId = Game.Data.Const.AnimalBaseSkinId;
            }

            foreach (var animalSkin in skinList)
            {
                if (animalSkin == null)
                    continue;

                var skinCellData = new SkinCell.Data()
                {
                    IListener = this,
                    AnimalSkin = animalSkin,
                    Sprite = ResourceManager.Instance?.AtalsLoader?.GetAnimalSkinSprite(animalSkin.ImgName),
                    ToggleGroup = skinToggleGroup,
                    ToggleOn = currenctSkinId == animalSkin.Id,
                };

                var findSkinCell = _skinCellList?.Find(skinCell => skinCell != null && !skinCell.IsActivate);
                //Debug.Log(findSkinCell.SkinId);
                if (findSkinCell != null)
                {
                    findSkinCell.Initialize(skinCellData);
                    findSkinCell.Activate();

                    continue;
                }

                var component = new ComponentCreator<SkinCell, SkinCell.Data>()
                    .SetRootRectTm(skinRootRectTm)
                    .SetData(skinCellData)
                    .Create();

                _skinCellList.Add(component);
            }
        }

        private void DeactivateSkinCellList()
        {
            if (_skinCellList == null)
                return;

            foreach(var skinCell in _skinCellList)
            {
                skinCell?.Deactivate();
            }
        }

        void SkinCell.IListener.Select(SkinCell skinCell)
        {
            if (skinCell == null)
                return;

            if (_data == null)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            var animalMgr = MainGameManager.Get<Game.AnimalManager>();
            if (animalMgr == null)
                return;

            int skinId = skinCell.SkinId;
            var animalSkinData = AnimalSkinContainer.Instance?.GetData(skinId, _data.Id);
            if (animalSkinData == null)
                return;

            int selectSkinId = SelectSkinId;

            // skin 을 보유하고 있을 경우, 선택 시 바로 적용.
            if (animalMgr.CheckExistSkin(_data.Id, skinId))
            {
                if (selectSkinId == skinId)
                    return;

                SetSelectSkinInfo(skinId);

                mainGameMgr.ChangeAnimalSkinToPlace(_data.Id, skinId);
            }
            else
            {
                // 미리보기.
                if (selectSkinId == skinId)
                {
                    // 한 번 더 클릭으로 구매.
                    new PopupCreator<BuyCash, BuyCash.Data>()
                        .SetReInitialize(true)
                        .SetData(new BuyCash.Data()
                        {
                            IListener = this,
                            Cash = animalSkinData.Cash,
                            targetSprite = ResourceManager.Instance?.AtalsLoader?.GetAnimalSkinSprite(animalSkinData.ImgName),

                            scale = 1.5f,
                        })
                        .Create();

                    return;
                }

                SetSelectSkinInfo(skinId);

                skinCell?.EnableBuyRoot(true);
            }

            _selectSkinCell = skinCell;
        }
        #endregion

        #region BuyCash.IListener
        void BuyCash.IListener.Buy(bool possible)
        {
            if(!possible)
            {
                var text = LocalizationSettings.StringDatabase.GetLocalizedString("UI", "not_enough_jewel", LocalizationSettings.SelectedLocale);

                Game.Toast.Get?.Show(text);

                return;
            }

            if (_data == null)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            var animalMgr = MainGameManager.Get<Game.AnimalManager>();
            if (animalMgr == null)
                return;

            var animalId = _data.Id;
            int selectSkinId = SelectSkinId;

            var animalSkinData = AnimalSkinContainer.Instance?.GetData(selectSkinId, animalId);
            if (animalSkinData == null)
                return;

            var userMgr = Info.UserManager.Instance;
            if (userMgr == null)
                return;

            userMgr?.User?.SetCash(-animalSkinData.Cash);

            ITop iTop = Game.UIManager.Instance?.Top;
            iTop?.SetCurrency();

            animalMgr.AddSkin(animalId, selectSkinId);
            mainGameMgr.ChangeAnimalSkinToPlace(animalId, selectSkinId);
            mainGameMgr.AddAcquire(Game.Type.EAcquire.AnimalSkin, Game.Type.EAcquireAction.Obtain, 1);

            _selectSkinCell?.EnableBuyRoot(false);

            Sequencer.EnqueueTask(
                () =>
                {
                    var popup = new GameSystem.PopupCreator<UI.Obtain, UI.Obtain.Data>()
                        .SetData(new UI.Obtain.Data()
                        {
                            EElement = Game.Type.EElement.Animal,
                            Id = animalId,
                            skinId = selectSkinId,
                            ClickAction = () =>
                            {
                                
                            },
                            keepRenderTexture = true,
                        })
                        .SetCoInit(true)
                        .SetReInitialize(true)
                        .Create();

                    return popup;
                });
        }
        #endregion
    }
}

