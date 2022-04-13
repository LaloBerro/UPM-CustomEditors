using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.CustomTools.Shortcut
{
    public class DeselectAllObjects : Editor
    {
        [Shortcut("DeselectAllObjects", null, KeyCode.C)]
        public static void DeselectAll(ShortcutArguments shortcutArguments)
        {
            Selection.activeObject = null;

            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                Selection.gameObjects[i] = null;
            }
        }
    }
}