using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
    [CustomEditor(typeof(BaseVariable), true)]
    [CanEditMultipleObjects]
    public class VariableDrawer : Editor
    {
        private SerializedProperty persistentProperty;
        private SerializedProperty valueProperty;
        private SerializedProperty defaultValueProperty;

        private void OnEnable()
        {
            persistentProperty = serializedObject.FindProperty("Persistent");
            valueProperty = serializedObject.FindProperty("Value");
            defaultValueProperty = serializedObject.FindProperty("DefaultValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(persistentProperty);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
                Repaint();    
            }
            
            EditorGUI.BeginChangeCheck();
            if (persistentProperty.boolValue || Application.isPlaying)
                EditorGUILayout.PropertyField(valueProperty, true);
            else
                EditorGUILayout.PropertyField(defaultValueProperty, true);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}