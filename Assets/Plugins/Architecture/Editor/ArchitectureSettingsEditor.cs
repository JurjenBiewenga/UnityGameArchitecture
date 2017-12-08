﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
	[CustomEditor(typeof(ArchitectureSettings), true)]
	[CanEditMultipleObjects]
	public class ArchitectureSettingsEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			if (GUILayout.Button("Generate"))
			{
				CodeGen.Generate();
			}
		}
	}
}