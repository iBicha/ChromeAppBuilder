using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using AOT;
using Newtonsoft.Json;
using Chrome.App;


namespace Chrome.Native
{

	public class Notifications {
		

		[Flags] 
		public enum UpdateFlags
		{
			None = 0x0,
			TemplateType = 0x1,
			IconUrl = 0x2,
			Title = 0x4,
			Message = 0x8,
			ContextMessage = 0x10,
			Priority = 0x20,
			EventTime = 0x40,
			Buttons = 0x60,
			Items = 0x80,
			Progress = 0x100,
			IsClickable = 0x200,
			RequireInteraction = 0x400,
			All = TemplateType | IconUrl | Title | Message | ContextMessage | Priority | EventTime | Buttons | Items | Progress | IsClickable | RequireInteraction
		}
#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
		const string ChromeAppNotificationsPrefix = "ChromeApp_Notifications_";
		[RuntimeInitializeOnLoadMethod]
		public static void Initialize ()
		{
			//TODO: FIX THIS
			/*AddListener ("onClosed", Notification.OnClosed);
			AddListener ("onClicked", Notification.OnClicked);
			AddListener ("onButtonClicked", Notification.OnButtonClicked);
			AddListener ("onPermissionLevelChanged", Notification.OnPermissionLevelChanged);
			AddListener ("onShowSettings", Notification.OnShowSettings);
			*/
		}

		[DllImport ("__Internal", EntryPoint = ChromeAppNotificationsPrefix + "addListener")]
		public static extern void AddListener (string eventName, Action callback);

		[DllImport ("__Internal", EntryPoint = ChromeAppNotificationsPrefix + "create")]
		public static extern void Create (string notificationId, string options, createNotificationCallback callback, int callbackId);

		[DllImport ("__Internal", EntryPoint = ChromeAppNotificationsPrefix + "update")]
		public static extern void Update (string notificationId, string options, updateNotificationCallback callback, int callbackId);

		[DllImport ("__Internal", EntryPoint = ChromeAppNotificationsPrefix + "clear")]
		public static extern void Clear (string notificationId, clearNotificationCallback callback, int callbackId);

		public delegate void createNotificationCallback (string notificationId, int callbackId);
		public delegate void updateNotificationCallback (bool wasUpdated, int callbackId);
		public delegate void clearNotificationCallback (bool wasCleared, int callbackId);

		[MonoPInvokeCallback (typeof(createNotificationCallback))]
		public static void onCreateNotificationCallback (string notificationId, int callbackId)
		{
			createNotificationCallback callback = CallbackRegisrty.GetCallback<createNotificationCallback> (callbackId);
			if (callback != null) {
				callback (notificationId, callbackId);
			}
		}

		[MonoPInvokeCallback (typeof(updateNotificationCallback))]
		public static void onUpdateNotificationCallback (bool wasUpdated, int callbackId)
		{
			updateNotificationCallback callback = CallbackRegisrty.GetCallback<updateNotificationCallback> (callbackId);
			if (callback != null) {
				callback (wasUpdated, callbackId);
			}
		}

		[MonoPInvokeCallback (typeof(clearNotificationCallback))]
		public static void onClearNotificationCallback (bool wasCleared, int callbackId)
		{
			clearNotificationCallback callback = CallbackRegisrty.GetCallback<clearNotificationCallback> (callbackId);
			if (callback != null) {
				callback (wasCleared, callbackId);
			}
		}
#endif

		public delegate void onClosedCallback (string notificationId);

		[MonoPInvokeCallback (typeof(onClosedCallback))]
		public static void OnClosed (string notificationId)
		{
			Debug.Log ("OnClosed " + notificationId);
		}

		[MonoPInvokeCallback (typeof(Action<string>))]
		public static void OnClicked (string notificationId)
		{
			Debug.Log ("OnClicked " + notificationId);
		}

		[MonoPInvokeCallback (typeof(Action<string, int>))]
		public static void OnButtonClicked (string notificationId, int buttonIndex)
		{
			Debug.Log ("OnButtonClicked " + notificationId + " " + buttonIndex.ToString());
		}

		[MonoPInvokeCallback (typeof(Action<string>))]
		public static void OnPermissionLevelChanged (string level)
		{
			//TODO: level to enum
			Debug.Log ("OnPermissionLevelChanged " + level);
		}

		[MonoPInvokeCallback (typeof(Action))]
		public static void OnShowSettings ()
		{
			Debug.Log ("OnShowSettings");
		}


		private static string GetOptions(Notification notification, UpdateFlags flags = UpdateFlags.All) {
			Dictionary<string, object> opts = new Dictionary<string, object> ();
			if ((flags & UpdateFlags.TemplateType) != 0) {
				opts ["type"] = notification.Type.ToString ();
			}
			if ((flags & UpdateFlags.IconUrl) != 0 && !string.IsNullOrEmpty (notification.IconUrl)) {
				opts ["iconUrl"] = notification.IconUrl;
			}
			if ((flags & UpdateFlags.Title) != 0) {
				opts ["title"] = notification.Title;
			}
			if ((flags & UpdateFlags.Message) != 0) {
				opts ["message"] = notification.Message;
			}
			if ((flags & UpdateFlags.ContextMessage) != 0 && !string.IsNullOrEmpty (notification.ContextMessage)) {
				opts ["contextMessage"] = notification.ContextMessage;
			}
			if ((flags & UpdateFlags.Priority) != 0) {
				opts ["priority"] = notification.Priority;
			}
			if ((flags & UpdateFlags.EventTime) != 0) {
				opts ["eventTime"] = notification.EventTime.Ticks;
			}
			if ((flags & UpdateFlags.Buttons) != 0 && notification.Buttons !=null && notification.Buttons.Count > 0) {
				ArrayList buttons = new ArrayList ();
				foreach (var button in notification.Buttons) {
					Dictionary<string, object> buttonOpts = new Dictionary<string, object> ();
					buttonOpts ["title"] = button.Title;
					if (!string.IsNullOrEmpty (button.IconUrl)) {
						buttonOpts ["iconUrl"] = button.IconUrl;
					}
					buttons.Add (buttonOpts);
				}
				opts ["buttons"] = buttons;
			}

			if ((flags & UpdateFlags.Items) != 0 && notification.Type == Chrome.App.Notification.TemplateType.list && notification.Items !=null && notification.Items.Count > 0) {
				ArrayList items = new ArrayList ();
				foreach (var item in notification.Items) {
					Dictionary<string, object> itemOpts = new Dictionary<string, object> ();
					itemOpts ["title"] = item.Title;
					itemOpts ["message"] = item.Message;
					items.Add (itemOpts);
				}
				opts ["items"] = items;
			}
			if ((flags & UpdateFlags.Progress) != 0 && notification.Type == Chrome.App.Notification.TemplateType.progress) {
				opts ["progress"] = notification.Progress;
			}
			if ((flags & UpdateFlags.IsClickable) != 0) {
				opts ["isClickable"] = notification.IsClickable;
			}
			if ((flags & UpdateFlags.RequireInteraction) != 0) {
				opts ["requireInteraction"] = notification.RequireInteraction;
			}
			return JsonConvert.SerializeObject (opts);
		}

		public static void Show(Notification notification) {
#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			int callbackID = Native.CallbackRegisrty.RegisterCallback (
				(createNotificationCallback)((notificationId, callbackId) => {
					notification.ID = notificationId;
				}));
			Create (notification.ID, GetOptions(notification), onCreateNotificationCallback, callbackID);
#else
			Debug.LogException( new System.NotSupportedException("This only works in a Chrome App."));
#endif
		}

		public static void Clear(Notification notification) {
#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			if (!string.IsNullOrEmpty (notification.ID)) {
				int callbackID = Native.CallbackRegisrty.RegisterCallback (
					(clearNotificationCallback)((wasCleared, callbackId) => {
						Debug.Log ("clearNotificationCallback " + notification.ID + " " + wasCleared.ToString());
					}));
				Clear (notification.ID, onClearNotificationCallback, callbackID);
			}
#else
			Debug.LogException( new System.NotSupportedException("This only works in a Chrome App."));
#endif
		}

		public static void Update(Notification notification, UpdateFlags flags = UpdateFlags.All) {
#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
			if (!string.IsNullOrEmpty (notification.ID)) {
				int callbackID = Native.CallbackRegisrty.RegisterCallback (
					(updateNotificationCallback)((wasUpdated, callbackId) => {
						Debug.Log ("updateNotificationCallback " + notification.ID + " " + wasUpdated.ToString());
					}));
				Update (notification.ID, GetOptions(notification, flags), onUpdateNotificationCallback, callbackID);
			}
#else
			Debug.LogException( new System.NotSupportedException("This only works in a Chrome App."));
#endif
		}

	}
}