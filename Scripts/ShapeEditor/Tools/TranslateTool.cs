﻿#if UNITY_EDITOR

using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace AeternumGames.ShapeEditor
{
    public class TranslateTool : BoxSelectTool
    {
        /// <summary>The constraints during single-use mode.</summary>
        private enum Constraints
        {
            /// <summary>No translation constraint (default behaviour).</summary>
            None,
            /// <summary>Constrains all translation to the global X-Axis.</summary>
            GlobalX,
            /// <summary>Constrains all translation to the global Y-Axis.</summary>
            GlobalY,
        }

        private bool isSingleUseDone = false;
        private TranslationWidget translationWidget = new TranslationWidget();
        protected bool registerTranslateUndoOperation = true;

        private Constraints constraint = Constraints.None;
        private static readonly Color constraintGlobalXColor = new Color(1.000f, 0.486f, 0.549f);
        private static readonly Color constraintGlobalYColor = new Color(0.486f, 0.886f, 0.392f);

        public override void OnActivate()
        {
            base.OnActivate();

            if (isSingleUse)
            {
                ToolOnBeginTranslating(editor);
            }
            else
            {
                editor.AddWidget(translationWidget);
                translationWidget.onBeginTranslating = () => ToolOnBeginTranslating(editor);
                translationWidget.onMouseDrag = (screenDelta, gridDelta) => ToolOnMouseDrag(editor, gridDelta);
            }
        }

        public override void OnRender()
        {
            base.OnRender();

            if (isSingleUse)
            {
                editor.SetMouseCursor(MouseCursor.MoveArrow);

                var bounds = editor.GetViewportRect();
                switch (constraint)
                {
                    case Constraints.GlobalX:
                        GLUtilities.DrawGui(() => GLUtilities.DrawGridLine(new float2(bounds.x, editor.selectedSegmentsAveragePosition.y), new float2(bounds.width, editor.selectedSegmentsAveragePosition.y), constraintGlobalXColor));
                        break;

                    case Constraints.GlobalY:
                        GLUtilities.DrawGui(() => GLUtilities.DrawGridLine(new float2(editor.selectedSegmentsAveragePosition.x, bounds.y), new float2(editor.selectedSegmentsAveragePosition.x, bounds.height), constraintGlobalYColor));
                        break;
                }
            }
            else
            {
                if (editor.selectedSegmentsCount > 0)
                {
                    translationWidget.position = editor.selectedSegmentsAveragePosition;
                    translationWidget.visible = true;
                }
                else
                {
                    translationWidget.visible = false;
                }
            }
        }

        public override void OnMouseMove(float2 screenDelta, float2 gridDelta)
        {
            if (isSingleUse && !isSingleUseDone)
            {
                ToolOnMouseDrag(editor, gridDelta);
            }
        }

        public override void OnMouseDown(int button)
        {
            if (isSingleUse)
            {
                if (button == 0)
                {
                    isSingleUseDone = true;
                }
            }
        }

        public override void OnMouseDrag(int button, float2 screenDelta, float2 gridDelta)
        {
            if (isSingleUse)
            {
                if (button == 0)
                {
                    // we do not want the marquee in this mode.
                    return;
                }
            }

            base.OnMouseDrag(button, screenDelta, gridDelta);
        }

        public override void OnGlobalMouseUp(int button)
        {
            if (isSingleUse)
            {
                if (button == 0)
                {
                    editor.SwitchTool(parent);
                }
            }
            else
            {
                base.OnGlobalMouseUp(button);
            }
        }

        public override bool IsBusy()
        {
            if (isSingleUse)
            {
                return !isSingleUseDone;
            }
            return false;
        }

        public override bool OnKeyDown(KeyCode keyCode)
        {
            if (isSingleUse)
            {
                switch (keyCode)
                {
                    case KeyCode.X:
                        constraint = constraint == Constraints.GlobalX ? Constraints.None : Constraints.GlobalX;
                        return true;

                    case KeyCode.Y:
                        constraint = constraint == Constraints.GlobalY ? Constraints.None : Constraints.GlobalY;
                        return true;
                }
            }
            return base.OnKeyDown(keyCode);
        }

        private float2 deltaAccumulator;

        private void ToolOnBeginTranslating(ShapeEditorWindow editor)
        {
            if (registerTranslateUndoOperation)
                editor.RegisterUndo("Translate Selection");

            deltaAccumulator = float2.zero;

            // store the initial position of all selected segments.
            foreach (var segment in editor.ForEachSelectedObject())
                segment.gpVector1 = segment.position;
        }

        private void ToolOnMouseDrag(ShapeEditorWindow editor, float2 gridDelta)
        {
            deltaAccumulator += gridDelta;
            float2 position = deltaAccumulator;

            foreach (var segment in editor.ForEachSelectedObject())
            {
                // optionally snap to the grid.
                if (editor.isSnapping)
                    position = position.Snap(editor.gridSnap);

                if (isSingleUse)
                {
                    switch (constraint)
                    {
                        case Constraints.GlobalX:
                            segment.position = segment.gpVector1 + new float2(position.x, 0f);
                            continue;

                        case Constraints.GlobalY:
                            segment.position = segment.gpVector1 + new float2(0f, position.y);
                            continue;
                    }
                }

                segment.position = segment.gpVector1 + position;
            }
        }
    }
}

#endif