using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
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
			EditorGUILayout.PropertyField(persistentProperty);
			if(persistentProperty.boolValue)
				EditorGUILayout.PropertyField(defaultValueProperty);
			else
				EditorGUILayout.PropertyField(valueProperty);
			serializedObject.ApplyModifiedProperties();
		}
	}
}