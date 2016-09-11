using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Chrome.Social
{
	public class Instagram : OAuth<Instagram>
	{

		public event Action<User> OnConnect;

		public Instagram () : base ("Instagram")
		{
			AuthURL = "https://instagram.com/oauth/authorize";
			ClientID = "<CLIENT_ID_HERE>";
		}

		//Instagram returns Url fragments.
		protected override string GetUrlParameter (string url, string key)
		{
			url = url.Substring (url.IndexOf ("?") + 1);
			string[] arrayParams = url.Split ('&', '#'); //here it is.
			Dictionary<string, string> parameters = new Dictionary<string, string> ();
			foreach (string paramPair in arrayParams) {
				string[] keyValue = paramPair.Split ('=');
				if (keyValue.Length == 2) {
					parameters.Add (keyValue [0], keyValue [1]);
				}
			}
			if (parameters.ContainsKey (key)) {
				return parameters [key];
			}
			return "";
		}

		public override void Authenticate (bool interactive, Action callback = null)
		{
			LaunchFlow (interactive, (responsetUrl) => {
				Debug.Log ("responsetUrl:" + responsetUrl);
				string token = GetUrlParameter (responsetUrl, "access_token");
				if (token != "") {
					AccessToken = token;
					Debug.Log ("AccessToken:" + AccessToken);
					if (callback != null) {
						callback ();
					}
					GetUserInfo();
				}
			}); 
		}

		public void GetUserInfo ()
		{
			string baseUrl = "https://api.instagram.com/v1/users/self";
			Dictionary<string, string> parameters = new Dictionary<string, string> ();
			parameters.Add ("access_token", AccessToken);
			WebRequest.Get (UrlEncode (baseUrl, parameters), (res) => {
				if (res.isDone) {
					User user = new User(res.text);
					WebRequest.Get (user.PictureUrl, (res2) => {
						if (res2.isDone) {
							user.Picture = res2.texture;
						} else {
							//well, profile picture wasn't downloaded. no biggy.
						}
						Action<User> handler = OnConnect;
						if (handler != null) {
							handler (user);
						}
					});

				} else {
					//something went wrong here.
				}
			});
		}

		public override void Deauthorize ()
		{
			throw new NotImplementedException ();
		}

		public class User
		{
			//Nested Properties cannot be handled by annotations. let's do it manually, i guess.
			public User(string jsonString){
				IDictionary<string, JToken> json = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(jsonString);
				if((int)(json["meta"]["code"])==200){
					this.ID = (string)json["data"]["id"];
					this.Username = (string)json["data"]["username"];
					this.Bio = (string)json["data"]["bio"];
					this.Website = (string)json["data"]["website"];
					this.FullName = (string)json["data"]["full_name"];
					this.PictureUrl = (string)json["data"]["profile_picture"];
					this.Media = (int)json["data"]["counts"]["media"];
					this.FollowedBy = (int)json["data"]["counts"]["followed_by"];
					this.Follows = (int)json["data"]["counts"]["follows"];
				}
			}
			public string ID;
			public string Username;
			public string Bio;
			public string Website;
			public string FullName;
			[JsonIgnore]
			public Texture Picture;
			public string PictureUrl;

			public int Media;
			public int FollowedBy;
			public int Follows;


			public override string ToString ()
			{
				return JsonConvert.SerializeObject (this, Formatting.Indented);
			}
		}
	}
}