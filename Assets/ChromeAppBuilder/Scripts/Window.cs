using System;
using UnityEngine;

namespace Chrome.App
{
	/// <summary>
	/// Access the Chrome app window functionalities.
	/// </summary>
	public class Window
	{
		/// <summary>
		/// Bounds of a window.
		/// </summary>
		public class Bounds
		{
			public bool inner {
				get;
				private set;
			}

			public Bounds (bool inner)
			{
				this.inner = inner;
			}

			public int Left {
				get {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					return Native.Window.GetBounds (inner, "left");
					#else
					return 0;
					#endif
				}
				set {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					Native.Window.SetBounds (inner, "left", value);
					#else
					Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
					#endif
				}
			}

			public int Top {
				get {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					return Native.Window.GetBounds (inner, "top");
					#else
					return 0;
					#endif
				}
				set {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					Native.Window.SetBounds (inner, "top", value);
					#else
					Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
					#endif
				}
			}

			public int Width {
				get {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					return Native.Window.GetBounds (inner, "width");
					#else
					return Screen.width;
					#endif
				}
				set {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					Native.Window.SetBounds (inner, "width", value);
					#else
					Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
					#endif
				}
			}

			public int Height {
				get {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					return Native.Window.GetBounds (inner, "height");
					#else
					return Screen.height;
					#endif
				}
				set {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					Native.Window.SetBounds (inner, "height", value);
					#else
					Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
					#endif
				}
			}

			public int MinWidth {
				get {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					return Native.Window.GetBounds (inner, "minWidth");
					#else
					return Screen.width;
					#endif
				}
				set {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					Native.Window.SetBounds (inner, "minWidth", value);
					#else
					Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
					#endif
				}
			}

			public int MinHeight {
				get {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					return Native.Window.GetBounds (inner, "minHeight");
					#else
					return Screen.height;
					#endif
				}
				set {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					Native.Window.SetBounds (inner, "minHeight", value);
					#else
					Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
					#endif
				}
			}

			public int MaxWidth {
				get {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					return Native.Window.GetBounds (inner, "maxWidth");
					#else
					return Screen.width;
					#endif
				}
				set {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					Native.Window.SetBounds (inner, "maxWidth", value);
					#else
					Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
					#endif
				}
			}

			public int MaxHeight {
				get {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					return Native.Window.GetBounds (inner, "maxHeight");
					#else
					return Screen.height;
					#endif
				}
				set {
					#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
					Native.Window.SetBounds (inner, "maxHeight", value);
					#else
					Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
					#endif
				}
			}

			public void SetPosition (int left, int top)
			{
				this.Left = left;
				this.Top = top;
			}

			public void SetSize (int width, int height)
			{
				this.Width = width;
				this.Height = height;
			}

			public void SetMinimumSize (int minWidth, int minHeight)
			{
				this.MinWidth = minWidth;
				this.MinHeight = minHeight;
			}

			public void SetMaximumSize (int maxWidth, int maxHeight)
			{
				this.MaxWidth = maxWidth;
				this.MaxHeight = maxHeight;
			}

			public Rect ToRect ()
			{
				return new Rect (this.Left, this.Top, this.Width, this.Height);
			}

			public void FromRect (Rect rect)
			{
				this.Left = (int)rect.x;
				this.Top = (int)rect.y;
				this.Width = (int)rect.width;
				this.Height = (int)rect.height;
			}
		}

		public static Bounds innerBounds = new Bounds (true);
		public static Bounds outerBounds = new Bounds (false);

		#pragma warning disable 67
		public static event Action onBoundsChanged;
		public static event Action onClosed;
		public static event Action onFullscreened;
		public static event Action onMaximized;
		public static event Action onMinimized;
		public static event Action onRestored;
		#pragma warning restore 67

		#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
		public static void OnBoundsChanged ()
		{
			Action handler = onBoundsChanged;
			if (handler != null) {
				handler ();
			}
		}
		public static void OnClosed ()
		{
			Action handler = onClosed;
			if (handler != null) {
				handler ();
			}
		}
		public static void OnFullscreened ()
		{
			Action handler = onFullscreened;
			if (handler != null) {
				handler ();
			}
		}
		public static void OnMaximized ()
		{
			Action handler = onMaximized;
			if (handler != null) {
				handler ();
			}
		}
		public static void OnMinimized ()
		{
			Action handler = onMinimized;
			if (handler != null) {
				handler ();
			}
		}
		public static void OnRestored ()
		{
			Action handler = onRestored;
			if (handler != null) {
				handler ();
			}
		}
		#endif
		public static bool fullscreen {
			get {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				return Native.Window.IsFullscreen ();
				#else
				return Screen.fullScreen;
				#endif
			}
			set {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				if (value) {
					Native.Window.FullScreen ();
				} else {
					Native.Window.Restore ();
				}
				#else
				Screen.fullScreen = value;
				#endif
			}
		}

		public static void ToggleFullscreen ()
		{
			fullscreen = !fullscreen;
		}

		public static bool maximized {
			get {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				return Native.Window.IsMaximized ();
				#else
				return false;
				#endif
			}
			set {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				if (value) {
					Native.Window.Maximize ();
				} else {
					Native.Window.Restore ();
				}
				#else
				Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
				#endif
			}
		}

		public static bool minimized {
			get {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				return Native.Window.IsMinimized ();
				#else
				return false;
				#endif
			}
			set {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				if (value) {
					Native.Window.Minimize ();
				} else {
					Native.Window.Restore ();
				}
				#else
				Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
				#endif
			}
		}

		public static void Restore ()
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Window.Restore ();
			#else
			Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
			#endif
		}

		public static void Close ()
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Window.Close ();
			#else
			Application.Quit ();
			#endif
		}

		public static void Focus ()
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Window.Focus ();
			#else
			Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
			#endif
		}

		public static void DrawAttention ()
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Window.DrawAttention ();
			#else
			Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
			#endif
		}

		public static void ClearAttention ()
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Window.ClearAttention ();
			#else
			Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
			#endif
		}

		public static void Show (bool focused)
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Window.Show (focused);
			#else
			Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
			#endif
		}

		public static void Hide ()
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Window.Hide ();
			#else
			Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
			#endif
		}

		public static bool alwaysOnTop {
			get {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				return Native.Window.IsAlwaysOnTop ();
				#else
				return false;
				#endif
			}
			set {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				Native.Window.SetAlwaysOnTop (value);
				#else
				Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
				#endif
			}
		}

		public static bool CanSetVisibleOnAllWorkspaces ()
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			return Chrome.Native.Window.CanSetVisibleOnAllWorkspaces ();
			#else
			return false;
			#endif
		}

		public static void SetVisibleOnAllWorkspaces (bool alwaysVisible)
		{
			#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			Chrome.Native.Window.SetVisibleOnAllWorkspaces (alwaysVisible);
			#else
			Debug.LogException (new System.NotSupportedException ("This only works in a Chrome App."));
			#endif
		}

		public static string ID {
			get {
				#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
				return Native.Window.ID ();
				#else
				return "";
				#endif
			}
		}
	}
}