using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Architecture
{
    public class TypePicker : EditorWindow
    {
        private TypePickerTreeView tree;
        private string searchString = "";
        private string[] folders;
        private static Type[] selectedItems;

        private static int controlId;

        private void OnEnable()
        {
            tree = new TypePickerTreeView(new TreeViewState());
        }

        public static void Show(int controlId)
        {
            Show(controlId, null);
        }
        
        public static void Show(int controlId, Type baseType)
        {
            PropertyInfo prop = Assembly.GetAssembly(typeof(EditorWindow)).GetType("UnityEditor.GUIView")
                                        .GetProperty("current", BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.Public);
            object o = prop.GetValue(null, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty, null, null,
                                     null);

            TypePicker picker = ScriptableObject.CreateInstance<TypePicker>();
            picker.ShowAuxWindow();
            TypePicker.controlId = controlId;

            Rect position = picker.position;
            position.width = EditorPrefs.GetFloat("ObjectSelectorWidth", 200f);
            position.height = EditorPrefs.GetFloat("ObjectSelectorHeight", 390f);
            position.x = EditorPrefs.GetFloat("ObjectSelectorXPos", 100);
            position.y = EditorPrefs.GetFloat("ObjectSelectorYPos", 100);
            picker.minSize = new Vector2(200f, 335f);
            picker.maxSize = new Vector2(10000f, 10000f);

            picker.position = position;

            picker.tree = new TypePickerTreeView(new TreeViewState(), baseType);

            picker.tree.OnItemDoubleClicked = id =>
                                              {
                                                  selectedItems = new Type[1] {picker.tree.GetItemById(id)};
                                                  Event e = EditorGUIUtility.CommandEvent("TypePickerClosed");
                                                  if (o != null)
                                                  {
                                                      MethodInfo m = o.GetType().GetMethod("SendEvent",
                                                                                           BindingFlags.Instance | BindingFlags.NonPublic |
                                                                                           BindingFlags.FlattenHierarchy);
                                                      m.Invoke(o, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] {e}, null);
                                                  }
                                                  if (selectedItems[0] != null)
                                                      picker.Close();
                                              };

            picker.tree.OnSelectionChanged = selectedIds =>
                                             {
                                                 selectedItems = new Type[selectedIds.Count];
                                                 for (int i = 0; i < selectedIds.Count; i++)
                                                 {
                                                     int selectedId = selectedIds[i];
                                                     selectedItems[i] = picker.tree.GetItemById(selectedId);
                                                 }

                                                 selectedItems = selectedItems.Where(x => x != null).ToArray();
                                                 Event e = EditorGUIUtility.CommandEvent("TypePickerSelectionChanged");
                                                 if (o != null)
                                                 {
                                                     MethodInfo m = o.GetType().GetMethod("SendEvent",
                                                                                          BindingFlags.Instance | BindingFlags.NonPublic |
                                                                                          BindingFlags.FlattenHierarchy);
                                                     m.Invoke(o, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] {e}, null);
                                                 }
                                             };
        }

        public static Type[] GetSelection()
        {
            return selectedItems;
        }

        public static int GetControlId()
        {
            return controlId;
        }

        public static string SearchField(Rect position, string text)
        {
            GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);

            Rect position2 = position;
            position2.width -= 17f;
            text = EditorGUI.TextField(position2, text, skin.GetStyle("SearchTextField"));
            Rect position3 = position;
            position3.x += position.width - 17f;
            position3.width = 15f;
            if (GUI.Button(position3, GUIContent.none, (text == "") ? skin.GetStyle("SearchCancelButtonEmpty") : skin.GetStyle("SearchCancelButton")) &&
                text != "")
            {
                text = string.Empty;
                GUIUtility.keyboardControl = 0;
            }
            return text;
        }


        private void OnGUI()
        {
            EditorPrefs.SetFloat("ObjectSelectorWidth", position.width);
            EditorPrefs.SetFloat("ObjectSelectorHeight", position.height);
            EditorPrefs.SetFloat("ObjectSelectorXPos", position.x);
            EditorPrefs.SetFloat("ObjectSelectorYPos", position.y);

            Rect r = GUILayoutUtility.GetRect(new GUIContent(), new GUIStyle("ProjectBrowserTopBarBg"));
            searchString = SearchField(r, searchString);
            tree.searchString = searchString;
            tree.OnGUI(new Rect(0, 21, Screen.width, Screen.height - 21));
            Event e = Event.current;
        }
    }
}