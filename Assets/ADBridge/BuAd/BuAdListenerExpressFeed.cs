using System.Collections.Generic;
using ByteDance.Union;
using UnityEngine;

namespace ADBridge.BuAd
{
    internal class BuAdListenerExpressFeed : IExpressAdListener, IExpressAdInteractionListener, IDislikeInteractionListener, IAdListener
    {
        private readonly AdNative _adNative;

        private IAdNotify _adTempNotify;
        private IAdNotify _adAlwayNotify;

        private ExpressAd mExpressFeedad;

        private AdUnit _adUnit;

        private int _width = 0;
        private int _height = 0;

        private bool _isShowing = false;


        public BuAdListenerExpressFeed(AdNative adNative)
        {
            this._adNative = adNative;

            //BuAdBridge.Log($"ExpressFeed dpi:{Screen.dpi}, width:{Screen.width}");
            float pxWidth = Screen.width * 0.9f;
            float dpWidth = pxWidth * 160 / Screen.dpi;
            _width = (int)dpWidth;
            //_height = (int)(dpWidth / 1.78f);
            //ADBridge.AdBridge.onApplicationPause += OnApplicationPause;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
#if UNITY_ANDROID
                if (_isShowing)
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
            _adUnit = adUnit;
           
            var adSlot = new AdSlot.Builder()

                     .SetCodeId(adUnit.id)
                     ////期望模板广告view的size,单位dp，//高度设置为0,则高度会自适应
                     .SetExpressViewAcceptedSize(_width, _height)
                     .SetSupportDeepLink(true)
                     .SetImageAcceptedSize(1080, 1920)
                     .SetAdCount(1)
                     .SetOrientation(AdOrientation.Vertical)
                     .Build();
            this._adNative.LoadNativeExpressAd(adSlot, this);
            BuAdBridge.Log("ExpressFeed Request");
        }

        public bool IsAdReady()
        {
            return mExpressFeedad != null;
        }

        public void ShowAd()
        {
            if (!IsAdReady())
            {
                return;
            }
#if UNITY_IOS
            this.mExpressFeedad.SetExpressInteractionListener(this);
            this.mExpressFeedad.ShowExpressAd(5, 100);
#elif UNITY_ANDROID
            this.mExpressFeedad.SetExpressInteractionListener(this);
            NativeAdManager.Instance().ShowExpressFeedAd(BuAdBridge.mainActivity, mExpressFeedad.handle, this, this);
#endif
            _isShowing = true;
        }

        public void CloseAd()
        {
#if UNITY_IOS
            if (this.mExpressFeedad != null)
            {
                 mExpressFeedad.Dispose();
                 mExpressFeedad = null;
            }
#elif UNITY_ANDROID
            if (this.mExpressFeedad != null)
            {
                NativeAdManager.Instance().DestoryExpressAd(mExpressFeedad.handle);
                mExpressFeedad = null;
            }
#endif
            Request(_adUnit);
        }

        public void OnError(int code, string message)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoadFailed();
                _adAlwayNotify?.OnAdLoadFailed();
                BuAdBridge.Log($"ExpressFeed OnLoadFailed {code}, {message}");
            });
            Loom.QueueOnMainThread(() => Request(_adUnit), BuAdBridge.FAILED_RETRY_DELAY);
        }

        public void OnExpressAdLoad(List<ExpressAd> ads)
        {
            IEnumerator<ExpressAd> enumerator = ads.GetEnumerator();
            if (enumerator.MoveNext())
            {
                mExpressFeedad = enumerator.Current;
                mExpressFeedad.SetExpressInteractionListener(this);
#if UNITY_IOS
                mExpressFeedad.SetDislikeCallback(this);
#endif
                Loom.QueueOnMainThread(() => {
                    _adTempNotify?.OnAdLoad();
                    _adAlwayNotify?.OnAdLoad();
                    BuAdBridge.Log($"ExpressFeed OnAdLoad");
                });
            }
        }

#if UNITY_IOS
        public void OnExpressBannerAdLoad(ExpressBannerAd ad)
        {

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
                BuAdBridge.Log($"ExpressFeed OnAdShow");
            });
        }

        public void OnAdClicked(ExpressAd ad)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
                BuAdBridge.Log($"ExpressFeed OnAdClick");
            });
        }

        public void OnAdClose(ExpressAd ad)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
                BuAdBridge.Log($"ExpressFeed OnAdClose");
                Request(_adUnit);
            });
            _isShowing = false;
        }

        public void OnSelected(int var1, string var2)
        {

        }

        public void OnCancel()
        {

        }
    }
}
