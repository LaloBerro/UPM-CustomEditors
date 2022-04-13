using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.Windows
{
    public class RemoveComponentsOfPrefabsWindow : EditorWindow
    {
        private MonoScript _targetComponent;

        [MenuItem("Custom Editor/Remove Component on prefabs and apply")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(RemoveComponentsOfPrefabsWindow), false, "Remove Component on prefabs");
        }

        private void OnGUI()
        {
            _targetComponent = (MonoScript)EditorGUILayout.ObjectField(_targetComponent, typeof(MonoScript), false);

            if (GUILayout.Button("Remove"))
            {
                RemoveAndApplyComponentOnPrefabs(_targetComponent.GetClass());
            }
        }

        private void RemoveAndApplyComponentOnPrefabs(System.Type type)
        {
            List<GameObject> objs = new List<GameObject>(FindObjectsOfType<GameObject>());
            List<GameObject> gos = new List<GameObject>();

            Debug.Log("type " + type);
            Debug.Log("objs count " + objs.Count);

            for (int i = 0; i < objs.Count; i++)
            {
                gos.Add(objs[i] as GameObject);
            }

            Debug.Log("gos count " + gos[0].name);

            foreach (GameObject go in gos)
            {
                bool isPrefab = PrefabUtility.IsPartOfAnyPrefab(go);

                if (!isPrefab) continue;

                RemoveRemoved(go, type);

                List<Component> components = new List<Component>(go.GetComponents<Component>());

                foreach (var component in components)
                {
                    if (component.GetType() == type)
                        DestroyImmediate(component);
                }

                List<RemovedComponent> removedComponents = PrefabUtility.GetRemovedComponents(go);

                for (int i = 0; i < removedComponents.Count; i++)
                {
                    removedComponents[i].Apply();
                }
            }
        }

        private void RemoveRemoved(GameObject go, System.Type type)
        {
            List<RemovedComponent> removedComponents = PrefabUtility.GetRemovedComponents(go);

            foreach (var removedComponent in removedComponents)
            {
                if (removedComponent.assetComponent.GetType() == type)
                    removedComponent.Apply();
            }
        }
    }
}