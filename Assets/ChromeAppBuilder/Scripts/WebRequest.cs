/*This is a helper class to wrap around the WWW class and the coroutine system 
* so it can be called without a MonoBehaviour or a game object
* basically we will create one, and destroy it when we're done.
* Of course other solutions exists, but this is a way to use unity's stock option.
* Note that this can be replaced with the newer UnityWebRequest
* https://docs.unity3d.com/Manual/UnityWebRequest.html
*/

using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.Networking;

public class WebRequest : MonoBehaviour {
	public static void Get(string url, Action<DownloadHandler> callback, Dictionary<string, string> headers=null){
		GameObject go = new GameObject ("{WebRequest}");
		WebRequest req = go.AddComponent<WebRequest> ();
		req.OnResponse = callback;

		req.uRequest = UnityWebRequest.Get (url);
		if(headers != null) {
			foreach (string header in headers.Keys) {
				req.uRequest.SetRequestHeader (header, headers [header]);
			}
		}
		req.Send();
	}

	UnityWebRequest uRequest;
	private Action<DownloadHandler> OnResponse;

	private void Send () {
		Destroy (gameObject, 30); //30 seconds timeout
		StartCoroutine (GetResponse());
	}
	

	private IEnumerator GetResponse(){
		yield return uRequest.Send();
		Action<DownloadHandler> handler = OnResponse;
		if (handler != null) {
			handler (uRequest.downloadHandler);
		}
		Destroy (gameObject);
	}
}
