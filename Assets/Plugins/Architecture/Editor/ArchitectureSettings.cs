using System.Collections;
using System.Collections.Generic;
using Bodybuilder.Editor.Drawers;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class ArchitectureSettings : ScriptableObject
{
    public string sourceGenerationPath = "Scripts/SO/";
    public string namespaceName = "Architecture";
    
    public SerializableSystemType[] variableTypes;
    public SerializableSystemType[] runtimeSetTypes;
    public SerializableSystemType[] staticSetTypes;
    public SerializableSystemType[] eventTypes;

    public static string settingsPath = "Assets/Plugins/Architecture/Editor/Resources/Settings.asset";

    public static ArchitectureSettings GetSettings()
    {
        ArchitectureSettings settings = AssetDatabase.LoadAssetAtPath<ArchitectureSettings>(settingsPath);
        return settings;
    }
}
