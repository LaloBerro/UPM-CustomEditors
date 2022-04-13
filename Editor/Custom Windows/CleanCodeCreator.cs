using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.Windows
{
    public class CleanCodeCreator : EditorWindow
    {
        private string _scriptsFolderPath;
        private string _folderName;
        private string _sceneName;

        [MenuItem("Custom Editor/Clean Code Creator")]
        private static void ShowWindow()
        {
            GetWindow(typeof(CleanCodeCreator), false, "Clean Code Creator");
        }

        private async void OnGUI()
        {
            GUILayout.Label("Scene Name", EditorStyles.boldLabel);
            _sceneName = EditorGUILayout.TextField(_sceneName, GUILayout.ExpandWidth(false));

            GUILayout.Label("Folder Name", EditorStyles.boldLabel);
            _folderName = EditorGUILayout.TextField(_folderName, GUILayout.ExpandWidth(false));

            GUILayout.Label("Scripts Folder Path", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField(_scriptsFolderPath, GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
                _scriptsFolderPath = EditorUtility.SaveFolderPanel("Path to save scripts", _scriptsFolderPath, Application.dataPath);

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Generate Class"))
            {
                await CreateAll();
            }
        }

        private async Task CreateAll()
        {
            string namespaceDefault = "Project." + _sceneName + "." + _folderName;
            string namespaceEntity = namespaceDefault + ".Domain.Entities";
            string namespaceUseCase = namespaceDefault + ".Domain.UseCase";
            string namespaceDPresenters = namespaceDefault + ".Domain.Presenters";
            string namespacePresenters = namespaceDefault + ".Presenters";
            string namespaceDRepositories = namespaceDefault + ".Domain.Repositories";
            string namespaceRepositories = namespaceDefault + ".Repositories";

            string entityName = "Entity";
            string useCaseName = _folderName + "UseCase";
            string iPresenterName = "I" + _folderName + "Presenter";
            string iRepositoryName = "I" + _folderName + "Repository";
            string repositoryName = _folderName + "Repository";
            string presenterName = _folderName + "Presenter";
            string iViewName = "I" + _folderName + "View";
            string viewName = _folderName + "View";

            string entityCode = "namespace " + namespaceEntity + " \n{ \n    public class " + entityName + "\n    { \n \n    } \n }";
            string useCaseCode = "using CleanCore.CleanArchitectureBases.Domain.UseCases; \n \n" + "namespace " + namespaceUseCase + "\n{ \n    public class " + useCaseName + " : IUseCase \n    { \n          public void Begin() \n          {\n\n       }\n\n    public void Finish()\n        {\n\n       }\n     } \n }";
            string iPresenterCode = "using CleanCore.CleanArchitectureBases.Domain.Presenters; \n \n " + "namespace " + namespaceDPresenters + " \n{ \n    public interface " + iPresenterName + " : IPresenter \n    { \n \n    } \n }";
            string iRepositoryCode = "using CleanCore.CleanArchitectureBases.Domain.Repositories; \n \n " + "namespace " + namespaceDRepositories + " \n{ \n    public interface " + iRepositoryName + " : IRepository \n    { \n \n    } \n }";

            string repositoryCode = "using " + namespaceDRepositories + "; \n \n " + "namespace " + namespaceRepositories + " \n{ \n    public class " + repositoryName + " : " + iRepositoryName + " \n    { \n \n    } \n }";
            string PresenterCode = "using " + namespaceDPresenters + "; \n \n " + "namespace " + namespacePresenters + " \n{ \n    public class " + presenterName + " : " + iPresenterName + " \n    { \n \n    } \n }";
            string iViewCode = "using CleanCore.CleanArchitectureBases.Presenters; \n" + "using " + namespaceDPresenters + "; \n \n " + "namespace " + namespacePresenters + " \n{ \n    public interface " + iViewName + " : IView  \n    { \n \n    } \n }";
            string viewCode = "using " + namespaceDPresenters + "; \n \n " + "namespace " + namespacePresenters + " \n{ \n    public class " + viewName + " :  " + iViewName + " \n    { \n \n    } \n }";

            var folderPath = Directory.CreateDirectory(_scriptsFolderPath + "/" + _folderName);
            var domainPath = Directory.CreateDirectory(folderPath.FullName + "/" + "Domain");
            var entityPath = Directory.CreateDirectory(domainPath.FullName + "/" + "Entities");
            var useCasePath = Directory.CreateDirectory(domainPath.FullName + "/" + "UseCase");
            var repositoryPath = Directory.CreateDirectory(useCasePath.FullName + "/" + "Respositories");
            var presentersPath = Directory.CreateDirectory(useCasePath.FullName + "/" + "Presenters");
            var presenterPath = Directory.CreateDirectory(folderPath.FullName + "/" + "Presenters");
            var detailsPPath = Directory.CreateDirectory(presenterPath.FullName + "/" + "Details");
            var repositoriesPath = Directory.CreateDirectory(folderPath.FullName + "/" + "Repositories");
            var detailsRPath = Directory.CreateDirectory(repositoriesPath.FullName + "/" + "Details");

            await Task.Delay(1);

            File.WriteAllText(entityPath.FullName + "/" + entityName + ".cs", entityCode);
            File.WriteAllText(useCasePath.FullName + "/" + useCaseName + ".cs", useCaseCode);
            File.WriteAllText(repositoryPath.FullName + "/" + iRepositoryName + ".cs", iRepositoryCode);
            File.WriteAllText(presentersPath.FullName + "/" + iPresenterName + ".cs", iPresenterCode);
            File.WriteAllText(presenterPath.FullName + "/" + presenterName + ".cs", PresenterCode);
            File.WriteAllText(presenterPath.FullName + "/" + iViewName + ".cs", iViewCode);
            File.WriteAllText(detailsPPath.FullName + "/" + viewName + ".cs", viewCode);
            File.WriteAllText(repositoriesPath.FullName + "/" + repositoryName + ".cs", repositoryCode);

            AssetDatabase.Refresh();
        }
    }
}