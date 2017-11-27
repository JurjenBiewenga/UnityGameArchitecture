using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
    public class GameEventDrawer : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Invoke event"))
            {
                ScriptableObject gameEvent = target as ScriptableObject;
                if (gameEvent != null)
                {
                    Type t = gameEvent.GetType();
                    var method = t.GetMethod("InvokeWithTestingParams");
                    method.Invoke(gameEvent, null);
                }
            }
        }
    }
}