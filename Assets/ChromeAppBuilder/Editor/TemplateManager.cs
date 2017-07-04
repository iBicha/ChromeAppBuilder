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
            string lastUsedTemplate = PlayerSettings.WebGL.template;
            PlayerSettings.WebGL.template = newTemplate;
			return lastUsedTemplate;
		}


		public static void ProcessTemplateFiles (string path)
		{
			string[] files = new string[]{"AspectRatio.js", "Background.js","index.html", "Loader.js"};
			foreach (string file in files) {
				ProcessTemplateFile(path, file);
			}	
		}

		public static void ProcessTemplateFile (string path,string file)
		{
			string buildName = Path.GetFileName (path);
            string fullpath = Path.Combine(path, file);
            if (!File.Exists(fullpath))
            {
                return;
            }
            string filecontent = File.ReadAllText(fullpath);
            filecontent = filecontent.Replace ("%UNITY_WIDTH%", PlayerSettings.defaultWebScreenWidth.ToString ());
			filecontent = filecontent.Replace ("%UNITY_HEIGHT%", PlayerSettings.defaultWebScreenHeight.ToString ());
			filecontent = filecontent.Replace ("%UNITY_WEB_NAME%", PlayerSettings.productName);
			filecontent = filecontent.Replace ("%UNITY_WEBGL_LOADER_URL%", "Build/UnityLoader.js");
			filecontent = filecontent.Replace ("%UNITY_WEBGL_BUILD_URL%", "Build/" + buildName + ".json");

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





        /*private static Type tyWebGlBuildPostprocessor;
		private static Type tyBuildPostProcessArgs;

		private static MethodInfo MIProcessTemplateFile;

		private static string ProcessTemplateFileInternal (object args, string templatePath) {
			if (tyWebGlBuildPostprocessor == null) {
				Assembly webglExtensiosn = Assembly.LoadFile (EditorApplication.applicationContentsPath + "/PlaybackEngines/WebGLSupport/UnityEditor.WebGL.Extensions.dll");
				tyWebGlBuildPostprocessor = webglExtensiosn.GetType ("UnityEditor.WebGL.WebGlBuildPostprocessor");
			}
			if (tyWebGlBuildPostprocessor != null) {
				if (MIProcessTemplateFile == null) {
					if (tyBuildPostProcessArgs == null) {
						tyBuildPostProcessArgs = Assembly.GetAssembly (typeof(UnityEditor.Editor)).GetType ("UnityEditor.Modules.BuildPostProcessArgs");
					}
					MIProcessTemplateFile = tyWebGlBuildPostprocessor.GetMethod ("ProcessTemplateFile", BindingFlags.NonPublic | BindingFlags.Static, null, new Type[] {
						tyWebGlBuildPostprocessor,
						typeof(string)
					}, null);
				}
				if (MIProcessTemplateFile != null) {
					return (string)MIProcessTemplateFile.Invoke (null,new object[]{ args, templatePath });
				}
			}
			return string.Empty;
		}*/

    }
}