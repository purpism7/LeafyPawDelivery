using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using TMPro;
using UnityEngine.Localization.Settings;
using UI.Component;
using GameSystem;

namespace UI
{
    public class Profile : BasePopup<Profile.Data>, SkinCell.IListener
    {
        public class Data : BaseData
        {
            public int Id = 0;
            public Game.Type.EElement EElement = Game.Type.EElement.None;
        }

        [SerializeField] private Image iconImg = null;
        [SerializeField] private RectTransform renderTextureRootRectTm = null;
        [SerializeField] private TextMeshProUGUI nameTMP = null;
        [SerializeField] private TextMeshProUGUI descTMP = null;
        [SerializeField] private RectTransform getCurrencyRootRectTm = null;

        [Header("Skin")]
        [SerializeField]
        private RectTransform skinRootRectTm = null;
        [SerializeField]
        private ToggleGroup skinToggleGroup = null;

        private List<SkinCell> _skinCellList = new();
        private int _selectSkinId = -1;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            UIUtils.SetActive(iconImg?.gameObject, false);
            UIUtils.SetActive(renderTextureRootRectTm, false);

            if (_data == null)
                return;

            SetNameTMP();
            SetDescTMP();

            if (_data.EElement == Game.Type.EElement.Animal)
            {
                var animalInfo = MainGameManager.Instance?.AnimalMgr.GetAnimalInfo(data.Id);
                if (animalInfo == null)
                    return;

                SetRenderTexture(animalInfo.SkinId);
            }
            else if (_data.EElement == Game.Type.EElement.Object)
            {
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

            base.Deactivate();
        }

        private void SetNameTMP()
        {
            if (_data == null)
                return;

            var localName = GameUtils.GetName(_data.EElement, _data.Id);

            nameTMP?.SetText(localName);
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

        }

        private UI.Component.OpenCondition CreateGetCurrency(OpenCondition.Data openConditionData)
        {
            return new ComponentCreator<OpenCondition, OpenCondition.Data>()
                    .SetData(openConditionData)
                    .SetRootRectTm(getCurrencyRootRectTm)
                    .Create();
        }

        #region Skin
        private void SetAnimalSkinList()
        {
            if (_data == null)
                return;

            if (_data.EElement != Game.Type.EElement.Animal)
                return;

            var skinList = AnimalSkinContainer.Instance.GetSkinList(_data.Id);
            if (skinList == null)
                return;

            int currenctSkinId = MainGameManager.Instance.AnimalMgr.GetCurrenctSkinId(_data.Id);
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
                    Sprite = ResourceManager.Instance.AtalsLoader.GetSprite("AnimalSkinIcon_1", animalSkin.ImgName),
                    ToggleGroup = skinToggleGroup,
                    ToggleOn = currenctSkinId == animalSkin.Id,
                };

                var findSkinCell = _skinCellList?.Find(skinCell => skinCell != null && !skinCell.IsActivate);
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

        void SkinCell.IListener.Select(int id, System.Action<bool> enableBuyRootAction)
        {
            if (_data == null)
                return;

            var mainGameMgr = MainGameManager.Instance;
            if (mainGameMgr == null)
                return;

            var animalMgr = mainGameMgr.AnimalMgr;
            if (animalMgr == null)
                return;

            var animalSkinData = AnimalSkinContainer.Instance?.GetData(id, _data.Id);
            if (animalSkinData == null)
                return;

            // skin 을 보유하고 있을 경우, 선택 시 바로 적용.
            if (animalMgr.CheckExistSkin(_data.Id, id))
            {
                if (_selectSkinId == id)
                    return;

                SetRenderTexture(id);

                mainGameMgr.ChangeAnimalSkinToPlace(_data.Id, id);
            }
            else
            {
                // 미리보기.
                if (_selectSkinId == id)
                {
                    // 한 번 더 클릭으로 구매.

                    return;
                }

                SetRenderTexture(id);

                enableBuyRootAction?.Invoke(true);
            }

            _selectSkinId = id;
        }
        #endregion
    }
}

