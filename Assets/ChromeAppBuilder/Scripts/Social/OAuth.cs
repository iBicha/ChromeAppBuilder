using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


namespace Chrome.Social
{
	public abstract class OAuth <T> where T : class, new()
	{
#region Singleton
		protected static T GetInstance()
		{
			if (_instance == null)
			{
				lock (_lockObj)
				{
					if (_instance == null)
						_instance = new T();
				}
			}
			return _instance;
		}

		public static T Instance
		{
			get { return GetInstance(); }
		}

		private volatile static T _instance;
		private static object _lockObj = new object();
#endregion
		protected string AccessToken { get; set; }

		protected delegate void LaunchFlowCallback (string responsetUrl);

		public string ProviderName { get; private set; }

		public string AuthURL { get; set; }

		public string ClientID { get; set; }
		//Calling this will also print the RedirectURL to the chrome console.
		public virtual string RedirectURL {
			get {
#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				return Chrome.Native.Identity.GetRedirectURL (WWW.EscapeURL (ProviderName.ToLower ()));
#else
				return "";
#endif
			}
		}

		public List<string> scope;

		public virtual string RequestUrl { 
			get {
				Dictionary<string, string> parameters = new Dictionary<string, string> ();
				parameters.Add ("client_id", ClientID);
				parameters.Add ("redirect_uri", RedirectURL);
				if (scope.Count > 0) {
					parameters.Add ("scope", string.Join (" ", scope.ToArray ()));
				}
				parameters.Add ("response_type", "token");
				return UrlEncode (AuthURL, parameters);
			} 
		}

		public OAuth (string name)
		{
			ProviderName = name;
			scope = new List<string> ();
		}

		public void SetScope (params string[] scopes)
		{
			scope.Clear ();
			scope.AddRange (scopes);
		}

		public static string UrlEncode (string baseUrl, Dictionary<string, string>parameters)
		{
			List<string> arrayParams = new List<string> ();
			foreach (string key in parameters.Keys) {
				arrayParams.Add (WWW.EscapeURL (key) + "=" + WWW.EscapeURL (parameters [key]));
			}
			return baseUrl + "?" + string.Join ("&", arrayParams.ToArray ());
		}

		protected virtual string GetUrlParameter (string url, string key)
		{
			url = url.Substring (url.IndexOf ("?") + 1);
			string[] arrayParams = url.Split ('&');
			Dictionary<string, string> parameters = new Dictionary<string, string> ();
			foreach (string paramPair in arrayParams) {
				string[] keyValue = paramPair.Split ('=');
				if (keyValue.Length == 2) {
					parameters.Add (keyValue [0], keyValue [1]);
				}
			}
			if (parameters.ContainsKey (key)) {
				return parameters [key];
			}
			return "";
		}

		protected void LaunchFlow (bool interactive, LaunchFlowCallback callback, string url = null)
		{
#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			int callbackID = Native.CallbackRegisrty.RegisterCallback (
				                 (Native.Identity.launchWebAuthFlowCallback)((responseUrl, callbackId) => {
					if (callback != null) {
						callback (responseUrl);
					}
				}));
			if (string.IsNullOrEmpty (url)) {
				url = RequestUrl;
			}
			Native.Identity.LaunchWebAuthFlow (url, interactive, Native.Identity.onLaunchWebAuthFlowCallback, callbackID);
#else
			Debug.LogException( new System.NotSupportedException("This only works in a Chrome App."));
#endif
		}

		public abstract void Authenticate (bool interactive, Action callback = null);

		public abstract void Deauthorize ();

	}
}

