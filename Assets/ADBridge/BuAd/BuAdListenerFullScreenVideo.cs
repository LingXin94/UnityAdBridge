using ByteDance.Union;

namespace ADBridge.BuAd
{
    internal class BuAdListenerFullScreenVideo : IFullScreenVideoAdListener, IFullScreenVideoAdInteractionListener, IAdListener
    {
        private readonly AdNative _adNative;

        private IAdNotify _adTempNotify;
        private IAdNotify _adAlwayNotify;

        private FullScreenVideoAd _fullScreenVideoAd;
        private AdUnit _adUnit;
        private bool _isShowing;

        public BuAdListenerFullScreenVideo(AdNative adNative)
        {
            this._adNative = adNative;
            //ADBridge.AdBridge.onApplicationPause += OnApplicationPause;
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
#if UNITY_ANDROID
                if (_isShowing)
                {
                    OnAdClose();
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
            this._adUnit = adUnit;
            AdSlot adSlot = new AdSlot.Builder()
                 .SetCodeId(adUnit.id)
                 .SetSupportDeepLink(true)
                 .SetImageAcceptedSize(1080, 1920)
                 .SetUserID("user")
                 .SetOrientation(AdOrientation.Vertical)
                 .Build();
            _adNative.LoadFullScreenVideoAd(adSlot, this);
            BuAdBridge.Log("Interstitial Request");
        }

        public bool IsAdReady()
        {
            return _fullScreenVideoAd != null;
        }

        public void ShowAd()
        {
            _fullScreenVideoAd?.ShowFullScreenVideoAd();
            _isShowing = true;
        }

        public void CloseAd()
        {

        }

        public void OnError(int code, string message)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoadFailed();
                _adAlwayNotify?.OnAdLoadFailed();
                BuAdBridge.Log($"Interstitial OnLoadFailed {code}, {message}");
            });
            Loom.QueueOnMainThread(() => Request(_adUnit), BuAdBridge.FAILED_RETRY_DELAY);
        }

        public void OnFullScreenVideoAdLoad(FullScreenVideoAd ad)
        {
            _fullScreenVideoAd = ad;
            _fullScreenVideoAd.SetFullScreenVideoAdInteractionListener(this);
        }

        public void OnFullScreenVideoCached()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoad();
                _adAlwayNotify?.OnAdLoad();
                BuAdBridge.Log("Interstitial OnLoaded");
            });
        }

        public void OnExpressFullScreenVideoAdLoad(ExpressFullScreenVideoAd ad)
        {
            BuAdBridge.Log("Interstitial OnExpressFullScreenVideoAdLoad");
        }

        public void OnAdShow()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdShow();
                _adAlwayNotify?.OnAdShow();
                BuAdBridge.Log("Interstitial OnShow");
            });
        }

        public void OnAdVideoBarClick()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
                BuAdBridge.Log("Interstitial OnClick");
            });
        }

        public void OnAdClose()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
                BuAdBridge.Log("Interstitial OnClose");
#if UNITY_IOS
                _fullScreenVideoAd.Dispose();
#endif
                _fullScreenVideoAd = null;
                Request(_adUnit);
            });
            _isShowing = false;
        }

        public void OnVideoComplete()
        {
            BuAdBridge.Log("Interstitial OnVideoComplete");
        }

        public void OnSkippedVideo()
        {
            BuAdBridge.Log("Interstitial OnSkippedVideo");
        }

        public void OnVideoError()
        {
            BuAdBridge.Log("Interstitial OnVideoError");
        }
    }
}
