using UnityEditor;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions
{
    public class AssemblyReloadEditor : EditorWindow
    {
        [MenuItem("Tools/Assembly Reload Editor")]
        public static void Init()
        {
            GetWindow<AssemblyReloadEditor>();
        }

        private void OnGUI()
        {
            if (GUILayout.Button(EditorGUIUtility.IconContent("IN LockButton on act@2x")))
            {
                Debug.Log("Lock Assemblies");
                EditorApplication.LockReloadAssemblies();
            }

            if (GUILayout.Button(EditorGUIUtility.IconContent("IN LockButton act@2x")))
            {
                Debug.Log("Unlock Assemblies");
                EditorApplication.UnlockReloadAssemblies();

                AssetDatabase.Refresh();
            }

            Repaint();
        }

        [MenuItem("Assets/Lock Reload Assemblies")]
        public static void LockReloadAssemblies()
        {
            Debug.Log("Lock Assemblies");
            EditorApplication.LockReloadAssemblies();
        }

        [MenuItem("Assets/Unlock Reload Assemblies")]
        public static void UnlockReloadAssemblies()
        {
            Debug.Log("Unlock Assemblies");
            EditorApplication.UnlockReloadAssemblies();

            AssetDatabase.Refresh();
        }
    }
}