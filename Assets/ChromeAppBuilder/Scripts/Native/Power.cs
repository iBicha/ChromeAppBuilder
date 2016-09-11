#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using System;
using System.Collections.Generic;

namespace Chrome.Native
{

	[ChromeMinimumVersion (27)]
	public class Power
	{
		const string ChromeAppPowerPrefix = "ChromeApp_Power_";

		[DllImport ("__Internal", EntryPoint = ChromeAppPowerPrefix + "requestKeepAwake")]
		public static extern void RequestKeepAwake (string level);

		[DllImport ("__Internal", EntryPoint = ChromeAppPowerPrefix + "releaseKeepAwake")]
		public static extern void ReleaseKeepAwake ();

	}

}
#endif