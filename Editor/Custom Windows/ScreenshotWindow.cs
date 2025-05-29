using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.Windows
{
    [ExecuteInEditMode]
    public class ScreenshotWindow : EditorWindow
    {
        private int _resolutionWidth = Screen.width * 4;
        private int _resolutionHeight = Screen.height * 4;
        private int _scale = 1;

        private Camera _camera;
        private string _savePath = string.Empty;
        private string _lastScreenshotPath = string.Empty;

        private bool _hasToIncludeCanvas = true;
        private bool _mustTakeHighResShot = false;

        [MenuItem("Tools/Screenshot")]
        private static void OpenWindow()
        {
            ScreenshotWindow window = GetWindow<ScreenshotWindow>();
            window.autoRepaintOnSceneChange = true;
            window.titleContent = new GUIContent("Screenshot");
            window.Show();
        }

        private void OnGUI()
        {
            DrawResolutionSettings();
            DrawCanvasToggle();
            DrawSavePathField();
            DrawCameraSelection();
            DrawDefaultOptions();
            DrawScreenshotButton();
            DrawBottomActions();

            if (_mustTakeHighResShot)
                CaptureScreenshot();
        }

        private void DrawResolutionSettings()
        {
            EditorGUILayout.LabelField("Resolution", EditorStyles.boldLabel);
            _resolutionWidth = EditorGUILayout.IntField("Width", _resolutionWidth);
            _resolutionHeight = EditorGUILayout.IntField("Height", _resolutionHeight);
            _scale = EditorGUILayout.IntSlider("Scale", _scale, 1, 15);
            EditorGUILayout.Space();
        }

        private void DrawCanvasToggle()
        {
            _hasToIncludeCanvas = EditorGUILayout.Toggle("Include Canvas", _hasToIncludeCanvas);
            EditorGUILayout.Space();
        }

        private void DrawSavePathField()
        {
            EditorGUILayout.LabelField("Save Path", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.TextField(_savePath, GUILayout.ExpandWidth(false));
            if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
                _savePath = EditorUtility.SaveFolderPanel("Path to Save Images", _savePath, Application.dataPath);

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }

        private void DrawCameraSelection()
        {
            EditorGUILayout.LabelField("Select Camera", EditorStyles.boldLabel);
            _camera = EditorGUILayout.ObjectField(_camera, typeof(Camera), true, null) as Camera;
            if (_camera == null) 
                _camera = Camera.main;
            
            EditorGUILayout.Space();
        }

        private void DrawDefaultOptions()
        {
            EditorGUILayout.LabelField("Default Options", EditorStyles.boldLabel);

            if (GUILayout.Button("Set To Screen Size"))
            {
                Vector2 size = Handles.GetMainGameViewSize();
                _resolutionWidth = (int)size.x;
                _resolutionHeight = (int)size.y;
            }

            if (GUILayout.Button("Default Size"))
            {
                _resolutionWidth = 1920;
                _resolutionHeight = 1080;
                _scale = 1;
            }

            EditorGUILayout.Space();
        }

        private void DrawScreenshotButton()
        {
            EditorGUILayout.LabelField($"Screenshot will be taken at {_resolutionWidth * _scale} x {_resolutionHeight * _scale} px", EditorStyles.boldLabel);

            if (GUILayout.Button("Take Screenshot", GUILayout.MinHeight(60)))
            {
                if (string.IsNullOrEmpty(_savePath))
                    _savePath = EditorUtility.SaveFolderPanel("Path to Save Images", _savePath, Application.dataPath);

                StartScreenshotCapture();
            }

            EditorGUILayout.Space();
        }

        private void DrawBottomActions()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Open Last Screenshot", GUILayout.MaxWidth(160), GUILayout.MinHeight(40)))
                OpenLastScreenshot();

            if (GUILayout.Button("Open Folder", GUILayout.MaxWidth(100), GUILayout.MinHeight(40)))
                OpenScreenshotFolder();

            EditorGUILayout.EndHorizontal();
        }

        private void CaptureScreenshot()
        {
            if (!_camera)
                throw new InvalidOperationException("Camera is null. Please assign a camera.");

            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);

            if (_hasToIncludeCanvas)
            {
                foreach (Canvas canvas in canvases)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = _camera;
                }
            }

            int width = _resolutionWidth * _scale;
            int height = _resolutionHeight * _scale;

            RenderTexture renderTexture = new RenderTexture(width, height, 24);
            _camera.targetTexture = renderTexture;
            RenderTexture.active = renderTexture;

            _camera.Render();

            Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
            screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            screenshot.Apply();

            _camera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(renderTexture);

            string filePath = GetScreenshotPath(width, height);
            File.WriteAllBytes(filePath, screenshot.EncodeToPNG());

            Debug.Log($"✅ Screenshot saved to: {filePath}");
            Application.OpenURL(filePath);

            if (_hasToIncludeCanvas)
            {
                foreach (Canvas canvas in canvases)
                    canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            _mustTakeHighResShot = false;
        }

        private string GetScreenshotPath(int width, int height)
        {
            _lastScreenshotPath = Path.Combine(_savePath, $"screen_{width}x{height}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.png");
            return _lastScreenshotPath;
        }

        private void StartScreenshotCapture()
        {
            Debug.Log("📸 Taking Screenshot...");
            _mustTakeHighResShot = true;
        }

        private void OpenLastScreenshot()
        {
            if (!string.IsNullOrEmpty(_lastScreenshotPath))
                Application.OpenURL("file://" + _lastScreenshotPath);
        }

        private void OpenScreenshotFolder()
        {
            if (!string.IsNullOrEmpty(_savePath))
                Application.OpenURL("file://" + _savePath);
        }
    }
}