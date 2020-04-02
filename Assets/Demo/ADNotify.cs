using System;
using ADBridge;


public class ADNotify : IAdNotify
{

    public event Action onAdClick;
    public event Action onAdClose;
    public event Action onAdLoad;
    public event Action onAdLoadFailed;
    public event Action onAdShow;

    public void OnAdClick()
    {
        onAdClick?.Invoke();
    }

    public virtual void OnAdClose()
    {
        onAdClose?.Invoke();
    }

    public void OnAdLoad()
    {
        onAdLoad?.Invoke();
    }

    public void OnAdLoadFailed()
    {
        onAdLoadFailed?.Invoke();
    }

    public void OnAdShow()
    {
        onAdShow?.Invoke();
    }
}

public class RewardADNotify : ADNotify, IRewardADNotify
{

    public event Action onAdReward;
    public event Action onAdSkip;

    private bool _isRewarded = false;

    public void OnAdReward()
    {
        _isRewarded = true;
    }

    public override void OnAdClose()
    {
        if (_isRewarded)
        {
            onAdReward?.Invoke();
        }
        else
        {
            onAdSkip?.Invoke();
        }
        _isRewarded = false;
        base.OnAdClose();
    }
}

