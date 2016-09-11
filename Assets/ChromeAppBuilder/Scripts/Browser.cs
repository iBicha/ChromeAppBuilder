using System;
using UnityEngine;
using System.ComponentModel;

namespace Chrome.App
{
	/// <summary>
	/// Access the Chrome app browser functionalities.
	/// </summary>
	public class Browser
	{
        /// <summary>
        /// Opens the URL in a new Chrome tab.
        /// </summary>
        /// <param name="url">The URL string to be opened.</param>
        public static void OpenTab (string url)
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Browser.OpenTab(url);
			#else
			Application.OpenURL (url);
			#endif
		}
	}
}