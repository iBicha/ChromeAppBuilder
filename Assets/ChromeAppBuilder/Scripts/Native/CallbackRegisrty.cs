#if UNITY_CHROME && UNITY_WEBGL && !UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using AOT;
using System;
using System.Collections.Generic;

namespace Chrome.Native
{

	public class CallbackRegisrty
	{
		private static int callbackId = 1;
		private static Dictionary<int, object> callbackRegistry = new Dictionary<int, object> ();

		public static int RegisterCallback (object callback)
		{
			while (callbackRegistry.ContainsKey (callbackId)) {
				callbackId++;
			}
			callbackRegistry.Add (callbackId, callback);
			return callbackId;
		}

		public static T GetCallback <T> (int id, bool keepAlive = false)
		{
			T callback = default(T);
			if (callbackRegistry.ContainsKey (id)) {
				callback = (T)callbackRegistry [id];
				if (!keepAlive) {
					callbackRegistry.Remove (id);
				}
			}
			return callback;
		}

	}

}
#endif