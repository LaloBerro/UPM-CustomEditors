using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace CustomEditors
{
    public class RenderCubeMapOfACamera : EditorWindow
    {
        [SerializeField] private Camera _camera;
        private int size = 512;
        public string newCubmapPath = "cubemap";

        [MenuItem("Tools/RenderCubeMapOfACamera")]
        private static void ShowWindow()
        {
            var window = GetWindow<RenderCubeMapOfACamera>();
            window.titleContent = new GUIContent("RenderCubeMapOfACamera");
            window.Show();
        }

        private void OnGUI()
        {
            _camera = (Camera)EditorGUILayout.ObjectField("Camera", _camera, typeof(Camera), true);
            size = EditorGUILayout.IntField("Resolution", size);

            if (!GUILayout.Button("Render CubeMap"))
                return;

            CreateCubeMap();
        }

        private void CreateCubeMap()
        {
            Cubemap cubeMap = new Cubemap(size, TextureFormat.RGBA32, false);
            _camera.RenderToCubemap(cubeMap);

            CreateCubeMapAwait(cubeMap);
        }

        private async void CreateCubeMapAwait(Cubemap cubeMap)
        {
            await Task.Delay(100);
            OnWizardCreate(cubeMap);
        }


        void OnWizardCreate(Cubemap cubemap)
        {
            try
            {
                // convert cubemap to single horizontal texture
                var texture = new Texture2D(size * 6, size, cubemap.format, false);
                int texturePixelCount = (size * 6) * size;
                var texturePixels = new Color[texturePixelCount];

                var cubeFacePixels = cubemap.GetPixels(CubemapFace.PositiveX);
                CopyTextureIntoCubemapRegion(cubeFacePixels, texturePixels, size * 0);
                cubeFacePixels = cubemap.GetPixels(CubemapFace.NegativeX);
                CopyTextureIntoCubemapRegion(cubeFacePixels, texturePixels, size * 1);

                cubeFacePixels = cubemap.GetPixels(CubemapFace.PositiveY);
                CopyTextureIntoCubemapRegion(cubeFacePixels, texturePixels, size * 3);
                cubeFacePixels = cubemap.GetPixels(CubemapFace.NegativeY);
                CopyTextureIntoCubemapRegion(cubeFacePixels, texturePixels, size * 2);

                cubeFacePixels = cubemap.GetPixels(CubemapFace.PositiveZ);
                CopyTextureIntoCubemapRegion(cubeFacePixels, texturePixels, size * 4);
                cubeFacePixels = cubemap.GetPixels(CubemapFace.NegativeZ);
                CopyTextureIntoCubemapRegion(cubeFacePixels, texturePixels, size * 5);

                texture.SetPixels(texturePixels, 0);

                // write texture as png to disk
                var textureData = texture.EncodeToPNG();
                File.WriteAllBytes(Path.Combine(Application.dataPath, $"{newCubmapPath}.png"), textureData);

                // save to disk
                AssetDatabase.SaveAssetIfDirty(cubemap);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            finally
            {

            }
        }

        private void CopyTextureIntoCubemapRegion(Color[] srcPixels, Color[] dstPixels, int xOffsetDst)
        {
            Debug.Log(srcPixels.Length);
            Debug.Log(dstPixels.Length);
            int cubemapWidth = size * 6;
            for (int y = 0; y != size; ++y)
            {
                for (int x = 0; x != size; ++x)
                {
                    int iSrc = x + (y * size);
                    int iDst = (x + xOffsetDst) + (y * cubemapWidth);
                    dstPixels[iDst] = srcPixels[iSrc];
                }
            }
        }

        void ConvertToPng(Cubemap cubemap)
        {
            Debug.Log(Application.dataPath + "/" + cubemap.name + "_PositiveX.png");
            var tex = new Texture2D(cubemap.width, cubemap.height, TextureFormat.RGB24, false);
            // Read screen contents into the texture        
            tex.SetPixels(cubemap.GetPixels(CubemapFace.PositiveX));
            // Encode texture into PNG
            var bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + cubemap.name + "_PositiveX.png", bytes);

            tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeX));
            bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + cubemap.name + "_NegativeX.png", bytes);

            tex.SetPixels(cubemap.GetPixels(CubemapFace.PositiveY));
            bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + cubemap.name + "_PositiveY.png", bytes);

            tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeY));
            bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + cubemap.name + "_NegativeY.png", bytes);

            tex.SetPixels(cubemap.GetPixels(CubemapFace.PositiveZ));
            bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + cubemap.name + "_PositiveZ.png", bytes);

            tex.SetPixels(cubemap.GetPixels(CubemapFace.NegativeZ));
            bytes = tex.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/" + cubemap.name + "_NegativeZ.png", bytes);
            DestroyImmediate(tex);

        }
    }
}