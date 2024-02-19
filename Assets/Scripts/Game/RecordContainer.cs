using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game
{
    public class RecordContainer : Info.Holder.Base
    {
        protected override string JsonFilePath => RootJsonFilePath;

        private string JsonFileName = "Record.txt";
        private const string _secretKey = "hanKYUlrecOrd";

        private List<Info.Record> _recordList = new();

        public void Initialize()
        {
            RootJsonFilePath = Utility.GetInfoPath();

            LoadInfo();

            //AnimalManager.Event?.AddListener(OnChangedAnimalInfo);
        }

        public override void LoadInfo()
        {
            
        }

        private void SaveInfo()
        {
            if (_recordList == null)
                return;

            var jsonStr = JsonHelper.ToJson(_recordList.ToArray());
            var encryptStr = jsonStr.Encrypt(_secretKey);
            var fullPath = Path.Combine(JsonFilePath, JsonFileName);

            System.IO.File.WriteAllText(fullPath, encryptStr);
        }

        private int GetFindIndex(Info.Record record)
        {
            if (record == null)
                return -1;

            if (_recordList == null)
                return -1;

            for(int i = 0; i < _recordList.Count; ++i)
            {
                var compRecord = _recordList[i];
                if (compRecord == null)
                    continue;

                if (record.EAcquire != compRecord.EAcquire ||
                    record.EAcquireAction != compRecord.EAcquireAction)
                    continue;

                return i;
            }

            return -1;
        }

        private void Add(Info.Record record)
        {
            if (record == null)
                return;

            int findIndex = GetFindIndex(record);
            if (findIndex <= -1)
            {
                _recordList.Add(record);
            }
            else
            {
                _recordList[findIndex].Value += record.Value;
            }

            SaveInfo();
        }

        public void Add(Type.EAcquire eAcquire, Type.EAcquireAction eAcquireAction, int value)
        {
            Add(new Info.Record(eAcquire, eAcquireAction)
            {
                Value = value,
            });
        }

        public Info.Record Get(Type.EAcquire eAcquire, Type.EAcquireAction eAcquireAction)
        {
            if (_recordList == null)
                return null;

            foreach(var record in _recordList)
            {
                if (record == null)
                    continue;

                if (record.EAcquire != eAcquire ||
                    record.EAcquireAction != eAcquireAction)
                    continue;

                return record;
            }

            return null;
        }

        //private void OnChangedAnimalInfo(Game.Event.AnimalData animalData)
        //{
        //    if (animalData == null)
        //        return;
        //}
    }
}

