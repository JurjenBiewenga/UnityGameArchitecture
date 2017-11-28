using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
    public static class Utils
    {
        public static string GetTypeName(Type type)
        {
            if (type == typeof(float))
            {
                return "Float";
            }
            else if (type == typeof(int))
            {
                return "Int";
            }
            else if (type == typeof(bool))
            {
                return "Bool";
            }

            return type.Name;
        }

        public static string GetPropertyType(SerializedProperty property)
        {
            var type = property.type;
            var match = Regex.Match(type, @"PPtr<\$(.*?)>");
            if (match.Success)
                type = match.Groups[1].Value;
            return type;
        }

        public static void GenerateAndSetVariable(SerializedProperty useConstantProperty, SerializedProperty variableProperty, string path)
        {
            useConstantProperty.boolValue = false;

            Directory.CreateDirectory(Path.Combine(Application.dataPath,
                                                   Path.GetDirectoryName(path)));
            string assetDatabasePath = Path.Combine("Assets", path);
            var assetAtPath =
                AssetDatabase.LoadAssetAtPath(assetDatabasePath, typeof(object));
            if (assetAtPath != null)
            {
                if (assetAtPath.GetType().Name == Utils.GetPropertyType(variableProperty))
                    return;
            }

            assetDatabasePath = AssetDatabase.GenerateUniqueAssetPath(assetDatabasePath);

            var so = ScriptableObject.CreateInstance(Utils.GetPropertyType(variableProperty));
            AssetDatabase.CreateAsset(so, assetDatabasePath);

            variableProperty.objectReferenceValue = so;

            useConstantProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}