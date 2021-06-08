﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PathCreation;

namespace PathCreationEditor
{
    public static class PathHandle
    {
        public static readonly float ExtraInputRadius = .005f;

        private static Vector2 handleDragMouseStart;
        private static Vector2 handleDragMouseEnd;
        private static Vector3 handleDragWorldStart;
        private static int selectedHandleID;
        private static bool mouseIsOverAHandle;
        private static float dstMouseToDragPointStart;
        private static HashSet<int> idHash;
        private static List<int> ids;

        public enum HandleInputType
        {
            None,
            LMBPress,
            LMBClick,
            LMBDrag,
            LMBRelease,
        };


        static PathHandle()
        {
            ids = new List<int>();
            idHash = new HashSet<int>();

            dstMouseToDragPointStart = float.MaxValue;
        }

        public static Vector3 DrawHandle(Vector3 position, PathSpace space, bool isInteractive, float handleDiameter, Handles.CapFunction capFunc, HandleColours colours, out HandleInputType inputType, int handleIndex)
        {
            var id = GetID(handleIndex);
            var screenPosition = Handles.matrix.MultiplyPoint(position);
            var cachedMatrix = Handles.matrix;

            inputType = HandleInputType.None;

            EventType eventType = Event.current.GetTypeForControl(id);

            var handleRadius = handleDiameter / 2f;
            var dstToHandle = HandleUtility.DistanceToCircle(position, handleRadius + ExtraInputRadius);
            var dstToMouse = HandleUtility.DistanceToCircle(position, 0);

            // Handle input events
            if (isInteractive)
            {
                // Repaint if mouse is entering/exiting handle (for highlight colour)
                if (dstToHandle == 0)
                {
                    if (!mouseIsOverAHandle)
                    {
                        HandleUtility.Repaint();
                        mouseIsOverAHandle = true;
                    }
                }
                else
                {
                    if (mouseIsOverAHandle)
                    {
                        HandleUtility.Repaint();
                        mouseIsOverAHandle = false;
                    }
                }
                switch (eventType)
                {
                    case EventType.MouseDown:
                        if (Event.current.button == 0 && Event.current.modifiers != EventModifiers.Alt)
                        {
                            if (dstToHandle == 0 && dstToMouse < dstMouseToDragPointStart)
                            {
                                dstMouseToDragPointStart = dstToMouse;
                                GUIUtility.hotControl = id;
                                handleDragMouseEnd = handleDragMouseStart = Event.current.mousePosition;
                                handleDragWorldStart = position;
                                selectedHandleID = id;
                                inputType = HandleInputType.LMBPress;
                            }
                        }
                        break;

                    case EventType.MouseUp:
                        dstMouseToDragPointStart = float.MaxValue;
                        if (GUIUtility.hotControl == id && Event.current.button == 0)
                        {
                            GUIUtility.hotControl = 0;
                            selectedHandleID = -1;
                            Event.current.Use();

                            inputType = HandleInputType.LMBRelease;

                            if (Event.current.mousePosition == handleDragMouseStart)
                                inputType = HandleInputType.LMBClick;
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == id && Event.current.button == 0)
                        {
                            handleDragMouseEnd += new Vector2(Event.current.delta.x, -Event.current.delta.y);
                            var position2 = Camera.current.WorldToScreenPoint(Handles.matrix.MultiplyPoint(handleDragWorldStart))
                                + (Vector3)(handleDragMouseEnd - handleDragMouseStart);
                            inputType = HandleInputType.LMBDrag;
                            // Handle can move freely in 3d space
                            if (space == PathSpace.xyz)
                                position = Handles.matrix.inverse.MultiplyPoint(Camera.current.ScreenToWorldPoint(position2));
                            // Handle is clamped to xy or xz plane
                            else
                                position = MouseUtility.GetMouseWorldPosition(space);

                            GUI.changed = true;
                            Event.current.Use();
                        }
                        break;
                }
            }

            switch (eventType)
            {
                case EventType.Repaint:
                    var originalColor = Handles.color;

                    Handles.color = (isInteractive) ? colours.DefaultColor : colours.DisabledColor;

                    if (id == GUIUtility.hotControl)
                        Handles.color = colours.SelectedColor;
                    else if (dstToHandle == 0 && selectedHandleID == -1 && isInteractive)
                        Handles.color = colours.HighlightedColor;

                    Handles.matrix = Matrix4x4.identity;

                    var lookForward = Vector3.up;
                    var cam = Camera.current;

                    if (cam != null)
                    {
                        if (cam.orthographic)
                            lookForward = -cam.transform.forward;
                        else
                            lookForward = (cam.transform.position - position);
                    }

                    capFunc(id, screenPosition, Quaternion.LookRotation(lookForward), handleDiameter, EventType.Repaint);
                    Handles.matrix = cachedMatrix;

                    Handles.color = originalColor;
                    break;

                case EventType.Layout:
                    Handles.matrix = Matrix4x4.identity;
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(screenPosition, handleDiameter / 2f));
                    Handles.matrix = cachedMatrix;
                    break;
            }

            return position;
        }

        public struct HandleColours
        {
            public Color DefaultColor;
            public Color HighlightedColor;
            public Color SelectedColor;
            public Color DisabledColor;

            public HandleColours(Color defaultColor, Color highlightedColor, Color selectedColor, Color disabledColor)
            {
                DefaultColor = defaultColor;
                HighlightedColor = highlightedColor;
                SelectedColor = selectedColor;
                DisabledColor = disabledColor;
            }
        }

        private static void AddIDs(int upToIndex)
        {
            var numIDAtStart = ids.Count;
            var numToAdd = (upToIndex - numIDAtStart) + 1;

            for (int i = 0; i < numToAdd; i++)
            {
                var hashString = string.Format("pathhandle({0})", numIDAtStart + i);
                var hash = hashString.GetHashCode();
                var id = GUIUtility.GetControlID(hash, FocusType.Passive);
                var numIts = 0;

                // This is a bit of a shot in the dark at fixing a reported bug that I've been unable to reproduce.
                // The problem is that multiple handles are being selected when just one is clicked on.
                // I assume this is because they're somehow being assigned the same id.
                while (idHash.Contains(id))
                {
                    numIts++;
                    id += numIts * numIts;
                    if (numIts > 100)
                    {
                        Debug.LogError("Failed to generate unique handle id.");
                        break;
                    }
                }

                idHash.Add(id);
                ids.Add(id);
            }
        }

        private static int GetID(int handleIndex)
        {
            if (handleIndex >= ids.Count)
                AddIDs(handleIndex);

            return ids[handleIndex];
        }
    }
}