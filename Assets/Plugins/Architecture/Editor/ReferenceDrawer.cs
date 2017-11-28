using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Architecture
{
    public class ReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
            GUIStyle style = skin.GetStyle("PaneOptions");

            var constantProperty = property.FindPropertyRelative("ConstantValue");
            var useConstantProperty = property.FindPropertyRelative("UseConstant");
            var variableProperty = property.FindPropertyRelative("Variable");
            Rect pos = position;
            pos.width = 15;
            pos.x += EditorGUIUtility.labelWidth - 17;
            pos.y += 4;

            if (GUI.Button(pos, "", style))
            {
                GenerateMenu(property, useConstantProperty, variableProperty).DropDown(pos);
            }

            if (useConstantProperty.boolValue)
                EditorGUI.PropertyField(position, constantProperty, new GUIContent(property.displayName));
            else
            {
                Rect pos1 = new Rect(position.x, position.y, (position.width + EditorGUIUtility.labelWidth) / 2, position.height);
                Rect pos2 = new Rect(position.x + pos1.width, position.y, position.width - pos1.width, position.height);
                if (variableProperty.objectReferenceValue != null)
                {
                    EditorGUI.PropertyField(pos1, variableProperty, new GUIContent(property.displayName));
                    if (variableProperty.objectReferenceValue != null)
                    {
                        SerializedObject variable = new SerializedObject(variableProperty.objectReferenceValue);
                        SerializedProperty persistentProperty = variable.FindProperty("Persistent");
                        SerializedProperty valueProperty = variable.FindProperty("Value");
                        SerializedProperty defaultValueProperty = variable.FindProperty("DefaultValue");
                        if (persistentProperty.boolValue)
                        {
                            EditorGUI.PropertyField(pos2, valueProperty, new GUIContent());
                        }
                        else
                        {
                            EditorGUI.PropertyField(pos2, defaultValueProperty, new GUIContent());
                        }
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, variableProperty, new GUIContent(property.displayName));
                }
            }
        }

        public static string GetPropertyType(SerializedProperty property)
        {
            var type = property.type;
            var match = Regex.Match(type, @"PPtr<\$(.*?)>");
            if (match.Success)
                type = match.Groups[1].Value;
            return type;
        }

        public GenericMenu GenerateMenu(SerializedProperty property, SerializedProperty useConstantProperty, SerializedProperty variableProperty)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Use Constant"), useConstantProperty.boolValue, () =>
                                                                                        {
                                                                                            useConstantProperty.boolValue = !useConstantProperty.boolValue;
                                                                                            useConstantProperty.serializedObject.ApplyModifiedProperties();
                                                                                        });
            menu.AddSeparator("");
            GameObject go = (property.serializedObject.targetObject as GameObject);
            string path = "";
            if (go.scene.IsValid())
            {
                path = string.Format(@"Scriptable Objects\{0}\{1}\{2}.asset", go.scene.name, property.serializedObject.targetObject.name,
                                            property.displayName);
            }
            else
            {
                path = string.Format(@"Scriptable Objects\{0}\{1}\{2}.asset", "Prefabs", property.serializedObject.targetObject.name,
                                            property.displayName);
            }

            menu.AddItem(new GUIContent("Create variable at " + path), false, () =>
                                                                              {
                                                                                  useConstantProperty.boolValue = false;

                                                                                  Directory.CreateDirectory(Path.Combine(Application.dataPath,
                                                                                                                         Path.GetDirectoryName(path)));
                                                                                  string assetDatabasePath = Path.Combine("Assets", path);
                                                                                  var assetAtPath =
                                                                                      AssetDatabase.LoadAssetAtPath(assetDatabasePath, typeof(object));
                                                                                  if (assetAtPath != null)
                                                                                  {
                                                                                      if (assetAtPath.GetType().Name == GetPropertyType(variableProperty))
                                                                                          return;
                                                                                  }

                                                                                  assetDatabasePath = AssetDatabase.GenerateUniqueAssetPath(assetDatabasePath);

                                                                                  var so = ScriptableObject.CreateInstance(GetPropertyType(variableProperty));
                                                                                  AssetDatabase.CreateAsset(so, assetDatabasePath);

                                                                                  variableProperty.objectReferenceValue = so;

                                                                                  useConstantProperty.serializedObject.ApplyModifiedProperties();
                                                                              });

            string[] assetGuids = AssetDatabase.FindAssets("t:" + GetPropertyType(variableProperty));
            if (assetGuids.Length > 0)
            {
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("Load existing"));
            }
            for (int i = 0; i < assetGuids.Length; i++)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(assetGuids[i]);
                menu.AddItem(new GUIContent(assetPath), false, () =>
                                                               {
                                                                   Object asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(Object));
                                                                   variableProperty.objectReferenceValue = asset;
                                                                   useConstantProperty.boolValue = false;

                                                                   useConstantProperty.serializedObject.ApplyModifiedProperties();
                                                               });
            }

            return menu;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
    }
}