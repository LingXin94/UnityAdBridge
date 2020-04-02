namespace ADBridge
{

    public interface IAdBridge
    {

        bool IsInited { get; }

        void Request(AdUnit adUnit);

        void ShowAd(AdUnit adUnit);

        void ShowAd(AdUnit adUnit, IAdNotify adNotify);

        void CloseAd(AdUnit adUnit);

        void SetNotify(AdType adType, IAdNotify adNotify);

        void SetAlwayNotify(AdType adType, IAdNotify adNotify);

        bool IsAdReady(AdUnit adUnit);

        void Log(string info);
    }
}