using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADBridge.MTG
{
    internal class MTGListenerReward
    {

        private IRewardADNotify _adTempNotify;
        private IRewardADNotify _adAlwayNotify;

        public MTGListenerReward()
        {
            MintegralManager.onRewardedVideoLoadSuccessEvent += onRewardedVideoLoadSuccessEvent;
            MintegralManager.onRewardedVideoLoadedEvent += onRewardedVideoLoadedEvent;
            MintegralManager.onRewardedVideoFailedEvent += onRewardedVideoFailedEvent;
            MintegralManager.onRewardedVideoShownFailedEvent += onRewardedVideoShownFailedEvent;
            MintegralManager.onRewardedVideoShownEvent += onRewardedVideoShownEvent;
            MintegralManager.onRewardedVideoClickedEvent += onRewardedVideoClickedEvent;
            MintegralManager.onRewardedVideoClosedEvent += onRewardedVideoClosedEvent;
            MintegralManager.onRewardedVideoPlayCompletedEvent += onRewardedVideoPlayCompletedEvent;
            MintegralManager.onRewardedVideoEndCardShowSuccessEvent += onRewardedVideoEndCardShowSuccessEvent;
        }


        public void SetAlwayNotify(IRewardADNotify adNotify)
        {
            this._adTempNotify = adNotify;
        }


        public void SetNotify(IRewardADNotify adNotify)
        {
            this._adAlwayNotify = adNotify;
        }

        private void onRewardedVideoLoadSuccessEvent(string adUnitId)
        {
            MTGBridge.Log("[Reward] onRewardedVideoLoadSuccessEvent");
        }

        private void onRewardedVideoLoadedEvent(string adUnitId)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoad();
                _adAlwayNotify?.OnAdLoad();
            });
            MTGBridge.Log("[Reward] onAdLoaded");
        }

        private void onRewardedVideoFailedEvent(string errorMsg)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoadFailed();
                _adAlwayNotify?.OnAdLoadFailed();
            });
            Loom.QueueOnMainThread(() => {
                Mintegral.requestRewardedVideo(MTGBridge.rewardUnit.id);
            }, MTGBridge.FAILED_RETRY_DELAY);
            MTGBridge.Log($"[Reward] OnAdLoadFailed {errorMsg}");
        }

        private void onRewardedVideoShownFailedEvent(string adUnitId)
        {
            MTGBridge.Log($"[Reward] onRewardedVideoShownFailedEvent, {adUnitId}");
        }

        private void onRewardedVideoShownEvent()
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdShow();
                _adAlwayNotify?.OnAdShow();
            });
            MTGBridge.Log("[Reward] OnAdClick");
        }

        private void onRewardedVideoClickedEvent(string errorMsg)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
            });
            MTGBridge.Log("[Reward] OnAdClick");
        }

        private void onRewardedVideoClosedEvent(MintegralManager.MTGRewardData rewardData)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
            });
            Loom.QueueOnMainThread(() => {
                Mintegral.requestRewardedVideo(MTGBridge.rewardUnit.id);
            }, MTGBridge.FAILED_RETRY_DELAY);
            MTGBridge.Log("[Reward] OnAdClose");
        }

        private void onRewardedVideoPlayCompletedEvent(string adUnitId)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdReward();
                _adAlwayNotify?.OnAdReward();
            });
            MTGBridge.Log("[Reward] OnAdReward");
        }

        private void onRewardedVideoEndCardShowSuccessEvent(string adUnitId)
        {
            MTGBridge.Log("[Reward] onRewardedVideoEndCardShowSuccessEvent");
        }
    }
}