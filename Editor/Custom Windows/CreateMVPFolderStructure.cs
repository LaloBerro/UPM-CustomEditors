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