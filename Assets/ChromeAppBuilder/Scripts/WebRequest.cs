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

public class WebRequest : MonoBehaviour {
	/// <summary>
	///     Retrieves information about the specified window. 
	///     The function also retrieves the value at a specified offset into the extra window memory.
	///     From <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms633585(v=vs.85).aspx">this</see> MSDN-Link.
	///     AHref <a href="http://stackoverflow.com">here</a>.
	///     see-href <see href="http://stackoverflow.com">here</see>.
	/// </summary>
	/// <param name="hwnd"></param>
	/// <param name="index"></param>
	/// <returns>
	///     Testlink in return: <a href="http://stackoverflow.com">here</a>
	/// </returns>
	public static void Get(string url, Action<WWW> callback, Dictionary<string, string> headers=null){
		GameObject go = new GameObject ("{WebRequest}");
		WebRequest req = go.AddComponent<WebRequest> ();
		req.url = url;
		req.headers = headers;
		req.OnResponse = callback;
		req.Send();
	}

	private Action<WWW> OnResponse;
	private string url;
	private Dictionary<string, string> headers;

	private void Send () {
		Destroy (gameObject, 30); //30 seconds timeout
		StartCoroutine (GetResponse());
	}
	

	private IEnumerator GetResponse(){
		WWW req = new WWW(url,null,headers);
		yield return req;
		Action<WWW> handler = OnResponse;
		if (handler != null) {
			handler (req);
		}
		Destroy (gameObject);
	}
}
