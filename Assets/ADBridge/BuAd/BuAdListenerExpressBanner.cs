using System.Collections.Generic;
using ByteDance.Union;

namespace ADBridge.BuAd
{
    internal class BuAdListenerExpressBanner : IExpressAdListener, IExpressAdInteractionListener, IDislikeInteractionListener, IAdListener
    {
        private readonly AdNative _adNative;

        private IAdNotify _adTempNotify;
        private IAdNotify _adAlwayNotify;
#if UNITY_IOS
        private ExpressBannerAd iExpressBannerAd; // for iOS
#elif UNITY_ANDROID
        private ExpressAd mExpressBannerAd;
#endif
        private AdUnit _adUnit;

        private bool _isAutoShowOnLoaded = false;

        private enum State
        {
            Closed,
            Loading,
            Loaded,
            Showing
        }
        private State _state;

        public BuAdListenerExpressBanner(AdNative adNative)
        {
            this._adNative = adNative;
            //ADBridge.AdBridge.onApplicationPause += OnApplicationPause;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
#if UNITY_ANDROID
                if (_state == State.Showing)
                {
                    OnAdClose(null);
                }
#endif
            }
        }

        public void SetNotify(IAdNotify adNotify)
        {
            this._adTempNotify = adNotify;
        }

        public void SetAlwayNotify(IAdNotify adNotify)
        {
            this._adAlwayNotify = adNotify;
        }

        public void Request(AdUnit adUnit)
        {
            
            if (_state != State.Closed)
            {
                return;
            }
            _state = State.Loading;

            _adUnit = adUnit;
            var adSlot = new AdSlot.Builder()

                     .SetCodeId(adUnit.id)
                     ////期望模板广告view的size,单位dp，//高度设置为0,则高度会自适应
                     .SetExpressViewAcceptedSize(600, 0)
                     .SetSupportDeepLink(true)
                     .SetImageAcceptedSize(1080, 1920)
                     .SetAdCount(1)
                     .SetOrientation(AdOrientation.Horizontal)
                     .Build();
            this._adNative.LoadExpressBannerAd(adSlot, this);
            BuAdBridge.Log("ExpressBanner Request");
        }

        public bool IsAdReady()
        {
#if UNITY_IOS
            return iExpressBannerAd != null;
#elif UNITY_ANDROID
            return mExpressBannerAd != null;
#endif
        }

        public void ShowAd()
        {
            BuAdBridge.Log($"ExpressBanner Call ShowAd");
            if (!IsAdReady())
            {
                _isAutoShowOnLoaded = true;
                Request(_adUnit);
                return;
            }
            if (_state == State.Showing)
            {
                return;
            }

            BuAdBridge.Log($"ExpressBanner Call ShowAd2");
#if UNITY_IOS
            this.iExpressBannerAd.ShowExpressAd(5, 100);
#elif UNITY_ANDROID
            this.mExpressBannerAd.SetSlideIntervalTime(30 * 1000);
            NativeAdManager.Instance().ShowExpressBannerAd(BuAdBridge.mainActivity, mExpressBannerAd.handle, this, this);
#endif
            _state = State.Showing;
        }

        public void CloseAd()
        {
            BuAdBridge.Log($"ExpressBanner Call CloseAd");
            _isAutoShowOnLoaded = false;
            if (_state == State.Loading || _state == State.Loaded)
            {
                return;
            }
            BuAdBridge.Log($"ExpressBanner Call CloseAd2");
#if UNITY_IOS
            if (this.iExpressBannerAd != null)
            {
                this.iExpressBannerAd.Dispose();
                this.iExpressBannerAd = null;
            }
#elif UNITY_ANDROID
            if (this.mExpressBannerAd != null)
            {
                NativeAdManager.Instance().DestoryExpressAd(mExpressBannerAd.handle);
                mExpressBannerAd = null;
            }
#endif
            _state = State.Closed;
            Request(_adUnit);
        }

        public void OnError(int code, string message)
        {
            if (_state == State.Loading)
            {
                _state = State.Closed;
            }
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoadFailed();
                _adAlwayNotify?.OnAdLoadFailed();
                BuAdBridge.Log($"ExpressBanner OnLoadFailed {code}, {message}");
            });
            Loom.QueueOnMainThread(() => Request(_adUnit), BuAdBridge.FAILED_RETRY_DELAY);
        }

        public void OnExpressAdLoad(List<ExpressAd> ads)
        {
#if UNITY_IOS

#elif UNITY_ANDROID
            _state = State.Loaded;
            IEnumerator<ExpressAd> enumerator = ads.GetEnumerator();
            if (enumerator.MoveNext())
            {
                mExpressBannerAd = enumerator.Current;
                mExpressBannerAd.SetExpressInteractionListener(this);
                Loom.QueueOnMainThread(() => {
                    _adTempNotify?.OnAdLoad();
                    _adAlwayNotify?.OnAdLoad();
                    BuAdBridge.Log($"ExpressBanner OnAdLoad");
                    if (_isAutoShowOnLoaded)
                    {
                        _isAutoShowOnLoaded = false;
                        ShowAd();
                    }
                });
            }
#endif
        }

#if UNITY_IOS
        public void OnExpressBannerAdLoad(ExpressBannerAd ad)
        {
            _state = State.Loaded;
            iExpressBannerAd = ad;
            iExpressBannerAd.SetDislikeCallback(this);
            iExpressBannerAd.SetExpressInteractionListener(this);
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoad();
                _adAlwayNotify?.OnAdLoad();
                BuAdBridge.Log($"ExpressBanner OnAdLoad");
                if (_isAutoShowOnLoaded)
                {
                    _isAutoShowOnLoaded = false;
                    ShowAd();
                }
            });
           
        }

        public void OnExpressInterstitialAdLoad(ExpressInterstitialAd ad)
        {
            
        }
#endif

        public void OnAdViewRenderSucc(ExpressAd ad, float width, float height)
        {
        }

        public void OnAdViewRenderError(ExpressAd ad, int code, string message)
        {
        }

        public void OnAdShow(ExpressAd ad)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdShow();
                _adAlwayNotify?.OnAdShow();
                BuAdBridge.Log($"ExpressBanner OnAdShow");
            });
        }

        public void OnAdClicked(ExpressAd ad)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
                BuAdBridge.Log($"ExpressBanner OnAdClick");
            });
        }

        public void OnAdClose(ExpressAd ad)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
                BuAdBridge.Log($"ExpressBanner OnAdClose");

                Request(_adUnit);
            });
        }

        public void OnSelected(int var1, string var2)
        {

        }

        public void OnCancel()
        {

        }
    }
}
