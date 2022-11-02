using System;
using UnityEditor;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions
{
    public class TextToSingleLineConverter : EditorWindow
    {
        private string _textToConvert;
        private string _textOutput;

        [MenuItem("Tools/TextToSingleLineConverter")]
        private static void ShowWindow()
        {
            var window = GetWindow<TextToSingleLineConverter>();
            window.titleContent = new GUIContent("TextToSingleLineConverter");
            window.Show();
        }

        private void OnGUI()
        {
            _textToConvert = EditorGUILayout.TextArea(_textToConvert);

            if (GUILayout.Button("Convert & Copy"))
                ConvertText();
        }

        private void ConvertText()
        {
            string[] lines = _textToConvert.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            _textOutput = String.Join("\n", lines);

            EditorGUIUtility.systemCopyBuffer = _textOutput;
        }
    }
}
