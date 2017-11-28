using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Architecture
{
    [System.Serializable]
    public class Event<T>
    {
        [SerializeField]
        private List<EventCall> _listeners = new List<EventCall>();

        private List<Action<T>> _runtimeListeners = new List<Action<T>>();

        public void Invoke(T param)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                EventCall eventCall = _listeners[i];
                if (_listeners[i].methodInfo != null)
                {
                    if (eventCall.CustomParameters.Length == 0)
                        _listeners[i].methodInfo.Invoke(eventCall.Object, new object[] {param});
                    else
                        _listeners[i].methodInfo.Invoke(eventCall.Object, eventCall.CustomParameters);
                }
            }

            for (int i = 0; i < _runtimeListeners.Count; i++)
            {
                _runtimeListeners[i].Invoke(param);
            }
        }

        public void AddListener(Action<T> action)
        {
            _runtimeListeners.Add(action);
        }

        public void RemoveListener(Action<T> action)
        {
            _runtimeListeners.Remove(action);
        }

        public void RemoveAllListeners()
        {
            _runtimeListeners.Clear();
        }
    }

    [System.Serializable]
    public class Event<T, X>
    {
        [SerializeField]
        private List<EventCall> _listeners = new List<EventCall>();

        private List<Action<T, X>> _runtimeListeners = new List<Action<T, X>>();

        public void Invoke(T param, X param2)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                EventCall eventCall = _listeners[i];
                if (_listeners[i].methodInfo != null)
                {
                    if (eventCall.CustomParameters.Length == 0)
                        _listeners[i].methodInfo.Invoke(eventCall.Object, new object[] {param, param2});
                    else
                        _listeners[i].methodInfo.Invoke(eventCall.Object, eventCall.CustomParameters);
                }
            }

            for (int i = 0; i < _runtimeListeners.Count; i++)
            {
                _runtimeListeners[i].Invoke(param, param2);
            }
        }

        public void AddListener(Action<T, X> action)
        {
            _runtimeListeners.Add(action);
        }

        public void RemoveListener(Action<T, X> action)
        {
            _runtimeListeners.Remove(action);
        }

        public void RemoveAllListeners()
        {
            _runtimeListeners.Clear();
        }
    }

    [System.Serializable]
    public class Event<T, X, Y>
    {
        [SerializeField]
        private List<EventCall> _listeners = new List<EventCall>();

        private List<Action<T, X, Y>> _runtimeListeners = new List<Action<T, X, Y>>();

        public void Invoke(T param, X param2, Y param3)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                EventCall eventCall = _listeners[i];
                if (_listeners[i].methodInfo != null)
                {
                    if (eventCall.CustomParameters.Length == 0)
                        _listeners[i].methodInfo.Invoke(eventCall.Object, new object[] {param, param2, param3});
                    else
                        _listeners[i].methodInfo.Invoke(eventCall.Object, eventCall.CustomParameters);
                }
            }

            for (int i = 0; i < _runtimeListeners.Count; i++)
            {
                _runtimeListeners[i].Invoke(param, param2, param3);
            }
        }

        public void AddListener(Action<T, X, Y> action)
        {
            _runtimeListeners.Add(action);
        }

        public void RemoveListener(Action<T, X, Y> action)
        {
            _runtimeListeners.Remove(action);
        }

        public void RemoveAllListeners()
        {
            _runtimeListeners.Clear();
        }
    }

    [System.Serializable]
    public class Event<T, X, Y, Z>
    {
        [SerializeField]
        private List<EventCall> _listeners = new List<EventCall>();

        private List<Action<T, X, Y, Z>> _runtimeListeners = new List<Action<T, X, Y, Z>>();

        public void Invoke(T param, X param2, Y param3, Z param4)
        {
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                EventCall eventCall = _listeners[i];
                if (_listeners[i].methodInfo != null)
                {
                    if (eventCall.CustomParameters.Length == 0)
                        _listeners[i].methodInfo.Invoke(eventCall.Object, new object[] {param, param2, param3, param4});
                    else
                        _listeners[i].methodInfo.Invoke(eventCall.Object, eventCall.CustomParameters);
                }
            }

            for (int i = 0; i < _runtimeListeners.Count; i++)
            {
                _runtimeListeners[i].Invoke(param, param2, param3, param4);
            }
        }

        public void AddListener(Action<T, X, Y, Z> action)
        {
            _runtimeListeners.Add(action);
        }

        public void RemoveListener(Action<T, X, Y, Z> action)
        {
            _runtimeListeners.Remove(action);
        }

        public void RemoveAllListeners()
        {
            _runtimeListeners.Clear();
        }
    }

    [System.Serializable]
    public class EventCall
    {
        public UnityEngine.Object Object;
        public string FunctionName;
        public UnityEngine.Object[] CustomParameters;
        private MethodInfo _methodInfo;

        public MethodInfo methodInfo
        {
            get
            {
                if (_methodInfo == null)
                {
                    Type t = Object.GetType();
                    _methodInfo = t.GetMethod(FunctionName);
                }
                return _methodInfo;
            }
        }
    }
}