using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RecordContainer : Info.Holder.Base
    {
        protected override string JsonFilePath => RootJsonFilePath + "/Info/";
        private string JsonFileName = "Record.json";

        private List<Info.Record> _recordList = new();

        public RecordContainer()
        {
            LoadInfo();

            AnimalManager.Event?.AddListener(OnChangedAnimalInfo);
        }

        public override void LoadInfo()
        {
            
        }

        private void SaveInfo()
        {
            if (_recordList == null)
                return;

            var jsonString = JsonHelper.ToJson(_recordList.ToArray());
            Debug.Log("jsonString = " + jsonString);
            System.IO.File.WriteAllText(JsonFilePath + JsonFileName, jsonString);
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

        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return;
        }
    }
}

