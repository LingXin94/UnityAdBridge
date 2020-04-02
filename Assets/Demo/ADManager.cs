using System;
using ADBridge;

public class ADManager
{
    public static event Action onRewardADChange;

    public static bool isHideAD = false;

    private static IAdBridge adBridge;

    private static bool debug = true;

    public static void Init()
    {

#if UNITY_EDITOR
        return;
#endif
        AdUnit[] adUnits = new AdUnit[] { GameAdID.Reward, GameAdID.Interstitial, GameAdID.Banner };

        // Mopub广告
        var mopubBridge = new ADBridge.Mopub.MoPubBridge();
        mopubBridge.Init(adUnits, debug, RequestAd);
        adBridge = mopubBridge;

        // IronSource广告
        var ironsourceBridge = new ADBridge.Ironsouce.IronSourceBridge();
        ironsourceBridge.Init("app_id", debug, RequestAd);
        adBridge = ironsourceBridge;

        // 穿山甲广告
        var buadBridge = new ADBridge.BuAd.BuAdBridge();
        buadBridge.Init("app_id", debug, RequestAd);
        adBridge = buadBridge;

        // MTG广告
        var mtgBridge = new ADBridge.MTG.MTGBridge();
        mtgBridge.Init("app_id", "app_key", adUnits, debug, RequestAd);
        adBridge = mtgBridge;

        adBridge.SetAlwayNotify(AdType.Reward, GetAlwayRewardAdNotify());
        adBridge.SetAlwayNotify(AdType.Interstitial, GetAlwayInterstitialAdNotify());
        adBridge.SetAlwayNotify(AdType.Banner, GetAlwayBannerAdNotify());
        adBridge.SetAlwayNotify(AdType.Feed, GetAlwayFeedAdNotify());
    }

    private static void RequestAd()
    {
        adBridge.Request(GameAdID.Reward);
        if (!isHideAD)
        {
            adBridge.Request(GameAdID.Interstitial);
            adBridge.Request(GameAdID.Banner);
            adBridge.Request(GameAdID.Feed);
        }
    }

    public static bool IsCanShowAD(AdUnit adUnit)
    {
#if UNITY_EDITOR
        return true;
#endif
        return adBridge.IsAdReady(adUnit);
    }

    public static void CloseAD(AdUnit adUnit)
    {
#if UNITY_EDITOR
        return;
#endif
        adBridge.CloseAd(adUnit);
    }

    public static bool ShowAD(AdUnit adUnit, IAdNotify notify = null)
    {
        if (isHideAD && adUnit.adType != AdType.Reward)
        {
            return false;
        }
        if (!IsCanShowAD(adUnit))
        {
            return false;
        }
#if UNITY_EDITOR 
        if (adUnit.adType == AdType.Reward && notify != null)
        {
            IRewardADNotify rewardNotify = notify as IRewardADNotify;
            rewardNotify.OnAdShow();
            rewardNotify.OnAdReward();
            rewardNotify.OnAdClose();
        }
        return true;
#endif
        adBridge.ShowAd(adUnit, notify);
        return true;
    }

    #region Alway Ad Notify
    private static ADNotify GetAlwayBannerAdNotify()
    {
        ADNotify notify = new ADNotify();
        notify.onAdShow += () => {

        };
        return notify;
    }

    private static ADNotify GetAlwayInterstitialAdNotify()
    {
        ADNotify notify = new ADNotify();
        notify.onAdShow += () => {

        };
        return notify;
    }

    private static ADNotify GetAlwayFeedAdNotify()
    {
        ADNotify notify = new ADNotify();
        notify.onAdShow += () => {

        };
        return notify;
    }

    private static RewardADNotify GetAlwayRewardAdNotify()
    {
        RewardADNotify notify = new RewardADNotify();
        notify.onAdLoad += () => {
            onRewardADChange?.Invoke();
        };
        notify.onAdClose += () => {
            onRewardADChange?.Invoke();
        };
        notify.onAdSkip += () => {
        };

        notify.onAdReward += () => {

        };
        notify.onAdShow += () => {

        };
        return notify;
    }
    #endregion Alway Ad Notify
}