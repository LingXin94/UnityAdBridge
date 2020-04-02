using System;
using System.Collections.Generic;
using ByteDance.Union;
using UnityEngine;

namespace ADBridge.BuAd
{
    internal class BuAdBridge : IAdBridge
    {
        //doc: https://partner.oceanengine.com/union/media/union/download/detail?id=6&docId=5de8d86cb1afac00129330f8&osType=

        /// <summary>
        /// 广告加载失败后重新加载的等待时间
        /// </summary>
        internal const int FAILED_RETRY_DELAY = 8;

        private AdNative _adNative;

        private IAdListener _reward;
        private IAdListener _interstitial;
        private IAdListener _banner;
        private IAdListener _feed;

        private enum InitState
        {
            UnInit,
            Initing,
            Inited
        }

        private InitState _initState;
        public bool IsInited => _initState == InitState.Inited;

        public static AndroidJavaObject mainActivity { get; private set; }

        public void Init(string id, bool debug, Action onInit)
        {
            if (_initState != InitState.UnInit)
            {
                return;
            }
            _initState = InitState.Initing;
            Log("Init Start");

            new GameObject("Loom").AddComponent<Loom>();

#if UNITY_ANDROI
            AndroidJavaObject @object = new AndroidJavaObject("com.bytedance.android.UnionApplication");
            AndroidJavaClass @main = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            mainActivity = @main.GetStatic<AndroidJavaObject>("currentActivity");
            @object.Call("Init", mainActivity, id, debug);
#else
            //NativeiOS.InitNativeBuADiOS(id); 
#endif
            this._adNative = SDK.CreateAdNative();


#if UNITY_ANDROID
            this._reward = new BuAdListenerReward(_adNative);
            this._interstitial = new BuAdListenerFullScreenVideo(_adNative);
#else
            this._reward = new BuAdListenerExpressReward(_adNative);
            this._interstitial = new BuAdListenerExpressFullScreenVideo(_adNative);
#endif
            this._banner = new BuAdListenerExpressBanner(_adNative);
            this._feed = new BuAdListenerExpressFeed(_adNative);

            _initState = InitState.Inited;
            Log("Init Finish");

            onInit?.Invoke();
        }

        public bool IsAdReady(AdUnit adUnit)
        {
            if (!IsInited)
            {
                return false;
            }
            switch (adUnit.adType)
            {
                case AdType.Banner:
                    return true;
                case AdType.Interstitial:
                    return _interstitial.IsAdReady();
                case AdType.Reward:
                    return _reward.IsAdReady();
                case AdType.Feed:
                    return _feed.IsAdReady();
                default:
                    return false;
            }
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
                    _banner.Request(adUnit);
                    break;
                case AdType.Interstitial:
                    _interstitial.Request(adUnit);
                    break;
                case AdType.Reward:
                    _reward.Request(adUnit);
                    break;
                case AdType.Feed:
                    _feed.Request(adUnit);
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
                    _interstitial.SetAlwayNotify(adNotify);
                    break;
                case AdType.Reward:
                    _reward.SetAlwayNotify((adNotify as IRewardADNotify));
                    break;
                case AdType.Feed:
                    _feed.SetAlwayNotify(adNotify);
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
                    _interstitial.SetNotify(adNotify);
                    break;
                case AdType.Reward:
                    _reward.SetNotify(adNotify);
                    break;
                case AdType.Feed:
                    _feed.SetNotify(adNotify);
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
                    _banner.ShowAd();
                    break;
                case AdType.Interstitial:
                    _interstitial.ShowAd();
                    break;
                case AdType.Reward:
                    _reward.ShowAd();
                    break;
                case AdType.Feed:
                    _feed.ShowAd();
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
                    _banner.ShowAd();
                    break;
                case AdType.Interstitial:
                    _interstitial.SetNotify(adNotify);
                    _interstitial.ShowAd();
                    break;
                case AdType.Reward:
                    _reward.SetNotify(adNotify);
                    _reward.ShowAd();
                    break;
                case AdType.Feed:
                    _feed.SetNotify(adNotify);
                    _feed.ShowAd();
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
                    _banner.CloseAd();
                    break;
                case AdType.Feed:
                    _feed.CloseAd();
                    break;
                default:
                    break;
            }
        }

        internal static void Log(string info)
        {
            Debug.Log($"[ADBridge][BuAd] {info}");
        }

        void IAdBridge.Log(string info)
        {
            BuAdBridge.Log(info);
        }
    }
}
