using ADBridge;

public static class GameAdID
{
    public static readonly AdUnit Reward = new AdUnit(AdType.Reward, "");
    public static readonly AdUnit Interstitial = new AdUnit(AdType.Interstitial, "");
    public static readonly AdUnit Banner = new AdUnit(AdType.Banner, "");
    public static readonly AdUnit Feed = new AdUnit(AdType.Feed, "");
}
