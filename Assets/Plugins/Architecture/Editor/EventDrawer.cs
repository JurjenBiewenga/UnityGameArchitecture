using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Architecture
{
    public class EventDrawer : PropertyDrawer
    {
        private ReorderableList _reorderableList;
        private SerializedProperty property;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitReordableList(property);
            _reorderableList.DoList(position);
        }

        public void InitReordableList(SerializedProperty property)
        {
            this.property = property;
            if (_reorderableList == null)
            {
                _reorderableList = new ReorderableList(property.serializedObject, property.FindPropertyRelative("_listeners"));
                _reorderableList.drawElementCallback += DrawElementCallback;
                _reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 2f;
                _reorderableList.drawHeaderCallback += DrawHeaderCallback;
            }
        }

        private void DrawHeaderCallback(Rect rect)
        {
            GUI.Label(rect, property.displayName);
        }

        private void DrawElementCallback(Rect position, int index1, bool isActive, bool isFocused)
        {
            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            SerializedProperty eventCalls = property.FindPropertyRelative("_listeners");
            Rect pos2 = new Rect(position.x, position.y, position.width / 2, EditorGUIUtility.singleLineHeight);
            Rect pos3 = new Rect(position.x + position.width / 2, position.y, position.width / 2,
                                 EditorGUIUtility.singleLineHeight);

            SerializedProperty call = eventCalls.GetArrayElementAtIndex(index1);
            SerializedProperty objectField = call.FindPropertyRelative("Object");
            SerializedProperty functionName = call.FindPropertyRelative("FunctionName");
            EditorGUI.BeginChangeCheck();
            EditorGUI.ObjectField(pos2, objectField, new GUIContent());
            if (EditorGUI.EndChangeCheck())
            {
                functionName.stringValue = "";
            }

            if (GUI.Button(pos3, functionName.stringValue, skin.GetStyle("DropDownButton")))
            {
                GenericMenu menu = new GenericMenu();
                Type t = fieldInfo.FieldType.BaseType;
                if (t != null)
                {
                    while (!t.IsGenericType)
                    {
                        t = t.BaseType;
                    }

                    var types = t.GetGenericArguments();

                    Object targetObject = objectField.objectReferenceValue;
                    Type targetType = targetObject.GetType();
                    MethodInfo[] methods = targetType.GetMethods();
                    for (int x = 0; x < methods.Length; x++)
                    {
                        ParameterInfo[] parameters = methods[x].GetParameters();
                        bool success = true;
                        if (types.Length != parameters.Length)
                            continue;

                        foreach (ParameterInfo parameterInfo in parameters)
                        {
                            foreach (Type type in types)
                            {
                                if (!type.IsAssignableFrom(parameterInfo.ParameterType))
                                    success = false;
                            }
                        }

                        int index = x;
                        if (success)
                            menu.AddItem(new GUIContent(methods[x].Name), false, () =>
                                                                                 {
                                                                                     functionName.stringValue = methods[index].Name;
                                                                                     property.serializedObject.ApplyModifiedProperties();
                                                                                 });
                    }

                    if (menu.GetItemCount() > 0)
                        menu.DropDown(pos3);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            InitReordableList(property);
            return _reorderableList.GetHeight();
        }
    }
}