#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using System;
using System.Collections.Generic;

namespace Chrome.Native
{

	[ChromeMinimumVersion (29)]
	public class Identity
	{

		const string ChromeAppIdentityPrefix = "ChromeApp_Identity_";


		[RuntimeInitializeOnLoadMethod]
		public static void Initialize ()
		{
			OnSignInChangedAddListener (onSignInChanged, 0);
		}


		[DllImport ("__Internal", EntryPoint = ChromeAppIdentityPrefix + "getAuthToken")]
		public static extern void GetAuthToken (bool interactive, string scope, getAuthTokenCallback callback, int callbackId);

		[ChromeMinimumVersion (37)]
		[DllImport ("__Internal", EntryPoint = ChromeAppIdentityPrefix + "getProfileUserInfo")]
		public static extern void GetProfileUserInfo (getProfileUserInfoCallback callback, int callbackId);

		[DllImport ("__Internal", EntryPoint = ChromeAppIdentityPrefix + "removeCachedAuthToken")]
		public static extern string RemoveCachedAuthToken (string token);

		[DllImport ("__Internal", EntryPoint = ChromeAppIdentityPrefix + "launchWebAuthFlow")]
		public static extern string LaunchWebAuthFlow (string url, bool interactive, launchWebAuthFlowCallback callback, int callbackId);

		[ChromeMinimumVersion (33)]
		[DllImport ("__Internal", EntryPoint = ChromeAppIdentityPrefix + "getRedirectURL")]
		public static extern string GetRedirectURL (string path);

		[ChromeMinimumVersion (33)]
		[DllImport ("__Internal", EntryPoint = ChromeAppIdentityPrefix + "onSignInChangedAddListener")]
		public static extern void OnSignInChangedAddListener (getProfileUserInfoCallback callback, int callbackId);

		public delegate void getAuthTokenCallback (string token, int callbackId);

		public delegate void getProfileUserInfoCallback (string userInfo, int callbackId);

		public delegate void launchWebAuthFlowCallback (string responseUrl, int callbackId);

		[MonoPInvokeCallback (typeof(getAuthTokenCallback))]
		public static void onGetAuthToken (string token, int callbackId)
		{
			getAuthTokenCallback callback = CallbackRegisrty.GetCallback<getAuthTokenCallback> (callbackId);
			if (callback != null) {
				callback (token, callbackId);
			}
		}

		[MonoPInvokeCallback (typeof(getProfileUserInfoCallback))]
		public static void onGetProfileUserInfo (string userInfo, int callbackId)
		{
			getProfileUserInfoCallback callback = CallbackRegisrty.GetCallback<getProfileUserInfoCallback> (callbackId);
			if (callback != null) {
				callback (userInfo, callbackId);
			}
		}

		[MonoPInvokeCallback (typeof(launchWebAuthFlowCallback))]
		public static void onLaunchWebAuthFlowCallback (string responseUrl, int callbackId)
		{
			launchWebAuthFlowCallback callback = CallbackRegisrty.GetCallback<launchWebAuthFlowCallback> (callbackId);
			if (callback != null) {
				callback (responseUrl, callbackId);
			}
		}

		[MonoPInvokeCallback (typeof(getProfileUserInfoCallback))]
		public static void onSignInChanged (string userInfo, int callbackId)
		{
			Chrome.App.Identity.OnSignInChanged (userInfo);
		}
	}

}
#endif