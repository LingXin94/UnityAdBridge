using ByteDance.Union;

namespace ADBridge.BuAd
{
    internal class BuAdListenerReward : IRewardVideoAdListener, IRewardAdInteractionListener, IAdListener
    {
        private readonly AdNative _adNative;

        private IRewardADNotify _adTempNotify;
        private IRewardADNotify _adAlwayNotify;

        private RewardVideoAd _rewardVideoAd;
        private AdUnit _adUnit;
        private bool _isShowing;

        public BuAdListenerReward(AdNative adNative)
        {
            this._adNative = adNative;
            //ADBridge.AdBridge.onApplicationPause += OnApplicationPause;
        }

        private void OnApplicationPause(bool pause)
        {
            if (!pause)
            {
#if UNITY_ANDROID
                Loom.QueueOnMainThread(() => {
                    if (_isShowing)
                    {
                        BuAdBridge.Log("Reward OnApplicationPause CloseAd");
                        OnAdClose();
                    }
                }, 0.1f);
#endif
            }
        }

        public void SetNotify(IAdNotify adNotify)
        {
            this._adTempNotify = adNotify as IRewardADNotify;
        }

        public void SetAlwayNotify(IAdNotify adNotify)
        {
            this._adAlwayNotify = adNotify as IRewardADNotify;
        }

        public void Request(AdUnit adUnit)
        {
            _adUnit = adUnit;
            AdSlot adSlot = new AdSlot.Builder()
                     .SetCodeId(adUnit.id)
                     .SetSupportDeepLink(true)
                     .SetImageAcceptedSize(1080, 1920)
                     .SetUserID("user")
                     .SetOrientation(AdOrientation.Vertical)
                     .Build();
            _adNative.LoadRewardVideoAd(adSlot, this);
            BuAdBridge.Log("Reward Request");
        }

        public bool IsAdReady()
        {
            return _rewardVideoAd != null;
        }

        public void ShowAd()
        {
            _rewardVideoAd?.ShowRewardVideoAd();
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
                BuAdBridge.Log($"Reward OnLoaded Failed: {code}, {message}");
            });

            Loom.QueueOnMainThread(() => Request(_adUnit), BuAdBridge.FAILED_RETRY_DELAY);
        }

        public void OnRewardVideoAdLoad(RewardVideoAd ad)
        {
            _rewardVideoAd = ad;
            _rewardVideoAd.SetRewardAdInteractionListener(this);
        }

        public void OnRewardVideoCached()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoad();
                _adAlwayNotify?.OnAdLoad();
                BuAdBridge.Log("Reward OnLoaded");
            });
        }

        public void OnExpressRewardVideoAdLoad(ExpressRewardVideoAd ad)
        {
            BuAdBridge.Log("Reward OnExpressRewardVideoAdLoad");
        }

        public void OnAdShow()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdShow();
                _adAlwayNotify?.OnAdShow();
                BuAdBridge.Log("Reward OnShow");
            });
        }

        public void OnAdVideoBarClick()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
                BuAdBridge.Log("Reward OnClick");
            });
        }

        public void OnAdClose()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
                BuAdBridge.Log("Reward OnClose");
#if UNITY_IOS
                _rewardVideoAd?.Dispose();
#endif
                _rewardVideoAd = null;
                Request(_adUnit);
            });
            _isShowing = false;
        }

        public void OnVideoComplete()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdReward();
                _adAlwayNotify?.OnAdReward();
                BuAdBridge.Log("Reward OnReward");
            });
        }

        public void OnVideoError()
        {
            Request(_adUnit);
            BuAdBridge.Log("Reward OnVideoError");
        }

        public void OnRewardVerify(bool rewardVerify, int rewardAmount, string rewardName)
        {
            BuAdBridge.Log($"Reward OnRewardVerify: {rewardVerify}, {rewardAmount}, {rewardName}");
        }
    }
}
