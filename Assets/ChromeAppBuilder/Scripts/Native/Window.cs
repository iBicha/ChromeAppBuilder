#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using System;
using System.Collections.Generic;

namespace Chrome.Native
{
	[ChromeMinimumVersion (23)]
	public class Window
	{
		const string ChromeAppWindowPrefix = "ChromeApp_Window_";

		[RuntimeInitializeOnLoadMethod]
		public static void Initialize ()
		{
			AddListener ("onBoundsChanged", Window.onBoundsChanged);
			AddListener ("onClosed", Window.onClosed);
			AddListener ("onFullscreened", Window.onFullscreened);
			AddListener ("onMaximized", Window.onMaximized);
			AddListener ("onMinimized", Window.onMinimized);
			AddListener ("onRestored", Window.onRestored);
		}

		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "addListener")]
		public static extern void AddListener (string eventName, Action callback);

		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "focus")]
		public static extern void Focus ();

		[ChromeMinimumVersion (28)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "fullscreen")]
		public static extern void FullScreen ();

		[ChromeMinimumVersion (27)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "isFullscreen")]
		public static extern bool IsFullscreen ();

		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "minimize")]
		public static extern void Minimize ();

		[ChromeMinimumVersion (25)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "isMinimized")]
		public static extern bool IsMinimized ();

		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "maximize")]
		public static extern void Maximize ();

		[ChromeMinimumVersion (25)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "isMaximized")]
		public static extern bool IsMaximized ();

		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "restore")]
		public static extern void Restore ();

		[ChromeMinimumVersion (24)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "drawAttention")]
		public static extern void DrawAttention ();

		[ChromeMinimumVersion (24)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "clearAttention")]
		public static extern void ClearAttention ();

		[ChromeMinimumVersion (24)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "close")]
		public static extern void Close ();

		[ChromeMinimumVersion (24)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "show")]
		public static extern void Show (bool focused);

		[ChromeMinimumVersion (24)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "hide")]
		public static extern void Hide ();

		[ChromeMinimumVersion (32)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "isAlwaysOnTop")]
		public static extern bool IsAlwaysOnTop ();

		[ChromeMinimumVersion (32)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "setAlwaysOnTop")]
		public static extern void SetAlwaysOnTop (bool alwaysOnTop);

		[ChromeMinimumVersion (39)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "setVisibleOnAllWorkspaces")]
		public static extern void SetVisibleOnAllWorkspaces (bool alwaysVisible);

		[ChromeMinimumVersion (33)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "id")]
		public static extern string ID ();

		[ChromeMinimumVersion (42)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "canSetVisibleOnAllWorkspaces")]
		public static extern bool CanSetVisibleOnAllWorkspaces ();

		[ChromeMinimumVersion (35)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "getBounds")]
		public static extern int GetBounds (bool inner, string key);

		[ChromeMinimumVersion (35)]
		[DllImport ("__Internal", EntryPoint = ChromeAppWindowPrefix + "setBounds")]
		public static extern void SetBounds (bool inner, string key, int value);

		[ChromeMinimumVersion (26)]
		[MonoPInvokeCallback (typeof(Action))]
		public static void onBoundsChanged ()
		{
			Chrome.App.Window.OnBoundsChanged ();
		}

		[ChromeMinimumVersion (26)]
		[MonoPInvokeCallback (typeof(Action))]
		public static void onClosed ()
		{
			Chrome.App.Window.OnClosed ();
		}

		[ChromeMinimumVersion (27)]
		[MonoPInvokeCallback (typeof(Action))]
		public static void onFullscreened ()
		{
			Chrome.App.Window.OnFullscreened ();
		}

		[ChromeMinimumVersion (26)]
		[MonoPInvokeCallback (typeof(Action))]
		public static void onMaximized ()
		{
			Chrome.App.Window.OnMaximized ();
		}

		[ChromeMinimumVersion (26)]
		[MonoPInvokeCallback (typeof(Action))]
		public static void onMinimized ()
		{
			Chrome.App.Window.OnMinimized ();
		}

		[ChromeMinimumVersion (26)]
		[MonoPInvokeCallback (typeof(Action))]
		public static void onRestored ()
		{
			Chrome.App.Window.OnRestored ();
		}
	}

}
#endif