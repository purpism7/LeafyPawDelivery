using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Component
{
    public class HeartCell : BaseComponent<HeartCell.Data>
    {
        public class Data : BaseData
        {
            // public int Id = 0;
            public float Progress = 0;
        }

        [SerializeField] private Image progressImg = null;

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }
        
        public override void Activate(Data data)
        {
            base.Activate(data);
            
            SetProgress();
        }

        private void SetProgress()
        {
            if (progressImg == null)
                return;

            progressImg.fillAmount = 0;
            
            if (_data == null)
                return;

            // var animalInfo = MainGameManager.Get<AnimalManager>()?.GetAnimalInfo(_data.Id);
            // if (animalInfo == null)
            //     return;
            
            progressImg.fillAmount = _data.Progress;
        }
    }
}

