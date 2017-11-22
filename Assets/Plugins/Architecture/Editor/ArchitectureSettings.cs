using System.Collections;
using System.Collections.Generic;
using Bodybuilder.Editor.Drawers;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class ArchitectureSettings : ScriptableObject
{
    public string sourceGenerationPath = "Scripts/SO/";
    
    public SerializableSystemType[] variableTypes;
    public SerializableSystemType[] setTypes;
    public SerializableSystemType[] eventTypes;

    public static string settingsPath = "Assets/Plugins/Architecture/Editor/Resources/Settings.asset";

    public static ArchitectureSettings GetSettings()
    {
        ArchitectureSettings settings = AssetDatabase.LoadAssetAtPath<ArchitectureSettings>(settingsPath);
        return settings;
    }
}
