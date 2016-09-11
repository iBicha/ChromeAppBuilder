using System;
using UnityEngine;

namespace Chrome.App
{
	/// Access the Power functionalities.
	public class Power
	{
		/// <summary>
		/// The level requested for the keep awake function.
		/// </summary>
		public enum Level
		{
			System,
			Display
		}

		/// <summary>
		/// Requests the system to prevent sleep, according to Level.
		/// </summary>
		/// <param name="level">Level. Set "System" to prevent the computer from sleeping, or "Display" to prevent the screen from dimming the light of turning off.</param>
		public static void RequestKeepAwake (Level level)
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Power.RequestKeepAwake (level.ToString ().ToLower ());
			#else
			Debug.LogException( new System.NotSupportedException("This only works in a Chrome App."));
			#endif
		}
		/// <summary>
		/// Releases a previously requested prevent sleep call.
		/// </summary>
		public static void ReleaseKeepAwake ()
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Power.ReleaseKeepAwake ();
			#else
			Debug.LogException( new System.NotSupportedException("This only works in a Chrome App."));
			#endif
		}
	}
}