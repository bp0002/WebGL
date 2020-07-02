using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    public class HexGridComponent : MonoBehaviour
    {
        public static int counter = 0;
        public string Name = "HexGrid" + counter++;

        private Mesh mesh;
        private Material material;

        private Boolean needReCreate = true;
        private Boolean needUpdate = true;

        /// <summary>
        /// 网格样式
        /// </summary>
        public Boolean isHex = true;
        /// <summary>
        /// 网格样式
        /// </summary>
        public Boolean isRotate = false;

        /// <summary>
        /// 更新网格样式后记录，用于下次更新样式前比较
        /// </summary>
        private Boolean preIsHex = false;
        /// <summary>
        /// 更新网格样式后记录，用于下次更新样式前比较
        /// </summary>
        private Boolean preIsRotate = false;

        /// <summary>
        /// 网格层宽高
        /// </summary>
        public int width = 20;
        /// <summary>
        /// 网格层宽高
        /// </summary>
        public int height = 20;

        /// <summary>
        /// 更新网格层宽高后记录宽高，用于下次更新宽高前比较
        /// </summary>
        private int preWidth = 0;
        /// <summary>
        /// 更新网格层宽高后记录宽高，用于下次更新宽高前比较
        /// </summary>
        private int preHeight = 0;
        /// <summary>
        /// 网格尺寸设置
        /// </summary>
        public float cellSize = 1f;
        /// <summary>
        /// 更新网格尺寸后记录，用于下次更新尺寸前比较
        /// </summary>
        private float preCellSize = 0f;

        public Boolean displayCells = true;

        private List<HexCellComponent> list = new List<HexCellComponent>();

        public List<HexCellComponent> getHexCells()
        {
            var arr = gameObject.GetComponentsInChildren<HexCellComponent>();

            return new List<HexCellComponent>(arr);
        }

        /// <summary>
        /// 初始化网格
        /// </summary>
        public void init()
        {
            if (
                width != preWidth
                || height != preHeight
                )
            {
                needReCreate = true;
                needUpdate = true;
            }

            if (
                isHex != preIsHex
                || isRotate != preIsRotate
                || cellSize != preCellSize
                )
            {
                needUpdate = true;
                preIsHex = isHex;
                preIsRotate = isRotate;
                preCellSize = cellSize;
            }

            if (needReCreate)
            {
                CreateCells();
                needUpdate = true;
                needReCreate = false;
                preWidth = width;
                preHeight = height;
            }
            if (needUpdate)
            {
                UpdateCells();
                needUpdate = false;
            }
        }

        public void RemoveCell(HexCellComponent cell)
        {
            int index = list.IndexOf(cell);
            if (index >= 0)
            {
                list[index] = null;
            }
        }

        /// <summary>
        /// 创建网格中的网格单元
        /// </summary>
        public void CreateCells()
        {
            List<HexCellComponent> oldList = new List<HexCellComponent>();

            foreach (var cell in list)
            {
                if (cell != null)
                {
                    oldList.Add(cell);
                    //cell.destory();
                }
            }

            list.Clear();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    HexCellComponent cell = null;

                    int index = i * width + j;

                    if (j < preWidth && i < preHeight)
                    {
                        int oldIndex = i * preWidth + j;
                        if (oldIndex < oldList.Count)
                        {
                            cell = oldList[oldIndex];
                            if (checkCellInList(oldList, cell, oldIndex))
                            {
                                cell.initData(j, 0, i);
                                oldList[oldIndex] = null;
                            }
                            else
                            {
                                cell = null;
                            }
                        }
                    }

                    if (cell == null)
                    {
                        cell = CreateCell(j, i);
                    }

                    list.Add(cell);
                }
            }


            foreach (var cell in oldList)
            {
                if (cell != null)
                {
                    cell.destory();
                }
            }
            oldList.Clear();

            combine();
        }

        /// <summary>
        /// 网格的指定位置创建网格单元
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        HexCellComponent CreateCell(int x, int z)
        {
            GameObject go = new GameObject();
            HexCellComponent cell = go.AddComponent<HexCellComponent>();
            CellListComponent cellListComp = gameObject.GetComponentInChildren<CellListComponent>();
            cell.initData(x, 0, z);
            cell.CreatePlane();

            if (cellListComp == null)
            {
                cell.transform.parent = gameObject.transform;
            }
            else
            {
                cell.transform.parent = cellListComp.transform;
            }

            return cell;
        }
        public void UpdateSelect()
        {

            //GameObject[] goList = Selection.gameObjects;
            //int count = goList.Length;

            //Color color = new Color(1, 1, 1, 0.5f);

            //if (Parame.selectColorIndex < Parame.colorList.Count)
            //{
            //    color = Parame.colorList[Parame.selectColorIndex];
            //}

            //for (int i = 0; i < count; i++)
            //{
            //    HexCellComponent hexCellComp = goList[i].GetComponent<HexCellComponent>();

            //    if (hexCellComp != null)
            //    {
            //        hexCellComp.UpdateColor(color);
            //    }
            //}

        }
        public void UpdateCells()
        {
            CellListComponent cellListComp = gameObject.GetComponentInChildren<CellListComponent>();
            foreach(var cell in list)
            {
                if (checkCellInList(list, cell, -1))
                {
                    UpdateCell(cell);
                } else
                {
                    needReCreate = true;
                }
            }

            combine();
        }
        void UpdateCell(HexCellComponent hexCellComp)
        {
            hexCellComp.transform.localScale = new Vector3(cellSize - .05f, cellSize - .05f, cellSize - .05f);
            //hexCellComp.transform.localScale = new Vector3(cellSize, cellSize, cellSize);

            if (isHex)
            {
                if (!isRotate)
                {
                    hexCellComp.transform.localPosition = new Vector3(
                        (hexCellComp.x + hexCellComp.z * 0.5f - hexCellComp.z / 2) * (HexMetrics.innerRadius * 0.5f * cellSize * 2f),
                        0,
                        hexCellComp.z * (HexMetrics.outerRadius * 0.5f * cellSize * 1.5f)
                    );
                } else
                {
                    hexCellComp.transform.localPosition = new Vector3(
                        hexCellComp.x * (HexMetrics.outerRadius * 0.5f * cellSize * 1.5f),
                        0,
                        (hexCellComp.z + hexCellComp.x * 0.5f - hexCellComp.x / 2) * (HexMetrics.innerRadius * 0.5f * cellSize * 2f)
                    );
                }

                hexCellComp.UpdateHexPlane();
            } else
            {

                if (isRotate)
                {
                    hexCellComp.transform.localPosition = new Vector3(
                        (hexCellComp.x) * (HexMetrics.outerRadius * cellSize),
                        0,
                        (hexCellComp.z) * (HexMetrics.outerRadius * cellSize)
                    );
                } else
                {
                    hexCellComp.transform.localPosition = new Vector3(
                        (hexCellComp.x + (hexCellComp.z % 2) * 0.5f) * (HexMetrics.outerRadius * HexMetrics.squrt2 * cellSize),
                        0,
                        (hexCellComp.z) * (HexMetrics.outerRadius * HexMetrics.squrt2 / 2 * cellSize)
                    );
                }

                hexCellComp.UpdatePlane();
            }

            if (isRotate)
            {
                
                hexCellComp.transform.localRotation = Quaternion.Euler(0f, isHex ? 90f : 0f, 0f);
            }
            else
            {
                hexCellComp.transform.localRotation = Quaternion.Euler(0f, isHex ? 0f : 45f, 0f);
            }
        }
        public void combine()
        {
            //if (mesh == null)
            //{
            //    MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();
            //    if (meshFilter)
            //    {
            //        mesh = meshFilter.sharedMesh;
            //    }
            //    else
            //    {
            //        mesh = new Mesh();
            //        meshFilter = gameObject.AddComponent<MeshFilter>();
            //        meshFilter.sharedMesh = mesh;
            //    }
            //}

            //if (material == null)
            //{
            //    MeshRenderer mr = gameObject.GetComponent<MeshRenderer>();
            //    if (mr)
            //    {
            //        material = mr.sharedMaterial;
            //    }
            //    else
            //    {
            //        material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
            //        mr = gameObject.AddComponent<MeshRenderer>();
            //        mr.sharedMaterial = material;
            //    }

            //    Color[] colors = new Color[1];
            //    colors[0] = new Color(0, 0, 0, 0);
            //    material.SetColorArray("_Color", colors);
            //}

            //List<CombineInstance> meshes = new List<CombineInstance>();

            //foreach(var cell in list)
            //{
            //    if (cell)
            //    {
            //        CombineInstance meshIn = new CombineInstance();
            //        meshIn.mesh = cell.mesh;
            //        meshIn.transform = cell.transform.localToWorldMatrix;
            //        meshes.Add(meshIn);
            //    }
            //}

            //mesh.Clear();
            //mesh.CombineMeshes(meshes.ToArray());

            //transform.localPosition.Set(0, 0, 0);
        }

        private Boolean checkCellInList(List<HexCellComponent> _list, HexCellComponent cell, int index) 
        {
            Boolean flag = false;

            if (index < 0)
            {
                index = list.IndexOf(cell);
            }

            CellListComponent cellListComp = gameObject.GetComponentInChildren<CellListComponent>();
            if (cell && cell.transform)
            {
                if (cellListComp)
                {
                    flag = cellListComp.transform.FindChild(cell.transform.name) != null;
                } else
                {
                    flag = transform.FindChild(cell.transform.name) != null;
                }

                if (flag == false)
                {
                    _list[index] = null;
                }
            }
            else
            {
                flag = false;
            }

            return flag;
        }
    }
}
