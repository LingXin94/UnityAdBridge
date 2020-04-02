namespace ADBridge.Ironsouce {
    internal class IronSourceListenerBanner {

        private IAdNotify _alwayNotify;
        private IAdNotify _tempNotify;

        public IronSourceListenerBanner() {

            IronSourceEvents.onBannerAdLoadedEvent += OnAdLoad;
            IronSourceEvents.onBannerAdLoadFailedEvent += OnAdLoadFailed;
            IronSourceEvents.onBannerAdScreenPresentedEvent += OnAdShow;
            IronSourceEvents.onBannerAdScreenDismissedEvent += OnAdClose;
            IronSourceEvents.onBannerAdClickedEvent += OnAdClick;
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
                IronSourceBridge.Log("Banner OnLoaded");
            });
        }

        private void OnAdLoadFailed(IronSourceError error) {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdLoadFailed();
                _alwayNotify?.OnAdLoadFailed();
                IronSourceBridge.Log($"Banner OnLoaded Failed {error}");
            });
        }

        private void OnAdClick() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdClick();
                _alwayNotify?.OnAdClick();
                IronSourceBridge.Log("Banner OnClick");
            });
        }

        private void OnAdShow() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdShow();
                _alwayNotify?.OnAdShow();
                IronSourceBridge.Log("Banner OnShow");
            });
        }

        private void OnAdClose() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdClose();
                _alwayNotify?.OnAdClose();
                IronSourceBridge.Log("Banner OnClose");
            });
        }
    }
}
