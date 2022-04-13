using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.CustomTools.Shortcut
{
    public class ToolSelectorWithSpace : Editor
    {
        [Shortcut("TransformToolSelector", null, KeyCode.Space)]
        public static void AddComponentToSelectedGameObject(ShortcutArguments shortcutArguments)
        {
            switch (Tools.current)
            {
                case Tool.View:
                    Tools.current = Tool.Move;

                    break;
                case Tool.Move:
                    Tools.current = Tool.Rotate;

                    break;

                case Tool.Rotate:
                    Tools.current = Tool.Scale;

                    break;

                case Tool.Scale:
                    Tools.current = Tool.Move;

                    break;

                case Tool.Rect:
                    Tools.current = Tool.Move;

                    break;

                case Tool.Transform:
                    Tools.current = Tool.Move;

                    break;

                case Tool.Custom:
                    Tools.current = Tool.Move;

                    break;

                case Tool.None:
                    Tools.current = Tool.Move;

                    break;

                default:
                    Tools.current = Tool.Move;

                    break;
            }
        }
    }
}