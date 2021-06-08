using System.Collections.Generic;
using PathCreation;
using PathCreation.Utility;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace PathCreationEditor
{
    /// Editor class for the creation of Bezier and Vertex paths
    [CustomEditor(typeof(PathCreator))]
    public class PathEditor : Editor
    {
        #region Fields
        // Interaction:
        private const float segmentSelectDistanceThreshold = 10f;
        private const float screenPolylineMaxAngleError = .3f;
        private const float screenPolylineMinVertexDst = .01f;

        // Help messages:
        private const string helpInfo = "Shift-click to add or insert new points. Control-click to delete points. For more detailed infomation, please refer to the documentation.";
        private static readonly string[] spaceNames = { "3D (xyz)", "2D (xy)", "Top-down (xz)" };
        private static readonly string[] tabNames = { "Bézier Path", "Vertex Path" };
        private const string constantSizeTooltip = "If true, anchor and control points will keep a constant size when zooming in the editor.";

        // Display
        private const int inspectorSectionSpacing = 10;
        private const float constantHandleScale = .01f;
        private const float normalsSpacing = .2f;
        private GUIStyle boldFoldoutStyle;

        // References:
        private PathCreator creator;
        private Editor globalDisplaySettingsEditor;
        private ScreenSpacePolyLine screenSpaceLine;
        private ScreenSpacePolyLine.MouseInfo pathMouseInfo;
        private GlobalDisplaySettings globalDisplaySettings;
        private PathHandle.HandleColours splineAnchorColours;
        private PathHandle.HandleColours splineControlColours;
        private Dictionary<GlobalDisplaySettings.HandleType, Handles.CapFunction> capFunctions;
        private ArcHandle anchorAngleHandle = new ArcHandle();
        private VertexPath normalsVertexPath;

        // State variables:
        private int selectedSegmentIndex;
        private int draggingHandleIndex;
        private int mouseOverHandleIndex;
        private int handleIndexToDisplayAsTransform;
        private bool shiftLastFrame;
        private bool hasUpdatedScreenSpaceLine;
        private bool hasUpdatedNormalsVertexPath;
        private bool editingNormalsOld;
        private Vector3 transformPos;
        private Vector3 transformScale;
        private Quaternion transformRot;
        private Color handlesStartColor;
        // Constants
        private const int bezierPathTab = 0;
        private const int vertexPathTab = 1;

        #endregion

        #region Inspectors

        public override void OnInspectorGUI()
        {
            // Initialize GUI styles
            if (boldFoldoutStyle == null)
            {
                boldFoldoutStyle = new GUIStyle(EditorStyles.foldout);
                boldFoldoutStyle.fontStyle = FontStyle.Bold;
            }

            Undo.RecordObject(creator, "Path settings changed");

            // Draw Bezier and Vertex tabs
            var tabIndex = GUILayout.Toolbar(Getdata.tabIndex, tabNames);
            if (tabIndex != Getdata.tabIndex)
            {
                Getdata.tabIndex = tabIndex;
                TabChanged();
            }

            // Draw inspector for active tab
            switch (Getdata.tabIndex)
            {
                case bezierPathTab:
                    DrawBezierPathInspector();
                    break;
                case vertexPathTab:
                    DrawVertexPathInspector();
                    break;
            }

            // Notify of undo/redo that might modify the path
            if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")
                Getdata.PathModifiedByUndo();
        }

        private void DrawBezierPathInspector()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                // Path options:
                Getdata.showPathOptions = EditorGUILayout.Foldout(Getdata.showPathOptions, new GUIContent("Bézier Path Options"), true, boldFoldoutStyle);
                if (Getdata.showPathOptions)
                {
                    GetBezierPath.Space = (PathSpace)EditorGUILayout.Popup("Space", (int)GetBezierPath.Space, spaceNames);
                    GetBezierPath.ControlPointMode = (BezierPath.ControlMode)EditorGUILayout.EnumPopup(new GUIContent("Control Mode"), GetBezierPath.ControlPointMode);
                    if (GetBezierPath.ControlPointMode == BezierPath.ControlMode.Automatic)
                    {
                        GetBezierPath.AutoControlLength = EditorGUILayout.Slider(new GUIContent("Control Spacing"), GetBezierPath.AutoControlLength, 0, 1);
                    }

                    GetBezierPath.IsClosed = EditorGUILayout.Toggle("Closed Path", GetBezierPath.IsClosed);
                    Getdata.showTransformTool = EditorGUILayout.Toggle(new GUIContent("Enable Transforms"), Getdata.showTransformTool);

                    Tools.hidden = !Getdata.showTransformTool;

                    // Check if out of bounds (can occur after undo operations)
                    if (handleIndexToDisplayAsTransform >= GetBezierPath.NumPoints)
                    {
                        handleIndexToDisplayAsTransform = -1;
                    }

                    // If a point has been selected
                    if (handleIndexToDisplayAsTransform != -1)
                    {
                        EditorGUILayout.LabelField("Selected Point:");

                        using (new EditorGUI.IndentLevelScope())
                        {
                            var currentPosition = creator.BezierPath[handleIndexToDisplayAsTransform];
                            var newPosition = EditorGUILayout.Vector3Field("Position", currentPosition);

                            if (newPosition != currentPosition)
                            {
                                Undo.RecordObject(creator, "Move point");
                                creator.BezierPath.MovePoint(handleIndexToDisplayAsTransform, newPosition);
                            }
                            // Don't draw the angle field if we aren't selecting an anchor point/not in 3d space
                            if (handleIndexToDisplayAsTransform % 3 == 0 && creator.BezierPath.Space == PathSpace.xyz)
                            {
                                var anchorIndex = handleIndexToDisplayAsTransform / 3;
                                var currentAngle = creator.BezierPath.GetAnchorNormalAngle(anchorIndex);
                                var newAngle = EditorGUILayout.FloatField("Angle", currentAngle);

                                if (newAngle != currentAngle)
                                {
                                    Undo.RecordObject(creator, "Set Angle");
                                    creator.BezierPath.SetAnchorNormalAngle(anchorIndex, newAngle);
                                }
                            }
                        }
                    }

                    if (Getdata.showTransformTool & (handleIndexToDisplayAsTransform == -1))
                    {
                        if (GUILayout.Button("Centre Transform"))
                        {
                            var worldCentre = GetBezierPath.CalculateBoundsWithTransform(creator.transform).center;
                            var transformPos = creator.transform.position;

                            if (GetBezierPath.Space == PathSpace.xy)
                                transformPos = new Vector3(transformPos.x, transformPos.y, 0);
                            else if (GetBezierPath.Space == PathSpace.xz)
                                transformPos = new Vector3(transformPos.x, 0, transformPos.z);

                            var worldCentreToTransform = transformPos - worldCentre;

                            if (worldCentre != creator.transform.position)
                            {
                                if (worldCentreToTransform != Vector3.zero)
                                {
                                    var localCentreToTransform = MathUtility.InverseTransformVector(worldCentreToTransform, creator.transform, GetBezierPath.Space);

                                    for (int i = 0; i < GetBezierPath.NumPoints; i++)
                                        GetBezierPath.SetPoint(i, GetBezierPath.GetPoint(i) + localCentreToTransform, true);
                                }

                                creator.transform.position = worldCentre;
                                GetBezierPath.NotifyPathModified();
                            }
                        }
                    }

                    if (GUILayout.Button("Reset Path"))
                    {
                        Undo.RecordObject(creator, "Reset Path");
                        var in2DEditorMode = EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D;
                        Getdata.ResetBezierPath(creator.transform.position, in2DEditorMode);
                        EditorApplication.QueuePlayerLoopUpdate();
                    }

                    GUILayout.Space(inspectorSectionSpacing);
                }

                Getdata.showNormals = EditorGUILayout.Foldout(Getdata.showNormals, new GUIContent("Normals Options"), true, boldFoldoutStyle);
                if (Getdata.showNormals)
                {
                    GetBezierPath.FlipNormals = EditorGUILayout.Toggle(new GUIContent("Flip Normals"), GetBezierPath.FlipNormals);
                    if (GetBezierPath.Space == PathSpace.xyz)
                    {
                        GetBezierPath.GlobalNormalsAngle = EditorGUILayout.Slider(new GUIContent("Global Angle"), GetBezierPath.GlobalNormalsAngle, 0, 360);

                        if (GUILayout.Button("Reset Normals"))
                        {
                            Undo.RecordObject(creator, "Reset Normals");
                            GetBezierPath.FlipNormals = false;
                            GetBezierPath.ResetNormalAngles();
                        }
                    }
                    GUILayout.Space(inspectorSectionSpacing);
                }

                // Editor display options
                Getdata.showDisplayOptions = EditorGUILayout.Foldout(Getdata.showDisplayOptions, new GUIContent("Display Options"), true, boldFoldoutStyle);
                if (Getdata.showDisplayOptions)
                {
                    Getdata.showPathBounds = GUILayout.Toggle(Getdata.showPathBounds, new GUIContent("Show Path Bounds"));
                    Getdata.showPerSegmentBounds = GUILayout.Toggle(Getdata.showPerSegmentBounds, new GUIContent("Show Segment Bounds"));
                    Getdata.displayAnchorPoints = GUILayout.Toggle(Getdata.displayAnchorPoints, new GUIContent("Show Anchor Points"));

                    if (!(GetBezierPath.ControlPointMode == BezierPath.ControlMode.Automatic && globalDisplaySettings.hideAutoControls))
                        Getdata.displayControlPoints = GUILayout.Toggle(Getdata.displayControlPoints, new GUIContent("Show Control Points"));

                    Getdata.keepConstantHandleSize = GUILayout.Toggle(Getdata.keepConstantHandleSize, new GUIContent("Constant Point Size", constantSizeTooltip));
                    Getdata.bezierHandleScale = Mathf.Max(0, EditorGUILayout.FloatField(new GUIContent("Handle Scale"), Getdata.bezierHandleScale));
                    DrawGlobalDisplaySettingsInspector();
                }

                if (check.changed)
                {
                    SceneView.RepaintAll();
                    EditorApplication.QueuePlayerLoopUpdate();
                }
            }
        }

        private void DrawVertexPathInspector()
        {
            GUILayout.Space(inspectorSectionSpacing);
            EditorGUILayout.LabelField("Vertex count: " + creator.Path.NumPoints);
            GUILayout.Space(inspectorSectionSpacing);

            Getdata.showVertexPathOptions = EditorGUILayout.Foldout(Getdata.showVertexPathOptions, new GUIContent("Vertex Path Options"), true, boldFoldoutStyle);
            if (Getdata.showVertexPathOptions)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    Getdata.vertexPathMaxAngleError = EditorGUILayout.Slider(new GUIContent("Max Angle Error"), Getdata.vertexPathMaxAngleError, 0, 45);
                    Getdata.vertexPathMinVertexSpacing = EditorGUILayout.Slider(new GUIContent("Min Vertex Dst"), Getdata.vertexPathMinVertexSpacing, 0, 1);

                    GUILayout.Space(inspectorSectionSpacing);
                    if (check.changed)
                    {
                        Getdata.VertexPathSettingsChanged();
                        SceneView.RepaintAll();
                        EditorApplication.QueuePlayerLoopUpdate();
                    }
                }
            }

            Getdata.showVertexPathDisplayOptions = EditorGUILayout.Foldout(Getdata.showVertexPathDisplayOptions, new GUIContent("Display Options"), true, boldFoldoutStyle);
            if (Getdata.showVertexPathDisplayOptions)
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    Getdata.showNormalsInVertexMode = GUILayout.Toggle(Getdata.showNormalsInVertexMode, new GUIContent("Show Normals"));
                    Getdata.showBezierPathInVertexMode = GUILayout.Toggle(Getdata.showBezierPathInVertexMode, new GUIContent("Show Bezier Path"));

                    if (check.changed)
                    {
                        SceneView.RepaintAll();
                        EditorApplication.QueuePlayerLoopUpdate();
                    }
                }
                DrawGlobalDisplaySettingsInspector();
            }
        }

        private void DrawGlobalDisplaySettingsInspector()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                Getdata.globalDisplaySettingsFoldout = EditorGUILayout.InspectorTitlebar(Getdata.globalDisplaySettingsFoldout, globalDisplaySettings);
                if (Getdata.globalDisplaySettingsFoldout)
                {
                    CreateCachedEditor(globalDisplaySettings, null, ref globalDisplaySettingsEditor);
                    globalDisplaySettingsEditor.OnInspectorGUI();
                }
                if (check.changed)
                {
                    UpdateGlobalDisplaySettings();
                    SceneView.RepaintAll();
                }
            }
        }

        #endregion

        #region Scene GUI

        private void OnSceneGUI()
        {
            if (!globalDisplaySettings.visibleBehindObjects)
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;

            var eventType = Event.current.type;

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                handlesStartColor = Handles.color;
                switch (Getdata.tabIndex)
                {
                    case bezierPathTab:
                        if (eventType != EventType.Repaint && eventType != EventType.Layout)
                            ProcessBezierPathInput(Event.current);

                        DrawBezierPathSceneEditor();
                        break;
                    case vertexPathTab:
                        if (eventType == EventType.Repaint)
                            DrawVertexPathSceneEditor();
                        break;
                }

                // Don't allow clicking over empty space to deselect the object
                if (eventType == EventType.Layout)
                    HandleUtility.AddDefaultControl(0);

                if (check.changed)
                    EditorApplication.QueuePlayerLoopUpdate();
            }

            SetTransformState();
        }

        private void DrawVertexPathSceneEditor()
        {

            var bezierColor = globalDisplaySettings.bezierPath;
            bezierColor.a *= .5f;

            if (Getdata.showBezierPathInVertexMode)
            {
                for (int i = 0; i < GetBezierPath.NumSegments; i++)
                {
                    var points = GetBezierPath.GetPointsInSegment(i);

                    for (int j = 0; j < points.Length; j++)
                        points[j] = MathUtility.TransformPoint(points[j], creator.transform, GetBezierPath.Space);

                    Handles.DrawBezier(points[0], points[3], points[1], points[2], bezierColor, null, 2);
                }
            }

            Handles.color = globalDisplaySettings.vertexPath;

            for (int i = 0; i < creator.Path.NumPoints; i++)
            {
                var nextIndex = (i + 1) % creator.Path.NumPoints;

                if (nextIndex != 0 || GetBezierPath.IsClosed)
                    Handles.DrawLine(creator.Path.GetPoint(i), creator.Path.GetPoint(nextIndex));
            }

            if (Getdata.showNormalsInVertexMode)
            {
                Handles.color = globalDisplaySettings.normals;

                var normalLines = new Vector3[creator.Path.NumPoints * 2];

                for (int i = 0; i < creator.Path.NumPoints; i++)
                {
                    normalLines[i * 2] = creator.Path.GetPoint(i);
                    normalLines[i * 2 + 1] = creator.Path.GetPoint(i) + creator.Path.localNormals[i] * globalDisplaySettings.normalsLength;
                }
                Handles.DrawLines(normalLines);
            }
        }

        private void ProcessBezierPathInput(Event e)
        {
            // Find which handle mouse is over. Start by looking at previous handle index first, as most likely to still be closest to mouse
            var previousMouseOverHandleIndex = (mouseOverHandleIndex == -1) ? 0 : mouseOverHandleIndex;

            mouseOverHandleIndex = -1;

            for (int i = 0; i < GetBezierPath.NumPoints; i += 3)
            {
                var handleIndex = (previousMouseOverHandleIndex + i) % GetBezierPath.NumPoints;
                var handleRadius = GetHandleDiameter(globalDisplaySettings.anchorSize * Getdata.bezierHandleScale, GetBezierPath[handleIndex]) / 2f;
                var pos = MathUtility.TransformPoint(GetBezierPath[handleIndex], creator.transform, GetBezierPath.Space);
                var dst = HandleUtility.DistanceToCircle(pos, handleRadius);

                if (dst == 0)
                {
                    mouseOverHandleIndex = handleIndex;
                    break;
                }
            }

            // Shift-left click (when mouse not over a handle) to split or add segment
            if (mouseOverHandleIndex == -1)
            {
                if (e.type == EventType.MouseDown && e.button == 0 && e.shift)
                {
                    UpdatePathMouseInfo();
                    // Insert point along selected segment
                    if (selectedSegmentIndex != -1 && selectedSegmentIndex < GetBezierPath.NumSegments)
                    {
                        var newPathPoint = pathMouseInfo.ClosestWorldPointToMouse;

                        newPathPoint = MathUtility.InverseTransformPoint(newPathPoint, creator.transform, GetBezierPath.Space);
                        Undo.RecordObject(creator, "Split segment");
                        GetBezierPath.SplitSegment(newPathPoint, selectedSegmentIndex, pathMouseInfo.TimeOnBezierSegment);
                    }
                    // If path is not a closed loop, add new point on to the end of the path
                    else if (!GetBezierPath.IsClosed)
                    {
                        // insert new point at same dst from scene camera as the point that comes before it (for a 3d path)
                        var dstCamToEndpoint = (Camera.current.transform.position - GetBezierPath[GetBezierPath.NumPoints - 1]).magnitude;
                        var newPathPoint = MouseUtility.GetMouseWorldPosition(GetBezierPath.Space, dstCamToEndpoint);

                        newPathPoint = MathUtility.InverseTransformPoint(newPathPoint, creator.transform, GetBezierPath.Space);

                        Undo.RecordObject(creator, "Add segment");
                        if (e.control || e.command)
                            GetBezierPath.AddSegmentToStart(newPathPoint);
                        else
                            GetBezierPath.AddSegmentToEnd(newPathPoint);
                    }
                }
            }
            // Control click or backspace/delete to remove point
            if (e.keyCode == KeyCode.Backspace || e.keyCode == KeyCode.Delete || ((e.control || e.command) && e.type == EventType.MouseDown && e.button == 0))
            {
                if (mouseOverHandleIndex != -1)
                {
                    Undo.RecordObject(creator, "Delete segment");
                    GetBezierPath.DeleteSegment(mouseOverHandleIndex);

                    if (mouseOverHandleIndex == handleIndexToDisplayAsTransform)
                        handleIndexToDisplayAsTransform = -1;

                    mouseOverHandleIndex = -1;
                    Repaint();
                }
            }

            // Holding shift and moving mouse (but mouse not over a handle/dragging a handle)
            if (draggingHandleIndex == -1 && mouseOverHandleIndex == -1)
            {
                var shiftDown = e.shift && !shiftLastFrame;

                if (shiftDown || ((e.type == EventType.MouseMove || e.type == EventType.MouseDrag) && e.shift))
                {
                    UpdatePathMouseInfo();

                    if (pathMouseInfo.MouseDstToLine < segmentSelectDistanceThreshold)
                    {
                        if (pathMouseInfo.ClosestSegmentIndex != selectedSegmentIndex)
                        {
                            selectedSegmentIndex = pathMouseInfo.ClosestSegmentIndex;
                            HandleUtility.Repaint();
                        }
                    }
                    else
                    {
                        selectedSegmentIndex = -1;
                        HandleUtility.Repaint();
                    }

                }
            }
            shiftLastFrame = e.shift;

        }

        private void DrawBezierPathSceneEditor()
        {
            var displayControlPoints = Getdata.displayControlPoints && (GetBezierPath.ControlPointMode != BezierPath.ControlMode.Automatic || !globalDisplaySettings.hideAutoControls);
            var bounds = GetBezierPath.CalculateBoundsWithTransform(creator.transform);

            if (Event.current.type == EventType.Repaint)
            {
                for (int i = 0; i < GetBezierPath.NumSegments; i++)
                {
                    var points = GetBezierPath.GetPointsInSegment(i);

                    for (int j = 0; j < points.Length; j++)
                        points[j] = MathUtility.TransformPoint(points[j], creator.transform, GetBezierPath.Space);

                    if (Getdata.showPerSegmentBounds)
                    {
                        var segmentBounds = CubicBezierUtility.CalculateSegmentBounds(points[0], points[1], points[2], points[3]);

                        Handles.color = globalDisplaySettings.segmentBounds;
                        Handles.DrawWireCube(segmentBounds.center, segmentBounds.size);
                    }

                    // Draw lines between control points
                    if (displayControlPoints)
                    {
                        Handles.color = (GetBezierPath.ControlPointMode == BezierPath.ControlMode.Automatic) ? globalDisplaySettings.handleDisabled : globalDisplaySettings.controlLine;
                        Handles.DrawLine(points[1], points[0]);
                        Handles.DrawLine(points[2], points[3]);
                    }

                    // Draw path
                    var highlightSegment = (i == selectedSegmentIndex && Event.current.shift && draggingHandleIndex == -1 && mouseOverHandleIndex == -1);
                    var segmentColor = (highlightSegment) ? globalDisplaySettings.highlightedPath : globalDisplaySettings.bezierPath;

                    Handles.DrawBezier(points[0], points[3], points[1], points[2], segmentColor, null, 2);
                }

                if (Getdata.showPathBounds)
                {
                    Handles.color = globalDisplaySettings.bounds;
                    Handles.DrawWireCube(bounds.center, bounds.size);
                }

                // Draw normals
                if (Getdata.showNormals)
                {
                    if (!hasUpdatedNormalsVertexPath)
                    {
                        normalsVertexPath = new VertexPath(GetBezierPath, creator.transform, normalsSpacing);
                        hasUpdatedNormalsVertexPath = true;
                    }

                    if (editingNormalsOld != Getdata.showNormals)
                    {
                        editingNormalsOld = Getdata.showNormals;
                        Repaint();
                    }

                    var normalLines = new Vector3[normalsVertexPath.NumPoints * 2];

                    Handles.color = globalDisplaySettings.normals;

                    for (int i = 0; i < normalsVertexPath.NumPoints; i++)
                    {
                        normalLines[i * 2] = normalsVertexPath.GetPoint(i);
                        normalLines[i * 2 + 1] = normalsVertexPath.GetPoint(i) + normalsVertexPath.GetNormal(i) * globalDisplaySettings.normalsLength;
                    }

                    Handles.DrawLines(normalLines);
                }
            }

            if (Getdata.displayAnchorPoints)
            {
                for (int i = 0; i < GetBezierPath.NumPoints; i += 3)
                    DrawHandle(i);
            }
            if (displayControlPoints)
            {
                for (int i = 1; i < GetBezierPath.NumPoints - 1; i += 3)
                {
                    DrawHandle(i);
                    DrawHandle(i + 1);
                }
            }
        }

        private void DrawHandle(int index)
        {
            var handlePosition = MathUtility.TransformPoint(GetBezierPath[index], creator.transform, GetBezierPath.Space);
            var anchorHandleSize = GetHandleDiameter(globalDisplaySettings.anchorSize * Getdata.bezierHandleScale, GetBezierPath[index]);
            var controlHandleSize = GetHandleDiameter(globalDisplaySettings.controlSize * Getdata.bezierHandleScale, GetBezierPath[index]);
            var isAnchorPoint = index % 3 == 0;
            var isInteractive = isAnchorPoint || GetBezierPath.ControlPointMode != BezierPath.ControlMode.Automatic;
            var handleSize = (isAnchorPoint) ? anchorHandleSize : controlHandleSize;
            var doTransformHandle = index == handleIndexToDisplayAsTransform;

            PathHandle.HandleColours handleColours = (isAnchorPoint) ? splineAnchorColours : splineControlColours;

            if (index == handleIndexToDisplayAsTransform)
                handleColours.DefaultColor = (isAnchorPoint) ? globalDisplaySettings.anchorSelected : globalDisplaySettings.controlSelected;

            var cap = capFunctions[(isAnchorPoint) ? globalDisplaySettings.anchorShape : globalDisplaySettings.controlShape];

            PathHandle.HandleInputType handleInputType;

            handlePosition = PathHandle.DrawHandle(handlePosition, GetBezierPath.Space, isInteractive, handleSize, cap, handleColours, out handleInputType, index);

            if (doTransformHandle)
            {
                // Show normals rotate tool 
                if (Getdata.showNormals && Tools.current == Tool.Rotate && isAnchorPoint && GetBezierPath.Space == PathSpace.xyz)
                {
                    Handles.color = handlesStartColor;

                    var attachedControlIndex = (index == GetBezierPath.NumPoints - 1) ? index - 1 : index + 1;
                    var dir = (GetBezierPath[attachedControlIndex] - handlePosition).normalized;
                    var handleRotOffset = (360 + GetBezierPath.GlobalNormalsAngle) % 360;

                    anchorAngleHandle.radius = handleSize * 3;
                    anchorAngleHandle.angle = handleRotOffset + GetBezierPath.GetAnchorNormalAngle(index / 3);
                    var handleDirection = Vector3.Cross(dir, Vector3.up);
                    var handleMatrix = Matrix4x4.TRS(
                        handlePosition,
                        Quaternion.LookRotation(handleDirection, dir),
                        Vector3.one
                    );

                    using (new Handles.DrawingScope(handleMatrix))
                    {
                        // draw the handle
                        EditorGUI.BeginChangeCheck();
                        anchorAngleHandle.DrawHandle();
                        if (EditorGUI.EndChangeCheck())
                        {
                            Undo.RecordObject(creator, "Set angle");
                            GetBezierPath.SetAnchorNormalAngle(index / 3, anchorAngleHandle.angle - handleRotOffset);
                        }
                    }

                }
                else
                {
                    handlePosition = Handles.DoPositionHandle(handlePosition, Quaternion.identity);
                }

            }

            switch (handleInputType)
            {
                case PathHandle.HandleInputType.LMBDrag:
                    draggingHandleIndex = index;
                    handleIndexToDisplayAsTransform = -1;
                    Repaint();
                    break;
                case PathHandle.HandleInputType.LMBRelease:
                    draggingHandleIndex = -1;
                    handleIndexToDisplayAsTransform = -1;
                    Repaint();
                    break;
                case PathHandle.HandleInputType.LMBClick:
                    draggingHandleIndex = -1;

                    if (Event.current.shift)
                    {
                        handleIndexToDisplayAsTransform = -1; // disable move tool if new point added
                    }
                    else
                    {
                        if (handleIndexToDisplayAsTransform == index)
                            handleIndexToDisplayAsTransform = -1; // disable move tool if clicking on point under move tool
                        else
                            handleIndexToDisplayAsTransform = index;
                    }

                    Repaint();
                    break;
                case PathHandle.HandleInputType.LMBPress:
                    if (handleIndexToDisplayAsTransform != index)
                    {
                        handleIndexToDisplayAsTransform = -1;
                        Repaint();
                    }
                    break;
            }

            var localHandlePosition = MathUtility.InverseTransformPoint(handlePosition, creator.transform, GetBezierPath.Space);

            if (GetBezierPath[index] != localHandlePosition)
            {
                Undo.RecordObject(creator, "Move point");
                GetBezierPath.MovePoint(index, localHandlePosition);
            }

        }

        #endregion

        #region Internal methods

        private void OnDisable() 
            => Tools.hidden = false;

        private void OnEnable()
        {
            var in2DEditorMode = EditorSettings.defaultBehaviorMode == EditorBehaviorMode.Mode2D;

            creator = (PathCreator)target;
            creator.InitializeEditorData(in2DEditorMode);

            Getdata.bezierCreated -= ResetState;
            Getdata.bezierCreated += ResetState;

            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;

            LoadDisplaySettings();
            UpdateGlobalDisplaySettings();
            ResetState();
            SetTransformState(true);
        }

        private void SetTransformState(bool initialize = false)
        {
            var creatorTransform = creator.transform;

            if (!initialize)
            {
                if (transformPos != creatorTransform.position || creatorTransform.localScale != transformScale || creatorTransform.rotation != transformRot)
                {
                    Getdata.PathTransformed();
                }
            }

            transformPos = creatorTransform.position;
            transformScale = creatorTransform.localScale;
            transformRot = creatorTransform.rotation;
        }

        private void OnUndoRedo()
        {
            hasUpdatedScreenSpaceLine = false;
            hasUpdatedNormalsVertexPath = false;
            selectedSegmentIndex = -1;

            Repaint();
        }

        private void TabChanged()
        {
            SceneView.RepaintAll();
            RepaintUnfocusedSceneViews();
        }

        private void LoadDisplaySettings()
        {
            globalDisplaySettings = GlobalDisplaySettings.Load();

            capFunctions = new Dictionary<GlobalDisplaySettings.HandleType, Handles.CapFunction>();

            capFunctions.Add(GlobalDisplaySettings.HandleType.Circle, Handles.CylinderHandleCap);
            capFunctions.Add(GlobalDisplaySettings.HandleType.Sphere, Handles.SphereHandleCap);
            capFunctions.Add(GlobalDisplaySettings.HandleType.Square, Handles.CubeHandleCap);
        }

        private void UpdateGlobalDisplaySettings()
        {
            var gds = globalDisplaySettings;

            splineAnchorColours = new PathHandle.HandleColours(gds.anchor, gds.anchorHighlighted, gds.anchorSelected, gds.handleDisabled);
            splineControlColours = new PathHandle.HandleColours(gds.control, gds.controlHighlighted, gds.controlSelected, gds.handleDisabled);

            anchorAngleHandle.fillColor = new Color(1, 1, 1, .05f);
            anchorAngleHandle.wireframeColor = Color.grey;
            anchorAngleHandle.radiusHandleColor = Color.clear;
            anchorAngleHandle.angleHandleColor = Color.white;
        }

        private void ResetState()
        {
            selectedSegmentIndex = -1;
            draggingHandleIndex = -1;
            mouseOverHandleIndex = -1;
            handleIndexToDisplayAsTransform = -1;
            hasUpdatedScreenSpaceLine = false;
            hasUpdatedNormalsVertexPath = false;

            GetBezierPath.OnModified -= OnPathModifed;
            GetBezierPath.OnModified += OnPathModifed;

            SceneView.RepaintAll();
            EditorApplication.QueuePlayerLoopUpdate();
        }

        private void OnPathModifed()
        {
            hasUpdatedScreenSpaceLine = false;
            hasUpdatedNormalsVertexPath = false;

            RepaintUnfocusedSceneViews();
        }

        private void RepaintUnfocusedSceneViews()
        {
            // If multiple scene views are open, repaint those which do not have focus.
            if (SceneView.sceneViews.Count > 1)
            {
                foreach (SceneView sv in SceneView.sceneViews)
                {
                    if (EditorWindow.focusedWindow != sv)
                        sv.Repaint();
                }
            }
        }

        private void UpdatePathMouseInfo()
        {
            if (!hasUpdatedScreenSpaceLine || (screenSpaceLine != null && screenSpaceLine.TransformIsOutOfDate()))
            {
                screenSpaceLine = new ScreenSpacePolyLine(GetBezierPath, creator.transform, screenPolylineMaxAngleError, screenPolylineMinVertexDst);
                hasUpdatedScreenSpaceLine = true;
            }

            pathMouseInfo = screenSpaceLine.CalculateMouseInfo();
        }

        private float GetHandleDiameter(float diameter, Vector3 handlePosition)
        {
            var scaledDiameter = diameter * constantHandleScale;

            if (Getdata.keepConstantHandleSize)
                scaledDiameter *= HandleUtility.GetHandleSize(handlePosition) * 2.5f;

            return scaledDiameter;
        }

        private BezierPath GetBezierPath => Getdata.bezierPath;
        private PathCreatorData Getdata => creator.EditorData;
        private bool EditingNormals => Tools.current == Tool.Rotate && handleIndexToDisplayAsTransform % 3 == 0 && GetBezierPath.Space == PathSpace.xyz;

        #endregion
    }
}