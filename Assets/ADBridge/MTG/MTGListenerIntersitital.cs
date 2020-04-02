using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADBridge.MTG
{
    internal class MTGListenerIntersitital
    {
        private IAdNotify _adTempNotify;
        private IAdNotify _adAlwayNotify;

        public MTGListenerIntersitital()
        {
            MintegralManager.onInterstitialVideoLoadSuccessEvent += onInterstitialVideoLoadSuccessEvent;
            MintegralManager.onInterstitialVideoLoadedEvent += onInterstitialVideoLoadedEvent;
            MintegralManager.onInterstitialVideoFailedEvent += onInterstitialVideoFailedEvent;
            MintegralManager.onInterstitialVideoShownEvent += onInterstitialVideoShownEvent;
            MintegralManager.onInterstitialVideoShownFailedEvent += onInterstitialVideoShownFailedEvent;
            MintegralManager.onInterstitialVideoClickedEvent += onInterstitialVideoClickedEvent;
            MintegralManager.onInterstitialVideoDismissedEvent += onInterstitialVideoDismissedEvent;
            MintegralManager.onInterstitialVideoPlayCompletedEvent += onInterstitialVideoPlayCompletedEvent;
            MintegralManager.onInterstitialVideoEndCardShowSuccessEvent += onInterstitialVideoEndCardShowSuccessEvent;
        }

        public void SetAlwayNotify(IAdNotify adNotify)
        {
            this._adTempNotify = adNotify;
        }

        public void SetNotify(IAdNotify adNotify)
        {
            this._adAlwayNotify = adNotify;
        }

        private void onInterstitialVideoLoadSuccessEvent(string adUnitId)
        {
            MTGBridge.Log("[Intersitital] onInterstitialVideoLoadSuccessEvent");
        }

        private void onInterstitialVideoLoadedEvent(string adUnitId)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoad();
                _adAlwayNotify?.OnAdLoad();
            });
            MTGBridge.Log("[Intersitital] onAdLoaded");
        }

        private void onInterstitialVideoFailedEvent(string errorMsg)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoadFailed();
                _adAlwayNotify?.OnAdLoadFailed();
            });
            Loom.QueueOnMainThread(() => {
                Mintegral.requestInterstitialVideoAd(MTGBridge.interUnit.id);
            }, MTGBridge.FAILED_RETRY_DELAY);
            MTGBridge.Log($"[Intersitital] OnAdLoadFailed, {errorMsg}");
        }

        private void onInterstitialVideoShownEvent(string errorMsg)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdShow();
                _adAlwayNotify?.OnAdShow();
            });
            MTGBridge.Log("[Intersitital] OnAdShow");
        }

        private void onInterstitialVideoShownFailedEvent(string adUnitId)
        {
            MTGBridge.Log($"[Intersitital] onInterstitialVideoShownFailedEvent, {adUnitId}");
        }

        private void onInterstitialVideoClickedEvent(string adUnitId)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
            });
            MTGBridge.Log("[Intersitital] OnAdClick");
        }

        private void onInterstitialVideoDismissedEvent(string errorMsg)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
            });
            Loom.QueueOnMainThread(() => {
                Mintegral.requestInterstitialVideoAd(MTGBridge.interUnit.id);
            }, MTGBridge.FAILED_RETRY_DELAY);
            MTGBridge.Log("[Intersitital] OnAdClose");
        }

        private void onInterstitialVideoPlayCompletedEvent(string adUnitId)
        {
            MTGBridge.Log("[Intersitital] onInterstitialVideoPlayCompletedEvent");
        }

        private void onInterstitialVideoEndCardShowSuccessEvent(string adUnitId)
        {
            MTGBridge.Log("[Intersitital] onInterstitialVideoEndCardShowSuccessEvent");
        }
    }
}