using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityToolbarExtender;

namespace CustomEditors
{
    static class ToolbarStyles
    {
        public static readonly GUIStyle commandButtonStyle;

        static ToolbarStyles()
        {
            commandButtonStyle = new GUIStyle("Command")
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                fontStyle = FontStyle.Bold,
                fixedHeight = 20,
                fixedWidth = 25,
                richText = true
            };
        }
    }

    [InitializeOnLoad]
    public class PlayWithMainMenu
    {
        static PlayWithMainMenu()
        {
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);

            EditorApplication.playModeStateChanged += SceneHelper.ExitOnPlay;
        }

        static void OnToolbarGUI()
        {
            if (EditorApplication.isPlaying)
                return;

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(new GUIContent("<color=black>►</color>", "Load Play from Init"), ToolbarStyles.commandButtonStyle))
                SceneHelper.StartScene("Init", EditorSceneManager.GetActiveScene().name);

        }
    }

    public static class SceneHelper
    {
        static string sceneToOpen;

        static string openScene;

        public static void StartScene(string sceneName, string currentScene)
        {
            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
            }

            sceneToOpen = sceneName;
            openScene = currentScene;
            PlayerPrefs.SetString("OpenScene", currentScene);
            PlayerPrefs.SetInt("MagicPlayButtonIsPressed", 1);
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate()
        {
            if (sceneToOpen == null ||
                EditorApplication.isPlaying || EditorApplication.isPaused ||
                EditorApplication.isCompiling || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            EditorApplication.update -= OnUpdate;

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                string scenePath = "";

                for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
                {
                    if (sceneToOpen == GetSceneName(GetSceneName(EditorBuildSettings.scenes[i].path)))
                    {
                        scenePath = EditorBuildSettings.scenes[i].path;
                        break;
                    }
                }

                string GetSceneName(string _path)
                {
                    char[] path = _path.ToCharArray();
                    string sceneName = "";

                    for (int i = path.Length - 1; i >= 0; i--)
                    {
                        if (path[i] == '/')
                            break;
                        sceneName += path[i];
                    }

                    path = sceneName.ToCharArray();
                    sceneName = "";

                    for (int i = path.Length - 1; i >= 0; i--)
                    {
                        if (path[i] == '.')
                            break;
                        sceneName += path[i];
                    }

                    return sceneName;
                }

                EditorSceneManager.OpenScene(scenePath);
                EditorApplication.isPlaying = true;
            }

            sceneToOpen = null;
        }

        public static void ExitOnPlay(PlayModeStateChange mode)
        {
            if (mode != PlayModeStateChange.EnteredEditMode || PlayerPrefs.GetInt("MagicPlayButtonIsPressed") == 0)
                return;

            string scenePath = "";

            openScene = PlayerPrefs.GetString("OpenScene");

            if (openScene == "") return;

            for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (openScene == GetSceneName(GetSceneName(EditorBuildSettings.scenes[i].path)))
                {
                    scenePath = EditorBuildSettings.scenes[i].path;
                    break;
                }

            }

            string GetSceneName(string _path)
            {
                char[] path = _path.ToCharArray();
                string sceneName = "";

                for (int i = path.Length - 1; i >= 0; i--)
                {
                    if (path[i] == '/')
                        break;
                    sceneName += path[i];
                }

                path = sceneName.ToCharArray();
                sceneName = "";

                for (int i = path.Length - 1; i >= 0; i--)
                {
                    if (path[i] == '.')
                        break;
                    sceneName += path[i];
                }

                return sceneName;
            }

            EditorApplication.playModeStateChanged -= ExitOnPlay;
            if (string.IsNullOrEmpty(scenePath))
                return;
            
            EditorSceneManager.OpenScene(scenePath);
            
            PlayerPrefs.SetString("OpenScene", "");

            PlayerPrefs.SetInt("MagicPlayButtonIsPressed", 0);
        }
    }
}
