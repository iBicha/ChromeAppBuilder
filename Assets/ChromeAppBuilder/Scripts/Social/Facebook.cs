using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using UnityEngine.Networking;

namespace Chrome.Social
{
	public class Facebook : OAuth<Facebook>
	{

		public event Action<User> OnConnect;

		public Facebook () : base ("Facebook")
		{
			AuthURL = "https://www.facebook.com/dialog/oauth";
			ClientID = "<CLIENT_ID_HERE>";
			SetScope ("public_profile", "email", "user_friends");
		}

		public override string RequestUrl {
			get {
				Dictionary<string, string> parameters = new Dictionary<string, string> ();
				parameters.Add ("client_id", ClientID);
				parameters.Add ("redirect_uri", RedirectURL);
				//comma seperated.
				parameters.Add ("scope", string.Join (",", scope.ToArray ()));
				parameters.Add ("response_type", "token");
				return UrlEncode (AuthURL, parameters);
			}
		}
		//And since url responses from facebook have weird hashes, we override this.
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
				string token = GetUrlParameter (responsetUrl, "access_token");
				if (token != "") {
					AccessToken = token;
					//we got a token, we might as well do a callback
					if (callback != null) {
						callback ();
					}
					GetUserInfo ("id", "email", "first_name", "last_name", "picture.width(320).height(320)");
				}
			}); 
		}

		public void GetUserInfo (params string[] fields)
		{
			string baseUrl = "https://graph.facebook.com/me";
			Dictionary<string, string> parameters = new Dictionary<string, string> ();
			parameters.Add ("access_token", AccessToken);
			if (fields.Length > 0) {
				parameters.Add ("fields", string.Join (",", fields));
			}

			WebRequest.Get (UrlEncode (baseUrl, parameters), (res) => {
				if (res.isDone) {
					User user = new User(res.text);
					WebRequest.Get (user.PictureUrl, (res2) => {
						if (res2.isDone) {
							user.Picture = ((DownloadHandlerTexture)res2).texture;
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
				this.ID = (string)json["id"];
				this.Email = (string)json["email"];
				this.FirstName = (string)json["first_name"];
				this.LastName = (string)json["last_name"];
				this.PictureUrl = (string)json["picture"]["data"]["url"];
			}
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
}
