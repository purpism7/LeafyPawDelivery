
using System;
using UnityEngine;
using UnityEngine.UI;

using UI.Common;
using GameSystem;
using TMPro;

namespace UI.WorldUI
{
    public interface ISowSeeds
    {
        IPoolable Poolable { get; }

        void Activate();
        void Deactivate();

        void UpdateTimerText(float seconds);
    }

    public class SowSeeds : BaseWorldUI<SowSeeds.Data>, ISowSeeds
    {
        public class Data : BaseWorldUI<SowSeeds.Data>.Data
        {
            public int BloomTimeSeconds { get; private set; } = 0;

            public Data WithBloomTimeSeconds(int bloomTimeSeconds)
            {
                BloomTimeSeconds = bloomTimeSeconds;
                return this;
            }
        }

        [SerializeField] private TextMeshProUGUI remainingTimeText = null;

        public IPoolable Poolable => this;

        private void LateUpdate()
        {
            ChainLateUpdate();
        }

        public override void Initialize(Data data)
        {
            base.Initialize(data);
        }

        public override void Activate()
        {
            base.Activate();
        }

        void ISowSeeds.UpdateTimerText(float seconds)
        {
            if (seconds < 0) 
                seconds = 0;

            TimeSpan time = TimeSpan.FromSeconds(seconds);

            string format = time.TotalHours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            string formattedTime = time.ToString(format);

            remainingTimeText?.SetText(formattedTime);
        }
    }
}