using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UtilitiesCustomPackage.EditorExtensions.CustomTools.Button
{
    public class LogMissingPrefabsAndComponents
    {
        private static List<string> _results = new List<string>();

        [MenuItem("Tools/Log Missing Prefabs And Components")]
        private static void Search()
        {
            _results.Clear();
            GameObject[] gos = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject go in gos) Traverse(go.transform);
            Debug.Log("> Total Results: " + _results.Count);
            foreach (string result in _results) Debug.Log("> " + result);
        }

        private static void AppendComponentResult(string childPath, int index)
        {
            _results.Add("Missing Component " + index + " of " + childPath);
        }

        private static void AppendTransformResult(string childPath, string name)
        {
            _results.Add("Missing Prefab for \"" + name + "\" of " + childPath);
        }

        private static void Traverse(Transform transform, string path = "")
        {
            string thisPath = path + "/" + transform.name;
            Component[] components = transform.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null) AppendComponentResult(thisPath, i);
            }
            for (int c = 0; c < transform.childCount; c++)
            {
                Transform t = transform.GetChild(c);
                PrefabAssetType pt = PrefabUtility.GetPrefabAssetType(t.gameObject);
                if (pt == PrefabAssetType.MissingAsset)
                {
                    AppendTransformResult(path + "/" + transform.name, t.name);
                }
                else
                {
                    Traverse(t, thisPath);
                }
            }
        }
    }
}