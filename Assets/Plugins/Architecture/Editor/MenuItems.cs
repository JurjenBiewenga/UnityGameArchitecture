using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Architecture
{
    public static class MenuItems
    {
        [MenuItem("Tools/Select Architecture Settings")]
        public static void SelectSettings()
        {
            ArchitectureSettings settings = ArchitectureSettings.GetSettings();
            Selection.activeObject = settings;
        }
    }
}