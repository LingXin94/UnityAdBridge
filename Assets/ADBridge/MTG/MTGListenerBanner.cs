using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ADBridge.MTG
{
    internal class MTGListenerBanner
    {
        private IAdNotify _adTempNotify;
        private IAdNotify _adAlwayNotify;

        public MTGListenerBanner()
        {
            MintegralManager.onBannerLoadedEvent += onBannerLoadedEvent;
            MintegralManager.onBannerFailedEvent += onBannerFailedEvent;
            MintegralManager.onBannerDidClickEvent += onBannerDidClickEvent;
            MintegralManager.onBannerLoggingImpressionEvent += onBannerLoggingImpressionEvent;
            MintegralManager.onBannerLeaveAppEvent += onBannerLeaveAppEvent;
            MintegralManager.onBannerShowFullScreenEvent += onBannerShowFullScreenEvent;
            MintegralManager.onBannerCloseFullScreenEvent += onBannerCloseFullScreenEvent;
        }


        public void SetAlwayNotify(IAdNotify adNotify)
        {
            this._adTempNotify = adNotify;
        }


        public void SetNotify(IAdNotify adNotify)
        {
            this._adAlwayNotify = adNotify;
        }

        private void onBannerLoadedEvent(string info)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoad();
                _adAlwayNotify?.OnAdLoad();
            });
            MTGBridge.Log("[Banner] onAdLoaded");
        }

        private void onBannerFailedEvent(string info)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdLoadFailed();
                _adAlwayNotify?.OnAdLoadFailed();
            });
            Loom.QueueOnMainThread(() => {
                //请求广告
            }, MTGBridge.FAILED_RETRY_DELAY);
            MTGBridge.Log($"[Banner] OnAdLoadFailed , {info}");
        }

        private void onBannerDidClickEvent(string info)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClick();
                _adAlwayNotify?.OnAdClick();
            });
            MTGBridge.Log("[Banner] OnAdClick");
        }

        private void onBannerLoggingImpressionEvent(string info)
        {
            MTGBridge.Log("[Banner] onBannerLoggingImpressionEvent");
        }

        private void onBannerLeaveAppEvent(string info)
        {
            MTGBridge.Log("[Banner] onBannerLeaveAppEvent");
        }

        private void onBannerShowFullScreenEvent(string info)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdShow();
                _adAlwayNotify?.OnAdShow();
            });
            MTGBridge.Log("[Banner] OnAdShow");
        }

        private void onBannerCloseFullScreenEvent(string info)
        {
            Loom.QueueOnMainThread(() => {
                _adTempNotify?.OnAdClose();
                _adAlwayNotify?.OnAdClose();
            });
            Loom.QueueOnMainThread(() => {
                //请求广告
            }, MTGBridge.FAILED_RETRY_DELAY);
            MTGBridge.Log("[Banner] OnAdClose");
        }
    }
}