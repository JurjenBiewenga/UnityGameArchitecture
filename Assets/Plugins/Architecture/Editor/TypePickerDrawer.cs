using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Architecture
{
    [CustomPropertyDrawer(typeof(TypePickerAttribute))]
    public class TypePickerDrawer : PropertyDrawer
    {
        int controlId = 0;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Event e = Event.current;
            controlId = EditorGUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
            switch (e.GetTypeForControl(controlId))
            {
                default:

                    position = EditorGUI.IndentedRect(position);
                    Rect pos2 = position;
                    pos2.width -= 32;
                    Rect pos3 = position;
                    pos3.x += pos2.width;
                    pos3.width = 15;
                    Rect pos4 = position;
                    pos4.x += pos2.width + pos3.width;
                    pos4.width = 15;

                    var typeName = property.FindPropertyRelative("m_Name");
                    string name = (!string.IsNullOrEmpty(typeName.stringValue) ? typeName.stringValue : "None");

                    GUI.Label(pos2, name);

                    if (GUI.Button(pos3, "s"))
                    {
                        TypePickerAttribute attr = attribute as TypePickerAttribute;
                        if(attr != null)
                            TypePicker.Show(controlId, attr.baseType);
                    }
                    
                    if (GUI.Button(pos4, "x"))
                    {
                        var assemblyQualifiedName = property.FindPropertyRelative("m_AssemblyQualifiedName");
                        var nameProperty = property.FindPropertyRelative("m_Name");
                        var assemblyName = property.FindPropertyRelative("m_AssemblyName");
                                
                        assemblyQualifiedName.stringValue = "";
                        nameProperty.stringValue = "";
                        assemblyName.stringValue = "";

                        property.serializedObject.ApplyModifiedProperties();
                    }

                    if (e.commandName == "TypePickerSelectionChanged" || e.commandName == "TypePickerClosed")
                    {
                        if (TypePicker.GetControlId() == controlId)
                        {
                            GUI.changed = true;
                            var selection = TypePicker.GetSelection();
                            if (selection.Length > 0 && selection[0] != null)
                            {
                                var assemblyQualifiedName = property.FindPropertyRelative("m_AssemblyQualifiedName");
                                var nameProperty = property.FindPropertyRelative("m_Name");
                                var assemblyName = property.FindPropertyRelative("m_AssemblyName");
                                
                                assemblyQualifiedName.stringValue = selection[0].AssemblyQualifiedName;
                                nameProperty.stringValue = selection[0].Name;
                                assemblyName.stringValue = selection[0].FullName;

                                property.serializedObject.ApplyModifiedProperties();
                            }
                        }
                    }
                    break;
            }
        }
    }
}