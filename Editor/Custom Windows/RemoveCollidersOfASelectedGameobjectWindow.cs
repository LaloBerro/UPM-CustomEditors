using UnityEngine;
using UnityEditor;

namespace BaseProject.EditorExtensions.Windows
{
    public class RemoveCollidersOfASelectedGameobjectWindow : EditorWindow
    {
        private bool _showBtn = true;

        [MenuItem("Custom Editor/Remove Collider")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(RemoveCollidersOfASelectedGameobjectWindow), false, "Remove Collider");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");

            EditorGUILayout.LabelField("Remover collider de los hijos");
            _showBtn = EditorGUILayout.Toggle("SI", _showBtn);

            EditorGUILayout.EndVertical();

            if (_showBtn)
            {
                if (GUILayout.Button("Remove hijos tambien"))
                {
                    //Remove all collider of your childrens
                    RemoveColliders(Selection.gameObjects);
                }
            }
            else
            {
                if (GUILayout.Button("Remove solo el del objeto"))
                {
                    //Solo remueve la suya
                    RemoveCollider(Selection.activeGameObject);
                }
            }
        }

        private void RemoveColliders(GameObject[] selecteds)
        {
            foreach (GameObject obj in selecteds)
            {
                Undo.RecordObject(obj, "Collider");

                if (obj.GetComponent<Collider>())
                {
                    foreach (Collider col in obj.GetComponents<Collider>())
                    {
                        Undo.DestroyObjectImmediate(col);
                        DestroyImmediate(col);
                    }

                    EditorUtility.SetDirty(obj);
                }

                GameObject[] childs = new GameObject[obj.transform.childCount];

                for (int i = 0; i < obj.transform.childCount; i++)
                {
                    childs[i] = obj.transform.GetChild(i).gameObject;
                }

                RemoveColliders(childs);
            }
        }

        private void RemoveCollider(GameObject obj)
        {           
            if (obj.GetComponent<Collider>())
            {
                foreach (Collider col in obj.GetComponents<Collider>())
                {
                    Undo.DestroyObjectImmediate(col);
                    DestroyImmediate(col);
                }

                EditorUtility.SetDirty(obj);
            }
        }
    }
}