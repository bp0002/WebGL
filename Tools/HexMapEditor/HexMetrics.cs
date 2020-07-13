using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HexMapEditor
{
    public static class HexMetrics
    {
        public static float outerRadius = 1f;

        public static float innerRadius = outerRadius * 0.866025404f;

        public static float squrt2 = 1.414213562373f;

        public static List<Vector3> PlaneVertex = new List<Vector3>();

        public static List<int> PlaneTriangles = new List<int>();

        public static List<Vector3> HexPlaneVertex = new List<Vector3>();

        public static List<int> HexPlaneTriangles = new List<int>();

        private static Mesh HexMesh = null;
        private static Mesh PlanMesh = null;

        public static float sin45 = Mathf.Sin(0.25f * Mathf.PI);
        public static float cos45 = Mathf.Cos(0.25f * Mathf.PI);

        public static void changeSize(float num)
        {
            outerRadius = num;
            innerRadius = outerRadius * 0.866025404f;
        }

        public static void Init()
        {
            HexMetrics.changeSize(1.0f);

            HexMetrics.PlaneVertex.Clear();
            HexMetrics.PlaneVertex.Add(new Vector3(HexMetrics.outerRadius * 0.5f, 0, HexMetrics.outerRadius * 0.5f));
            HexMetrics.PlaneVertex.Add(new Vector3(-HexMetrics.outerRadius * 0.5f, 0, -HexMetrics.outerRadius * 0.5f));
            HexMetrics.PlaneVertex.Add(new Vector3(-HexMetrics.outerRadius * 0.5f, 0, HexMetrics.outerRadius * 0.5f));

            HexMetrics.PlaneVertex.Add(new Vector3(HexMetrics.outerRadius * 0.5f, 0, HexMetrics.outerRadius * 0.5f));
            HexMetrics.PlaneVertex.Add(new Vector3(HexMetrics.outerRadius * 0.5f, 0, -HexMetrics.outerRadius * 0.5f));
            HexMetrics.PlaneVertex.Add(new Vector3(-HexMetrics.outerRadius * 0.5f, 0, -HexMetrics.outerRadius * 0.5f));

            HexMetrics.PlaneTriangles.Clear();
            HexMetrics.PlaneTriangles.Add(0);
            HexMetrics.PlaneTriangles.Add(1);
            HexMetrics.PlaneTriangles.Add(2);

            HexMetrics.PlaneTriangles.Add(3);
            HexMetrics.PlaneTriangles.Add(4);
            HexMetrics.PlaneTriangles.Add(5);


            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(HexMetrics.innerRadius * 0.5f, 0, HexMetrics.outerRadius * 0.5f * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(HexMetrics.innerRadius * 0.5f, 0, -HexMetrics.outerRadius * 0.5f * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, HexMetrics.outerRadius * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(HexMetrics.innerRadius * 0.5f, 0, HexMetrics.outerRadius * 0.5f * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(-HexMetrics.innerRadius * 0.5f, 0, HexMetrics.outerRadius * 0.5f * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, HexMetrics.outerRadius * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(-HexMetrics.innerRadius * 0.5f, 0, -HexMetrics.outerRadius * 0.5f * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(-HexMetrics.innerRadius * 0.5f, 0, HexMetrics.outerRadius * 0.5f * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, -HexMetrics.outerRadius * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(-HexMetrics.innerRadius * 0.5f, 0, -HexMetrics.outerRadius * 0.5f * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(HexMetrics.innerRadius * 0.5f, 0, -HexMetrics.outerRadius * 0.5f * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, -HexMetrics.outerRadius * 0.5f));

            HexMetrics.HexPlaneTriangles.Add(0);
            HexMetrics.HexPlaneTriangles.Add(1);
            HexMetrics.HexPlaneTriangles.Add(2);

            HexMetrics.HexPlaneTriangles.Add(3);
            HexMetrics.HexPlaneTriangles.Add(4);
            HexMetrics.HexPlaneTriangles.Add(5);

            HexMetrics.HexPlaneTriangles.Add(6);
            HexMetrics.HexPlaneTriangles.Add(7);
            HexMetrics.HexPlaneTriangles.Add(8);

            HexMetrics.HexPlaneTriangles.Add(9);
            HexMetrics.HexPlaneTriangles.Add(10);
            HexMetrics.HexPlaneTriangles.Add(11);

            HexMetrics.HexPlaneTriangles.Add(12);
            HexMetrics.HexPlaneTriangles.Add(13);
            HexMetrics.HexPlaneTriangles.Add(14);

            HexMetrics.HexPlaneTriangles.Add(15);
            HexMetrics.HexPlaneTriangles.Add(16);
            HexMetrics.HexPlaneTriangles.Add(17);
        }

        public static Mesh GetHexMesh()
        {
            if (HexMesh == null)
            {
                HexMesh = new Mesh();
                HexMesh.name = "HexCell_Base";
                HexMesh.SetVertices(HexMetrics.HexPlaneVertex);
                HexMesh.SetTriangles(HexMetrics.HexPlaneTriangles.ToArray(), 0);
                HexMesh.RecalculateNormals();
            }

            return HexMesh;
        }

        public static Mesh GetPlanMesh()
        {
            if (PlanMesh == null)
            {
                PlanMesh = new Mesh();
                PlanMesh.name = "HexCell_Base";
                PlanMesh.SetVertices(HexMetrics.PlaneVertex);
                PlanMesh.SetTriangles(HexMetrics.PlaneTriangles.ToArray(), 0);
                PlanMesh.RecalculateNormals();
            }

            return PlanMesh;
        }

        public static Vector3 RotatePos(float x, float y, float z, float angle)
        {
            float cos = Mathf.Cos(angle / 180 * Mathf.PI);
            float sin = Mathf.Sin(angle / 180 * Mathf.PI);

            return new Vector3(cos * x + sin * z, y, -sin * x + cos * z);
        }
    }
}
