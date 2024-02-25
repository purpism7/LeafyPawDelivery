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

        private void Initialize()
        {
            MobileAds.Initialize((InitializationStatus status) =>
            {
                Debug.Log("AdMob InitializationStatus = " + status);
            });

            _rewardedInterstitialAdDic = new();
            _rewardedInterstitialAdDic.Clear();
        }

        ///// Loads the rewarded interstitial ad.
        ///// </summary>
        public void LoadRewardedInterstitialAd(string id, System.Action callbck = null)
        {
            // Clean up the old ad before loading a new one.

            if(_rewardedInterstitialAdDic != null)
            {
                if (_rewardedInterstitialAdDic.TryGetValue(id, out RewardedInterstitialAd rewardedInterstitialAd))
                {
                    if(rewardedInterstitialAd != null)
                    {
                        _rewardedInterstitialAdDic[id].Destroy();
                        _rewardedInterstitialAdDic[id] = null;
                    }
                }
            }

            Debug.Log("Loading the rewarded interstitial ad = " + id);

            // create our request used to load the ad.
            var adRequest = new AdRequest();
            adRequest.Keywords.Add("unity-admob-sample");

            // send the request to load the ad.
            RewardedInterstitialAd.Load(id, adRequest,
                (ad, error) =>
                {
                    if (error != null || ad == null)
                    {
                        Debug.LogError(error);

                        return;
                    }

                    _rewardedInterstitialAdDic?.TryAdd(id, ad);

                    callbck?.Invoke();
                });
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

        public void ShowAd(string id, System.Action callback)
        {
            if (_rewardedInterstitialAdDic == null)
                return;

            if (_rewardedInterstitialAdDic.TryGetValue(id, out RewardedInterstitialAd rewardedInterstitialAd))
            {
                if (rewardedInterstitialAd == null)
                {
                    LoadRewardedInterstitialAd(id,
                        () =>
                        {
                            ShowAd(id, callback);
                        });

                    return;
                }

                if (rewardedInterstitialAd.CanShowAd())
                {
                    rewardedInterstitialAd.Show(
                        (reward) =>
                        {
                            callback?.Invoke();
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
            else
            {
                LoadRewardedInterstitialAd(id,
                    () =>
                    {
                        ShowAd(id, callback);
                    });
            }
        }

        //private void AdLoadCallback(RewardedInterstitialAd ad, LoadAdError error)
        //{
        //    if (error != null || ad == null)
        //    {
        //        Debug.LogError("rewarded interstitial ad failed to load an ad " +
        //                       "with error : " + error);
        //        return;
        //    }

        //    Debug.Log("Rewarded interstitial ad loaded with response : "
        //              + ad.GetResponseInfo());

        //    _rewardedInterstitialAd = ad;

           
        //    //if (_rewardedInterstitialAd.CanShowAd())
        //    //{
        //    //    //_rewardedInterstitialAd.
        //    //    _rewardedInterstitialAd.Show(
        //    //        (Reward reward) =>
        //    //        {
        //    //            Debug.Log(reward.Type + " / " + reward.Amount);

        //    //            _callback?.Invoke();
        //    //        });
        //    //}
        //}
    }
}

