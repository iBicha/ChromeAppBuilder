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
		public event Action<string> OnAccessToken;
		public event Action<User> OnUserInfo;

		public Facebook () : base ("Facebook")
		{
			AuthURL = "https://www.facebook.com/dialog/oauth";
			SetScope ("public_profile", "email");
		}

		public Facebook (string ClientID, params string[] scope) : this ()
		{
			this.ClientID = ClientID;
			if(scope != null && scope.Length > 0) {
				SetScope (scope);
			}
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
			return null;
		}

		public override void Authenticate (bool interactive)
		{
			LaunchFlow (interactive, (responsetUrl) => {
				string access_token = GetUrlParameter (responsetUrl, "access_token");
				Action<string> OnAccessTokenHandler = OnAccessToken;
				Action<User> OnUserInfoHandler = OnUserInfo;
				if((OnAccessTokenHandler != null) || (OnUserInfoHandler != null)) {
					if(!string.IsNullOrEmpty (access_token)) {
						AccessToken = access_token;
						if(OnAccessTokenHandler != null) {
							OnAccessTokenHandler(AccessToken);
						}
						if(OnUserInfoHandler != null) {
							GetUserInfo ("id", "email", "first_name", "last_name", "picture.width(320).height(320)");
						}
					}
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
					FacebookUser user = new FacebookUser(res.text);
					WebRequest.GetTexture(user.PictureUrl, (res2) => {
						if (res2.isDone) {
							user.Picture = ((DownloadHandlerTexture)res2).texture;
						} else {
							//well, profile picture wasn't downloaded. no biggy.
						}
						Action<User> OnUserInfoHandler = OnUserInfo;
						if(OnUserInfoHandler != null) {
							OnUserInfoHandler (user);
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

		public class FacebookUser : User
		{
			//Nested Properties cannot be handled by annotations. let's do it manually, i guess.
			public FacebookUser(string jsonString){
				IDictionary<string, JToken> json = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(jsonString);
				this.ID = (string)json["id"];
				this.Email = (string)json["email"];
				this.FirstName = (string)json["first_name"];
				this.LastName = (string)json["last_name"];
				this.PictureUrl = (string)json["picture"]["data"]["url"];
			}

		}
	}
}
