using System.IO;
using UnityEditor;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions
{
    public class AssemblyReloadEditor : EditorWindow
    {
        [MenuItem("Custom Editor/Assembly Reload Editor")]
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

    public class CreateMVPFolderStructure
    {
        [MenuItem("Assets/Create MVP Folders")]
        public static void Create()
        {
            string directoryPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!Directory.Exists(directoryPath))
            {
                directoryPath = Path.GetDirectoryName(directoryPath);
            }

            Debug.Log(directoryPath);

            CreateDirectory("Model", directoryPath);
            CreateDirectory("View", directoryPath);
            CreateDirectory("Presenter", directoryPath);
        }

        private static void CreateDirectory(string folderName, string existingPath)
        {
            string folderPath = Path.Combine(existingPath, folderName);

            if (Directory.Exists(folderPath))
            {
                Debug.Log("The path is already created: " + folderPath);
                return;
            }

            Directory.CreateDirectory(folderPath);

            AssetDatabase.Refresh();
        }
    }
}