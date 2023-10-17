using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class RecordContainer : Info.Holder.Base
    {
        protected override string JsonFilePath => RootJsonFilePath + "/Info/";
        private string JsonFileName = "Record.json";

        private List<Info.BaseRecord> _recordList = new();

        public RecordContainer()
        {
            LoadInfo();

            AnimalManager.Listener?.AddListener(OnChangedAnimalInfo);
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

        public void AddRecord(Info.BaseRecord baseRecord)
        {
            if (baseRecord == null)
                return;

            int findIndex = GetFindIndex(baseRecord);
            if (findIndex <= -1)
            {
                _recordList.Add(baseRecord);
            }
            else
            {
                _recordList[findIndex].Value += baseRecord.Value;
            }

            SaveInfo();
        }

        private int GetFindIndex(Info.BaseRecord baseRecord)
        {
            switch (baseRecord)
            {
                case Info.AcquireRecord record:
                    return GetFindIndex(record);

                default:
                    return -1;
            }
        }

        private int GetFindIndex(Info.AcquireRecord acquireRecord)
        {
            if (_recordList == null)
                return -1;

            for(int i = 0; i < _recordList.Count; ++i)
            {
                var record = _recordList[i];
                if (record == null)
                    continue;

                if (!(record is Info.AcquireRecord))
                    continue;

                var compRecord = record as Info.AcquireRecord;

                if (acquireRecord.EAcquire != compRecord.EAcquire ||
                    acquireRecord.EAcquireAction != compRecord.EAcquireAction)
                    continue;

                return i;
            }

            return -1;
        }

        private void OnChangedAnimalInfo(Info.Animal animalInfo)
        {
            if (animalInfo == null)
                return;
        }
    }
}

