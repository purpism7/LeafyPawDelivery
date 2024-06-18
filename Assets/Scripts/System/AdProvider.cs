using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

namespace GameSystem
{
    public class AdProvider : MonoBehaviour
    {
        private static AdProvider _instance = null;

        public static void Create()
        {
            if (_instance == null)
            {
                var gameObj = new GameObject(nameof(AdProvider));
                if (!gameObj)
                    return;

                _instance = gameObj.GetOrAddComponent<AdProvider>();
                _instance?.Initialize();
            }
        }

        public static AdProvider Get
        {
            get
            {
                Create();

                return _instance;
            }
        }

        private Dictionary<string, RewardedInterstitialAd> _rewardedInterstitialAdDic = null;

        private Reward _reward = null;
        private AD.Data _adData = null;
        private System.Action<double> _callback = null;

        private void Initialize()
        {
            MobileAds.Initialize((InitializationStatus status) =>
            {
                //Debug.Log("AdMob InitializationStatus = " + status);
            });

            InitializeRewardedInterstitialAdDic();
            // InitializeIronSource();
            
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
        }

        // private void InitializeIronSource()
        // {
        //     IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
        //     IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
        //     IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
        //     IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
        //     IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
        //     IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
        //     IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
        // }
        //
        // /************* RewardedVideo AdInfo Delegates *************/
        // // Indicates that there’s an available ad.
        // // The adInfo object includes information about the ad that was loaded successfully
        // // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
        // void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
        // {
        // }
        //
        // // Indicates that no ads are available to be displayed
        // // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
        // void RewardedVideoOnAdUnavailable()
        // {
        //     Game.UIManager.Instance?.DeactivateScreenSaver();
        // }
        //
        // // The Rewarded Video ad view has opened. Your activity will loose focus.
        // void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        // {
        //     Debug.Log("RewardedVideoOnAdOpenedEvent");
        // }
        //
        // // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
        // void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        // {
        //     Debug.Log("RewardedVideoOnAdClosedEvent");
        //     
        //     _callback?.Invoke(0);
        //     
        //     Game.UIManager.Instance?.DeactivateScreenSaver();
        // }
        //
        // // The user completed to watch the video, and should be rewarded.
        // // The placement parameter will include the reward data.
        // // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
        // void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        // {
        //     Debug.Log("RewardedVideoOnAdRewardedEvent");
        //     
        //     Game.UIManager.Instance?.DeactivateScreenSaver();
        //     
        //     if (!CheckPossibleReward(placement))
        //     {
        //         _callback?.Invoke(0);
        //         
        //         return;
        //     }
        //     
        //     _callback?.Invoke(placement.getRewardAmount());
        // }
        //
        // // The rewarded video ad was failed to show.
        // void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
        // {
        //     Debug.Log("RewardedVideoOnAdShowFailedEvent");
        //     
        //     _callback?.Invoke(0);
        //     
        //     Game.UIManager.Instance?.DeactivateScreenSaver();
        //     
        //     if (error == null)
        //         return;
        //
        //     ShowToastTryLater();
        //     // Game.Toast.Get?.Show(error.getDescription(), error.getErrorCode().ToString());
        // }
        //
        // // Invoked when the video ad was clicked.
        // // This callback is not supported by all networks, and we recommend using it only if
        // // it’s supported by all networks you included in your build.
        // void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        // {
        //     // Game.UIManager.Instance?.DeactivateScreenSaver();
        // }
        //
        // private bool CheckPossibleReward(IronSourcePlacement placement)
        // {
        //     if (placement == null)
        //         return false;
        //
        //     if (_adData == null)
        //         return false;
        //
        //     if (!_adData.Placement.Equals(placement.getPlacementName()))
        //         return false;
        //
        //     return true;
        // }

        // public void ShowAd(string adId, System.Action<double> callbackAction)
        // {
        //     _callback = callbackAction;
        //
        //     // IronSource.Agent.showRewardedVideo(adId);
        // }

        private void InitializeRewardedInterstitialAdDic()
        {
            if (_rewardedInterstitialAdDic != null)
                return;

            _rewardedInterstitialAdDic = new();
            _rewardedInterstitialAdDic?.Clear();
        }

        private void ShowToastTryLater()
        {
            var localKey = "desc_try_later";
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);
        
            Game.Toast.Get?.Show(local, localKey);
        }

        private void ShowToastInternetConnection()
        {
            var localKey = "check_internet_connection";
            var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

            Game.Toast.Get?.Show(local, localKey);
        }

        ///// Loads the rewarded interstitial ad.
        ///// </summary>
        private void LoadRewardedInterstitialAd(string id, System.Action callback = null)
        {
            // Clean up the old ad before loading a new one.

            if (_rewardedInterstitialAdDic != null)
            {
                if (_rewardedInterstitialAdDic.TryGetValue(id, out RewardedInterstitialAd rewardedInterstitialAd))
                {
                    _rewardedInterstitialAdDic[id]?.Destroy();
                    _rewardedInterstitialAdDic[id] = null;
                }
            }

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");
            // send the request to load the ad.

            RewardedInterstitialAd.Load(id, adRequest,
                (ad, error) =>
                {
                    if (error != null || ad == null)
                    {
                        // if (_adData != null)
                        // {
                        //     IronSource.Agent?.showRewardedVideo(_adData.Placement);
                        //
                        //     return;
                        // }
                        
                        _callback?.Invoke(0);

                        ShowToastTryLater();

                        Game.UIManager.Instance?.DeactivateScreenSaver();

                        return;
                    }

                    InitializeRewardedInterstitialAdDic();

                    if (_rewardedInterstitialAdDic.TryGetValue(id, out RewardedInterstitialAd rewardedInterstitialAd))
                    {
                        _rewardedInterstitialAdDic[id] = ad;
                    }
                    else
                    {
                        _rewardedInterstitialAdDic?.TryAdd(id, ad);
                    }

                    ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
                    ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;

                    callback?.Invoke();
                });
        }

        public void ShowAd(AD.Data adData, System.Action<double> callback)
        // public void ShowAd(string id, System.Action<double> callback)
        {
            if (adData == null)
                return;
            
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowToastInternetConnection();

                return;
            }

            _adData = adData;
            _callback = callback;

            _reward = null;

#if UNITY_IOS
            Game.UIManager.Instance?.ActivateSreenSaver(Game.Type.EScreenSaverType.ShowAD);
#else
            Game.UIManager.Instance?.ActivateSreenSaver();
#endif
            
            // IronSource.Agent?.showRewardedVideo(_adData.Placement);
            //
            // return;

            InitializeRewardedInterstitialAdDic();

            var adId = adData.adId;
            RewardedInterstitialAd rewardedInterstitialAd = null;
            
            _rewardedInterstitialAdDic?.TryGetValue(adData.adId, out rewardedInterstitialAd);

            if (rewardedInterstitialAd == null)
            {
                LoadRewardedInterstitialAd(adId,
                    () => { ShowAd(adData, callback); });

                return;
            }

            if (rewardedInterstitialAd.CanShowAd())
            {
                rewardedInterstitialAd.Show(
                    (reward) => { _reward = reward; });

                Game.UIManager.Instance?.DeactivateScreenSaver();
            }
            else
            {
                LoadRewardedInterstitialAd(adId,
                    () => { ShowAd(adData, callback); });
            }
        }

         private void OnAdFullScreenContentFailed(AdError adError)
        {
            if (adError == null)
                return;

            Game.Toast.Get?.Show(adError.GetMessage());

            Game.UIManager.Instance?.DeactivateScreenSaver();
        }

        private void OnAdFullScreenContentClosed()
        {
            Game.UIManager.Instance?.DeactivateScreenSaver();

            _callback?.Invoke(_reward != null ? _reward.Amount : 0);

            if(_adData != null && 
               !string.IsNullOrEmpty(_adData.adId))
            {
                LoadRewardedInterstitialAd(_adData.adId, null);
            }

            _adData = null;
            _reward = null;
            _callback = null;
        }
    }
}

