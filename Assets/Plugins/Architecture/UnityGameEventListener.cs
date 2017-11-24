using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Architecture
{
	public class UnityGameEventListener : MonoBehaviour
	{
		public UnityGameEvent UnityGameEvent;
		public UnityEvent Response;

		public void Invoke()
		{
			Response.Invoke();
		}

		protected virtual void OnEnable()
		{
			UnityGameEvent.RegisterListener(this);
		}
		
		protected virtual void OnDisable()
		{
			UnityGameEvent.RemoveListener(this);
		}
	}
}