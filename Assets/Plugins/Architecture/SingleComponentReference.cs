using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		}
	}
}