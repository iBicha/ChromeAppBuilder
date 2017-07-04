using System;
using UnityEngine;
using System.ComponentModel;
using System.Collections.Generic;
using Chrome.Native;

namespace Chrome.App
{
	public class Notification
	{

		public static bool CloneIconTexture = true;

		public enum TemplateType
		{
			basic,
			image,
			list,
			progress
		}

		public enum PermissionLevel
		{
			granted,
			denied
		}

		private TemplateType type = TemplateType.basic;
		private Texture2D icon;
		private string iconUrl;
		private string title;
		private string message;
		private string contextMessage;
		private int priority = 0;
		private DateTime eventTime;
		private List<Button> buttons;
		private List<Item> items;
		private int progress = 0;
		private bool isClickable;
		private bool requireInteraction = false;


		public string ID;
		public TemplateType Type {
			get {
				return type;
			}
			set {
				type = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.TemplateType);
			}
		}
		public Texture2D Icon {
			get {
				return icon;
			}
			set {
				if (value == null) {
					icon = value;
					IconUrl = null;
				} else {
					try {
						if(CloneIconTexture) {
							icon = CloneTexture(value);
						} else {
							icon = value;
						}
						if(icon != null) {
							IconUrl = "data:image/png;base64," + Convert.ToBase64String (icon.EncodeToPNG ());
						}
					} catch (Exception ex) {
						Debug.LogError (ex);
					}
				}
			}
		}
		public string IconUrl {
			get {
				return iconUrl;
			}
			set {
				iconUrl = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.IconUrl);
			}
		}
		public string Title {
			get {
				return title;
			}
			set {
				title = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.Title);
			}
		}
		public string Message {
			get {
				return message;
			}
			set {
				message = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.Message);
			}
		}

		[ChromeMinimumVersion (31)]
		public string ContextMessage {
			get {
				return contextMessage;
			}
			set {
				contextMessage = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.ContextMessage);
			}
		}
		public int Priority {
			get {
				return priority;
			}
			set {
				priority = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.Priority);
			}
		}
		public DateTime EventTime{
			get {
				return eventTime;
			}
			set {
				eventTime = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.EventTime);
			}
		}
		public List<Button> Buttons{
			get {
				return buttons;
			}
			set {
				buttons = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.Buttons);
			}
		}
		public List<Item> Items{
			get {
				return items;
			}
			set {
				items = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.Items);
			}
		}
		[ChromeMinimumVersion (30)]
		public int Progress{
			get {
				return progress;
			}
			set {
				progress = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.Progress);
			}
		}
		[ChromeMinimumVersion (32)]
		public bool IsClickable{
			get {
				return isClickable;
			}
			set {
				isClickable = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.IsClickable);
			}
		}
		[ChromeMinimumVersion (50)]
		public bool RequireInteraction{
			get {
				return requireInteraction;
			}
			set {
				requireInteraction = value;
				Notifications.Update (this, Chrome.Native.Notifications.UpdateFlags.RequireInteraction);
			}
		}


		public Notification() {
			type = TemplateType.basic;
		}

		public void Show() {
			Notifications.Show (this);
		}

		public void Hide() {
			Clear ();
		}

		public void Clear() {
			Notifications.Clear (this);
		}


		public class Button
		{
			public string Title;
			public string IconUrl; //TODO: Texture
			public Action onClicked;
		}

		public class Item
		{
			public string Title;
			public string Message; 
		}

		//This is bad. 
		private Texture2D CloneTexture(Texture2D src) {
			try {
				src.GetPixel(0,0);
				Texture2D dst = new Texture2D(src.width, src.height, TextureFormat.ARGB32, false, true);
				dst.SetPixels32 (src.GetPixels32 ());
				return dst;
			} catch (Exception ex) {
				Debug.LogError (ex);
				return null;
			}
		}
	}
}