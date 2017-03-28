using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Reflection;
using System.IO;


namespace ChromeAppBuilder
{
	[System.Serializable]
	public class BuildSettings : ScriptableObject
	{
		private const string settingsFile = "Assets/ChromeAppBuilder/Editor/Resources/Settings.asset";


		public string description = "";
		public string shortName = "";

		public int minWidth = 480;
		public int minHeight = 320;
		public int maxWidth = 1920;
		public int maxHeight = 1080;

		public WindowState windowState = WindowState.Normal;
		public bool resizable = true;
		public bool resizeConstrains = true;
		public bool frameless = false;
		public bool lockAspectRatio = true;

		public bool overrideIcons = false;
		public Texture2D[] icons = new Texture2D[3];

		public bool[] permissions = new bool[(int)Permissions.Count];
        public string permissionsExtra = "";

		public bool packExtension = true;
		public bool updateExtension = false;
		public string pemFile = "";

		private static BuildSettings get;
		public static BuildSettings Get{
			get{ 
				if (get == null) {
					Load ();

				}
				return get;
			}
		}
		 
		public static void Load(string fromFile = ""){
			if (string.IsNullOrEmpty (fromFile)) {
				fromFile = settingsFile;
			}
			UnityEngine.Object[] array = UnityEditorInternal.InternalEditorUtility.LoadSerializedFileAndForget(fromFile);
			if (array.Length > 0 && array[0]!=null) {
				get = array [0] as BuildSettings;
			} 
			if (get == null) { 
				get = ScriptableObject.CreateInstance<BuildSettings> ();
					
			}  
		}
		public static void Save(string toFile = ""){
			if (string.IsNullOrEmpty (toFile)) {
				toFile = settingsFile;
			}
			UnityEditorInternal.InternalEditorUtility.SaveToSerializedFileAndForget (new UnityEngine.Object[]{ Get },toFile, false);
			AssetDatabase.Refresh ();
		}
	}
}   