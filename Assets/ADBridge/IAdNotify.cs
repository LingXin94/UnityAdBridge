
namespace ADBridge
{
    public interface IAdNotify
    {
        void OnAdLoad();

        void OnAdLoadFailed();

        void OnAdShow();

        void OnAdClick();

        void OnAdClose();
    }

    public interface IRewardADNotify : IAdNotify
    {
        void OnAdReward();
    }
}