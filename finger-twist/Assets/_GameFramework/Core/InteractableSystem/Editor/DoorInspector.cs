using UnityEditor;
using UnityEngine;

namespace GameFramework.InteractableSystem
{
    [CustomEditor(typeof(Door), true)]
    public class DoorInspector : UnityEditor.Editor
    {
        private Door _door;

        private SerializedProperty _doorPointProperty;
        private SerializedProperty _startOpenPointProperty;
        private SerializedProperty _endOpenPointProperty;
        private SerializedProperty _startOpenRotationProperty;
        private SerializedProperty _endOpenRotationProperty;

        private void OnEnable()
        {
            _door = target as Door;
            _doorPointProperty = serializedObject.FindProperty("_doorPoint");
            _startOpenPointProperty = serializedObject.FindProperty("_startOpenPoint");
            _endOpenPointProperty = serializedObject.FindProperty("_endOpenPoint");
            _startOpenRotationProperty = serializedObject.FindProperty("_startOpenRotation");
            _endOpenRotationProperty = serializedObject.FindProperty("_endOpenRotation");
        }

        private void OnDisable()
        {
            _door = null;
            _startOpenPointProperty = null;
            _endOpenPointProperty = null;
            _startOpenRotationProperty = null;
            _endOpenRotationProperty = null;
        }

        private void OnSceneGUI()
        {
            var doorPoint = _doorPointProperty.objectReferenceValue as Transform;

            if (doorPoint == null)
                return;

            var start = doorPoint.TransformPoint(_startOpenPointProperty.vector3Value);
            var end = doorPoint.TransformPoint(_endOpenPointProperty.vector3Value);

            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                start = Handles.PositionHandle(start, Quaternion.AngleAxis(180, doorPoint.up) * doorPoint.rotation);
                Handles.Label(start, "Start", "button");
                Handles.Label(end, "End", "button");
                end = Handles.PositionHandle(end, doorPoint.rotation);

                if (cc.changed)
                {
                    Undo.RecordObject(doorPoint, "Move Handles");
                    _startOpenPointProperty.vector3Value = doorPoint.InverseTransformPoint(start);
                    _endOpenPointProperty.vector3Value = doorPoint.InverseTransformPoint(end);
                    serializedObject.ApplyModifiedProperties();
                }
            }

            Handles.color = Color.yellow;
            Handles.DrawDottedLine(start, end, 5);
            Handles.Label(Vector3.Lerp(start, end, 0.5f), "Distance:" + (end - start).magnitude);
        }
    }
}
