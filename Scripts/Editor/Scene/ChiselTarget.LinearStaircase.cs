﻿#if UNITY_EDITOR

using UnityEngine;

namespace AeternumGames.ShapeEditor
{
    public partial class ChiselTarget
    {
        // builds a linear staircase.

        [SerializeField]
        [Min(1)]
        internal int linearStaircasePrecision = 8;

        [SerializeField]
        [Min(MathEx.EPSILON_2)]
        internal float linearStaircaseDistance = 1f;

        [SerializeField]
        internal float linearStaircaseHeight = 0.75f;

        [SerializeField]
        internal bool linearStaircaseSloped = false;

        private void LinearStaircase_Rebuild()
        {
            RequireConvexPolygons2D();

            var parent = CleanAndGetBrushParent();

            var polygonMeshes = MeshGenerator.CreateLinearStaircaseMeshes(convexPolygons2D, linearStaircasePrecision, linearStaircaseDistance, linearStaircaseHeight, linearStaircaseSloped);
            var polygonMeshesCount = polygonMeshes.Count;
            for (int i = 0; i < polygonMeshesCount; i++)
            {
                var polygonMesh = polygonMeshes[i];
                ExternalChisel.CreateBrushFromPoints(parent, "Shape Editor Brush", polygonMesh.ToPoints().ToArray(), polygonMesh.booleanOperator);
            }

            ExternalChisel.AddChiselCompositeComponent(gameObject);
        }
    }
}

#endif