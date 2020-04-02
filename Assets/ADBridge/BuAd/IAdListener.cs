
namespace ADBridge.BuAd
{
    internal interface IAdListener
    {
        void SetNotify(IAdNotify adNotify);

        void SetAlwayNotify(IAdNotify adNotify);

        void Request(AdUnit adUnit);

        bool IsAdReady();

        void ShowAd();

        void CloseAd();
    }
}