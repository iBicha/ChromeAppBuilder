using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
 
namespace ChromeAppBuilder
{

	public class Builder
	{
		 
		public static void BuildAndRunPlayer (){
			BuildPlayer (true);
		}

		public static void BuildPlayer (bool run = false)
		{
			//Just in case, we save the settings
			BuildSettings.Save();

			string path = EditorUtility.SaveFolderPanel ("Choose Location of Built Game", EditorUserBuildSettings.GetBuildLocation (BuildTarget.WebGL), "");
			if (Directory.Exists (path)) {
				ProgressHelper progress = new ProgressHelper ();
				progress.Reset (5);


				//Set Template
				string lastUsedTemplate = TemplateManager.SetChromeTemplate("PROJECT:ChromeApp");
				//Define symbols
				Builder.SetDefineSymbol (true);
				//Create subfolder
				path = Builder.CreateSubFolder (path);

				progress.SetCleanupCallback (()=>{
					//Clean up
					//Reset Webgl template
					TemplateManager.SetChromeTemplate(lastUsedTemplate);
					//Define symbols
					Builder.SetDefineSymbol (false);
				});
				//BUILD
				if (!Builder.BuildPlayer (path)) {
					progress.Done ();
					return;
				}	
				//Process template
				progress.Step ("Building Player - Chrome App", "Postprocessing Template Variables");
				TemplateManager.ProcessTemplateFiles (path);
				//Creatig Icons
				progress.Step ("Building Player - Chrome App", "Creating icons");
				Icons.CreateIcons (path);
				//Creating Manifest
				progress.Step ("Building Player - Chrome App", "Creating manifest");
				Manifest.CreateManifestFile (path);
				//Packing Extention
				progress.Step ("Building Player - Chrome App", "Packing Extension into .crx file");
				if (BuildSettings.Get.packExtension) {
					if (!ChromeHelper.PackExtension (path)) {
						Debug.LogWarning ("Chrome app was not packed into a .crx file.");
					}
				}
				//launch player or reveal in explorer
				progress.Step ("Building Player - Chrome App", "Launching");
				if (run) {
					if (File.Exists (path + ".crx")) {
						CrxHeader header = new CrxHeader (path + ".crx");
						Manifest.AddKeyToManifest (Path.Combine(path, "manifest.json"), header.PublicKey);
					}
					ChromeHelper.LoadAndRunExtension (path);
				}else {
					EditorUtility.RevealInFinder (path);
				}
				//Clear progress and cleanup
				progress.Done ();
			}  
		}

		public static bool BuildPlayer (string path)
		{
			List<string> scenes = new List<string> ();
			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
				if (scene.enabled) {
					scenes.Add (scene.path);
				}
			}
			BuildOptions options = BuildOptions.None;
			if (EditorUserBuildSettings.development) {
				options |= BuildOptions.Development;
			}
			if (EditorUserBuildSettings.connectProfiler) {
				options |= BuildOptions.ConnectWithProfiler;
			}
			string ret = BuildPipeline.BuildPlayer (scenes.ToArray (), path, BuildTarget.WebGL, options);
			if (!string.IsNullOrEmpty (ret)) {
				Debug.LogError (ret);
				return false;
			}
			return true;
		}
		 
		public static string CreateSubFolder (string path)
		{
			char[] invalidchars = Path.GetInvalidFileNameChars ();
			string subfoldername = PlayerSettings.productName + "ChromeApp";
			foreach (char c in invalidchars) {
				subfoldername = subfoldername.Replace (c.ToString (), "");
			}
			subfoldername = subfoldername.Replace (" ", "");
			string newPath = Path.Combine (path, subfoldername);
			if (!Directory.Exists (newPath)) {
				Directory.CreateDirectory (newPath);
			}
			return newPath;
		}

		public static void SetDefineSymbol (bool value)
		{
			string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup (BuildTargetGroup.WebGL);
			symbols = symbols.Replace ("UNITY_CHROME", "");
			if (value) {
				symbols = symbols + ";" + "UNITY_CHROME";
			}
			PlayerSettings.SetScriptingDefineSymbolsForGroup (BuildTargetGroup.WebGL, symbols);

		}


	}
}