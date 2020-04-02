using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADBridge.MTG
{
    public class MTGBridge : IAdBridge
    {
        /// <summary>
        /// 广告加载失败后重新加载的等待时间
        /// </summary>
        internal const int FAILED_RETRY_DELAY = 8;
        internal static AdUnit rewardUnit;
        internal static AdUnit interUnit;
        internal static AdUnit bannerUnit;

        private MTGListenerBanner _banner;
        private MTGListenerIntersitital _intersitital;
        private MTGListenerReward _reward;

        private enum InitState
        {
            UnInit,
            Initing,
            Inited
        }
        private InitState _initState;

        public bool IsInited => _initState == InitState.Inited;

        public void Init(string appId, string appKey, IEnumerable<AdUnit> adUnits, bool debug, Action onInit)
        {
            if (_initState != InitState.UnInit)
            {
                return;
            }

            new GameObject("AdBridge_Loom").AddComponent<Loom>();

            _initState = InitState.Initing;
            Mintegral.initMTGSDK(appId, appKey);

            _banner = new MTGListenerBanner();
            _intersitital = new MTGListenerIntersitital();
            _reward = new MTGListenerReward();

            List<MTGInterstitialVideoInfo> interstitialInfo = new List<MTGInterstitialVideoInfo>();
            List<string> rewardIds = new List<string>();
            List<string> bannerIds = new List<string>();

            foreach (AdUnit adUnit in adUnits)
            {
                switch (adUnit.adType)
                {
                    case AdType.Banner:
                        bannerIds.Add(adUnit.id);
                        break;
                    case AdType.Interstitial:
                        MTGInterstitialVideoInfo info = new MTGInterstitialVideoInfo()
                        {
                            adUnitId = adUnit.id
                        };
                        interstitialInfo.Add(info);
                        break;
                    case AdType.Reward:
                        rewardIds.Add(adUnit.id);
                        break;
                    default:
                        break;
                }
            }

            if (interstitialInfo.Count > 0)
            {
                Mintegral.loadInterstitialVideoPluginsForAdUnits(interstitialInfo.ToArray());
            }
            if (bannerIds.Count > 0)
            {
                Mintegral.loadBannerPluginsForAdUnits(bannerIds.ToArray());
            }
            if (rewardIds.Count > 0)
            {
                Mintegral.loadRewardedVideoPluginsForAdUnits(rewardIds.ToArray());
            }

            _initState = InitState.Inited;
            onInit?.Invoke();

            Log("Inited");
        }

        public bool IsAdReady(AdUnit adUnit)
        {
            if (!IsInited)
            {
                return false;
            }
            switch (adUnit.adType)
            {
                case AdType.Banner: return true;
                case AdType.Interstitial: return true;
                case AdType.Reward: return Mintegral.isVideoReadyToPlay(adUnit.id);
                default:
                    break;
            }
            return false;
        }

        public void Request(AdUnit adUnit)
        {
            if (!IsInited)
            {
                return;
            }
            switch (adUnit.adType)
            {
                case AdType.Banner:
                    bannerUnit = adUnit;
                    break;
                case AdType.Interstitial:
                    interUnit = adUnit;
                    Mintegral.requestInterstitialVideoAd(adUnit.id);
                    break;
                case AdType.Reward:
                    rewardUnit = adUnit;
                    Mintegral.requestRewardedVideo(adUnit.id);
                    break;
                default:
                    break;
            }
        }

        public void SetAlwayNotify(AdType adType, IAdNotify adNotify)
        {
            switch (adType)
            {
                case AdType.Banner:
                    _banner.SetAlwayNotify(adNotify);
                    break;
                case AdType.Interstitial:
                    _intersitital.SetAlwayNotify(adNotify);
                    break;
                case AdType.Reward:
                    _reward.SetAlwayNotify(adNotify as IRewardADNotify);
                    break;
                default:
                    break;
            }
        }

        public void SetNotify(AdType adType, IAdNotify adNotify)
        {
            switch (adType)
            {
                case AdType.Banner:
                    _banner.SetNotify(adNotify);
                    break;
                case AdType.Interstitial:
                    _intersitital.SetNotify(adNotify);
                    break;
                case AdType.Reward:
                    _reward.SetNotify(adNotify as IRewardADNotify);
                    break;
                default:
                    break;
            }
        }

        public void ShowAd(AdUnit adUnit)
        {
            if (!IsInited)
            {
                return;
            }
            switch (adUnit.adType)
            {
                case AdType.Banner:
                    Mintegral.createBanner(adUnit.id, Mintegral.BannerAdPosition.BottomCenter, 600, 50, false);
                    break;
                case AdType.Interstitial:
                    Mintegral.showInterstitialVideoAd(adUnit.id);
                    break;
                case AdType.Reward:
                    Mintegral.showRewardedVideo(adUnit.id);
                    break;
                default:
                    break;
            }
        }

        public void ShowAd(AdUnit adUnit, IAdNotify adNotify)
        {
            if (!IsInited)
            {
                return;
            }
            switch (adUnit.adType)
            {
                case AdType.Banner:
                    _banner.SetNotify(adNotify);
                    Mintegral.createBanner(adUnit.id, Mintegral.BannerAdPosition.BottomCenter, 600, 50, false);
                    break;
                case AdType.Interstitial:
                    _intersitital.SetNotify(adNotify);
                    Mintegral.showInterstitialVideoAd(adUnit.id);
                    break;
                case AdType.Reward:
                    _reward.SetNotify(adNotify as IRewardADNotify);
                    Mintegral.showRewardedVideo(adUnit.id);
                    break;
                default:
                    break;
            }
        }

        public void CloseAd(AdUnit adUnit)
        {
            if (!IsInited)
            {
                return;
            }
            switch (adUnit.adType)
            {
                case AdType.Banner:
                    Mintegral.destroyBanner(adUnit.id);
                    break;
                case AdType.Interstitial:

                    break;
                case AdType.Reward:

                    break;
                default:
                    break;
            }
        }

        internal static void Log(string info)
        {
            Debug.Log($"[ADBridge][MTG] {info}");
        }

        void IAdBridge.Log(string info)
        {
            MTGBridge.Log(info);
        }
    }
}