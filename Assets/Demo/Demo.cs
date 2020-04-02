using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo : MonoBehaviour
{
    private void Start()
    {
        ADManager.Init();
        ADManager.onRewardADChange += OnRewardChange;
    }

    private void OnDestroy()
    {
        ADManager.onRewardADChange -= OnRewardChange;
    }

    private void OnRewardChange()
    {
        if (ADManager.IsCanShowAD(GameAdID.Reward))
        {
            Debug.Log("视频广告已加载");
        }
        else
        {
            Debug.Log("视频广告未加载");
        }
    }

    public void ShowRwardAD()
    {
        RewardADNotify notify = new RewardADNotify();
        notify.onAdShow += ()=> {
            Debug.Log("视频广告播放");
        };
        notify.onAdClick += () => {
            Debug.Log("视频广告被点击");
        };
        notify.onAdReward += () => {
            Debug.Log("视频广告看完");
        };
        notify.onAdSkip += () => {
            Debug.Log("视频广告中途退出");
        };
        notify.onAdClose += () => {
            Debug.Log("视频广告关闭");
        };
        ADManager.ShowAD(GameAdID.Reward, notify);
    }

    public void ShowFeed()
    {
        ADNotify notify = new ADNotify();
        notify.onAdShow += () => {
            Debug.Log("原生广告播放");
        };
        notify.onAdClick += () => {
            Debug.Log("原生广告被点击");
        };
        notify.onAdClose += () => {
            Debug.Log("原生广告关闭");
        };
        ADManager.ShowAD(GameAdID.Feed, notify);
    }

    public void ShowIntertitial()
    {
        ADNotify notify = new ADNotify();
        notify.onAdShow += () => {
            Debug.Log("插屏广告播放");
        };
        notify.onAdClick += () => {
            Debug.Log("插屏广告被点击");
        };
        notify.onAdClose += () => {
            Debug.Log("插屏广告关闭");
        };
        ADManager.ShowAD(GameAdID.Interstitial, notify);
    }

    public void ShowBanner()
    {
        ADNotify notify = new ADNotify();
        notify.onAdShow += () => {
            Debug.Log("banner广告播放");
        };
        notify.onAdClick += () => {
            Debug.Log("banner广告被点击");
        };
        notify.onAdClose += () => {
            Debug.Log("banner广告关闭");
        };
        ADManager.ShowAD(GameAdID.Banner, notify);
    }
}
