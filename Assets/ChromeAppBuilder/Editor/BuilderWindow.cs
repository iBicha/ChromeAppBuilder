using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using System.IO;
using System.Collections;
using UnityEngine.Events;

namespace ChromeAppBuilder
{
	
	public class BuilderWindow : EditorWindow
	{
		private static GUIStyle categoryBox; 
		private static int selectedSection = -1;
		private static Vector2 scrollPos;
		private static AnimBool[] SectionAnimators = new AnimBool[7];
		private static EditorBuildSettingsScene[] scenes;
		private static bool buildClicked = false;
		private static bool buildAndRunClicked = false;
        private static bool WebGLDebugSymbols;
        private static bool WebGLDataCaching;
        private static WebGLExceptionSupport WebGLExceptionSupport;
        private static WebGLCompressionFormat WebGLCompressionFormat;
        private static int WebGLMemorySize;

		[MenuItem ("Window/Chrome App Builder")]
		static void Init ()
		{ 
			// Get existing open window or if none, make a new one:
			BuilderWindow window = (BuilderWindow)EditorWindow.GetWindow (typeof(BuilderWindow));
			window.titleContent.text = "Chrome Build";
			window.titleContent.image = windowIcon;
			window.minSize = new Vector2 (300, 300);

			window.Show();
			 
		}

	  

		static Texture2D windowIcon;
		static  Texture2D WindowIcon {
			get {
				if (windowIcon == null) {
					windowIcon = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ChromeAppBuilder/Editor/Resources/icon.png");
				}
				return windowIcon;
			}
		}
	 
		void OnGUI ()
		{
#if !UNITY_5
			EditorGUILayout.HelpBox ("Unity 5 Required to build chrome apps.", MessageType.Error);
			return;
#endif

#if !UNITY_5_3 && !UNITY_5_3_OR_NEWER
			EditorGUILayout.HelpBox ("Unity 5.3 or newer recommended. Older versions may fail at building Chrome Apps.", MessageType.Warning);
#endif

#if !UNITY_EDITOR_64
			EditorGUILayout.HelpBox ("Building for Chrome (WebGL) requires a 64-bit Unity editor.", MessageType.Error);
			return;
#endif

			if (categoryBox == null) {
				categoryBox = new GUIStyle (GetStyle("HelpBox"));
				categoryBox.padding.left = 14; 
			}
			if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.WebGL) {
				//Scroll with animated settings box causes flickering. No scroll.
				//scrollPos= EditorGUILayout.BeginScrollView (scrollPos);
				ScenesSectionGUI ();
				BuildSettingsSectionGUI ();
				ResolutionAndPresentationSectionGUI ();
				IconSectionGUI ();
				SplashSectionGUI ();
				OtherSectionGUI ();
				PublishSectionGUI ();
				//EditorGUILayout.EndScrollView ();

				EditorGUILayout.Space ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button ("Build", GUILayout.MinWidth(110))) {
					buildClicked = true;
				}
				if (GUILayout.Button ("Build And Run",GUILayout.MinWidth(110))) {
					buildAndRunClicked = true;
				}
				GUILayout.Space (10);
				EditorGUILayout.EndHorizontal ();

				if (buildClicked) {
					buildClicked = false;
					if (RoutineChecks ()) {
						Builder.BuildPlayer ();
					}
				}
				if (buildAndRunClicked) {
					buildAndRunClicked = false;
					if (RoutineChecks()) {
						Builder.BuildAndRunPlayer ();
					} 
				}
			}else {
				EditorGUILayout.HelpBox ("Switch To WebGL to build a Chrome App.", MessageType.Warning);
				if (GUILayout.Button ("Switch To WebGL")) {
					EditorUserBuildSettings.SwitchActiveBuildTarget (BuildTarget.WebGL);
				}
			}
		}

		void ScenesSectionGUI(){
			if (BeginSettingsBox (0, new GUIContent ("Scenes Overview"))) {
				GUI.changed=false;
				scenes = EditorBuildSettings.scenes;
				EditorGUILayout.BeginVertical (EditorStyles.helpBox);
				if (scenes.Length == 0) {
					EditorGUILayout.HelpBox ("No scenes found in the build. You can add scenes from the Build Settings.", MessageType.Warning);
				} else {
					for (int i = 0; i < scenes.Length; i++) {
						GUI.enabled = File.Exists (scenes [i].path);
						scenes[i].enabled = EditorGUILayout.ToggleLeft (scenes[i].path.Replace("Assets/",""),scenes[i].enabled);
					}
					if (GUI.changed) {
						EditorBuildSettings.scenes = scenes;
					}
				}
				EditorGUILayout.EndVertical ();

				GUI.enabled = true;	
			}
			EndSettingsBox ();
		}
		void BuildSettingsSectionGUI(){
			if (BeginSettingsBox (1, new GUIContent ("Build Settings"))) {
				EditorUserBuildSettings.development = EditorGUILayout.ToggleLeft ("Development Build*", EditorUserBuildSettings.development);
				GUI.enabled = EditorUserBuildSettings.development;
                if(EditorUserBuildSettings.development)
                {
                    EditorGUILayout.HelpBox("Note that WebGL development builds are much larger than release builds and should not be published.", MessageType.None);

                    EditorUserBuildSettings.webGLUsePreBuiltUnityEngine = EditorGUILayout.ToggleLeft("Use pre-built Engine", EditorUserBuildSettings.webGLUsePreBuiltUnityEngine);
                }
                EditorUserBuildSettings.connectProfiler = EditorGUILayout.ToggleLeft ("Autoconnect Profiler*", EditorUserBuildSettings.connectProfiler);
				GUI.enabled = true;
				ShowSharedNote ();
			}
			EndSettingsBox ();
		}
		void ResolutionAndPresentationSectionGUI(){
			if (BeginSettingsBox (2, new GUIContent ("Resolution and Presentation"))) {
				GUILayout.Label("Resolution", EditorStyles.boldLabel, new GUILayoutOption[0]);
				EditorGUI.BeginChangeCheck();
				PlayerSettings.defaultWebScreenWidth =EditorGUILayout.IntField ("Default Screen Width*", PlayerSettings.defaultWebScreenWidth, 
					new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck() && (PlayerSettings.defaultWebScreenWidth < 1))
				{
					PlayerSettings.defaultWebScreenWidth = 1;
				}
				EditorGUI.BeginChangeCheck();
				PlayerSettings.defaultWebScreenHeight =EditorGUILayout.IntField ("Default Screen Height*", PlayerSettings.defaultWebScreenHeight, 
					new GUILayoutOption[0]);
				if (EditorGUI.EndChangeCheck() && (PlayerSettings.defaultWebScreenHeight < 1))
				{
					PlayerSettings.defaultWebScreenHeight = 1;
				}

				PlayerSettings.runInBackground=EditorGUILayout.ToggleLeft("Run In Background*",PlayerSettings.runInBackground, new GUILayoutOption[0]);
				EditorGUILayout.Space();
				BuildSettings.Get.windowState=(WindowState)EditorGUILayout.EnumPopup("Initial window state",BuildSettings.Get.windowState, new GUILayoutOption[0]);
				BuildSettings.Get.frameless=EditorGUILayout.ToggleLeft("Frameless window",BuildSettings.Get.frameless, new GUILayoutOption[0]);
				//BuildSettings.Get.resizable=EditorGUILayout.ToggleLeft("Resizable",BuildSettings.Get.resizable, new GUILayoutOption[0]);
				//GUI.enabled = BuildSettings.Get.resizable;
				BuildSettings.Get.resizable = EditorGUILayout.BeginToggleGroup("Resizable",BuildSettings.Get.resizable);
				BuildSettings.Get.lockAspectRatio=EditorGUILayout.ToggleLeft("Lock Aspect Ratio",BuildSettings.Get.lockAspectRatio, new GUILayoutOption[0]);
				BuildSettings.Get.resizeConstrains = EditorGUILayout.BeginToggleGroup("Size Limits",BuildSettings.Get.resizeConstrains);
				BuildSettings.Get.minWidth = EditorGUILayout.IntField ("Min Width", BuildSettings.Get.minWidth);
				BuildSettings.Get.minHeight = EditorGUILayout.IntField ("Min Height", BuildSettings.Get.minHeight);
				BuildSettings.Get.maxWidth = EditorGUILayout.IntField ("Max Width", BuildSettings.Get.maxWidth);
				BuildSettings.Get.maxHeight = EditorGUILayout.IntField ("Max Height", BuildSettings.Get.maxHeight);
				EditorGUILayout.EndToggleGroup ();
				EditorGUILayout.EndToggleGroup ();
 				ShowSharedNote ();
			}
			EndSettingsBox ();
		}
		void IconSectionGUI(){
			if (BeginSettingsBox (3, new GUIContent ("Icon"))) {
				GUI.changed = false;
				BuildSettings.Get.overrideIcons = GUILayout.Toggle(BuildSettings.Get.overrideIcons, "Override Icons", new GUILayoutOption[0]);
				GUI.enabled = BuildSettings.Get.overrideIcons;
				for (int i = 0; i < Icons.iconSizes.Length; i++)
				{
					int num = Mathf.Min(96, Icons.iconSizes[i]);
					Rect rect = GUILayoutUtility.GetRect(64f, (float)(Mathf.Max(64, num) + 6));
					float num2 = Mathf.Min(rect.width, EditorGUIUtility.labelWidth + 4f + 64f + 6f + 96f);
					string text = Icons.iconSizes[i] + "x" + Icons.iconSizes[i];
					GUI.Label(new Rect(rect.x, rect.y, num2 - 96f - 64f - 12f, 20f), text);

					Texture2D iconForPlatformAtSize=null;

					if (BuildSettings.Get.overrideIcons) {
						BuildSettings.Get.icons [i] = (Texture2D)EditorGUI.ObjectField (new Rect (rect.x + num2 - 96f - 64f - 6f, rect.y, 64f, 64f), BuildSettings.Get.icons [i], typeof(Texture2D), false);
						iconForPlatformAtSize = Icons.GetBestFitIcon (Icons.iconSizes [i]);
					} else {
						Texture2D[] array = PlayerSettings.GetIconsForTargetGroup (0);
						if (array.Length > 0) {
							iconForPlatformAtSize= array [0];
						}
					}

					Rect position = new Rect(rect.x + num2 - 96f, rect.y, (float)num, (float)num);

					if (iconForPlatformAtSize != null)
					{
						GUI.DrawTexture(position, iconForPlatformAtSize);
					}
					else
					{
						GUI.Box(position, string.Empty);
					}
				} 
				GUI.enabled = true;	
			}
			EndSettingsBox ();
		}
		void SplashSectionGUI(){
			if (BeginSettingsBox (4, new GUIContent ("Splash Image"))) {
				EditorGUI.BeginDisabledGroup(!UnityEditorInternal.InternalEditorUtility.HasAdvancedLicenseOnBuildTarget(BuildTarget.WebGL));
				PlayerSettings.SplashScreen.show = EditorGUILayout.Toggle ("Show Unity Splash Screen*", PlayerSettings.SplashScreen.show, new GUILayoutOption[0]);
				EditorGUI.EndDisabledGroup();
				ShowSharedNote ();
			}
			EndSettingsBox ();
		}
		void OtherSectionGUI(){
			if (BeginSettingsBox (5, new GUIContent ("Other Settings"))) {
 				GUILayout.Label("Description", EditorStyles.boldLabel, new GUILayoutOption[0]);
				GUILayout.Label("132 characters (" + (132 - BuildSettings.Get.description.Length) + " left)");
				BuildSettings.Get.description=GUILayout.TextArea (BuildSettings.Get.description,132,GUILayout.MinHeight(50));
				EditorGUILayout.Space ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label("Short Name (12 chars)");
				BuildSettings.Get.shortName = GUILayout.TextField (BuildSettings.Get.shortName, 12);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.Space ();
				EditorGUILayout.HelpBox ("Check the \"Other Settings\" section of the WebGL Player Settings.", MessageType.Info); 
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if (GUILayout.Button(new GUIContent("Player Settings..."), GUILayout.Width(150)))
				{
					Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");

				}
				GUILayout.Space (10);
				EditorGUILayout.EndHorizontal ();

			}
			EndSettingsBox ();
		}
		void PublishSectionGUI(){
			if (BeginSettingsBox (6, new GUIContent ("Publishing Settings"))) {
				GUILayout.Label("WebGL", EditorStyles.boldLabel, new GUILayoutOption[0]);
				GUI.changed = false;
				WebGLMemorySize = EditorGUILayout.IntField ("WebGL Memory Size*", WebGLMemorySize);
				if (GUI.changed) {
					WebGLMemorySize = Mathf.Clamp(WebGLMemorySize, 0x10, 0x7ff);
                    PlayerSettings.WebGL.memorySize = WebGLMemorySize;
                }
                GUI.changed = false;
                WebGLExceptionSupport = (WebGLExceptionSupport)EditorGUILayout.EnumPopup("Enable Exceptions*", PlayerSettings.WebGL.exceptionSupport);
                if (WebGLExceptionSupport == WebGLExceptionSupport.Full)
                {
                    EditorGUILayout.HelpBox("Full exception support adds a lot of code to do sanity checks, which costs a lot of performance and browser memory. Only use this for debugging, and make sure to test in a 64-bit browser.", MessageType.Warning);
                }
                if (GUI.changed)
                {
                    PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport;
                }
                GUI.changed = false;
                WebGLCompressionFormat = (WebGLCompressionFormat)EditorGUILayout.EnumPopup("Compression Format", PlayerSettings.WebGL.compressionFormat);
                if (GUI.changed)
                {
                    PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat;
                }
                GUI.changed = false;
                WebGLDataCaching = EditorGUILayout.Toggle("Data caching*", WebGLDataCaching);
                if (GUI.changed)
                {
                    PlayerSettings.WebGL.dataCaching = WebGLDataCaching;
                }
                GUI.changed = false;
                WebGLDebugSymbols = EditorGUILayout.Toggle("Debug Symbols*", WebGLDebugSymbols);
                if (GUI.changed)
                {
                    PlayerSettings.WebGL.debugSymbols = WebGLDebugSymbols;
                }

                EditorGUILayout.Space ();
				GUILayout.Label("Permissions", EditorStyles.boldLabel, new GUILayoutOption[0]);
				scrollPos= EditorGUILayout.BeginScrollView (scrollPos, EditorStyles.helpBox, GUILayout.MinHeight(110f) );
				for (int i = 0; i < BuildSettings.Get.permissions.Length; i++) {
					BuildSettings.Get.permissions[i] = EditorGUILayout.ToggleLeft (((Permissions)i).Name(), BuildSettings.Get.permissions[i]);
				}
				EditorGUILayout.EndScrollView ();

				EditorGUILayout.Space ();
				GUILayout.Label("Packing", EditorStyles.boldLabel, new GUILayoutOption[0]);
				BuildSettings.Get.packExtension = EditorGUILayout.BeginToggleGroup ("Pack Extension", BuildSettings.Get.packExtension);
				BuildSettings.Get.updateExtension = EditorGUILayout.BeginToggleGroup ("Update", BuildSettings.Get.updateExtension);
				GUI.enabled = BuildSettings.Get.updateExtension;
				EditorGUILayout.BeginHorizontal ();
				EditorGUILayout.LabelField (".pem file : " + Path.GetFileName(BuildSettings.Get.pemFile));
				if(GUILayout.Button("Browse...")){
					string ret = EditorUtility.OpenFilePanel ("Select you .pem file", EditorUserBuildSettings.GetBuildLocation (BuildTarget.WebGL), "pem");
					if (!string.IsNullOrEmpty (ret) && File.Exists(ret)) {
						BuildSettings.Get.pemFile = ret;
					}
					if (!File.Exists (BuildSettings.Get.pemFile)) {
						BuildSettings.Get.pemFile = "";
					}
				}
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndToggleGroup ();
				EditorGUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace ();
				if(GUILayout.Button("Pack Extension...")){
					string path = EditorUtility.OpenFolderPanel ("Select App Folder", EditorUserBuildSettings.GetBuildLocation (BuildTarget.WebGL), "");
					if (Directory.Exists (path)) {
						ProgressHelper progress = new ProgressHelper ();
						progress.Reset (1);
						progress.Step("Building Player - Chrome App", "Packing Extension into .crx file");
						if (BuildSettings.Get.packExtension) {
							if (ChromeHelper.PackExtension (path)) {
								EditorUtility.RevealInFinder (path); 
							}
						}
						progress.Done ();
					} 
				}
				GUILayout.Space (10);
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndToggleGroup ();
 				ShowSharedNote ();
			}
			EndSettingsBox ();
		}

		private void ShowSharedNote()
		{
			GUILayout.Label("* Shared setting between multiple platforms.", EditorStyles.miniLabel, new GUILayoutOption[0]);
		}

		private bool BeginSettingsBox (int nr, GUIContent header)
		{
			GUI.changed = false;
			bool enabled = GUI.enabled;
			GUI.enabled = true;
			EditorGUILayout.BeginVertical (categoryBox, new GUILayoutOption[0]);
			Rect rect = GUILayoutUtility.GetRect (20f, 18f);
			rect.x += 3f;
			rect.width += 6f;
			bool flag = GUI.Toggle (rect, selectedSection == nr, header, GetStyle ("IN TitleText"));
			if (GUI.changed) {
				selectedSection = ((!flag) ? -1 : nr);
			}
			SectionAnimators [nr].target = flag;
			GUI.enabled = enabled;
			return EditorGUILayout.BeginFadeGroup (SectionAnimators [nr].faded);
		}
		private void EndSettingsBox ()
		{
			EditorGUILayout.EndFadeGroup ();
			EditorGUILayout.EndVertical ();
		}
		private static GUIStyle GetStyle (string styleName)
		{
			GUIStyle gUIStyle = GUI.skin.FindStyle (styleName);
			if (gUIStyle == null) {
				gUIStyle = EditorGUIUtility.GetBuiltinSkin (EditorSkin.Inspector).FindStyle (styleName);
			}
			if (gUIStyle == null) {
				Debug.LogError ("Missing built-in guistyle " + styleName);
			}
			return gUIStyle;
		}
		private void OnEnable ()
		{
			BuildSettings.Load ();
            WebGLMemorySize = PlayerSettings.WebGL.memorySize;
            WebGLExceptionSupport = PlayerSettings.WebGL.exceptionSupport;
            WebGLCompressionFormat = PlayerSettings.WebGL.compressionFormat;
            WebGLDataCaching = PlayerSettings.WebGL.dataCaching;
            WebGLDebugSymbols = PlayerSettings.WebGL.debugSymbols;
            for (int j = 0; j < SectionAnimators.Length; j++) {
				SectionAnimators [j] = new AnimBool (selectedSection == j, new UnityAction (base.Repaint));
			}
		}

		private void OnDisable ()
		{
			BuildSettings.Save ();
		}
		private void OnFocus ()
		{
            WebGLMemorySize = PlayerSettings.WebGL.memorySize;
            WebGLExceptionSupport = PlayerSettings.WebGL.exceptionSupport;
            WebGLCompressionFormat = PlayerSettings.WebGL.compressionFormat;
            WebGLDataCaching = PlayerSettings.WebGL.dataCaching;
            WebGLDebugSymbols = PlayerSettings.WebGL.debugSymbols;
        }

        public bool RoutineChecks(){
			if (BuildSettings.Get.packExtension && BuildSettings.Get.updateExtension && !File.Exists (BuildSettings.Get.pemFile)) {
				ShowNotification (new GUIContent (".pem file not found. Select file or disable the pack extension option."));
				return false;
			}
			if (BuildSettings.Get.shortName == "" && PlayerSettings.productName.Length > 0) {
				BuildSettings.Get.shortName = PlayerSettings.productName.Substring (0, Mathf.Min(PlayerSettings.productName.Length,12));
			}
			if (ChromeHelper.GetChromeLocation () == "") {
				if (EditorUtility.DisplayDialog ("Chrome not found", "We can't seem to find Google Chrome on your system. would you like to manually search for it?", "Browse...", "Cancel")) {
					string chromeExe = EditorUtility.OpenFilePanel ("Select chrome.exe", System.Environment.GetFolderPath (System.Environment.SpecialFolder.ProgramFiles), "exe");
					if (File.Exists (chromeExe) && Path.GetFileName (chromeExe) == "chrome.exe") {
						EditorPrefs.SetString ("ChromeExeLocation", chromeExe);
					} else {
						ShowNotification (new GUIContent ("Invalid chrome executable"));
						return false;
					}
				} else {
					return false;
				}
			}
			return true;
		}
	}
}
