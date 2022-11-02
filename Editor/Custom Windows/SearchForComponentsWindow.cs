using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.Windows
{
    public class SearchForComponentsWindow : EditorWindow
    {
        private string[] _modes = new string[] { "Search for component usage", "Search for missing components" };

        private string _componentName = "";
        private List<string> _listResult;

        private int _editorMode;
        private int _editorModeOld;

        private MonoScript _targetComponent;
        private MonoScript _lastChecked;
        private Vector2 _scroll;

        [MenuItem("Tools/Search For Components")]
        private static void ShowWindow()
        {
            SearchForComponentsWindow window = (SearchForComponentsWindow)EditorWindow.GetWindow(typeof(SearchForComponentsWindow));
            window.Show();
            window.position = new Rect(20, 80, 400, 300);
        }

        private void OnGUI()
        {
            GUILayout.Space(3);
            int oldValue = GUI.skin.window.padding.bottom;
            GUI.skin.window.padding.bottom = -20;
            Rect windowRect = GUILayoutUtility.GetRect(1, 17);
            windowRect.x += 4;
            windowRect.width -= 7;
            _editorMode = GUI.SelectionGrid(windowRect, _editorMode, _modes, 2, "Window");
            GUI.skin.window.padding.bottom = oldValue;

            if (_editorModeOld != _editorMode)
            {
                _editorModeOld = _editorMode;
                _listResult = new List<string>();
                _componentName = _targetComponent == null ? "" : _targetComponent.name;
                _lastChecked = null;
            }

            switch (_editorMode)
            {
                case 0:
                    _targetComponent = (MonoScript)EditorGUILayout.ObjectField(_targetComponent, typeof(MonoScript), false);

                    if (_targetComponent != _lastChecked)
                    {
                        _lastChecked = _targetComponent;
                        _componentName = _targetComponent.name;
                        AssetDatabase.SaveAssets();
                        string targetPath = AssetDatabase.GetAssetPath(_targetComponent);
                        string[] allPrefabs = GetAllPrefabs();
                        _listResult = new List<string>();
                        foreach (string prefab in allPrefabs)
                        {
                            string[] single = new string[] { prefab };
                            string[] dependencies = AssetDatabase.GetDependencies(single);
                            foreach (string dependedAsset in dependencies)
                            {
                                if (dependedAsset == targetPath)
                                {
                                    _listResult.Add(prefab);
                                }
                            }
                        }
                    }
                    break;
                case 1:
                    if (GUILayout.Button("Search!"))
                    {
                        string[] allPrefabs = GetAllPrefabs();
                        _listResult = new List<string>();
                        foreach (string prefab in allPrefabs)
                        {
                            UnityEngine.Object o = AssetDatabase.LoadMainAssetAtPath(prefab);
                            GameObject go;
                            try
                            {
                                go = (GameObject)o;
                                Component[] components = go.GetComponentsInChildren<Component>(true);
                                foreach (Component c in components)
                                {
                                    if (c == null)
                                    {
                                        _listResult.Add(prefab);
                                    }
                                }
                            }
                            catch
                            {
                                Debug.Log("For some reason, prefab " + prefab + " won't cast to GameObject");

                            }
                        }
                    }
                    break;
            }

            if (_listResult != null)
            {
                if (_listResult.Count == 0)
                {
                    GUILayout.Label(_editorMode == 0 ? (_componentName == "" ? "Choose a component" : "No prefabs use component " + _componentName) : ("No prefabs have missing components!\nClick Search to check again"));
                }
                else
                {
                    GUILayout.Label(_editorMode == 0 ? ("The following prefabs use component " + _componentName + ":") : ("The following prefabs have missing components:"));
                    _scroll = GUILayout.BeginScrollView(_scroll);
                    foreach (string s in _listResult)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label(s, GUILayout.Width(position.width / 2));
                        if (GUILayout.Button("Select", GUILayout.Width(position.width / 2 - 10)))
                        {
                            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(s);
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.EndScrollView();
                }
            }
        }

        public static string[] GetAllPrefabs()
        {
            string[] temp = AssetDatabase.GetAllAssetPaths();
            List<string> result = new List<string>();
            foreach (string s in temp)
            {
                if (s.Contains(".prefab")) result.Add(s);
            }
            return result.ToArray();
        }
    }
}