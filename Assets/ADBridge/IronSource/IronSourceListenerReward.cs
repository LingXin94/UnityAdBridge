namespace ADBridge.Ironsouce {

    internal class IronSourceListenerReward {

        private IRewardADNotify _alwayNotify;
        private IRewardADNotify _tempNotify;

        public IronSourceListenerReward() {

            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += OnAdLoad;
            IronSourceEvents.onRewardedVideoAdOpenedEvent += OnAdShow;
            IronSourceEvents.onRewardedVideoAdClickedEvent += OnAdClick;
            IronSourceEvents.onRewardedVideoAdClosedEvent += OnAdClose;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += OnAdReward;
        }

        public void SetNotify(IRewardADNotify adNotify) {
            this._tempNotify = adNotify;
        }

        public void SetAlwayNotify(IRewardADNotify adNotify) {
            this._alwayNotify = adNotify;
        }

        private void OnAdLoad(bool isloaded) {
            Loom.QueueOnMainThread(() => {
                if (isloaded) {
                    _tempNotify?.OnAdLoad();
                    _alwayNotify?.OnAdLoad();
                    IronSourceBridge.Log("Reward OnLoaded");
                } else {
                    _tempNotify?.OnAdLoadFailed();
                    _alwayNotify?.OnAdLoadFailed();
                    IronSourceBridge.Log("Reward OnLoaded Failed");
                }
            });
        }

        private void OnAdShow() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdShow();
                _alwayNotify?.OnAdShow();
                IronSourceBridge.Log("Reward OnShow");
            });
        }

        private void OnAdClick(IronSourcePlacement p) {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdClick();
                _alwayNotify?.OnAdClick();
                IronSourceBridge.Log("Reward OnClick");
            });
        }

        private void OnAdClose() {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdClose();
                _alwayNotify?.OnAdClose();

                IronSourceBridge.Log("Reward OnAdClose");
            });
        }

        private void OnAdReward(IronSourcePlacement p) {
            Loom.QueueOnMainThread(() => {
                _tempNotify?.OnAdReward();
                _alwayNotify?.OnAdReward();
                IronSourceBridge.Log("Reward OnAdReward");
            });
        }
    }
}
