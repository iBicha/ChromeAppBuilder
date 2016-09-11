using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System;
using System.Reflection;
namespace ChromeAppBuilder
{

	public class TemplateManager
	{

		public static string SetChromeTemplate (string newTemplate)
		{
			string lastUsedTemplate = PlayerSettings.GetPropertyString ("template", BuildTargetGroup.WebGL);
			PlayerSettings.SetPropertyString ("template", newTemplate, BuildTargetGroup.WebGL);
			return lastUsedTemplate;
		}


		public static void ProcessTemplateFiles (string path)
		{
			string[] files = new string[]{"AspectRatio.js", "Background.js","Config.js","index.html"};
			foreach (string file in files) {
				ProcessTemplateFile(path, file);
			}	
		}

		public static void ProcessTemplateFile (string path,string file)
		{
			string fullpath = Path.Combine (path, file);
			string filecontent = File.ReadAllText (fullpath);
			filecontent = filecontent.Replace ("%UNITY_WIDTH%", PlayerSettings.defaultWebScreenWidth.ToString ());
			filecontent = filecontent.Replace ("%UNITY_HEIGHT%", PlayerSettings.defaultWebScreenHeight.ToString ());
			filecontent = filecontent.Replace ("%UNITY_WEB_NAME%", PlayerSettings.productName);
			filecontent = filecontent.Replace ("%UNITY_DEVELOPMENT_PLAYER%", EditorUserBuildSettings.development ? "1" : "0");
			filecontent = filecontent.Replace ("%UNITY_WEBGL_DATA_FOLDER%", EditorUserBuildSettings.development ? "Development" : "Release");
			filecontent = filecontent.Replace ("%UNITY_WEBGL_FILE_NAME%", Path.GetFileName (path));
			filecontent = filecontent.Replace ("%UNITY_WEBGL_TOTAL_MEMORY%", ((uint)((PlayerSettings.GetPropertyInt ("memorySize", BuildTargetGroup.WebGL) * 0x400) * 0x400)).ToString ());

			/*
			 * Custom values requires System.Reflection, and will fail silently in case methods didn't load 
			 * (if unity decided to change these), and then, it will do nothing. so no harm.
			 * This is here just in case someone decides to edit the chrome app template and add custom tags.
		     */
			foreach (string str in templateCustomKeys)
			{
				filecontent = filecontent.Replace ("%UNITY_CUSTOM_" + str.ToUpper() + "%", GetTemplateCustomValue(str));
			}
 			//Finally, our custom keys.
			filecontent = filecontent.Replace ("%UNITY_CHROME_CONSTRAINTS%", BuildSettings.Get.resizeConstrains.ToString().ToLower());
			filecontent = filecontent.Replace ("%UNITY_CHROME_MIN_WIDTH%", BuildSettings.Get.minWidth.ToString());
			filecontent = filecontent.Replace ("%UNITY_CHROME_MIN_HEIGHT%", BuildSettings.Get.minHeight.ToString());
			filecontent = filecontent.Replace ("%UNITY_CHROME_MAX_WIDTH%", BuildSettings.Get.maxWidth.ToString());
			filecontent = filecontent.Replace ("%UNITY_CHROME_MAX_HEIGHT%", BuildSettings.Get.maxHeight.ToString());
			filecontent = filecontent.Replace ("%UNITY_CHROME_WINDOW_STATE%", BuildSettings.Get.windowState.ToString().ToLower());
			filecontent = filecontent.Replace ("%UNITY_CHROME_WINDOW_RESIZABLE%", BuildSettings.Get.resizable.ToString().ToLower());
			filecontent = filecontent.Replace ("%UNITY_CHROME_LOCK_ASPECT_RATIO%", BuildSettings.Get.lockAspectRatio.ToString().ToLower());
			filecontent = filecontent.Replace ("%UNITY_CHROME_FRAMELESS%", BuildSettings.Get.frameless ? "none" : "chrome");

			File.WriteAllText (fullpath, filecontent);
		}

		private static Type tyPlayerSettings;
		private static MethodInfo MIGetTemplateCustomValue;
		private static PropertyInfo PItemplateCustomKeys;
		private static string GetTemplateCustomValue(string key){
			if (tyPlayerSettings == null) {
				tyPlayerSettings = typeof(PlayerSettings);
			}
			if (tyPlayerSettings != null) {
				if (MIGetTemplateCustomValue == null) {
					MIGetTemplateCustomValue = tyPlayerSettings.GetMethod ("GetTemplateCustomValue", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] {
						typeof(string)
					}, null);
				}
				if (MIGetTemplateCustomValue != null) {
					return (string)MIGetTemplateCustomValue.Invoke (null,new object[]{ key });
				}
			}
			return string.Empty;
		}

		private static string[] templateCustomKeys
		{
			get{
				if (tyPlayerSettings == null) {
					tyPlayerSettings = typeof(PlayerSettings);
				}
				if (tyPlayerSettings != null) {
					if (PItemplateCustomKeys == null) {
						PItemplateCustomKeys = tyPlayerSettings.GetProperty ("templateCustomKeys", BindingFlags.NonPublic | BindingFlags.Static);
					}
					if (PItemplateCustomKeys != null) {
						return (string[]) PItemplateCustomKeys.GetValue (null, null);;
					}
				}
				return new string[]{};
			}
		}
	}
}