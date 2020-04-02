using System.Collections.Generic;
using System;
using UnityEngine;

namespace ADBridge.Ironsouce {

    public class IronSourceBridge : IAdBridge {

        private IronSourceListenerBanner _banner;
        private IronSourceListenerInterstitial _interstitial;
        private IronSourceListenerReward _reward;

        private enum InitState {
            UnInit,
            Initing,
            Inited
        }

        private InitState _initState;

        public bool IsInited => _initState == InitState.Inited;


        public void Init(string id, bool debug, Action onInit) {
            if (_initState != InitState.UnInit) {
                return;
            }
            Log("Init Start");
            _initState = InitState.Initing;

            new GameObject("Loom").AddComponent<Loom>();

            _banner = new IronSourceListenerBanner();
            _interstitial = new IronSourceListenerInterstitial();
            _reward = new IronSourceListenerReward();

            IronSourceConfig.Instance.setClientSideCallbacks(true);
            IronSource.Agent.validateIntegration();
            IronSource.Agent.setAdaptersDebug(debug);
            IronSource.Agent.init(id);

            IronSource.Agent.loadInterstitial();
            IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);

            _initState = InitState.Inited;
            onInit?.Invoke();
            Log("Init Finish");
        }

        public void Request(AdUnit adUnit) {
            if (!IsInited) {
                return;
            }

            switch (adUnit.adType) {
                case AdType.Reward:
                    break;
                case AdType.Banner:
                    IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM);
                    break;
                case AdType.Interstitial:
                    IronSource.Agent.loadInterstitial();
                    break;
                default:
                    break;
            }
        }

        public void ShowAd(AdUnit adUnit) {
            if (!IsInited) {
                return;
            }
            switch (adUnit.adType) {
                case AdType.Reward:
                    IronSource.Agent.showRewardedVideo();
                    break;
                case AdType.Banner:
                    IronSource.Agent.displayBanner();
                    break;
                case AdType.Interstitial:
                    IronSource.Agent.showInterstitial();
                    break;
                default:
                    break;
            }
        }

        public void ShowAd(AdUnit adUnit, IAdNotify adNotify = null) {
            if (!IsInited) {
                return;
            }

            switch (adUnit.adType) {
                case AdType.Reward:
                    _reward.SetNotify((adNotify as IRewardADNotify));
                    IronSource.Agent.showRewardedVideo();
                    break;
                case AdType.Banner:
                    _banner.SetNotify(adNotify);
                    IronSource.Agent.displayBanner();
                    break;
                case AdType.Interstitial:
                    _interstitial.SetNotify(adNotify);
                    IronSource.Agent.showInterstitial();
                    break;
                default:
                    break;
            }
        }

        public void CloseAd(AdUnit adUnit) {
            if (!IsInited) {
                return;
            }

            switch (adUnit.adType) {
                case AdType.Reward:
                    break;
                case AdType.Banner:
                    IronSource.Agent.hideBanner();
                    break;
                case AdType.Interstitial:
                    break;
                default:
                    break;
            }
        }

        public void SetNotify(AdType adType, IAdNotify adNotify) {

            switch (adType) {
                case AdType.Reward:
                    _reward.SetNotify((adNotify as IRewardADNotify));
                    break;
                case AdType.Banner:
                    _banner.SetNotify(adNotify);

                    break;
                case AdType.Interstitial:
                    _interstitial.SetNotify(adNotify);
                    break;
                default:
                    break;
            }
        }

        public void SetAlwayNotify(AdType adType, IAdNotify adNotify) {

            switch (adType) {
                case AdType.Reward:
                    _reward.SetAlwayNotify((adNotify as IRewardADNotify));
                    break;
                case AdType.Banner:
                    _banner.SetAlwayNotify(adNotify);
                    break;
                case AdType.Interstitial:
                    _interstitial.SetAlwayNotify(adNotify);
                    break;
                default:
                    break;
            }
        }

        public bool IsAdReady(AdUnit adUnit) {
            if (!IsInited) {
                return false;
            }

            switch (adUnit.adType) {
                case AdType.Reward:
                    return IronSource.Agent.isRewardedVideoAvailable();
                case AdType.Banner:
                    return true;
                case AdType.Interstitial:
                    return IronSource.Agent.isInterstitialReady();
                default:
                    return false;
            }
        }

        internal static void Log(string info) {
            Debug.LogWarning($"[ADBridge][IronSource] {info}");
        }

        void IAdBridge.Log(string info) {
            IronSourceBridge.Log(info);
        }
    }
}
