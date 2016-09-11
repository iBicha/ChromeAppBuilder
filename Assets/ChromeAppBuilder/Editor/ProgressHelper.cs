using UnityEditor;
using System;

namespace ChromeAppBuilder
{
	public class ProgressHelper
	{
 
		internal float m_CurrentBuildStep;

		internal float m_NumBuildSteps;

		internal Action CleanupCallback;

		public void SetCleanupCallback(Action callback){
			CleanupCallback = callback;
		}

		public float Advance ()
		{
			return (this.m_CurrentBuildStep += 1) / this.m_NumBuildSteps;
		}

		public float Get ()
		{
			return this.m_CurrentBuildStep / this.m_NumBuildSteps;
		}

		public float LastValue ()
		{
			return (this.m_CurrentBuildStep - 1) / this.m_NumBuildSteps;
		}

		public void Reset (float numSteps)
		{
			this.m_CurrentBuildStep = 0;
			this.m_NumBuildSteps = numSteps;
		}

		public void Show (string title, string message)
		{
			if (EditorUtility.DisplayCancelableProgressBar (title, message, this.Get ())) {
				Done ();
				throw new Exception ("Cancelled:" + title + " : " + message);
			}
		}

		public void Step (string title, string message)
		{
			this.Advance ();
			this.Show (title, message);
		}
		public void Done ()
		{
			if (CleanupCallback != null) {
				CleanupCallback ();
			}
			EditorUtility.ClearProgressBar ();
		}
	}
}