using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Architecture
{
	public abstract class SingleComponentReference<T> : ScriptableObject where T : Component
	{
		public T Value;
		public bool FindOnEnable;
		public bool CreateIfNotFound;

		protected virtual void OnEnable()
		{
			if (Application.isPlaying)
			{
				if (FindOnEnable)
				{
					Value = FindObjectOfType<T>();
					if (Value == null && CreateIfNotFound)
					{
						GameObject go = new GameObject(typeof(T).Name);
						Value = go.AddComponent<T>();
					}
				}
			}
#if UNITY_EDITOR
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				if (FindOnEnable)
				{
					Value = FindObjectOfType<T>();
					if (Value == null && CreateIfNotFound)
					{
						GameObject go = new GameObject(typeof(T).Name);
						Value = go.AddComponent<T>();
					}
				}
			}
#endif
		}
	}
}