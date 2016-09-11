using UnityEditor;
using System.IO;
using Microsoft.Win32;
using System.Collections;
using UnityEngine;
using System.Runtime.InteropServices;
 
namespace ChromeAppBuilder{
	public class ChromeHelper {
 

		public static bool LoadAndRunExtension(string path){
			string ChromeExecutablePath = GetChromeLocation();
			if (ChromeExecutablePath != "") {
				string args ="--load-and-launch-app=" + "\"" + path + "\"";
				System.Diagnostics.Process chrome = new System.Diagnostics.Process ();
				chrome.StartInfo.FileName = ChromeExecutablePath;
				chrome.StartInfo.Arguments = args;
				if(chrome.Start ()){
					if(chrome.WaitForExit (30 * 1000)){
						return chrome.ExitCode==0;	
					}
					try{
						chrome.Kill ();
					}
					catch{}
				}
			}
			return false;
		}

		public static bool PackExtension (string path)
		{
			string ChromeExecutablePath = GetChromeLocation();
			if (ChromeExecutablePath != "") {
				string args = "--pack-extension=" + "\"" + path + "\"";
				if (BuildSettings.Get.updateExtension && File.Exists (BuildSettings.Get.pemFile)) {
					args += " --pack-extension-key=" + "\"" + BuildSettings.Get.pemFile + "\"";
				}else{
					string oldPemFile = path + ".pem";
					if(File.Exists(oldPemFile)){
						if(!File.Exists(oldPemFile+".old")){
							File.Move(oldPemFile, oldPemFile + ".old");
						}else{
							int i=1;
							while(File.Exists(oldPemFile + " (" + i + ')'+".old")){
								i++;
							}
							File.Move(oldPemFile, oldPemFile + " (" + i + ')'+".old");
						}
					}
				}
				System.Diagnostics.Process chrome = new System.Diagnostics.Process ();
				chrome.StartInfo.FileName = ChromeExecutablePath;
				chrome.StartInfo.Arguments = args;
				if(chrome.Start ()){
					//wait about 5 minutes, even that is too much for large projects.
					if(chrome.WaitForExit (5 * 60 * 1000)){
						return chrome.ExitCode==0;	
					}
					try{
						chrome.Kill ();
					}
					catch{}
				}
			} else {
				Debug.LogWarning ("Could find Google Chrome. App will not be packed into .crx file.");
			}
			return false;
		}
		public static string GetChromeLocation(){
#if UNITY_EDITOR_WIN
			//EditorPrefs
			string chromeExe = EditorPrefs.GetString("ChromeExeLocation","");
			if(File.Exists(chromeExe)){
				return chromeExe;
			}
			//Registry
			RegistryKey key = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\chrome.exe");
			if(key!=null){
				chromeExe = (string)key.GetValue("","");
				if(File.Exists(chromeExe)){
					EditorPrefs.SetString("ChromeExeLocation",chromeExe);
					return chromeExe;
				}
			}
			//Default Locations. We search these, just in case.
			string[] programLocations = new string[]{
				//Probably here
				System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles),
				System.Environment.ExpandEnvironmentVariables("%ProgramFiles%"),
				System.Environment.ExpandEnvironmentVariables("%ProgramFiles(x86)%"),
				//These are fallbacks
				System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData),
				System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData)
			};
			foreach(string location in programLocations){
				chromeExe = Path.Combine(location,"Google\\Chrome\\Application\\chrome.exe");
				if(File.Exists(chromeExe)){
					EditorPrefs.SetString("ChromeExeLocation",chromeExe);
					return chromeExe;
				}
			}
			/* Last but not least, chrome might be open and in use. we might as well find out where it is.
			 * Yes, we are doing the best so that we don't prompt the user to search for chrome
			 * MANUALLY. UGH.
			 */
			System.Diagnostics.Process[] procs = System.Diagnostics.Process.GetProcessesByName ("chrome");
			foreach (var proc in procs) {
				try {
					chromeExe = proc.MainModule.FileName;
					if(File.Exists(chromeExe)){
						EditorPrefs.SetString("ChromeExeLocation",chromeExe);
						return chromeExe;
					}
				}
				catch{}
			} 
			return "";
#elif UNITY_EDITOR_OSX
			string chromeExe = EditorPrefs.GetString("ChromeExeLocation","");
			if(File.Exists(chromeExe)){
				return chromeExe;
			}
			chromeExe = "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
			if(File.Exists(chromeExe)){
				EditorPrefs.SetString("ChromeExeLocation",chromeExe);
				return chromeExe;
			}
			return "";
#endif
		}
	}
}

