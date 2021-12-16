#if UNITY_EDITOR

using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace AeternumGames.ShapeEditor
{
    /// <summary>
    /// The 2D Shape Editor Window.
    /// </summary>
    public partial class ShapeEditorWindow : EditorWindow
    {
        /// <summary>
        /// The currently loaded project.
        /// </summary>
        [SerializeField]
        private Project project = new Project();

        [MenuItem("Window/2D Shape Editor")]
        public static void Init()
        {
            // get existing open window or if none, make a new one:
            ShapeEditorWindow window = GetWindow<ShapeEditorWindow>();
            window.minSize = new float2(800, 600);
            window.Show();
            window.titleContent = new GUIContent("Shape Editor", ShapeEditorResources.Instance.shapeEditorIcon);
            window.minSize = new float2(128, 128);
        }

        public static ShapeEditorWindow InitAndGetHandle()
        {
            Init();
            return GetWindow<ShapeEditorWindow>();
        }

        private void OnRepaint()
        {
            DrawViewport();

            foreach (Shape shape in project.shapes)
            {
                foreach (Segment segment in shape.segments)
                {
                    // draw pivots of the segments.
                    var segmentScreenPosition = GridPointToScreen(segment.position);
                    Handles.DrawSolidRectangleWithOutline(new Rect(segmentScreenPosition - halfPivotScale, new float2(pivotScale)), Color.white, Color.black);
                }
            }
        }

        private void OnMouseDown(int button)
        {
            Undo.RecordObject(this, "Move Pivot");
            project.shapes[0].segments[0].position += new float2(0.1f, 0.0f);
            Repaint();
        }

        private void OnMouseUp(int button)
        {
            Repaint();
        }

        private void OnMouseDrag(int button, float2 delta)
        {
            // pan the viewport around with the right mouse button.
            if (isRightMousePressed)
            {
                gridOffset += delta;
            }

            Repaint();
        }

        private void OnMouseScroll(float delta)
        {
            var mouseBeforeZoom = ScreenPointToGrid(mousePosition);

            gridZoom *= math.pow(2, -delta / 24.0f); // what about math.exp(-delta / 24.0f); ?

            // recalculate the grid offset to zoom into whatever is under the mouse cursor.
            var mouseAfterZoom = ScreenPointToGrid(mousePosition);
            var mouseDifference = mouseAfterZoom - mouseBeforeZoom;
            gridOffset = RenderTextureGridPointToScreen(mouseDifference);

            Repaint();
        }

        private bool OnKeyDown(KeyCode keyCode)
        {
            switch (keyCode)
            {
                case KeyCode.H:
                    GridResetOffset();
                    GridResetZoom();
                    Repaint();
                    return true;
            }
            return false;
        }

        private bool OnKeyUp(KeyCode keyCode)
        {
            return false;
        }

        private void OnTopToolbarGUI()
        {
            if (GUILayout.Button(new GUIContent(ShapeEditorResources.Instance.shapeEditorNew, "New Project (N)"), ShapeEditorResources.toolbarButtonStyle))
            {
            }

            GUILayout.FlexibleSpace();
        }

        private void OnBottomToolbarGUI()
        {
            GUILayout.Label("2D Shape Editor");

            GUILayout.FlexibleSpace();

            GUILayout.Label("Snap");
            gridSnap = EditorGUILayout.FloatField(gridSnap, GUILayout.Width(64f));

            GUILayout.Label("Zoom");
            gridZoom = EditorGUILayout.FloatField(gridZoom, GUILayout.Width(64f));
        }

        /// <summary>
        /// Gets the next segment.
        /// </summary>
        /// <param name="segment">The segment to find the next segment for.</param>
        /// <returns>The next segment (wraps around).</returns>
        private Segment GetNextSegment(Segment segment)
        {
            Shape parent = segment.shape;
            int index = parent.segments.IndexOf(segment);
            if (index + 1 > parent.segments.Count - 1)
                return parent.segments[0];
            return parent.segments[index + 1];
        }
    }
}

#endif