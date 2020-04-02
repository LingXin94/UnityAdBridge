
using UnityEngine;
using System.Collections.Generic;


#if UNITY_ANDROID

public class MintegralAndroidInterstitialVideo
{
	private readonly AndroidJavaObject _interstitialVideoPlugin;

	public MintegralAndroidInterstitialVideo (MTGInterstitialVideoInfo info)
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_interstitialVideoPlugin = new AndroidJavaObject ("com.mintegral.msdk.unity.MTGInterstitialVideo", info.adUnitId);
	}


	// Starts loading an interstitial ad
	public void requestInterstitialVideoAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_interstitialVideoPlugin.Call ("preloadInterstitialVideo");
	}


	// If an interstitial ad is loaded this will take over the screen and show the ad
	public void showInterstitialVideoAd ()
	{
		if (Application.platform != RuntimePlatform.Android)
		return;

		_interstitialVideoPlugin.Call ("showInterstitialVideo");

	}
}
#endif
