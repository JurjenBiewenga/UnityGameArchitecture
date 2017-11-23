using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
    [CreateAssetMenu]
    public class ArchitectureSettings : ScriptableObject
    {
        public string sourceGenerationPath = "Scripts/SO/";
        public string namespaceName = "Architecture";

        [TypePicker]
        public SerializableSystemType[] variableTypes;
        [TypePicker]
        public SerializableSystemType[] runtimeSetTypes;
        [TypePicker]
        public SerializableSystemType[] staticSetTypes;
        [TypePicker]
        public SerializableSystemType[] eventTypes;
        [TypePicker(typeof(Component))]
        public SerializableSystemType[] singleComponentReferenceTypes;

        public static string settingsPath = "Assets/Plugins/Architecture/Editor/Resources/Settings.asset";

        public static ArchitectureSettings GetSettings()
        {
            ArchitectureSettings settings = AssetDatabase.LoadAssetAtPath<ArchitectureSettings>(settingsPath);
            return settings;
        }
    }
}