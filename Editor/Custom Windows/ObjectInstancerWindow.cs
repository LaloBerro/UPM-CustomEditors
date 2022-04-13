using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.Windows
{
    public class ObjectInstancerWindow : EditorWindow
    {
        #region VAR

        private bool useOnEditMode;

        private GameObject[] _objectsToInstance;

        private bool _useRandomRotation;
        private bool _useGroundRotation;

        private Transform _parent;

        private LayerMask _mask;

        private Vector3 _offset;

        #endregion

        [MenuItem("Custom Editor/Gameobject Instancer")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(ObjectInstancerWindow), false, "Gameobject Instancer");
        }

        void OnGUI()
        {
            useOnEditMode = EditorGUILayout.Toggle("Activar modo de edicion", useOnEditMode);

            if (useOnEditMode)
                DrawWindow();
        }

        private void DrawWindow()
        {
            EditorGUILayout.BeginVertical("box");
            {
                //GameObject Array
                {
                    ScriptableObject scriptableObj = this;
                    SerializedObject serialObj = new SerializedObject(scriptableObj);
                    SerializedProperty serialProp = serialObj.FindProperty("objectsToInstance");

                    EditorGUILayout.PropertyField(serialProp, true);
                    serialObj.ApplyModifiedProperties();
                }

                //Parent
                {
                    _parent = (Transform)EditorGUILayout.ObjectField("Parent ", _parent, typeof(Transform), true);
                }

                //Offset
                {
                    _offset = EditorGUILayout.Vector3Field("Offset", _offset);
                }

                //Rotacion
                {
                    _useRandomRotation = EditorGUILayout.Toggle("Random Rotacion", _useRandomRotation);
                    _useGroundRotation = EditorGUILayout.Toggle("Use ground rotation", _useGroundRotation);
                }

                //Layer Mask popup
                {
                    EditorGUILayout.LabelField("LayerMask");
                    LayerMask tempMask = EditorGUILayout.MaskField(InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_mask), InternalEditorUtility.layers);
                    _mask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView sv)
        {
            if (!useOnEditMode) return;

            Vector3 mousePos = Event.current.mousePosition;
            mousePos.y = sv.camera.pixelHeight - mousePos.y;

            //Ray ray = sv.camera.ScreenPointToRay(mousePos);
            RaycastHit hit;

            Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Handles.color = Color.red;
                Handles.DrawWireDisc(hit.point, hit.normal, 0.3f);

                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                    InstaceObject(hit.point, hit.normal);

                sv.Repaint();
            }
        }

        private void InstaceObject(Vector3 point, Vector3 normal)
        {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(_objectsToInstance[Random.Range(0, _objectsToInstance.Length)]);

            Undo.RegisterCreatedObjectUndo(go, "Instancer");

            go.transform.position = point + _offset;

            if (_useGroundRotation)
            {
                go.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);


                if (_useRandomRotation)
                    go.transform.Rotate(new Vector3(0, Random.Range(0, 180), 0));
            }

            go.transform.SetParent(_parent);
        }
    }
}