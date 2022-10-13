using System.IO;
using UnityEditor;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions
{
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
            CreateDirectory("Installers", directoryPath);
        }

        [MenuItem("Assets/Create CC Folders")]
        public static void CreateCC()
        {
            string directoryPath = AssetDatabase.GetAssetPath(Selection.activeObject);

            if (!Directory.Exists(directoryPath))
            {
                directoryPath = Path.GetDirectoryName(directoryPath);
            }

            Debug.Log(directoryPath);

            CreateDirectory("Domain", directoryPath);
            CreateDirectory("UseCases", Path.Combine(directoryPath, "Domain"));
            CreateDirectory("Entities", Path.Combine(directoryPath, "Domain"));

            CreateDirectory("InterfaceAdapters", directoryPath);
            CreateDirectory("Gateways", Path.Combine(directoryPath, "InterfaceAdapters"));
            CreateDirectory("Presenters", Path.Combine(directoryPath, "InterfaceAdapters"));
            CreateDirectory("Controllers", Path.Combine(directoryPath, "InterfaceAdapters"));

            CreateDirectory("Installers", directoryPath);
            CreateDirectory("Domain", Path.Combine(directoryPath, "Installers"));
            CreateDirectory("InterfaceAdapters", Path.Combine(directoryPath, "Installers"));

            CreateDirectory("View", directoryPath);
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