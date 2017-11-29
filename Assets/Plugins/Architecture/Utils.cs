using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
            else if (type == typeof(short))
            {
                return "Short";
            }
            else if (type == typeof(ushort))
            {
                return "UShort";
            }
            else if (type == typeof(int))
            {
                return "Int";
            }
            else if (type == typeof(uint))
            {
                return "UInt";
            }
            else if (type == typeof(ulong))
            {
                return "ULong";
            }
            else if (type == typeof(long))
            {
                return "Long";
            }
            else if (type == typeof(bool))
            {
                return "Bool";
            }

            return type.Name;
        }
        
        public static string GetImplicitConversionTypeName(Type type)
        {
            if (type == typeof(float))
            {
                return "float";
            }
            else if (type == typeof(short))
            {
                return "short";
            }
            else if (type == typeof(ushort))
            {
                return "ushort";
            }
            else if (type == typeof(int))
            {
                return "int";
            }
            else if (type == typeof(uint))
            {
                return "uint";
            }
            else if (type == typeof(ulong))
            {
                return "ulong";
            }
            else if (type == typeof(long))
            {
                return "long";
            }
            else if (type == typeof(bool))
            {
                return "bool";
            }

            return type.Name;
        }

        public static string GetPropertyType(SerializedProperty property)
        {
            string type = property.type;
            Match match = Regex.Match(type, @"PPtr<\$(.*?)>");
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
            Object assetAtPath =
                AssetDatabase.LoadAssetAtPath(assetDatabasePath, typeof(object));
            if (assetAtPath != null)
            {
                if (assetAtPath.GetType().Name == Utils.GetPropertyType(variableProperty))
                    return;
            }

            assetDatabasePath = AssetDatabase.GenerateUniqueAssetPath(assetDatabasePath);

            ScriptableObject so = ScriptableObject.CreateInstance(Utils.GetPropertyType(variableProperty));
            AssetDatabase.CreateAsset(so, assetDatabasePath);

            variableProperty.objectReferenceValue = so;

            useConstantProperty.serializedObject.ApplyModifiedProperties();
        }
    }
}