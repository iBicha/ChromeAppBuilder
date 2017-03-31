using System.Collections.Generic;
using UnityEditor;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChromeAppBuilder
{
	public class Manifest
	{
		public string name;
		public string author;
		public string description;
		public string short_name;
		public string key;
		public string version;
		public string version_name;
		public int manifest_version;
		public bool offline_enabled;
		public Dictionary<string, object> app;
		public Dictionary<string, object> icons;
		public Dictionary<string, object> requirements;
		public List<string> permissions;

		//Predefined Properties
		public Manifest ()
		{
			const string staticProperties = @"{
				""manifest_version"": 2,
				""offline_enabled"": true,
				""app"": {
					""background"": {
						""scripts"": [
							""Background.js""
						]
					}
				},
				""requirements"": {
					""3D"": {
						""features"": [
							""webgl""
						]
					}
				},
				""permissions"": []
			}";
			JsonConvert.PopulateObject (staticProperties, this);
		}

		public static string GenerateManifest (string path)
		{
			Manifest manifest = new Manifest ();
			manifest.name = PlayerSettings.productName;
			manifest.author = PlayerSettings.companyName;
			manifest.description = BuildSettings.Get.description;
			manifest.short_name = BuildSettings.Get.shortName;
			manifest.version = PlayerSettings.bundleVersion;
			manifest.version_name = PlayerSettings.bundleVersion;
			string iconsFolder = Path.Combine (path, "Icons");
			if (Directory.Exists (iconsFolder)) {
                Dictionary<string, object> icons = new Dictionary<string, object>();
                int[] iconSizes = new int[]{ 16, 48, 128 };
				foreach (int size in iconSizes) {
					if (File.Exists (Path.Combine (iconsFolder, "icon" + size.ToString () + ".png"))) {
                        icons[size.ToString ()] = "Icons/icon" + size.ToString () + ".png";
					}
				}
                if (icons.Count > 0)
                {
                    manifest.icons = icons;
                }
			}
			for (int i = 0; i < (int)Permissions.Count; i++) {
                if (i < BuildSettings.Get.permissions.Length && BuildSettings.Get.permissions[i])
                {
                    Permissions perm = (Permissions)i;
					if (!manifest.permissions.Contains (perm.ToPermission ())) {
						manifest.permissions.Add (perm.ToPermission ());
					}
					string[] extras = perm.ToPermissionsExtra ();
					if (extras != null) {
						foreach (string extra in extras) {
							if (!manifest.permissions.Contains (extra)) {
								manifest.permissions.Add (extra);
							}	
						}
					}
				} 
			}
            string[] extraPermissions = BuildSettings.Get.permissionsExtra.Split(new string[] { "\r\n", "\n" },System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var extraPermission in extraPermissions)
            {
                if (!manifest.permissions.Contains(extraPermission))
                {
                    manifest.permissions.Add(extraPermission);
                }
            }
            JsonSerializerSettings settings = new JsonSerializerSettings () {
				Formatting = Formatting.Indented,
				NullValueHandling = NullValueHandling.Ignore
			};
			return JsonConvert.SerializeObject (manifest, settings);
		}

		public bool ShouldSerializeshort_name ()
		{
			return !string.IsNullOrEmpty (short_name);
		}

		public bool ShouldSerializekey ()
		{
			return !string.IsNullOrEmpty (key);
		}

		public static void CreateManifestFile (string path)
		{
			string manifestPath = Path.Combine (path, "manifest.json");
			File.WriteAllText (manifestPath, GenerateManifest (path));
		}

		public static void AddKeyToManifest (string path, string key)
		{
			if (string.IsNullOrEmpty (key) || !File.Exists (path)) {
				return;
			}
			string manifestContent = File.ReadAllText (path);
			JObject ob = JObject.Parse (manifestContent);
            ob["key"] = key;
			manifestContent = ob.ToString ();
			File.WriteAllText (path, manifestContent);
		}
	}
}