namespace ADBridge.Mopub
{
    internal class MoPubListenerInterstitial
    {
        private IAdNotify _adTempNotify;
        private IAdNotify _adAlwayNotify;

        public MoPubListenerInterstitial()
        {
            MoPubManager.OnInterstitialLoadedEvent += OnAdLoad;
            MoPubManager.OnInterstitialFailedEvent += OnAdLoadFailed;
            MoPubManager.OnInterstitialClickedEvent += OnAdClick;
            MoPubManager.OnInterstitialShownEvent += OnAdShow;
            MoPubManager.OnInterstitialDismissedEvent += OnAdClose;
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
            MoPub.RequestInterstitialAd(adUnit.id);
            MoPubBridge.Log("Interstitial Request");
        }

        private void OnAdLoad(string id)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoad();
                _adAlwayNotify?.OnAdLoad();
                MoPubBridge.Log("Interstitial OnLoaded");
            });
        }

        private void OnAdLoadFailed(string id, string error)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoadFailed();
                _adAlwayNotify?.OnAdLoadFailed();
                MoPubBridge.Log($"Interstitial OnLoadFailed {error}");
            });
            Loom.QueueOnMainThread(() => MoPub.RequestInterstitialAd(id), MoPubBridge.FAILED_RETRY_DELAY);
        }

        private void OnAdClick(string id)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
                MoPubBridge.Log("Interstitial OnClick");
            });
        }

        private void OnAdShow(string id)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdShow();
                _adAlwayNotify?.OnAdShow();
                MoPubBridge.Log("Interstitial OnShow");
            });
        }

        private void OnAdClose(string id)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
                MoPub.RequestInterstitialAd(id);
                MoPubBridge.Log("Interstitial OnClose");
            });
        }
    }
}
