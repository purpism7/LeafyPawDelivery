using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActivityArea : Base
    {
        public interface IListener
        {
            void PlaceAnimal(ActivityArea activityArea);
        }

        public Transform AnimalRootTm;
        public Transform DecoRootTm;
        public Transform AreaTm;
        public Transform DropItemRootTm;

        private IListener _iListener = null;
        private List<DropItem> _dropItemList = new();

        public bool PlayingAnimal { get; private set; } = false;

        public override void Init(params object[] objs)
        {
            if (objs != null &&
                objs.Length > 0)
            {
                _iListener = objs[0] as IListener;
            }

            _dropItemList.Clear();

            Enable(false);
        }

        public override void ChainUpdate()
        {
            return;
        }

        public void Enable(bool enable)
        {
            AreaTm.SetActive(enable);
        }

        public bool PlaceAnimal(int animalId)
        {
            if (PlayingAnimal)
            {
                return false;
            }

            var animal = new GameSystem.AnimalCreator()
                .SetAnimalId(animalId)
                .SetRoot(AnimalRootTm)
                .SetDropItemAction(OnDropItem)
                .Create();

            PlayingAnimal = animal != null;

            return PlayingAnimal;
        }

        private void OnDropItem(Data.DropItem dropItemData, Transform rootTm)
        {
            Debug.Log(dropItemData.Id);

            _dropItemList.Add(new DropItemCreator()
                .SetRootTm(DropItemRootTm)
                .SetStartRootTm(rootTm)
                .SetDropItemData(dropItemData)
                .Create());
        }

        public override void OnTouch()
        {
            base.OnTouch();

            _iListener?.PlaceAnimal(this);
        }
    }
}

