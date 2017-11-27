using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Architecture
{
	[CreateAssetMenu]
	public class UnityGameEvent : ScriptableObject
	{
		private List<UnityGameEventListener> _listeners = new List<UnityGameEventListener>();

		public void Invoke()
		{
			for (var i = _listeners.Count - 1; i >= 0; i--)
			{
				_listeners[i].Invoke();
			}
		}

		public void RegisterListener(UnityGameEventListener listener)
		{
			if (!_listeners.Contains(listener))
				_listeners.Add(listener);
		}

		public void RemoveListener(UnityGameEventListener listener)
		{
			_listeners.Remove(listener);
		}
	}
}