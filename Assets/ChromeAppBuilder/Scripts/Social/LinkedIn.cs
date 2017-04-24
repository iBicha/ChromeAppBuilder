using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.Linq;

namespace Chrome.Social
{
	public class LinkedIn : OAuth<LinkedIn>
	{
		public event Action<string> OnAccessToken;
		public event Action<User> OnUserInfo;

		public string ClientSecret { get; set; }
		public string State { get; set; }

		public LinkedIn () : base ("LinkedIn")
		{
			AuthURL = "https://www.linkedin.com/oauth/v2/authorization";
			State = GetRandomizedString();
			SetScope ("r_basicprofile","r_emailaddress");
		}

		public LinkedIn (string ClientID, string ClientSecret, params string[] scope) : this ()
		{
			this.ClientID = ClientID;
			this.ClientSecret = ClientSecret;
			if(scope != null && scope.Length > 0) {
				SetScope (scope);
			}
		}


		private static System.Random random = new System.Random();
		public static string GetRandomizedString(int length = 32)
		{
			const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
			return new string(Enumerable.Repeat(chars, length)
				.Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public override string RequestUrl { 
			get {
				Dictionary<string, string> parameters = new Dictionary<string, string> ();
				parameters.Add ("response_type", "code");
				parameters.Add ("client_id", ClientID);
				parameters.Add ("redirect_uri", RedirectURL);
				parameters.Add ("state", State);
				if (scope.Count == 0) {
					SetScope ("r_basicprofile");
				}
				parameters.Add ("scope", string.Join (" ", scope.ToArray ()));
				return UrlEncode (AuthURL, parameters);
			} 
		}

		private string TokenRequestUrl { 
			get {
				Dictionary<string, string> parameters = new Dictionary<string, string> ();
				parameters.Add ("response_type", "code");
				parameters.Add ("client_id", ClientID);
				parameters.Add ("redirect_uri", RedirectURL);
				parameters.Add ("state", State);
				if (scope.Count == 0) {
					SetScope ("r_basicprofile");
				}
				parameters.Add ("scope", string.Join (" ", scope.ToArray ()));
				return UrlEncode (AuthURL, parameters);
			} 
		}


		public override void Authenticate (bool interactive)
		{
			LaunchFlow (interactive, (responsetUrl) => {
				string code = GetUrlParameter (responsetUrl, "code");
				string state = GetUrlParameter (responsetUrl, "state");
				if (state == this.State && !string.IsNullOrEmpty (code)) {
					Action<string> OnAccessTokenHandler = OnAccessToken;
					Action<User> OnUserInfoHandler = OnUserInfo;
					if((OnAccessTokenHandler != null) || (OnUserInfoHandler != null)) {
						GetAccessToken(code, (access_token) => {
							if(!string.IsNullOrEmpty (access_token)) {
								AccessToken = access_token;
								if(OnAccessTokenHandler != null) {
									OnAccessTokenHandler(AccessToken);
								}
								if(OnUserInfoHandler != null) {
									GetUserInfo("id", "email-address", "first-name", "last-name", "headline", "picture-urls::(original)");
								}
							}
						});
					}
				}
			}); 
		}

		private void GetAccessToken(string code, Action<string> callback){
			string url = "https://www.linkedin.com/oauth/v2/accessToken";
			Dictionary<string, string> parameters = new Dictionary<string, string> ();
			parameters.Add ("grant_type", "authorization_code");
			parameters.Add ("code", code);
			parameters.Add ("redirect_uri", RedirectURL);
			parameters.Add ("client_id", ClientID);
			parameters.Add ("client_secret", ClientSecret);
			WebRequest.Post(url,parameters, (res) => {
				if (res.isDone) {
					string resTstring = res.text;
					IDictionary<string, JToken> json = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(resTstring);
					if(json.ContainsKey("access_token")){
						if (callback != null) {
							callback ((string)json["access_token"]);
						}
					} else {
						if (callback != null) {
							callback (null);
						}
					}
				} else {
					if (callback != null) {
						callback (null);
					}
				}
			});
		}

		public void GetUserInfo (params string[] fields)
		{
			string baseUrl = "https://api.linkedin.com/v1/people/~:(" + string.Join (",", fields) + ")";
			Dictionary<string, string> headers = new Dictionary<string, string> ();
			headers.Add ("Authorization", "Bearer " + AccessToken);
			headers.Add ("x-li-format", "json");

			WebRequest.Get (baseUrl, (res) => {
				if (res.isDone) {
					LinkedInUser user = new LinkedInUser(res.text);
					WebRequest.GetTexture (user.PictureUrl, (res2) => {
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
			}, headers);
		}


		public override void Deauthorize ()
		{
			throw new NotImplementedException ();
		}

		public class LinkedInUser : User
		{
			public string Headline;

			//Nested Properties cannot be handled by annotations. let's do it manually, i guess.
			public LinkedInUser(string jsonString){
				IDictionary<string, JToken> json = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(jsonString);
				this.ID = (string)json["id"];
				this.Email = (string)json["emailAddress"];
				this.FirstName = (string)json["firstName"];
				this.LastName = (string)json["lastName"];
				this.PictureUrl = (string)json["pictureUrls"]["values"][0];
				this.Headline = (string)json["headline"];
			}
		}
	}
}