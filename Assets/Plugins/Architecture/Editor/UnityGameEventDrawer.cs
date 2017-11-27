using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
	[CustomEditor(typeof(UnityGameEvent))]
	public class UnityGameEventDrawer : Editor
	{
		public override void OnInspectorGUI()
		{
			GUI.enabled = Application.isPlaying;

			if (GUILayout.Button("Invoke event"))
			{
				UnityGameEvent unityGameEvent = target as UnityGameEvent;
				if(unityGameEvent != null)
					unityGameEvent.Invoke();
			}
		}
	}
}