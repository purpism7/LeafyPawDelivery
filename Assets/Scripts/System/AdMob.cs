using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;
using UnityEngine.Localization.Settings;

namespace GameSystem
{
    public class AdMob : MonoBehaviour
    {
        private static AdMob _instance = null;
        public static void Create()
        {
            if (_instance == null)
            {
                var gameObj = new GameObject(typeof(AdMob).Name);
                if (!gameObj)
                    return;

                _instance = gameObj.GetOrAddComponent<AdMob>();
                _instance?.Initialize();
            }
        }

        public static AdMob Get
        {
            get
            {
                Create();

                return _instance;
            }
        }

        private Dictionary<string, RewardedInterstitialAd> _rewardedInterstitialAdDic = null;
        //private RewardedInterstitialAd _rewardedInterstitialAd = null;

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
        }

        private void InitializeRewardedInterstitialAdDic()
        {
            if (_rewardedInterstitialAdDic != null)
                return;

            _rewardedInterstitialAdDic = new();
            _rewardedInterstitialAdDic?.Clear();
        }

        ///// Loads the rewarded interstitial ad.
        ///// </summary>
        public void LoadRewardedInterstitialAd(string id, System.Action callbck = null)
        {
            // Clean up the old ad before loading a new one.

            if(_rewardedInterstitialAdDic != null)
            {
                if(_rewardedInterstitialAdDic.TryGetValue(id, out RewardedInterstitialAd ad))
                {
                    _rewardedInterstitialAdDic[id].Destroy();
                    _rewardedInterstitialAdDic[id] = null;
                }
            }

            //if(_rewardedInterstitialAd != null)
            //{
            //    _rewardedInterstitialAd.Destroy();
            //    _rewardedInterstitialAd = null;
            //}

            //Debug.Log("Loading the rewarded interstitial ad = " + id);

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");
            // send the request to load the ad.

            //RewardedAd.Load(id, new AdRequest.Builder().Build(),
            //    (ad, error) =>
            //    {
            //        if (error != null || ad == null)
            //        {
            //            Debug.LogError(error);

            //            return;
            //        }

            //        _rewardedInterstitialAdDic?.TryAdd(id, ad);

            //        callbck?.Invoke();
            //    });
            RewardedInterstitialAd.Load(id, adRequest,
                (ad, error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError(error);

                        return;
                    }

                    InitializeRewardedInterstitialAdDic();

                    _rewardedInterstitialAdDic?.TryAdd(id, ad);

                    ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
                    ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                   
                    callbck?.Invoke();
                });
        }

        public void ShowAd(string id, System.Action<double> callback)
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                var localKey = "check_internet_connection";
                var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                Game.Toast.Get?.Show(local, localKey);

                return;
            }

            _adId = id;
            _callback = callback;

            Game.UIManager.Instance?.ActivateSreenSaver(Game.Type.EScreenSaverType.ShowAD);

            InitializeRewardedInterstitialAdDic();

            RewardedInterstitialAd ad = null;

            _rewardedInterstitialAdDic?.TryGetValue(id, out ad);

            if (ad == null)
            {
                LoadRewardedInterstitialAd(id,
                    () =>
                    {
                        ShowAd(id, callback);
                    });

                return;
            }

            if (ad.CanShowAd())
            {
                ad.Show(
                    (reward) =>
                    {                        
                        _reward = reward;
                    });
            }
            else
            {
                LoadRewardedInterstitialAd(id,
                    () =>
                    {
                        ShowAd(id, callback);
                    });
            }
        }

        private void OnAdFullScreenContentFailed(AdError adError)
        {
            if (adError == null)
                return;

            Debug.Log(adError.GetMessage());

            Game.Toast.Get?.Show(adError.GetMessage());

            Game.UIManager.Instance?.DeactivateScreenSaver();
        }

        private void OnAdFullScreenContentClosed()
        {
            _callback?.Invoke(_reward != null ? _reward.Amount : 0);
            _callback = null;

            Game.UIManager.Instance?.DeactivateScreenSaver();

            if(!string.IsNullOrEmpty(_adId))
            {
                LoadRewardedInterstitialAd(_adId, null);
            }

            _adId = string.Empty;
        }
    }
}

