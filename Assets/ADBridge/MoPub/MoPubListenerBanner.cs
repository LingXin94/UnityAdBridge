namespace ADBridge.Mopub
{
    internal class MoPubListenerBanner
    {

        private IAdNotify _adAlwayNotify;
        private IAdNotify _adTempNotify;

        public MoPubListenerBanner()
        {
            MoPubManager.OnAdLoadedEvent += OnAdLoad;
            MoPubManager.OnAdFailedEvent += OnAdLoadFailed;
            MoPubManager.OnAdClickedEvent += OnAdClick;
            MoPubManager.OnAdExpandedEvent += OnAdShow;
            MoPubManager.OnAdCollapsedEvent += OnAdClose;
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
            MoPub.RequestBanner(adUnit.id, MoPub.AdPosition.BottomCenter);
            MoPubBridge.Log("Banner Request");
        }

        private void OnAdLoad(string id, float height)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoad();
                _adAlwayNotify?.OnAdLoad();
                MoPubBridge.Log("Banner OnLoaded");
            });
        }

        private void OnAdLoadFailed(string id, string error)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoadFailed();
                _adAlwayNotify?.OnAdLoadFailed();
                MoPubBridge.Log($"Banner OnLoaded Failed {error}");
            });
            Loom.QueueOnMainThread(() => MoPub.RequestBanner(id, MoPub.AdPosition.BottomCenter), MoPubBridge.FAILED_RETRY_DELAY);
        }

        private void OnAdClick(string id)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
                MoPubBridge.Log("Banner OnClick");
            });
        }

        private void OnAdShow(string id)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdShow();
                _adAlwayNotify?.OnAdShow();
                MoPubBridge.Log("Banner OnShow");
            });
        }

        private void OnAdClose(string id)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
                MoPub.RequestBanner(id, MoPub.AdPosition.BottomCenter);
                MoPubBridge.Log("Banner OnClose");
            });
        }
    }
}
