using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;
using UnityEngine.Localization.Settings;
using Cysharp.Threading.Tasks;

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
        //private int _retryCnt = 0;
        //private bool _showAd = false;

        private void Initialize()
        {
            MobileAds.Initialize((InitializationStatus status) =>
            {
                //Debug.Log("AdMob InitializationStatus = " + status);
            });

            InitializeRewardedInterstitialAdDic();

            MobileAds.RaiseAdEventsOnUnityMainThread = true;
        }

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

        ///// Loads the rewarded interstitial ad.
        ///// </summary>
        public void LoadRewardedInterstitialAd(string id, System.Action callbck = null)
        {
            // Clean up the old ad before loading a new one.

            if(_rewardedInterstitialAdDic != null)
            {
                if(_rewardedInterstitialAdDic.TryGetValue(id, out RewardedInterstitialAd rewardedInterstitialAd))
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
                        Debug.Log(error);

                        _callback?.Invoke(0);

                        ShowToastTryLater();

                        Game.UIManager.Instance?.DeactivateScreenSaver();

                        return;
                    }

                    InitializeRewardedInterstitialAdDic();

                    if(_rewardedInterstitialAdDic.TryGetValue(id, out RewardedInterstitialAd rewardedInterstitialAd))
                    {
                        _rewardedInterstitialAdDic[id] = ad;
                    }
                    else
                    {
                        _rewardedInterstitialAdDic?.TryAdd(id, ad);
                    }

                    ad.OnAdFullScreenContentFailed += OnAdFullScreenContentFailed;
                    ad.OnAdFullScreenContentClosed += OnAdFullScreenContentClosed;
                   
                    callbck?.Invoke();
                });
        }

        public void ShowAd(string id, System.Action<double> callback)
        {
            //_showAd = false;

            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                var localKey = "check_internet_connection";
                var local = LocalizationSettings.StringDatabase.GetLocalizedString("UI", localKey, LocalizationSettings.SelectedLocale);

                Game.Toast.Get?.Show(local, localKey);

                return;
            }

            //if(_retryCnt >= 3)
            //{
            //    _adId = string.Empty;
            //    _reward = null;
            //    _retryCnt = 0;

            //    callback?.Invoke(0);

            //    ShowToastTryLater();

            //    return;
            //}

            _adId = id;
            _callback = callback;

            _reward = null;

            //++_retryCnt;

#if UNITY_IOS
            Game.UIManager.Instance?.ActivateSreenSaver(Game.Type.EScreenSaverType.ShowAD);
#else
            Game.UIManager.Instance?.ActivateSreenSaver();
#endif

            InitializeRewardedInterstitialAdDic();

            RewardedInterstitialAd ad = null;
            _rewardedInterstitialAdDic?.TryGetValue(id, out ad);

            //DeactivateScreenSaverAsync().Forget();

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

                //_showAd = true;
                //_retryCnt = 0;
                Game.UIManager.Instance?.DeactivateScreenSaver();
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

        //private async UniTask DeactivateScreenSaverAsync()
        //{
        //    await UniTask.WaitForSeconds(4f);

        //    if(!_showAd)
        //    {
        //        Game.UIManager.Instance?.DeactivateScreenSaver();
        //    }
        //}

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
            Game.UIManager.Instance?.DeactivateScreenSaver();

            _callback?.Invoke(_reward != null ? _reward.Amount : 0);
           
            //if(!string.IsNullOrEmpty(_adId))
            //{
            //    LoadRewardedInterstitialAd(_adId, null);
            //}

            _adId = string.Empty;
            _reward = null;
            _callback = null;
            //_retryCnt = 0; 
        }
    }
}

