using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;


namespace ChromeAppBuilder {
	public class Icons  {
		public static int[] iconSizes = new int[]{128,48,16};

		public static Texture2D GetBestFitIcon(int size){
			int index = -1;
			for (int i = 0; i <BuildSettings.Get.icons.Length; i++) {
				if (BuildSettings.Get.icons [i] != null && (index == -1 || size <= iconSizes [i])) {
					index = i;
				}
			}
			if (index == -1) {
				return null;
			} else {
				return BuildSettings.Get.icons [index];
			}
		}
		public static void CreateIcons (string path)
		{
			string iconsFolder = Path.Combine (path, "Icons");
			if (!Directory.Exists (iconsFolder)) {
				Directory.CreateDirectory (iconsFolder);
			}
			if (BuildSettings.Get.overrideIcons) {
				foreach (int iconSize in iconSizes) {
					CreateIcon(iconsFolder, iconSize);
				}
			} else {
				foreach (int iconSize in iconSizes) {
					CreateIconFromDefault (iconsFolder, iconSize);
				}
			}
		}
		public static void CreateIcon (string iconsFolder, int size){
			Texture2D tex = GetBestFitIcon (size);
			if (tex != null) {
				byte[] buffer = LoadTexture (tex, size);
				File.WriteAllBytes (Path.Combine (iconsFolder,"icon" + size.ToString () + ".png"), buffer);
			}
		}

		//Get The closest match to create the icon
		public static void CreateIconFromDefault (string iconsFolder, int size)
		{
			Texture2D[] icons = PlayerSettings.GetIconsForTargetGroup (0);
			if (icons.Length == 0) {
				return;
			}
			int MinDiff = icons [0].width - size;
			int index = 0;
			for (int i = 1; i < icons.Length; i++) {
				int diff = icons [i].width - size;
				if (diff < MinDiff && diff > 0) {
					MinDiff = diff;
					index = i;
				}
			}
			byte[] buffer = LoadTexture (icons [index], size);
			File.WriteAllBytes (Path.Combine (iconsFolder, "icon" + size.ToString () + ".png"), buffer);
		}

		/* 
		 * Why are we doing this?
		 * 1-because of the unity non-readable texture thingy, you need to set it, reimport,
		 * and make the user wait a bit longer, and then don't forget to reset the settings
		 * back.
		 * 2-because texture resizing in unity only resizes memory, and not the data.
		 * dear unity, if i wanted to do that, i would dispose of the texture, and
		 * create a new one with the desired size. Please don't be stupid.
		 */
		public static byte[] LoadTexture(Texture2D tex, int newSize = 0){
			bool wasReadable = SetTextureImporterFormat (tex, true);
			newSize = newSize > 0 ? newSize : Mathf.Min(tex.width, tex.height);
			Texture2D retTex = ResizeTexture (tex, ImageFilterMode.Average, newSize, newSize);
			SetTextureImporterFormat (tex, wasReadable);
			byte[] buffer = retTex.EncodeToPNG ();
			Object.DestroyImmediate (retTex);
			return buffer;
		}
		/* Resizing Textures, the way unity is supposed to handle in the first place.
		 * 3 Filter modes available, Average for best quality (blog post and testing says so)
		 * http://blog.collectivemass.com/2014/03/resizing-textures-in-unity/
		 */
		public enum ImageFilterMode : int {
			Nearest = 0,
			Biliner = 1,
			Average = 2
		} 
		public static Texture2D ResizeTexture(Texture2D pSource, ImageFilterMode pFilterMode, int newWidth, int newHeight){

			//*** Variables
			int i;

			//*** Get All the source pixels
			Color[] aSourceColor = pSource.GetPixels(0);
			Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);

			//*** Calculate New Size
			float xWidth = (float)newWidth;                     
			float xHeight = (float)newHeight;

			//*** Make New
			Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.ARGB32, false);

			//*** Make destination array
			int xLength = (int)xWidth * (int)xHeight;
			Color[] aColor = new Color[xLength];

			Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

			//*** Loop through destination pixels and process
			Vector2 vCenter = new Vector2();
			for(i=0; i<xLength; i++){

				//*** Figure out x&y
				float xX = (float)i % xWidth;
				float xY = Mathf.Floor((float)i / xWidth);

				//*** Calculate Center
				vCenter.x = (xX / xWidth) * vSourceSize.x;
				vCenter.y = (xY / xHeight) * vSourceSize.y;

				//*** Do Based on mode
				//*** Nearest neighbour (testing)
				if(pFilterMode == ImageFilterMode.Nearest){

					//*** Nearest neighbour (testing)
					vCenter.x = Mathf.Round(vCenter.x);
					vCenter.y = Mathf.Round(vCenter.y);

					//*** Calculate source index
					int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);

					//*** Copy Pixel
					aColor[i] = aSourceColor[xSourceIndex];
				}

				//*** Bilinear
				else if(pFilterMode == ImageFilterMode.Biliner){

					//*** Get Ratios
					float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
					float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);

					//*** Get Pixel index's
					int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
					int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
					int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
					int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));

					//*** Calculate Color
					aColor[i] = Color.Lerp(
						Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
						Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
						xRatioY
					);
				}

				//*** Average
				else if(pFilterMode == ImageFilterMode.Average){

					//*** Calculate grid around point
					int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
					int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
					int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
					int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

					//*** Loop and accumulate
					Color oColorTemp = new Color();
					float xGridCount = 0;
					for(int iy = xYFrom; iy < xYTo; iy++){
						for(int ix = xXFrom; ix < xXTo; ix++){

							//*** Get Color
							oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

							//*** Sum
							xGridCount++;
						}
					}

					//*** Average Color
					aColor[i] = oColorTemp / (float)xGridCount;
				}
			}

			//*** Set Pixels
			oNewTex.SetPixels(aColor);
			oNewTex.Apply();

			//*** Return
			return oNewTex;
		}
		public static bool SetTextureImporterFormat( Texture2D texture, bool isReadable)
		{
			if ( null == texture ) return false;

			string assetPath = AssetDatabase.GetAssetPath( texture );
			var tImporter = AssetImporter.GetAtPath( assetPath ) as TextureImporter;
			if ( tImporter != null )
			{
				//tImporter.textureType = TextureImporterType.Advanced;
				bool wasReadable = tImporter.isReadable;
				tImporter.isReadable = isReadable;
				AssetDatabase.ImportAsset( assetPath );
				AssetDatabase.Refresh();
				return wasReadable;
			}
			return false;
		}
	}
}