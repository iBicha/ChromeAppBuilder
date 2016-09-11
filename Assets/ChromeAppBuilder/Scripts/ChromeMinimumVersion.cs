using UnityEngine;
using System.Collections;
namespace Chrome{
	class ChromeMinimumVersion : System.Attribute {
		public int minimumVersion=0;
		public ChromeMinimumVersion(int min){
			minimumVersion = min;
		}
	}
}