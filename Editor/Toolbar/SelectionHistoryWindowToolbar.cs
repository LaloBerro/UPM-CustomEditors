using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityToolbarExtender;
using Object = UnityEngine.Object;

[InitializeOnLoad]
public class SelectionHistoryWindowToolbar
{
    private static List<Object> _selectionHistory = new List<Object>();
    private static int _selectedIndex = -1;

    private static Object _currentSelectionObject;

    static SelectionHistoryWindowToolbar()
    {
        ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUI);

        Selection.selectionChanged += SelectionChanged;
    }

    private static void SelectionChanged()
    {
        if (!Selection.activeObject) return;

        AddToHistory();
    }

    private static void AddToHistory()
    {
        //Skip selected folders and such
        if (Selection.activeObject.GetType() == typeof(UnityEditor.DefaultAsset)) return;

        if (Selection.activeObject == _currentSelectionObject)
        {
            Debug.Log("Same object");
            return;
        }

        _selectionHistory.Add(Selection.activeObject);
        _selectedIndex = _selectionHistory.Count - 1;

        if (_selectionHistory.Count - 1 == 100) _selectionHistory.RemoveAt(0);
    }

    static void OnToolbarGUI()
    {
        GUILayout.FlexibleSpace();

        if (GUILayout.Button(
                        new GUIContent(EditorGUIUtility.IconContent("ArrowNavigationLeft").image,
                            "Select previous (Left bracket key)"), EditorStyles.toolbarButton))
        {

            SelectPrevious();
        }

        if (GUILayout.Button(
                        new GUIContent(EditorGUIUtility.IconContent("ArrowNavigationRight").image,
                            "Select previous (Left bracket key)"), EditorStyles.toolbarButton))
        {
            SelectNext();
        }

    }

    private static void SelectPrevious()
    {
        if (_selectionHistory.Count <= 0)
            return;

        _selectedIndex--;
        _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _selectionHistory.Count - 1);

        SetSelection(_selectionHistory[_selectedIndex], _selectedIndex);
    }

    private static void SelectNext()
    {
        if (_selectionHistory.Count <= 0)
            return;

        _selectedIndex++;
        _selectedIndex = Mathf.Clamp(_selectedIndex, 0, _selectionHistory.Count - 1);

        SetSelection(_selectionHistory[_selectedIndex], _selectedIndex);
    }

    private static void SetSelection(Object target, int index)
    {
        _currentSelectionObject = target;
        Selection.activeObject = target;
        EditorGUIUtility.PingObject(target);
    }
}