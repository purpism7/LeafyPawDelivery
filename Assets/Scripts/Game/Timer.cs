using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using TMPro;
using Cysharp.Threading.Tasks;

namespace Game
{
    public class Timer : Statics<Timer>
    {
        public class Data
        {
            public bool initialize = false;
            public string key = string.Empty;
            public string ShowRootType = null;

            public TextMeshProUGUI timeTMP = null;
            public Button btn = null;
            public System.Action endAction = null;
            public float addSec = 0;

            private DateTime? _startDateTime = null;
            private DateTime? _endDateTime = null;
            private float _offsetSec = 0;

            public void SetDateTime(DateTime? startDateTime, DateTime? endDateTime, float offsetSec = 0)
            {
                _startDateTime = startDateTime;
                _endDateTime = endDateTime;
                _offsetSec = offsetSec;
            }

            public double RemainSec
            {
                get
                {
                    if (_endDateTime == null)
                        return 0;

                    if (_startDateTime == null)
                        return 0;
                    
                    return (_endDateTime.Value - _startDateTime.Value).TotalSeconds + _offsetSec - Time.realtimeSinceStartup;
                }
            }
        }

        private List<Data> _dataList = null;
        private GameSystem.WorldTime _worldTime = null;
        private string _currRootType = null; 

        public void Initialize()
        {

        }

        private void Update()
        {
            UpdateTime();
        }

        private void UpdateTime()
        {
            if (_dataList == null ||
                _dataList.Count <= 0)
                return;

            for (int i = 0; i < _dataList.Count; ++i)
            {
                var data = _dataList[i];
                if (data == null)
                    continue;

                if(_currRootType == null ||
                   _currRootType != data.ShowRootType)
                    continue;
                
                double remainSec = data.RemainSec;
                if (remainSec > 0)
                {
                    //Debug.Log("remainSec = " + i + " / " + remainSec);
                    data.timeTMP?.SetText(TimeSpan.FromSeconds(remainSec).ToString(@"hh\:mm\:ss"));
                }
                else
                {
                    data.btn?.SetInteractable(true);
                    data.endAction?.Invoke();

                    RemoveTimer(data);

                    break;
                }
            }
        }

        private async UniTask CheckTimeSyncAsync(System.Action<bool> callback)
        {
            if (_worldTime == null)
            {
                _worldTime = GameSystem.WorldTime.Get;
            }

            await _worldTime.RequestAsync();

            callback?.Invoke(_worldTime.Sync);
        }

        private async UniTask AddAsync(Data data)
        {
            data.btn?.SetInteractable(false);

            await CheckTimeSyncAsync(
                (sync) =>
                {
                    if(!sync)
                    {
                        data.timeTMP?.SetText("-");
             
                        return;
                    }
                });

            if(_worldTime.DateTime == null ||
               !_worldTime.DateTime.HasValue)
            {
                data.timeTMP?.SetText("-");

                return;
            }
            
            if (_dataList == null)
            {
                _dataList = new();
                _dataList.Clear();
            }

            bool setDateTime = false;
            var worldUtcDateTime = _worldTime.DateTime.Value.ToUniversalTime();
            
            if (!string.IsNullOrEmpty(data.key))
            {
                var dateTimeStr = PlayerPrefs.GetString(data.key);
                if(!string.IsNullOrEmpty(dateTimeStr))
                {
                    if (System.DateTime.TryParse(dateTimeStr, out DateTime dateTime))
                    {
                        if ((dateTime - worldUtcDateTime).TotalSeconds <= 0)
                        {
                            RemoveTimer(data);
                        }
                        else
                        {
                            if (CheckExist(data.key))
                                return;
                            
                            data.SetDateTime(worldUtcDateTime, dateTime);

                            setDateTime = true;
                        }
                    }
                }
            }

            if(!setDateTime)
            {
                if(data.initialize)
                {
                    data.btn?.SetInteractable(true);
                    data.endAction?.Invoke();

                    return;
                }
                else
                {
                    if (data.addSec <= 0)
                    {
                        data.btn?.SetInteractable(true);
                        data.endAction?.Invoke();

                        return;
                    }
                    
                    DateTime endDateTime = worldUtcDateTime.AddSeconds(data.addSec);
                    
                    data.SetDateTime(worldUtcDateTime, endDateTime, Time.realtimeSinceStartup);

                    PlayerPrefs.SetString(data.key, endDateTime.ToString());
                }    
            }

            _dataList?.Add(data);
        }

        private void RemoveTimer(Timer.Data data)
        {
            if (data == null)
                return;

            PlayerPrefs.SetString(data.key, string.Empty);

            if(_dataList != null)
            {
                bool isRemove = _dataList.Remove(data);
            }
        }

        public Timer SetRootType(string rootType)
        {
            _currRootType = rootType;

            return this;
        }
        
        public void Add(Data data)
        {
            if (data == null)
                return;

            AddAsync(data).Forget();
        }

        private bool CheckExist(string key)
        {
            if (_dataList == null)
                return false;

            var findData = _dataList.Find(data => data.key == key);
            if (findData == null)
                return false;

            return findData.RemainSec > 0;
        }
    }
}

