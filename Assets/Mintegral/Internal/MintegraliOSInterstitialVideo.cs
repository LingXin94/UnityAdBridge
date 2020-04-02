using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using AOT;


#if UNITY_IPHONE

public class MintegraliOSInterstitialVideo
{
	private System.IntPtr interstitialVideoManager;

	[DllImport ("__Internal")]
	private static extern System.IntPtr initInterstitialVideo (string unitId);

	[DllImport ("__Internal")]
	private static extern void preloadInterstitialVideo (System.IntPtr instance);

	[DllImport ("__Internal")]
	private static extern void showInterstitialVideo (System.IntPtr instance);



	public void showInterstitialVideoAd()
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interstitialVideoManager == System.IntPtr.Zero) return;

				showInterstitialVideo (interstitialVideoManager);
			}
		}
	}

	public void requestInterstitialVideoAd()
	{
		if (Application.platform != RuntimePlatform.OSXEditor) {

			if (Application.platform == RuntimePlatform.IPhonePlayer){
				if (interstitialVideoManager == System.IntPtr.Zero)  return;

				preloadInterstitialVideo (interstitialVideoManager);
			}
		}
	}

	public  MintegraliOSInterstitialVideo(MTGInterstitialVideoInfo info)
	{
		if (Application.platform != RuntimePlatform.OSXEditor) { 
			if (Application.platform == RuntimePlatform.IPhonePlayer){
				interstitialVideoManager = initInterstitialVideo (info.adUnitId);
			}
		}
	}

}
#endif