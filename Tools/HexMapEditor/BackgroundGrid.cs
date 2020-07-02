using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HexMapEditor
{
    public class BackgroundGrid : MonoBehaviour
    {
        public int width = 100;
        public int height = 100;
        public Boolean isHex = false;
        public Boolean isRotate = true;

        public int cellSize = 1;

        private string _name = "BackgroundGrid";

        public static List<Vector3> PlaneVertex;

        public static List<int> PlaneTriangles;

        public static List<Vector3> HexPlaneVertex;

        public static List<int> HexPlaneTriangles;

        public readonly static float size = 1f;

        public static float outerRadius = 1f - 0.05f;

        public static float innerRadius = outerRadius * 0.866025404f;

        public static float squrt2 = 1.414213562373f;

        private static int MAX_PLAN_CELL_SIZE = 100 * 100;
        private static int MAX_HEX_CELL_SIZE = 60 * 60;

        private static float sin0   = Mathf.Sin(Mathf.PI * 0 / 180  ) * outerRadius * 0.5f;
        private static float sin30  = Mathf.Sin(Mathf.PI * 30 / 180 ) * outerRadius * 0.5f;
        private static float sin60  = Mathf.Sin(Mathf.PI * 60 / 180 ) * outerRadius * 0.5f;
        private static float sin90  = Mathf.Sin(Mathf.PI * 90 / 180 ) * outerRadius * 0.5f;
        private static float sin120 = Mathf.Sin(Mathf.PI * 120 / 180) * outerRadius * 0.5f;
        private static float sin150 = Mathf.Sin(Mathf.PI * 150 / 180) * outerRadius * 0.5f;
        private static float sin180 = Mathf.Sin(Mathf.PI * 180 / 180) * outerRadius * 0.5f;

        private static float cos0   = Mathf.Cos(Mathf.PI * 0 / 180  ) * outerRadius * 0.5f;
        private static float cos30  = Mathf.Cos(Mathf.PI * 30 / 180 ) * outerRadius * 0.5f;
        private static float cos60  = Mathf.Cos(Mathf.PI * 60 / 180 ) * outerRadius * 0.5f;
        private static float cos90  = Mathf.Cos(Mathf.PI * 90 / 180 ) * outerRadius * 0.5f;
        private static float cos120 = Mathf.Cos(Mathf.PI * 120 / 180) * outerRadius * 0.5f;
        private static float cos150 = Mathf.Cos(Mathf.PI * 150 / 180) * outerRadius * 0.5f;
        private static float cos180 = Mathf.Cos(Mathf.PI * 180 / 180) * outerRadius * 0.5f;

        private static int CELL_COUNTER = 0;

        private Material material = null;

        public void Create()
        {
            if (material == null)
            {
                material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
            }

            var go = transform.gameObject;

            go.name = _name;
            transform.name = _name;

            int count = transform.childCount;

            for (int i = count - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i);
                GameObject.DestroyImmediate(child.gameObject);
            }

            if (isHex)
            {
                CreateHex();
            } else
            {
                CreatePlan();
            }

            transform.localScale = new Vector3(cellSize, cellSize, cellSize);
        }

        private void CreatePlan()
        {
            CELL_COUNTER = 0;
            PlaneVertex = new List<Vector3>();
            PlaneTriangles = new List<int>();

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    CreatePlanCell(x, 0, z);
                }
            }

            if (CELL_COUNTER > 0)
            {
                CreatePlanCellMesh();

                CELL_COUNTER = 0;
                PlaneVertex = new List<Vector3>();
                PlaneTriangles = new List<int>();
            }
        }

        private void CreatePlanCell(int x, int y, int z)
        {
            CELL_COUNTER++;
            CreatePlanCellData(x, y, z);

            if (CELL_COUNTER >= MAX_PLAN_CELL_SIZE)
            {
                CreatePlanCellMesh();

                CELL_COUNTER = 0;
                PlaneVertex = new List<Vector3>();
                PlaneTriangles = new List<int>();
            }

        }

        private void CreatePlanCellMesh()
        {
            var go = new GameObject();
            go.name = "bg_" + transform.childCount;
            go.transform.parent = transform;
            go.transform.name = go.name;
            go.transform.hideFlags = HideFlags.NotEditable;
            go.AddComponent<LockPos>();
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localRotation = Quaternion.Euler(0, 0, 0);

            MeshFilter _filter = go.GetComponent<MeshFilter>();

            if (_filter == null)
            {
                var filter = go.AddComponent<MeshFilter>();
                filter.sharedMesh = new Mesh();
                filter.sharedMesh.name = go.name + "_Mesh";

                var renderer = go.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;
                material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
            _filter = go.GetComponent<MeshFilter>();

            var _mesh = _filter.sharedMesh;
            if (_mesh == null)
            {
                _filter.sharedMesh = new Mesh();
                _filter.sharedMesh.name = go.name + "_Mesh";
            }

            _mesh = _filter.sharedMesh;
            _mesh.Clear();
            _mesh.SetVertices(PlaneVertex);
            _mesh.SetTriangles(PlaneTriangles.ToArray(), 0);
            _mesh.RecalculateNormals();
        }

        private void CreatePlanCellData(int x, int y, int z)
        {
            int preVertexCount = PlaneVertex.Count;

            float planPosBase = outerRadius * 0.5f;

            float diffX = 0.0f;
            float diffZ = 0.0f;

            if (isRotate)
            {

                diffX = (x) * (size);
                diffZ = (z) * (size);

                PlaneVertex.Add(new Vector3(planPosBase + diffX, y, planPosBase + diffZ));
                PlaneVertex.Add(new Vector3(-planPosBase + diffX, y, -planPosBase + diffZ));
                PlaneVertex.Add(new Vector3(-planPosBase + diffX, y, planPosBase + diffZ));

                PlaneVertex.Add(new Vector3(planPosBase + diffX, y, planPosBase + diffZ));
                PlaneVertex.Add(new Vector3(planPosBase + diffX, y, -planPosBase + diffZ));
                PlaneVertex.Add(new Vector3(-planPosBase + diffX, y, -planPosBase + diffZ));
            }
            else
            {
                diffX = (x + (z % 2) * 0.5f) * (size * squrt2);
                diffZ = (z) * (size * squrt2 / 2);

                PlaneVertex.Add(new Vector3(planPosBase * squrt2 + diffX, y, 0 + diffZ));
                PlaneVertex.Add(new Vector3(-planPosBase * squrt2 + diffX, y, 0 + diffZ));
                PlaneVertex.Add(new Vector3(0 + diffX, y, planPosBase * squrt2 + diffZ));

                PlaneVertex.Add(new Vector3(planPosBase * squrt2 + diffX, y, 0 + diffZ));
                PlaneVertex.Add(new Vector3(0 + diffX, y, -planPosBase * squrt2 + diffZ));
                PlaneVertex.Add(new Vector3(-planPosBase * squrt2 + diffX, y, -0 + diffZ));
            }


            PlaneTriangles.Add(preVertexCount + 0);
            PlaneTriangles.Add(preVertexCount + 1);
            PlaneTriangles.Add(preVertexCount + 2);

            PlaneTriangles.Add(preVertexCount + 3);
            PlaneTriangles.Add(preVertexCount + 4);
            PlaneTriangles.Add(preVertexCount + 5);
        }

        private void CreateHex()
        {
            CELL_COUNTER = 0;
            HexPlaneVertex = new List<Vector3>();
            HexPlaneTriangles = new List<int>();

            for (int z = 0; z < height; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    CreateHexCell(x, 0, z);
                }
            }

            if (CELL_COUNTER > 0)
            {
                CreateHexCellMesh();

                CELL_COUNTER = 0;
                HexPlaneVertex = new List<Vector3>();
                HexPlaneTriangles = new List<int>();
            }
        }

        private void CreateHexCell(int x, int y, int z)
        {
            CELL_COUNTER++;
            CreateHexCellData(x, y, z);

            if (CELL_COUNTER >= MAX_HEX_CELL_SIZE)
            {
                CreateHexCellMesh();

                CELL_COUNTER = 0;
                HexPlaneVertex = new List<Vector3>();
                HexPlaneTriangles = new List<int>();
            }
        }

        private void CreateHexCellData(int x, int y, int z)
        {
            int preVertexCount = HexPlaneVertex.Count;

            float planPosBase = outerRadius * 0.5f;

            float diffX = 0.0f;
            float diffZ = 0.0f;

            if (isRotate)
            {

                diffX = x * (size * 0.5f * 1.5f);
                diffZ = (z + x * 0.5f - x / 2) * (size * 0.5f * 0.866025404f * 2f);

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos60 + diffX, y, sin60 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos0 + diffX, y, sin0 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos120 + diffX, y, sin120 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos60 + diffX, y, sin60 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos180 + diffX, y, sin180 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos120 + diffX, y, sin120 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos0 + diffX, y, sin0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos60 + diffX, y, -sin60 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos60 + diffX, y, -sin60 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos120 + diffX, y, -sin120 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos120 + diffX, y, -sin120 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos180 + diffX, y, -sin180 + diffZ));
            }
            else
            {
                diffX = (x + z * 0.5f - z / 2) * (size * 0.5f * 0.866025404f * 2f);
                diffZ = z * (size * 0.5f * 1.5f);

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos30 + diffX, y, sin30 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos30 + diffX, y, -sin30 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos90 + diffX, y, sin90 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos30 + diffX, y, sin30 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos150 + diffX, y, sin150 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos90 + diffX, y, sin90 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos30 + diffX, y, -sin30 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos90 + diffX, y, -sin90 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos90 + diffX, y, -sin90 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos150 + diffX, y, -sin150 + diffZ));

                HexPlaneVertex.Add(new Vector3(0 + diffX, y, 0 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos150 + diffX, y, -sin150 + diffZ));
                HexPlaneVertex.Add(new Vector3(cos150 + diffX, y, sin150 + diffZ));
            }

            HexPlaneTriangles.Add(preVertexCount + 0);
            HexPlaneTriangles.Add(preVertexCount + 1);
            HexPlaneTriangles.Add(preVertexCount + 2);

            HexPlaneTriangles.Add(preVertexCount + 3);
            HexPlaneTriangles.Add(preVertexCount + 4);
            HexPlaneTriangles.Add(preVertexCount + 5);

            HexPlaneTriangles.Add(preVertexCount + 6);
            HexPlaneTriangles.Add(preVertexCount + 7);
            HexPlaneTriangles.Add(preVertexCount + 8);

            HexPlaneTriangles.Add(preVertexCount + 9);
            HexPlaneTriangles.Add(preVertexCount + 10);
            HexPlaneTriangles.Add(preVertexCount + 11);

            HexPlaneTriangles.Add(preVertexCount + 12);
            HexPlaneTriangles.Add(preVertexCount + 13);
            HexPlaneTriangles.Add(preVertexCount + 14);

            HexPlaneTriangles.Add(preVertexCount + 15);
            HexPlaneTriangles.Add(preVertexCount + 16);
            HexPlaneTriangles.Add(preVertexCount + 17);
        }

        private void CreateHexCellMesh()
        {
            var go = new GameObject();
            go.name = "bg_" + transform.childCount;
            go.transform.parent = transform;
            go.transform.name = go.name;
            go.AddComponent<LockPos>();
            go.transform.localPosition = new Vector3(0, 0, 0);
            go.transform.localScale = new Vector3(1, 1, 1);
            go.transform.localRotation = Quaternion.Euler(0, 0, 0);

            MeshFilter _filter = go.GetComponent<MeshFilter>();

            if (_filter == null)
            {
                var filter = go.AddComponent<MeshFilter>();
                filter.sharedMesh = new Mesh();
                filter.sharedMesh.name = go.name + "_Mesh";

                var renderer = go.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;
                material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
            }
            _filter = go.GetComponent<MeshFilter>();

            var _mesh = _filter.sharedMesh;
            if (_mesh == null)
            {
                _filter.sharedMesh = new Mesh();
                _filter.sharedMesh.name = go.name + "_Mesh";
            }

            _mesh = _filter.sharedMesh;
            _mesh.Clear();
            _mesh.SetVertices(HexPlaneVertex);
            _mesh.SetTriangles(HexPlaneTriangles.ToArray(), 0);
            _mesh.RecalculateNormals();
        }

        public List<CustomRayInfo> rayCast(Ray ray)
        {
            List<CustomRayInfo> list = new List<CustomRayInfo>();
            var arr = gameObject.GetComponentsInChildren<LockPos>();
            float num = float.PositiveInfinity;

            foreach (var cell in arr)
            {
                RaycastHit hit;
                MeshFilter meshFilter = cell.gameObject.GetComponent<MeshFilter>();
                Mesh sharedMesh = meshFilter.sharedMesh;
                if (sharedMesh
                    && RXLookingGlass.IntersectRayMesh(ray, sharedMesh, meshFilter.transform.localToWorldMatrix, out hit)
                    && hit.distance < num)
                {

                    if (isHex)
                    {
                        var vec = new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z);
                        var tempList = HexCoordinates.FromPixelPositionHex(vec.x, vec.y, vec.z, cellSize, isRotate, Parame.distance);

                        foreach (var v in tempList)
                        {
                            CustomRayInfo info = new CustomRayInfo();
                            info.ix = v.hx;
                            info.iy = v.hy;
                            info.iz = v.hz;

                            info.fx = v.fx + transform.position.x;
                            info.fz = v.fz + transform.position.z;
                            list.Add(info);
                        }
                    } else
                    {

                        var vec = new Vector3(hit.point.x - transform.position.x, 0, hit.point.z - transform.position.z);
                        var tempList = HexCoordinates.FromPixelPositionSquare(vec.x, vec.y, vec.z, cellSize, isRotate, Parame.distance);

                        foreach (var v in tempList)
                        {
                            CustomRayInfo info = new CustomRayInfo();
                            info.ix = v.hx;
                            info.iy = v.hy;
                            info.iz = v.hz;

                            info.fx = v.fx + transform.position.x;
                            info.fz = v.fz + transform.position.z;
                            list.Add(info);
                        }

                    //    CustomRayInfo info = null;
                    //    info = new CustomRayInfo();

                    //    if (!isRotate)
                    //    {
                    //        float x = hit.point.x - transform.position.x;
                    //        float z = hit.point.z - transform.position.z;

                    //        float s = Mathf.Sin(Mathf.PI * 45 / 180);
                    //        float c = Mathf.Cos(Mathf.PI * 45 / 180);

                    //        // translate point back to origin:
                    //        float srcx = x - 0;
                    //        float srcz = z - 0;

                    //        // rotate point
                    //        float xnew = srcx * c - srcz * s;
                    //        float znew = srcx * s + srcz * c;

                    //        // translate point back:
                    //        srcx = xnew + 0;
                    //        srcz = znew + 0;

                    //        int centerx = Mathf.RoundToInt(srcx / size / cellSize);
                    //        int centerz = Mathf.RoundToInt(srcz / size / cellSize);

                    //        srcx = centerx * size * cellSize - 0;
                    //        srcz = centerz * size * cellSize - 0;

                    //        // rotate point
                    //        xnew = srcx * c - srcz * s * -1;
                    //        znew = srcx * s * -1 + srcz * c;

                    //        srcx = xnew + 0;
                    //        srcz = znew + 0;

                    //        int ix = centerx;
                    //        int iz = centerz;

                    //        info.ix = ix;
                    //        info.iy = 0;
                    //        info.iz = iz;

                    //        info.fx = srcx + transform.position.x;
                    //        info.fz = srcz + transform.position.z;
                    //    }
                    //    else
                    //    {
                    //        float x = hit.point.x - transform.position.x;
                    //        float z = hit.point.z - transform.position.z;

                    //        int ix = Mathf.RoundToInt(x / size / cellSize);
                    //        int iz = Mathf.RoundToInt(z / size / cellSize);

                    //        info.ix = ix;
                    //        info.iy = 0;
                    //        info.iz = iz;

                    //        info.fx = ix * size * cellSize + transform.position.x;
                    //        info.fz = iz * size * cellSize + transform.position.z;
                    //    }

                    //    list.Add(info);
                    }

                    break;

                }
            }

            return list;
        }
    }
}
