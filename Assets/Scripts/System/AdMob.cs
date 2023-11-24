using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;

namespace GameSystem
{
    public class AdMob : MonoBehaviour
    {
        private static AdMob _instance = null;
        public static AdMob Create()
        {
            if (_instance == null)
            {
                var gameObj = new GameObject(typeof(AdMob).Name);
                if (!gameObj)
                    return null;

                _instance = gameObj.GetOrAddComponent<AdMob>();
                _instance?.Initialize();
            }

            return _instance;
        }

        private RewardedInterstitialAd _rewardedInterstitialAd = null;

        //public AdMob()
        //{
        //    Initialize();
        //}

        private void Initialize()
        {
            MobileAds.Initialize((InitializationStatus status) =>
            {
                Debug.Log("AdMob InitializationStatus = " + status);
            });
        }

        /// Loads the rewarded interstitial ad.
        /// </summary>
        public void LoadRewardedInterstitialAd(string id)
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
            RewardedInterstitialAd.Load(id, adRequest, AdLoadCallback);
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

            if(_rewardedInterstitialAd.CanShowAd())
            {
                //_rewardedInterstitialAd.
                _rewardedInterstitialAd.Show(
                    (Reward reward) =>
                    {
                        Debug.Log(reward.Type + " / " + reward.Amount);
                    });
            }
        }
    }
}

