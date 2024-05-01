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
                var gameObj = new GameObject(typeof(AdProvider).Name);
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
        private string _adId = string.Empty;
        private System.Action<double> _callback = null;

        private void Initialize()
        {
            MobileAds.Initialize((InitializationStatus status) =>
            {
                //Debug.Log("AdMob InitializationStatus = " + status);
            });

            InitializeRewardedInterstitialAdDic();
            InitializeIronSource();
            
            MobileAds.RaiseAdEventsOnUnityMainThread = true;
        }

        private void InitializeIronSource()
        {
            IronSourceRewardedVideoEvents.onAdOpenedEvent += RewardedVideoOnAdOpenedEvent;
            IronSourceRewardedVideoEvents.onAdClosedEvent += RewardedVideoOnAdClosedEvent;
            IronSourceRewardedVideoEvents.onAdAvailableEvent += RewardedVideoOnAdAvailable;
            IronSourceRewardedVideoEvents.onAdUnavailableEvent += RewardedVideoOnAdUnavailable;
            IronSourceRewardedVideoEvents.onAdShowFailedEvent += RewardedVideoOnAdShowFailedEvent;
            IronSourceRewardedVideoEvents.onAdRewardedEvent += RewardedVideoOnAdRewardedEvent;
            IronSourceRewardedVideoEvents.onAdClickedEvent += RewardedVideoOnAdClickedEvent;
        }

        /************* RewardedVideo AdInfo Delegates *************/
        // Indicates that there’s an available ad.
        // The adInfo object includes information about the ad that was loaded successfully
        // This replaces the RewardedVideoAvailabilityChangedEvent(true) event
        void RewardedVideoOnAdAvailable(IronSourceAdInfo adInfo)
        {
        }
        
        // Indicates that no ads are available to be displayed
        // This replaces the RewardedVideoAvailabilityChangedEvent(false) event
        void RewardedVideoOnAdUnavailable()
        {
        }
        
        // The Rewarded Video ad view has opened. Your activity will loose focus.
        void RewardedVideoOnAdOpenedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("RewardedVideoOnAdOpenedEvent");
        }
        
        // The Rewarded Video ad view is about to be closed. Your activity will regain its focus.
        void RewardedVideoOnAdClosedEvent(IronSourceAdInfo adInfo)
        {
            Debug.Log("RewardedVideoOnAdClosedEvent");
        }
        
        // The user completed to watch the video, and should be rewarded.
        // The placement parameter will include the reward data.
        // When using server-to-server callbacks, you may ignore this event and wait for the ironSource server callback.
        void RewardedVideoOnAdRewardedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
            if (placement == null)
                return;
        
            _callback?.Invoke(placement.getRewardAmount());
        }
        
        // The rewarded video ad was failed to show.
        void RewardedVideoOnAdShowFailedEvent(IronSourceError error, IronSourceAdInfo adInfo)
        {
            if (error == null)
                return;
        
            Game.Toast.Get?.Show(error.getDescription(), error.getErrorCode().ToString());
        }
        
        // Invoked when the video ad was clicked.
        // This callback is not supported by all networks, and we recommend using it only if
        // it’s supported by all networks you included in your build.
        void RewardedVideoOnAdClickedEvent(IronSourcePlacement placement, IronSourceAdInfo adInfo)
        {
        }

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

        public void ShowAd(string id, System.Action<double> callback)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                ShowToastInternetConnection();

                return;
            }

            _adId = id;
            _callback = callback;

            _reward = null;

#if UNITY_IOS
            Game.UIManager.Instance?.ActivateSreenSaver(Game.Type.EScreenSaverType.ShowAD);
#else
            Game.UIManager.Instance?.ActivateSreenSaver();
#endif

            InitializeRewardedInterstitialAdDic();

            RewardedInterstitialAd ad = null;
            _rewardedInterstitialAdDic?.TryGetValue(id, out ad);

            if (ad == null)
            {
                LoadRewardedInterstitialAd(id,
                    () => { ShowAd(id, callback); });

                return;
            }

            if (ad.CanShowAd())
            {
                ad.Show(
                    (reward) => { _reward = reward; });

                Game.UIManager.Instance?.DeactivateScreenSaver();
            }
            else
            {
                LoadRewardedInterstitialAd(id,
                    () => { ShowAd(id, callback); });
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

            if(!string.IsNullOrEmpty(_adId))
            {
                LoadRewardedInterstitialAd(_adId, null);
            }

            _adId = string.Empty;
            _reward = null;
            _callback = null;
        }
    }
}

