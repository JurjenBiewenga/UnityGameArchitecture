using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Architecture
{
    [CustomPropertyDrawer(typeof(BaseReference), true)]
    [CanEditMultipleObjects]
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
                        EditorGUI.BeginChangeCheck();
                        if (persistentProperty.boolValue || Application.isPlaying)
                        {
                            EditorGUI.PropertyField(pos2, valueProperty, new GUIContent(), true);
                        }
                        else
                        {
                            EditorGUI.PropertyField(pos2, defaultValueProperty, new GUIContent(), true);
                        }
                        if (EditorGUI.EndChangeCheck())
                        {
                            variable.ApplyModifiedProperties();
                        }
                    }
                }
                else
                {
                    EditorGUI.PropertyField(position, variableProperty, new GUIContent(property.displayName), true);
                }
            }
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
            Component go = (property.serializedObject.targetObject as Component);
            string path = "";
            if (go != null)
            {
                if (go.gameObject.scene.IsValid() && PrefabUtility.GetPrefabObject(go.gameObject) == null)
                {
                    path = string.Format(@"Scriptable Objects\{0}\{1}\{2}.asset", go.gameObject.scene.name, property.serializedObject.targetObject.name,
                                         property.displayName);
                }
                else
                {
                    path = string.Format(@"Scriptable Objects\{0}\{1}\{2}.asset", "Prefabs", property.serializedObject.targetObject.name,
                                         property.displayName);
                }
            }
            else
            {
                path = string.Format(@"Scriptable Objects\{0}\{1}.asset", property.serializedObject.targetObject.name,
                                     property.displayName);
            }

            menu.AddItem(new GUIContent("Create variable at " + path), false,
                         () => { Utils.GenerateAndSetVariable(useConstantProperty, variableProperty, path); });

            string[] assetGuids = AssetDatabase.FindAssets("t:" + Utils.GetPropertyType(variableProperty));
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