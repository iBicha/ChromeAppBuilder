using System;
using UnityEngine;
using Newtonsoft.Json;

namespace Chrome.App
{
	/// <summary>
	/// Access the Chrome app identity and user information functionalities.
	/// </summary>
	public class Identity
	{
		public class UserInfo
		{
			public string email;
			public string id;
			public bool signedIn;
		}
		/// <summary>
		/// Gets the profile user info.
		/// </summary>
		/// <param name="callback">Callback action called when the user information has been retrived. it will indicate the user's email and id. signedIn is true if the user is signed in to the browser, false otherwise.</param>
		public static void GetProfileUserInfo (Action<UserInfo> callback)
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			int callbackID = Native.CallbackRegisrty.RegisterCallback (
				                 (Native.Identity.getProfileUserInfoCallback)((userInfoString, callbackId) => {
					UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo> (userInfoString);
					if (!string.IsNullOrEmpty (userInfo.email) || !string.IsNullOrEmpty (userInfo.id)) {
						userInfo.signedIn = true;
					}
					if (callback != null) {
						callback (userInfo);
					}
				}));
			Native.Identity.GetProfileUserInfo (Native.Identity.onGetProfileUserInfo, callbackID);
			#else
			Debug.LogException( new System.NotSupportedException("This only works in a Chrome App."));
			#endif
		}
		#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
		public static void OnSignInChanged (string userInfoString)
		{
			UserInfo userInfo = JsonConvert.DeserializeObject<UserInfo> (userInfoString);
			Action<UserInfo> handler = onSignInChanged;
			if (handler != null) {
				handler (userInfo);
			}
		}
		#endif

		/// <summary>
		/// Occurs when the user signs in or out of the browser.
		/// </summary>
		public static event Action<UserInfo> onSignInChanged;

	}

}