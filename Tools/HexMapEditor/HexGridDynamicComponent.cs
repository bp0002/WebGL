using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    public class HexGridDynamicComponent : MonoBehaviour
    {
        public static int counter = 0;
        public string Name = "HexGridDynamic" + counter++;

        [ShowOnly]
        public float CellEdgeWidth = 0.0f;

        [ShowOnly]
        public Boolean EnableCellEdge = false;

        [ShowOnly]
        public int MinX = int.MaxValue;
        [ShowOnly]
        public int MaxX = int.MinValue;
        [ShowOnly]
        public int MinZ = int.MaxValue;
        [ShowOnly]
        public int MaxZ = int.MinValue;

        public HexGridDynamicComponent SetCellEdgeWidth(float width)
        {
            this.CellEdgeWidth = width;
            return this;
        }

        public HexGridDynamicComponent SetEnableCellEdge(Boolean flag)
        {
            this.EnableCellEdge = flag;
            return this;
        }

        public List<HexCellDynamicComponent> getHexCells()
        {
            var arr = gameObject.GetComponentsInChildren<HexCellDynamicComponent>();

            return new List<HexCellDynamicComponent>(arr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">网格系统坐标</param>
        /// <param name="y">网格系统坐标</param>
        /// <param name="z">网格系统坐标</param>
        /// <param name="isHex">是否为六边形</param>
        /// <param name="isRotate">是否为旋转</param>
        /// <param name="cellSize">单元格尺寸</param>
        /// <param name="position">单元格相对背景网格原点坐标</param>
        /// <param name="templateCellDyn">模板数据</param>
        /// <param name="cellType">网格类型 - 块 - 边(4/6种) - 点(4/6种)</param>
        public void CreateCellDynamic(int x, int y, int z, Boolean isHex, Boolean isRotate, float bgCellSize, Vector3 position, HexCellDynamicTemplate templateCellDyn, byte cellType, string specialName)
        {

            CellListComponent cellListComp = gameObject.GetComponentInChildren<CellListComponent>();

            Transform parentTransform = gameObject.transform;

            if (cellListComp != null)
            {
                parentTransform = cellListComp.transform;
            }

            if (HexCellDynamicComponent.CheckCanCreate(EnableCellEdge, isRotate, isHex))
            {
                if (isHex)
                {
                    GameObject go = new GameObject();
                    go.transform.parent = parentTransform;
                    var comp = go.AddComponent<HexCellDynamicComponent>();
                    var flag = comp.initData(x, y, z, templateCellDyn, bgCellSize, position, isRotate, isHex, cellType, CellEdgeWidth, specialName);
                    comp.CreatePlane();
                    comp.UpdateFromTemplate(templateCellDyn);
                    comp.UpdateHexPlane();

                    Undo.RegisterCreatedObjectUndo(go, go.name);
                    EditorUtility.SetDirty(go);
                }
                else
                {
                    GameObject go = new GameObject();
                    go.transform.parent = parentTransform;
                    var comp = go.AddComponent<HexCellDynamicComponent>();
                    var flag = comp.initData(x, y, z, templateCellDyn, bgCellSize, position, isRotate, isHex, cellType, CellEdgeWidth, specialName);
                    comp.CreatePlane();
                    comp.UpdateFromTemplate(templateCellDyn);
                    comp.UpdatePlane();

                    Undo.RegisterCreatedObjectUndo(go, go.name);
                    EditorUtility.SetDirty(go);
                }
            }
            else
            {
                Debug.LogWarning("暂不支持的编辑模式 - 检查 Grid 是不是启用了不该启用的 CellEdge");
            }
        }

        public void UpdateMinMax(List<int> nearList)
        {
            int count = nearList.Count / 3;

            if (this.getHexCells().Count < 1)
            {
                this.MinX = int.MaxValue;
                this.MaxX = int.MinValue;
                this.MinZ = int.MaxValue;
                this.MaxZ = int.MinValue;
            }
            
            this.MinX = Math.Min(nearList[0 * 3], this.MinX);
            this.MaxX = Math.Max(nearList[0 * 3], this.MaxX);
            this.MinZ = Math.Min(nearList[0 * 3 +2], this.MinZ);
            this.MaxZ = Math.Max(nearList[0 * 3 +2], this.MaxZ);
        }

        public void RefreshMinMax()
        {
            this.MinX = int.MaxValue;
            this.MaxX = int.MinValue;
            this.MinZ = int.MaxValue;
            this.MaxZ = int.MinValue;

            var list = this.getHexCells();

            foreach(var temp in list)
            {
                this.MinX = Math.Min(temp.x, this.MinX);
                this.MaxX = Math.Max(temp.x, this.MaxX);
                this.MinZ = Math.Min(temp.z, this.MinZ);
                this.MaxZ = Math.Max(temp.z, this.MaxZ);
            }
        }
    }
}
