using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GameFramework.Dialogue
{
    //TODO - Fix Selection Subscribing
    public class DialogueEditor : EditorWindow
    {
        [NonSerialized] private GUIStyle _nodeStyle;
        [NonSerialized] private GUIStyle _playerNodeStyle;
        [NonSerialized] private DialogueNode _draggingNode;
        [NonSerialized] private DialogueNode creatingNode;
        [NonSerialized] private DialogueNode deletingNode;
        [NonSerialized] private DialogueNode linkingParentNode;
        [NonSerialized] private Vector2 _draggingOffset;
        [NonSerialized] private Vector2 draggingCanvasOffset;
        [NonSerialized] private bool draggingCanvas;

        private Dialogue _selectedDialogue;
        private Vector2 _scrollPosition;

        private const float CanvasSize = 4000;
        private const float BackgroundSize = 50;

        [MenuItem("Window/Dialogue Editor")]
        public static void ShowEditorWindow()
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            var dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;
            if (dialogue == null)
                return false;

            ShowEditorWindow();

            return true;

        }

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            _nodeStyle = new GUIStyle
            {
                normal = { background = EditorGUIUtility.Load("node0") as Texture2D, textColor = Color.white },
                padding = new RectOffset(20, 20, 20, 20),
                border = new RectOffset(12, 12, 12, 12)
            };

            _playerNodeStyle = new GUIStyle
            {
                normal = { background = EditorGUIUtility.Load("node1") as Texture2D, textColor = Color.white },
                padding = new RectOffset(20, 20, 20, 20),
                border = new RectOffset(12, 12, 12, 12)
            };
        }

        private void OnDisable()
        {
            _nodeStyle = null;
            _playerNodeStyle = null;
        }

        private void OnSelectionChanged()
        {
            var newDialogue = Selection.activeObject as Dialogue;
            if (newDialogue == null)
                return;

            _selectedDialogue = newDialogue;
            Repaint();
        }

        private void OnGUI()
        {
            if (_selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();

                _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

                var canvas = GUILayoutUtility.GetRect(CanvasSize, CanvasSize);
                var backgroundTex = Resources.Load("background") as Texture2D;
                var texCoords = new Rect(0, 0, CanvasSize / BackgroundSize, CanvasSize / BackgroundSize);

                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

                foreach (var node in _selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }

                foreach (var node in _selectedDialogue.GetAllNodes())
                {
                    DrawNode(node);
                }

                EditorGUILayout.EndScrollView();

                if (creatingNode != null)
                {
                    _selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }

                if (deletingNode != null)
                {
                    _selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            switch (Event.current.type)
            {
                case EventType.MouseDown when _draggingNode == null:
                    {
                        _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                        if (_draggingNode != null)
                        {
                            _draggingOffset = _draggingNode.GetRect().position - Event.current.mousePosition;
                            Selection.activeObject = _draggingNode;
                        }
                        else
                        {
                            draggingCanvas = true;
                            draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                            Selection.activeObject = _selectedDialogue;
                        }

                        break;
                    }
                case EventType.MouseDrag when _draggingNode != null:
                    _draggingNode.SetPosition(Event.current.mousePosition + _draggingOffset);

                    GUI.changed = true;
                    break;
                case EventType.MouseDrag when draggingCanvas:
                    _scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

                    GUI.changed = true;
                    break;
                case EventType.MouseUp when _draggingNode != null:
                    _draggingNode = null;
                    break;
                case EventType.MouseUp when draggingCanvas:
                    draggingCanvas = false;
                    break;
                default:
                    return;
            }
        }

        private void DrawNode(DialogueNode node)
        {
            var style = _nodeStyle;

            if (node.IsPlayerSpeaking())
            {
                style = _playerNodeStyle;
            }

            GUILayout.BeginArea(node.GetRect(), style);
            EditorGUI.BeginChangeCheck();

            node.SetText(EditorGUILayout.TextField(node.GetText()));

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("x"))
            {
                deletingNode = node;
            }

            DrawLinkButtons(node);
            if (GUILayout.Button("+"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("unlink"))
                {
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else
            {
                if (GUILayout.Button("child"))
                {
                    Undo.RecordObject(_selectedDialogue, "Add Dialogue Link");
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)
        {
            var startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach (var childNode in _selectedDialogue.GetAllChildren(node))
            {
                var endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y);
                var controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;
                Handles.DrawBezier(
                    startPosition, endPosition,
                    startPosition + controlPointOffset,
                    endPosition - controlPointOffset,
                    Color.white, null, 4f);
            }
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;

            foreach (var node in _selectedDialogue.GetAllNodes())
            {
                if (node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }

            return foundNode;
        }
    }
}
