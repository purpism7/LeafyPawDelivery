using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class ActivityArea : Base<ActivityArea.Data>
    {
        public interface IListener
        {
            void PlaceAnimal(ActivityArea activityArea);
        }

        public class Data : BaseData
        {
            public IListener IListener = null;
        }

        public Transform AnimalRootTm;
        public Transform DecoRootTm;
        public Transform AreaTm;
        public Transform DropItemRootTm;

        private IListener _iListener = null;
        private List<DropItem> _dropItemList = new();

        public bool PlayingAnimal { get; private set; } = false;

        public override void Initialize(Data data)
        {
            base.Initialize(data);

            _iListener = data?.IListener;

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

        //public bool PlaceAnimal(int animalId)
        //{
        //    if (PlayingAnimal)
        //    {
        //        return false;
        //    }

        //    var animal = new GameSystem.AnimalCreator()
        //        .SetAnimalId(animalId)
        //        .SetRoot(AnimalRootTm)
        //        .SetDropItemAction(OnDropItem)
        //        .Create();

        //    PlayingAnimal = animal != null;

        //    return PlayingAnimal;
        //}

        //private void OnDropItem(Data.DropItem dropItemData, Transform rootTm)
        //{
        //    Debug.Log(dropItemData.Id);

        //    _dropItemList.Add(new DropItemCreator()
        //        .SetRootTm(DropItemRootTm)
        //        .SetStartRootTm(rootTm)
        //        .SetDropItemData(dropItemData)
        //        .Create());
        //}

        public override void OnTouch(Touch touch)
        {
            base.OnTouch(touch);

            _iListener?.PlaceAnimal(this);
        }
    }
}

