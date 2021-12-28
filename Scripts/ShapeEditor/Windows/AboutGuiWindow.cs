﻿#if UNITY_EDITOR

using Unity.Mathematics;
using UnityEngine;

namespace AeternumGames.ShapeEditor
{
    /// <summary>The about window that displays the 2D Shape Editor credits.</summary>
    public class AboutGuiWindow : GuiWindow
    {
        private static float2 windowSize = new float2(300, 200);
        private GuiButton patreonButton;

        public AboutGuiWindow() : base(GetCenterPosition(), windowSize)
        {
            colorWindowBackground = new Color(0.192f, 0.192f, 0.192f);

            Add(patreonButton = new GuiButton(ShapeEditorResources.Instance.shapeEditorAboutPatreon, new float2(71, 99), new float2(158, 50), () =>
            {
                Application.OpenURL("https://patreon.com/henrydejongh");
            }));
        }

        private static float2 GetCenterPosition()
        {
            return new float2(
                (ShapeEditorWindow.Instance.position.width / 2f) - (windowSize.x / 2f),
                (ShapeEditorWindow.Instance.position.height / 2f) - (windowSize.y / 2f)
            );
        }

        public override void OnRender()
        {
            base.OnRender();

            GLUtilities.DrawGuiTextured(ShapeEditorResources.Instance.shapeEditorAbout, () =>
            {
                var rect = drawRect;
                GLUtilities.DrawFlippedUvRectangle(rect.x, rect.y, rect.width, rect.height);
            });

            if (patreonButton.isMouseOver)
            {
                editor.SetMouseCursor(ShapeEditorResources.Instance.shapeEditorMouseCursorHand, new float2(6f, 0f));
            }

            if (!IsActiveOrHasActiveChild())
            {
                Close();
            }
        }
    }
}

#endif