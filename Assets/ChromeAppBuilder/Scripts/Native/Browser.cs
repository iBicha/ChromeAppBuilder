#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using System;
using System.Collections.Generic;


namespace Chrome.Native
{

	[ChromeMinimumVersion (42)]
	public class Browser
	{

		const string ChromeAppBrowserPrefix = "ChromeApp_Browser_";

		[DllImport ("__Internal", EntryPoint = ChromeAppBrowserPrefix + "openTab")]
		public static extern void OpenTab (string url);
	}

}
#endif