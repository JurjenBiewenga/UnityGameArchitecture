using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu]
	public class UnityGameEvent : ScriptableObject
	{
		private List<UnityGameEventListener> Listeners = new List<UnityGameEventListener>();

		public void Invoke()
		{
			for (var i = Listeners.Count - 1; i >= 0; i--)
			{
				Listeners[i].Invoke();
			}
		}

		public void RegisterListener(UnityGameEventListener listener)
		{
			if (!Listeners.Contains(listener))
				Listeners.Add(listener);
		}

		public void RemoveListener(UnityGameEventListener listener)
		{
			Listeners.Remove(listener);
		}
	}
}