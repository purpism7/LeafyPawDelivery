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

            public TextMeshProUGUI timeTMP = null;
            public Button btn = null;
            public System.Action endAction = null;
            public float addSec = 0;

            private DateTime? _startDateTime = null;
            private DateTime? _endDateTime = null;
            private double _sec = 0;

            public void SetDateTime(DateTime? startDateTime, DateTime? endDateTime)
            {
                _startDateTime = startDateTime;
                _endDateTime = endDateTime;
                _sec = 0;
            }

            public double RemainSec
            {
                get
                {
                    _sec += Time.deltaTime;
                    Debug.Log((_endDateTime.Value - _startDateTime.Value).TotalSeconds);
                    return (_endDateTime.Value - _startDateTime.Value).TotalSeconds - _sec;
                }
            }
        }

        private List<Data> _dataList = null;
        private GameSystem.WorldTime _worldTime = null;

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

            if (_worldTime == null)
                return;

            if (_worldTime.DateTime == null ||
               !_worldTime.DateTime.HasValue)
                return;

            var worldDateTime = _worldTime.DateTime.Value;

            //int timeSec = (int)MainGameManager.Instance.GamePlayTimeSec;

            for (int i = 0; i < _dataList.Count; ++i)
            {
                var data = _dataList[i];
                if (data == null)
                    continue;

                //var endDateTime = data.endDateTime;
                //if (data.endDateTime == null)
                //{
                //    _dataList.Remove(data);

                //    break;
                //}

                double remainSec = data.RemainSec;
                if (remainSec > 0)
                {
                    data.timeTMP?.SetText(TimeSpan.FromSeconds(remainSec).ToString(@"hh\:mm\:ss"));
                }
                else
                {
                    data.btn?.SetInteractable(true);
                    data.endAction?.Invoke();

                    _dataList.Remove(data);

                    break;
                }
            }
        }

        public async UniTask CheckTimeSyncAsync(System.Action<bool> callback)
        {
            if (_worldTime == null)
            {
                _worldTime = GameSystem.WorldTime.Get;
            }

            await _worldTime.RequestAsync();

            callback?.Invoke(_worldTime.Sync);
        }

        public void Add(Data data)
        {
            if (data == null)
                return;

            AddAsync(data).Forget();
        }

        private async UniTask AddAsync(Data data)
        {
            data.btn?.SetInteractable(false);

            await CheckTimeSyncAsync(
                (sync) =>
                {
                    if(!sync)
                    {
                        if (data != null)
                        {
                            data.timeTMP?.SetText("-");
                            //data.endDateTime = null;
                        }
             
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
            var worldUTCDateTime = _worldTime.DateTime.Value.ToUniversalTime();

            if (!string.IsNullOrEmpty(data.key))
            {
                var dateTimeStr = PlayerPrefs.GetString(data.key);
                if(!string.IsNullOrEmpty(dateTimeStr))
                {
                    if (System.DateTime.TryParse(dateTimeStr, out DateTime dateTime))
                    {
                        if ((dateTime - worldUTCDateTime).TotalSeconds <= 0)
                        {
                            PlayerPrefs.SetString(data.key, string.Empty);
                        }
                        else
                        {
                            //data.endDateTime = dateTime;

                            data.SetDateTime(worldUTCDateTime, dateTime);

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
                    DateTime endDateTime = worldUTCDateTime.AddSeconds(data.addSec);
                    data.SetDateTime(worldUTCDateTime, endDateTime);

                    PlayerPrefs.SetString(data.key, endDateTime.ToString());

                    setDateTime = true;
                }    
            }

            if (setDateTime)
            {
                _dataList?.Add(data);
            }

            //if(_updateCoroutine == null)
            //{
            //    StartUpdateCor();
            //}
        }
    }
}

