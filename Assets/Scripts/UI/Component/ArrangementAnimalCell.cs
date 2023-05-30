using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Component
{
    public class ArrangementAnimalCell : UI.Base<ArrangementAnimalCell.Data>
    {
        public class Data : BaseData
        {
            public IListener IListener = null;
            public Animal AnimalData = null;
        }

        public interface IListener
        {
            void Edit(int animalId);
        }

        [SerializeField] private TextMeshProUGUI nameTMP;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            SetNameTMP();
        }

        private void SetNameTMP()
        {
            nameTMP?.SetText(_data.AnimalData?.Name);
        }

        public void OnClick()
        {
            var animalData = _data?.AnimalData;
            if (animalData == null)
                return;
            
            _data?.IListener?.Edit(_data.AnimalData.Id);
        }
    }
}

