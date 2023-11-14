using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;

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

            if (_instance != null)
            {
                DontDestroyOnLoad(_instance);
            }
        }
        
        private RewardedInterstitialAd _rewardedInterstitialAd = null;

        public string AdUnitId { get; private set; } = string.Empty;

        private void Initialize()
        {
            MobileAds.Initialize((InitializationStatus status) =>
            {
                Debug.Log("AdMob InitializationStatus = " + status);

                SetAdUnitId();
            });
        }

        private void SetAdUnitId()
        {
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                AdUnitId = "ca-app-pub-3940256099942544/6978759866";
            }
            else
            {
                AdUnitId = "ca-app-pub-3940256099942544/5354046379";
            }
        }

        /// Loads the rewarded interstitial ad.
        /// </summary>
        public void LoadRewardedInterstitialAd()
        {
            // Clean up the old ad before loading a new one.
            if (_rewardedInterstitialAd != null)
            {
                _rewardedInterstitialAd.Destroy();
                _rewardedInterstitialAd = null;
            }

            Debug.Log("Loading the rewarded interstitial ad.");

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            RewardedInterstitialAd.Load(AdUnitId, adRequest, AdLoadCallback);
              //  (RewardedInterstitialAd ad, LoadAdError error) =>
              //  {
              //// if error is not null, the load request failed.
              //if (error != null || ad == null)
              //      {
              //          Debug.LogError("rewarded interstitial ad failed to load an ad " +
              //                         "with error : " + error);
              //          return;
              //      }

              //      Debug.Log("Rewarded interstitial ad loaded with response : "
              //                + ad.GetResponseInfo());

              //      _rewardedInterstitialAd = ad;
              //  });
        }

        private void AdLoadCallback(RewardedInterstitialAd ad, LoadAdError error)
        {
            if (error != null || ad == null)
            {
                Debug.LogError("rewarded interstitial ad failed to load an ad " +
                               "with error : " + error);
                return;
            }

            Debug.Log("Rewarded interstitial ad loaded with response : "
                      + ad.GetResponseInfo());

            _rewardedInterstitialAd = ad;

            //if(_rewardedInterstitialAd.CanShowAd())
            //{
            //    _rewardedInterstitialAd.
            //}
        }
    }
}

