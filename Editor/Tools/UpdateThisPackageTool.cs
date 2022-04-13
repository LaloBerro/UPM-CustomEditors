using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.CustomTools
{
    public class UpdateThisPackageTool : Editor
    {
        private static AddRequest Request;

        [MenuItem("Custom Editor/Update Utilities package", priority = 0)]
        public static void Add()
        {
            Debug.Log("Updating Utilities package");
            Request = Client.Add("https://github.com/TGL-Games/UnityPackage-Utilities.git");
            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            if (Request.IsCompleted)
            {
                if (Request.Status == StatusCode.Success)
                    Debug.Log("Updated: " + Request.Result.name + " to version: " + Request.Result.version);
                else if (Request.Status >= StatusCode.Failure)
                    Debug.Log(Request.Error.message);

                EditorApplication.update -= Progress;
            }
        }
    }
}