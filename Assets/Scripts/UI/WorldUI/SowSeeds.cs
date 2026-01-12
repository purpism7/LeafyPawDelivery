
using System;
using Cysharp.Threading.Tasks;
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
            
        }

        [SerializeField] private TextMeshProUGUI remainingTimeText = null;
        [SerializeField] private Animator sowSeedsAnimator = null;
        [SerializeField] private Animator waterAnimator = null;

        public IPoolable Poolable => this;

        //private void LateUpdate()
        //{
        //    ChainLateUpdate();
        //}

        public override void Initialize(Data data)
        {
            base.Initialize(data);
            
            // EndSowSeeds();
        }

        public override void Activate()
        {
            base.Activate();
            
            EndSowSeeds();
        }

        private void EndSowSeeds()
        {
            sowSeedsAnimator?.SetActive(false);
            waterAnimator?.SetActive(true);
            
            var animationWater = waterAnimator?.GetBehaviour<AnimationWater>();
            if (animationWater != null)
            {
                animationWater.ExitAction -= EndWater;
                animationWater.ExitAction += EndWater;
            }
            
            waterAnimator?.SetTrigger("water");
        }

        private void EndWater()
        {
            waterAnimator?.SetActive(false);
            sowSeedsAnimator?.SetActive(true);
            
            var animationSowSeeds = sowSeedsAnimator?.GetBehaviour<AnimationSowSeeds>();
            if (animationSowSeeds != null)
            {
                animationSowSeeds.ExitAction -= EndSowSeeds;
                animationSowSeeds.ExitAction += EndSowSeeds;
            }
            
            sowSeedsAnimator?.SetTrigger("sow_seeds");
        }
        
        public void UpdateTimerText(float seconds)
        {
            if (seconds < 0) 
                seconds = 0;

            TimeSpan time = TimeSpan.FromSeconds(seconds);

            string format = time.TotalHours >= 1 ? @"hh\:mm\:ss" : @"mm\:ss";
            string formattedTime = seconds > 0 ? time.ToString(format) : string.Empty;

            remainingTimeText?.SetText(formattedTime);
        }
    }
}