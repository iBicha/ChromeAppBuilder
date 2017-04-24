using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace Chrome.Social
{
	public class User
	{
		public string ID;
		public string Email;
		public string FirstName;
		public string LastName;
		[JsonIgnore]
		public Texture Picture;
		public string PictureUrl;

		public override string ToString ()
		{
			return JsonConvert.SerializeObject (this, Formatting.Indented);
		}
	}
}