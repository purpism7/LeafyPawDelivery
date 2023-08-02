using System;
using System.Collections;
using System.Collections.Generic;
using GameSystem;
using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace UI.Component
{
    public class ArrangementCell : UI.Base<ArrangementCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            
            public int Id = 0;
            public Type.EElement EElement = Type.EElement.None; 
            public string Name = string.Empty;
            public bool Lock = true;
        }
        
        public interface IListener
        {
            void Edit(Type.EElement EElement, int id);
        }

        #region Inspector
        [SerializeField] private TextMeshProUGUI nameTMP;
        [SerializeField] private Button arrangementBtn = null;
        [SerializeField] private Image iconImg = null;

        [Header("Lock")]
        [SerializeField] private RectTransform lockRootRectTm = null;
        [SerializeField] private RectTransform openCondtionRootRectTm = null;
        #endregion

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetNameTMP();
            SetIconImg();
            SetButtonState();
            SetLockData();

            UIUtils.SetActive(lockRootRectTm, _data.Lock);
        }

        private void SetNameTMP()
        {
            nameTMP?.SetText(_data.Name);
        }

        private void SetIconImg()
        {
            if (_data == null)
                return;

            iconImg.sprite = GameUtils.GetShortIconSprite(_data.EElement, _data.Id);

            if (_data.Lock)
            {
                UIUtils.SetSilhouetteColorImg(iconImg);
            }
            else
            {
                UIUtils.SetOriginColorImg(iconImg);
            }
        }

        private void SetLockData()
        {
            if (_data == null)
                return;

            if (!_data.Lock)
                return;

            if(_data.EElement == Type.EElement.Animal)
            {
                SetAnimalOpenCondition();
            }
            else
            {
                SetObjectOpenCondition();
            }
        }

        private void SetAnimalOpenCondition()
        {
            var animalOpenConditionContainer = AnimalOpenConditionContainer.Instance;
            var openCondition = animalOpenConditionContainer?.GetData(_data.Id);
            if (openCondition == null)
                return;

            var openConditionData = new OpenCondition.Data()
            {
                Text = GetReqAnimalCurrency(openCondition.AnimalCurrency),
                IsPossible = animalOpenConditionContainer.CheckAnimalCurrency(_data.Id),
            };

            CreateOpenCondition(openConditionData);

            openConditionData = new OpenCondition.Data()
            {
                Text = GetReqObjectCurrency(openCondition.ObjectCurrency),
                IsPossible = animalOpenConditionContainer.CheckObjectCurrency(_data.Id),
            };

            CreateOpenCondition(openConditionData);
        }

        private void SetObjectOpenCondition()
        {
            var objectOpenConditionContainer = ObjectOpenConditionContainer.Instance;
            var openCondition = objectOpenConditionContainer?.GetData(_data.Id);
            if (openCondition == null)
                return;

            var openConditionData = new OpenCondition.Data()
            {
                Text = GetReqAnimalCurrency(openCondition.AnimalCurrency),
                IsPossible = objectOpenConditionContainer.CheckAnimalCurrency(_data.Id),
            };

            CreateOpenCondition(openConditionData);

            openConditionData = new OpenCondition.Data()
            {
                Text = GetReqObjectCurrency(openCondition.ObjectCurrency),
                IsPossible = objectOpenConditionContainer.CheckObjectCurrency(_data.Id),
            };

            CreateOpenCondition(openConditionData);
        }

        private string GetReqAnimalCurrency(long currency)
        {
            return string.Format("Arcon x{0}", currency);
        }

        private string GetReqObjectCurrency(long currency)
        {
            return string.Format("Leaf x{0}", currency);
        }

        private UI.Component.OpenCondition CreateOpenCondition(OpenCondition.Data openConditionData)
        {
            return new ComponentCreator<OpenCondition, OpenCondition.Data>()
                    .SetData(openConditionData)
                    .SetRootRectTm(openCondtionRootRectTm)
                    .Create();
        }

        private void SetButtonState()
        {
            // UIUtils.SetActive(buyBtn?.gameObject, _data.Lock);
            UIUtils.SetActive(arrangementBtn?.gameObject, !_data.Lock);
        }

        private void CreateUnlockPopup()
        {
            if (_data == null)
                return;

            if (Enum.TryParse(_data.EElement.ToString(), out Type.EOpen eOpen))
            {

                //OpenConditionContainer.Instance.a
                //var openCondition = MainGameManager.Instance?.OpenCondition;
                //if (openCondition == null)
                //    return;

                //if (openCondition.CheckOpenCondition(eOpen, _data.Id))
                //{
                //new PopupCreator<Unlock, Unlock.Data>()
                //    .SetData(new Unlock.Data()
                //    {
                //        EElement = _data.EElement,
                //        Id = _data.Id,
                //        ClickAction = () =>
                //        {

                //        },
                //    })
                //    .SetCoInit(true)
                //    .SetReInitialize(true)
                //    .Create();
                //}
                //else
                //{
                //    Debug.Log("오픈 조건 미 충족.");
                //}
            }
        }

        public void Unlock(Type.EElement EElement, int id)
        {
            if (_data == null)
                return;

            if (_data.EElement != EElement)
                return;

            if (_data.Id != id)
                return;
            
            _data.Lock = false;
            
            SetIconImg();
            SetButtonState();
            
            UIUtils.SetActive(lockRootRectTm, _data.Lock);
        }
        
        public void OnClickUnlock()
        {
            CreateUnlockPopup();
        }
        
        public void OnClick()
        {
            if (_data == null)
                return;
            
            _data.IListener?.Edit(_data.EElement, _data.Id);
        }
    }
}

