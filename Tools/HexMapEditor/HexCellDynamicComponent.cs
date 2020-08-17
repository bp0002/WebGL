using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    public class HexCellDynamicComponent : MonoBehaviour
    {

        private GameObject go;

        public List<string> attrNames = new List<string>();
        public List<string> attrValues = new List<string>();

        [ShowOnly]
        public int x;

        [ShowOnly]
        public int y;

        [ShowOnly]
        public int z;

        [ShowOnly]
        public string Name = "null";

        [ShowOnly]
        public float size;

        [ShowOnly]
        public Boolean isRotate;

        [ShowOnly]
        public Boolean isHex;

        [ShowOnly]
        public byte cellType;

        [ShowOnly]
        public float edgeWidth;

        [ShowOnly]
        public byte terrain = 0;

        [ShowOnly]
        public int terrainID = 0;

        [SerializeField]
        private string _TemplateName;
        public string TemplateName
        {
            get { return _TemplateName; }
        }

        [ShowOnly]
        public string CoordInfo = "";

        //[SerializeField]
        private UnityEngine.Object _prefab;
        //public UnityEngine.Object prefab
        //{
        //    get { return _prefab; }
        //}

        private Color _color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        private void OnEnable()
        {
            Debug.LogWarning("onenable");
        }

        private void OnMouseOver()
        {
            Debug.LogWarning("OnMouseOver");
        }

        private void SetCoordInfo(List<int> list)
        {
            this.CoordInfo = "";

            int count = list.Count;

            for (var i = 0; i < count; i++)
            {
                this.CoordInfo += "" + list[i];

                if (i < count - 1)
                {
                    this.CoordInfo += ",";
                }
            }

        }

        public Boolean initData(int i, int j, int k, HexCellDynamicTemplate template, float bgCellSize, Vector3 cellCenterLocPos, Boolean rotate, Boolean hex, byte type, float edge, string specialName)
        {
            Boolean flag = false;

            x = i;
            y = j;
            z = k;

            size = template.cellSize;
            _TemplateName = template.name;

            cellType = type;
            isHex = hex;
            isRotate = rotate;
            edgeWidth = edge;

            terrain = template.terrain;

            this.CoordInfo = specialName;
            Name = specialName != null ? specialName : x + "_" + y + "_" + z;

            go = transform.gameObject;
            go.name = Name;
            transform.name = Name;

            if (isHex)
            {
                HexTransform(template, bgCellSize, cellCenterLocPos);
            }
            else
            {
                PlaneTransform(template, bgCellSize, cellCenterLocPos);
            }

            // 定位结束后校正
            cellType = formatType(type);

            return flag;
        }

        /// <summary>
        /// 边: 属于 Cell 的 右边缘 或 上边缘
        /// 点: 属于 Cell 的 右上角点
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private byte formatType(byte type)
        {
            byte res = type;
            switch (type)
            {
                case (Parame.RSquareEdgeType2):
                    {
                        res = Parame.RSquareEdgeType4;
                        break;
                    }
                case (Parame.RSquareEdgeType3):
                    {
                        res = Parame.RSquareEdgeType1;
                        break;
                    }

                case (Parame.RSquarePointType1):
                case (Parame.RSquarePointType2):
                case (Parame.RSquarePointType3):
                case (Parame.RSquarePointType4):
                    {
                        res = Parame.RSquarePointType1;
                        break;
                    }
            }

            return res;
        }

        private void HexTransform(HexCellDynamicTemplate template, float bgCellSize, Vector3 cellCenter)
        {
            var hexGridDyn = gameObject.GetComponentInParent<HexGridDynamicComponent>();

            if (hexGridDyn.EnableCellEdge)
            {

            }
            else
            {
                edgeWidth = 0.02f;
                if (isRotate)
                {
                    go.transform.localRotation = Quaternion.Euler(0, 30, 0);
                }
                else
                {
                    go.transform.localRotation = Quaternion.Euler(0, 0, 0);
                }
                go.transform.localScale = new Vector3(template.cellSize - edgeWidth, template.cellSize - edgeWidth, template.cellSize - edgeWidth);
                go.transform.position = new Vector3(cellCenter.x, cellCenter.y, cellCenter.z);
            }
        }

        private void PlaneTransform(HexCellDynamicTemplate template, float bgCellSize, Vector3 cellCenter)
        {
            var hexGridDyn = gameObject.GetComponentInParent<HexGridDynamicComponent>();
            if (hexGridDyn.EnableCellEdge)
            {
                if (cellType > 40)
                {
                    PlaneTransformPoint(template, bgCellSize, cellCenter);
                }
                else if (cellType > 30)
                {
                    PlaneTransformEdge(template, bgCellSize, cellCenter);
                }
                else if (cellType > 20)
                {
                    PlaneTransformPoint(template, bgCellSize, cellCenter);
                }
                else if(cellType > 10)
                {
                    PlaneTransformEdge(template, bgCellSize, cellCenter);
                }
                else
                {
                    PlaneTransformBaseWithEdgeAndPoint(template, bgCellSize, cellCenter);
                }
            }
            else
            {
                edgeWidth = 0.02f;
                PlaneTransformBase(template, bgCellSize, cellCenter);
            }
        }

        private void PlaneTransformBase(HexCellDynamicTemplate template, float bgCellSize, Vector3 cellCenter)
        {
            if (isRotate)
            {
                if (Parame.planIndex == 0)
                {
                    go.transform.localPosition = new Vector3(cellCenter.x, cellCenter.y, cellCenter.z);
                }
                else if (Parame.planIndex == 1)
                {
                    go.transform.localPosition = new Vector3(cellCenter.x - (template.cellSize - bgCellSize) / 2.0f, cellCenter.y, cellCenter.z - (template.cellSize - bgCellSize) / 2.0f);
                }
                else if (Parame.planIndex == 2)
                {
                    go.transform.localPosition = new Vector3(cellCenter.x + (template.cellSize - bgCellSize) / 2.0f, cellCenter.y, cellCenter.z + (template.cellSize - bgCellSize) / 2.0f);
                }
            }
            else
            {
                if (Parame.diamondIndex == 0)
                {
                    go.transform.localPosition = new Vector3(cellCenter.x, cellCenter.y, cellCenter.z);
                }
                else if (Parame.diamondIndex == 1)
                {
                    go.transform.localPosition = new Vector3(cellCenter.x, cellCenter.y, cellCenter.z - (template.cellSize - bgCellSize) * HexMetrics.squrt2 / 2.0f);
                }
                else if (Parame.diamondIndex == 2)
                {
                    go.transform.localPosition = new Vector3(cellCenter.x, cellCenter.y, cellCenter.z + (template.cellSize - bgCellSize) * HexMetrics.squrt2 / 2.0f);
                }
            }

            go.transform.localScale = new Vector3(template.cellSize - 0.05f, template.cellSize - 0.05f, template.cellSize - 0.05f);
            if (isRotate)
            {
                go.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                go.transform.localRotation = Quaternion.Euler(0, 45, 0);
            }
        }

        private void PlaneTransformBaseWithEdgeAndPoint(HexCellDynamicTemplate template, float bgCellSize, Vector3 cellCenter)
        {
            PlaneTransformBase(template, bgCellSize, cellCenter);
            go.transform.localScale = new Vector3(template.cellSize - edgeWidth * template.cellSize, template.cellSize - edgeWidth * template.cellSize, template.cellSize - edgeWidth * template.cellSize);
        }

        private void PlaneTransformEdge(HexCellDynamicTemplate template, float bgCellSize, Vector3 cellCenter)
        {
            var tempDiff = this.isRotate ? size * 0.5f : template.cellSize * HexMetrics.squrt2 * 0.25f;
                switch (cellType)
                {
                    case (Parame.RSquareEdgeType1):
                        {
                            go.transform.localPosition = new Vector3(cellCenter.x + size * 0.5f, cellCenter.y, cellCenter.z);
                            go.transform.localScale = new Vector3(edgeWidth * template.cellSize, template.cellSize, template.cellSize - edgeWidth * template.cellSize);
                        break;
                        }
                    case (Parame.RSquareEdgeType2):
                        {
                            go.transform.localPosition = new Vector3(cellCenter.x, cellCenter.y, cellCenter.z - size * 0.5f);
                            go.transform.localScale = new Vector3(template.cellSize - edgeWidth * template.cellSize, template.cellSize, edgeWidth * template.cellSize);
                            break;
                        }
                    case (Parame.RSquareEdgeType3):
                        {
                            go.transform.localPosition = new Vector3(cellCenter.x - size * 0.5f, cellCenter.y, cellCenter.z);
                            go.transform.localScale = new Vector3(edgeWidth * template.cellSize, template.cellSize, template.cellSize - edgeWidth * template.cellSize);
                            break;
                        }
                    case (Parame.RSquareEdgeType4):
                        {
                            go.transform.localPosition = new Vector3(cellCenter.x, cellCenter.y, cellCenter.z + size * 0.5f);
                            go.transform.localScale = new Vector3(template.cellSize - edgeWidth * template.cellSize, template.cellSize, edgeWidth * template.cellSize);
                            break;
                        }
                    /////////////////////////////////////////////////
                    case (Parame.SquareEdgeType1):
                        {
                            go.transform.localPosition = new Vector3(cellCenter.x + tempDiff, cellCenter.y, cellCenter.z - tempDiff);
                            go.transform.localScale = new Vector3(edgeWidth * template.cellSize, template.cellSize, template.cellSize - edgeWidth * template.cellSize);
                            go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                            break;
                        }
                    case (Parame.SquareEdgeType2):
                        {
                            go.transform.localPosition = new Vector3(cellCenter.x - tempDiff, cellCenter.y, cellCenter.z - tempDiff);
                            go.transform.localScale = new Vector3(template.cellSize - edgeWidth * template.cellSize, template.cellSize, edgeWidth * template.cellSize);
                            go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                            break;
                        }
                    case (Parame.SquareEdgeType3):
                        {
                            go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                            go.transform.localPosition = new Vector3(cellCenter.x - tempDiff, cellCenter.y, cellCenter.z + tempDiff);
                            go.transform.localScale = new Vector3(edgeWidth * template.cellSize, template.cellSize, template.cellSize - edgeWidth * template.cellSize);
                            break;
                        }
                    case (Parame.SquareEdgeType4):
                        {
                            go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                            go.transform.localPosition = new Vector3(cellCenter.x + tempDiff, cellCenter.y, cellCenter.z + tempDiff);
                            go.transform.localScale = new Vector3(template.cellSize - edgeWidth * template.cellSize, template.cellSize, edgeWidth * template.cellSize);
                            break;
                        }
                }
        }

        private void PlaneTransformPoint(HexCellDynamicTemplate template, float bgCellSize, Vector3 cellCenter)
        {
            var tempDiff = this.isRotate ? size * 0.5f : template.cellSize * HexMetrics.squrt2 * 0.5f;
            switch (cellType)
            {
                case (Parame.RSquarePointType1):
                    {
                        go.transform.localPosition = new Vector3(cellCenter.x + size * 0.5f, cellCenter.y, cellCenter.z + size * 0.5f);
                        break;
                    }
                case (Parame.RSquarePointType2):
                    {
                        go.transform.localPosition = new Vector3(cellCenter.x + size * 0.5f, cellCenter.y, cellCenter.z - size * 0.5f);
                        break;
                    }
                case (Parame.RSquarePointType3):
                    {
                        go.transform.localPosition = new Vector3(cellCenter.x - size * 0.5f, cellCenter.y, cellCenter.z - size * 0.5f);
                        break;
                    }
                case (Parame.RSquarePointType4):
                    {
                        go.transform.localPosition = new Vector3(cellCenter.x - size * 0.5f, cellCenter.y, cellCenter.z + size * 0.5f);
                        break;
                    }
                ///////////////////////
                case (Parame.SquarePointType1):
                    {
                        go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                        go.transform.localPosition = new Vector3(cellCenter.x + tempDiff, cellCenter.y, cellCenter.z + 0);
                        break;
                    }
                case (Parame.SquarePointType2):
                    {
                        go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                        go.transform.localPosition = new Vector3(cellCenter.x + 0, cellCenter.y, cellCenter.z - tempDiff);
                        break;
                    }
                case (Parame.SquarePointType3):
                    {
                        go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                        go.transform.localPosition = new Vector3(cellCenter.x - tempDiff, cellCenter.y, cellCenter.z - 0);
                        break;
                    }
                case (Parame.SquarePointType4):
                    {
                        go.transform.localRotation = Quaternion.Euler(0, 45, 0);
                        go.transform.localPosition = new Vector3(cellCenter.x - 0, cellCenter.y, cellCenter.z + tempDiff);
                        break;
                    }
            }
            go.transform.localScale = new Vector3(edgeWidth * template.cellSize, template.cellSize, edgeWidth * template.cellSize);
        }

        public void CreatePlane()
        {
            go = transform.gameObject;

            go.name = Name;
            transform.name = Name;

            var filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = HexMetrics.GetPlanMesh();
            
            var renderer = go.AddComponent<MeshRenderer>();


            //_prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + _name + "_HexCell" + ".prefab");
            //PrefabUtility.ReplacePrefab(go, _prefab, ReplacePrefabOptions.ConnectToPrefab);

            //Undo.RecordObject(_prefab, go.name);
            //EditorUtility.SetDirty(_prefab);
        }

        public void UpdateHexPlane()
        {
            if (go == null)
            {
                go = transform.gameObject;
            }
            var filter = go.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = go.AddComponent<MeshFilter>();
            }
            filter.sharedMesh = HexMetrics.GetHexMesh();

            //Undo.RecordObject(_prefab, go.name);
            //EditorUtility.SetDirty(_prefab);

            //AssetDatabase.Refresh();
        }

        public void UpdatePlane()
        {
            if (go == null)
            {
                go = transform.gameObject;
            }
            var filter = go.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = go.AddComponent<MeshFilter>();
            }
            filter.sharedMesh = HexMetrics.GetPlanMesh();

            //Undo.RecordObject(_prefab, go.name);
            //EditorUtility.SetDirty(_prefab);


            //AssetDatabase.Refresh();
        }

        public void UpdateColor(Color color)
        {
            _color.r = color.r;
            _color.g = color.g;
            _color.b = color.b;
            _color.a = color.a;
            _updateColor();
        }

        public void UpdateMaterial(Material mat)
        {
            if (go == null)
            {
                go = transform.gameObject;
            }

            go.GetComponent<MeshRenderer>().sharedMaterial = mat;

            //Undo.RecordObject(_prefab, go.name);
            //EditorUtility.SetDirty(_prefab);
        }

        private void _updateColor()
        {
            Color[] colorList = new Color[1];
            colorList[0] = _color;
            if (go == null)
            {
                go = transform.gameObject;
            }

            go.GetComponent<MeshRenderer>().sharedMaterial.SetColorArray("_Color", colorList);
            go.GetComponent<MeshRenderer>().sharedMaterial.color = _color;

            Undo.RecordObject(_prefab, go.name);
            EditorUtility.SetDirty(_prefab);
        }

        public void destory()
        {
            if (go == null)
            {
                go = transform.gameObject;
            }
            GameObject.DestroyImmediate(go);
        }


        public Boolean checkActive()
        {
            Boolean flag = true;

            if (!gameObject.activeInHierarchy)
            {
                flag = false;
            }

            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null)
            {
                flag = false;
            }

            var render = gameObject.GetComponent<MeshRenderer>();
            if (render == null || render.sharedMaterial == null || render.sharedMaterial.color.a < 0.01f)
            {
                flag = false;
            }

            return flag;
        }

        /// <summary>
        /// 通过模板更新 数据
        /// </summary>
        /// <param name="template"></param>
        public void UpdateFromTemplate(HexCellDynamicTemplate template)
        {
            attrNames.Clear();
            attrValues.Clear();
            foreach (var str in template.attrNames)
            {
                attrNames.Add(str);
            }
            foreach (var value in template.attrValues)
            {
                attrValues.Add(value);
            }

            if (template.mat == null)
            {
                template.mat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
                template.mat.renderQueue = 4000;

                AssetDatabase.CreateAsset(template.mat, "Assets/" + template.transform.name + ".mat");
            }

            template.mat.color = template.color;

            size = template.cellSize;
            _TemplateName = template.name;

            //go.transform.localScale = new Vector3(template.cellSize - 0.05f, template.cellSize - 0.05f, template.cellSize - 0.05f);

            //UpdateColor(template.color);
            UpdateMaterial(template.mat);
        }

        public void customRender() {
            var mat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
            mat.renderQueue = 4000;
            var name = "";
            try
            {
                name = transform.name + "_" + transform.GetComponentInParent<HexGridDynamicComponent>().name + "_" + transform.GetComponentInParent<HexMapComponent>().name;
            }
            finally 
            {
                name = transform.name + DateTime.Now.ToLongTimeString();
            }

            AssetDatabase.CreateAsset(mat, "Assets/" + name + ".mat");

            _TemplateName = name;

            UpdateMaterial(mat);
        }

        public static Boolean CheckCanCreate(Boolean gridEnableEdge, Boolean rotate, Boolean hex)
        {
            Boolean flag = false;

            if (hex)
            {
                if (rotate)
                {
                    if (gridEnableEdge)
                    {

                    }
                    else
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (gridEnableEdge)
                    {

                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
            else
            {
                if (rotate)
                {
                    if (gridEnableEdge)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = true;
                    }
                }
                else
                {
                    if (gridEnableEdge)
                    {
                        flag = true;
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }

            return flag;
        }
    }
}
