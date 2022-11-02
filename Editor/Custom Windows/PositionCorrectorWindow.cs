using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UtilitiesCustomPackage.EditorExtensions.Windows
{
    public class PositionCorrectorWindow : EditorWindow
    {
        private LayerMask _mask;

        private float _offset;

        private bool _affectRotation;

        [MenuItem("Tools/Position Corrector")]
        private static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(PositionCorrectorWindow), false, "Position Corrector");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical("box");

            _affectRotation = EditorGUILayout.Toggle("Affect rotation", _affectRotation);

            EditorGUILayout.LabelField("LayerMask");
            LayerMask tempMask = EditorGUILayout.MaskField(InternalEditorUtility.LayerMaskToConcatenatedLayersMask(_mask), InternalEditorUtility.layers);
            _mask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

            EditorGUILayout.LabelField("Buried level");
            _offset = EditorGUILayout.FloatField(_offset);

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Set"))
            {
                SetPositions(Selection.gameObjects);
            }
        }

        private void SetPositions(GameObject[] gos)
        {
            for (int i = 0; i < gos.Length; i++)
            {
                Undo.RecordObject(gos[i].transform, "position");

                if (_affectRotation)
                    gos[i].transform.rotation = GetRotation(gos[i].transform);

                gos[i].transform.position = GetPosition(gos[i].transform) + (_affectRotation ? new Vector3(0, _offset, 0) : Vector3.zero);

                EditorUtility.SetDirty(gos[i]);
            }
        }

        private Vector3 GetPosition(Transform go)
        {
            if (Physics.Raycast(go.position, Vector3.down * 10, out RaycastHit hit, 1000, _mask))
            {
                return hit.point + hit.normal.normalized * _offset;
            }

            return go.position;
        }

        private Quaternion GetRotation(Transform go)
        {
            if (Physics.Raycast(go.position, Vector3.down * 10, out RaycastHit hit, 1000, _mask))
            {
                return Quaternion.FromToRotation(Vector3.up, hit.normal); ;
            }

            return go.rotation;
        }
    }
}