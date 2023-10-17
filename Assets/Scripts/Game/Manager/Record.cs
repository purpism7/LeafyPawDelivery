using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Manager
{
    public class Record
    {
        public class Data
        {
            
        }

        private List<Info.BaseRecord> _recordList = new();

        public void AddRecord(Info.BaseRecord baseRecord)
        {
            if (baseRecord == null)
                return;

            switch(baseRecord)
            {
                case Info.AcquireRecord record:
                    {
                        break;
                    }
            }
        }
    }
}

