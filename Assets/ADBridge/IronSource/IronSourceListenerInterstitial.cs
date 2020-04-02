namespace ADBridge.Ironsouce {
    internal class IronSourceListenerInterstitial {

        private IAdNotify _alwayNotify;
        private IAdNotify _tempNotify;

        public IronSourceListenerInterstitial() {
            IronSourceEvents.onInterstitialAdReadyEvent += OnAdLoad;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += OnAdLoadFailed;
            IronSourceEvents.onInterstitialAdOpenedEvent += OnAdShow;
            IronSourceEvents.onInterstitialAdClickedEvent += OnAdClick;
            IronSourceEvents.onInterstitialAdClosedEvent += OnAdClose;
            IronSourceEvents.onInterstitialAdRewardedEvent += OnAdReward;
        }

        public void SetNotify(IAdNotify adNotify) {
            this._tempNotify = adNotify;
        }

        public void SetAlwayNotify(IAdNotify adNotify) {
            this._alwayNotify = adNotify;
        }

        private void OnAdLoad() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdLoad();
                _alwayNotify?.OnAdLoad();
                IronSourceBridge.Log("Interstitial OnLoaded");
            });
        }

        private void OnAdLoadFailed(IronSourceError error) {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdLoadFailed();
                _alwayNotify?.OnAdLoadFailed();
                IronSourceBridge.Log($"Interstitial OnLoadFailed {error}");
                IronSource.Agent.loadInterstitial();
            });
            
        }

        private void OnAdClick() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdClick();
                _alwayNotify?.OnAdClick();
                IronSourceBridge.Log("Interstitial OnClick");
            });
        }

        private void OnAdShow() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdShow();
                _alwayNotify?.OnAdShow();
                IronSourceBridge.Log("Interstitial OnShow");
            });
        }

        private void OnAdClose() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdClose();
                _alwayNotify?.OnAdClose();
                IronSourceBridge.Log("Interstitial OnClose");
                IronSource.Agent.loadInterstitial();
            });
        }
        private void OnAdReward() {
            Loom.QueueOnMainThread(() => {
                IronSourceBridge.Log("Interstitial OnAdReward");
            });
        }
    }
}
