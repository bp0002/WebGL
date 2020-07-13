using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    public class EditorExport
    {
        public static void doExport()
        {
            if (Selection.gameObjects.Length == 1)
            {
                var root = Selection.gameObjects[0];
                var hexGrid = root.GetComponent<HexGridComponent>();
                var hexGridDyn = root.GetComponent<HexGridDynamicComponent>();
                var hexMapComponent = root.GetComponentInParent<HexMapComponent>();

                if (hexMapComponent)
                {
                    if (hexGridDyn)
                    {

                        string JSONSTR = EditorExport.exportGridDynamic(hexMapComponent, hexGridDyn);

                        Parame.exportFileName = Path.Combine(Parame.exportPath, hexGridDyn.Name + ".hexgrid");

                        var fs = File.Open(Parame.exportFileName, FileMode.Create);
                        var jsonWriter = new StreamWriter(fs);

                        jsonWriter.Write(JSONSTR);

                        jsonWriter.Close();
                        fs.Close();
                    }
                    else if (hexGrid)
                    {
                        Parame.exportFileName = Path.Combine(Parame.exportPath, hexGrid.Name + ".hexgrid");

                        string JSONSTR = EditorExport.exportGrid(hexMapComponent, hexGrid);

                        var fs = File.Open(Parame.exportFileName, FileMode.Create);
                        var jsonWriter = new StreamWriter(fs);

                        jsonWriter.Write(JSONSTR);

                        jsonWriter.Close();
                        fs.Close();
                    }
                }
            }
            else
            {
                Debug.LogWarning("请选择需要导出的 Grid 节点");
            }
        }

        public static string exportGrid(HexMapComponent hexMapComponent, HexGridComponent hexGrid)
        {

            BackgroundGrid background = hexMapComponent.GetComponentInChildren<BackgroundGrid>();

            string JSONSTR = getHexGridJSON(hexGrid, background);
            
            Debug.LogWarning(JSONSTR);

            return JSONSTR;
        }


        public static string exportGridDynamic(HexMapComponent hexMapComponent, HexGridDynamicComponent hexGridDynamicComponent)
        {

            string JSONSTR = "";

            BackgroundGrid background = hexMapComponent.GetComponentInChildren<BackgroundGrid>();

            if (!Parame.exportSimpleCellData)
            {
                if (Parame.exportHexGridDynamicTerrain)
                {
                    EditorExport.exportGridTerrainBin(hexGridDynamicComponent, background.isHex, background.isRotate);
                }

                JSONSTR += getHexGridDynaJSON_Release(hexGridDynamicComponent, background);
            }
            else
            {
                JSONSTR += getHexGridDynaSimpleJSON(hexGridDynamicComponent, background);
            }

            Debug.LogWarning(JSONSTR);

            return JSONSTR;
        }


        public static string getHexGridJSON(HexGridComponent hexGridComponent, BackgroundGrid background)
        {
            string JSONSTR = "";

            // 首
            {
                JSONSTR += "{\n";
            }

            // 名称
            {
                JSONSTR += "    ";
                JSONSTR += "\"name\":\"" + hexGridComponent.Name + "\",\n";
            }

            // 宽高,位置，尺寸
            {
                JSONSTR += "    ";
                JSONSTR += "\"width\":" + hexGridComponent.width + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"height\":" + hexGridComponent.height + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"cellSize\":" + hexGridComponent.cellSize + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"position\":" + hexGridComponent.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"rotation\":" + hexGridComponent.transform.localRotation.eulerAngles.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"isHex\":" + (hexGridComponent.isHex ? "true" : "false") + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"isRotate\":" + (hexGridComponent.isRotate ? "true" : "false") + ",\n";
            }

            // 网格单元数组
            {
                JSONSTR += "    ";
                JSONSTR += "\"cells\":[\n";

                List<HexCellComponent> cells = hexGridComponent.getHexCells();

                List<HexCellComponent> list = new List<HexCellComponent>();


                foreach (var temp in cells)
                {
                    if (temp.checkActive())
                    {
                        list.Add(temp);
                    }
                }

                int count = list.Count;

                for (int i = 0; i < count; i++)
                {
                    HexCellComponent cell = list[i];
                    if (cell != null)
                    {
                        // 网格单元
                        JSONSTR += "        ";
                        JSONSTR += "{\n";
                        {

                            // 坐标
                            {
                                JSONSTR += "            ";
                                JSONSTR += "\"x\":" + cell.x + ",\n";
                                JSONSTR += "            ";
                                JSONSTR += "\"y\":" + cell.y + ",\n";
                                JSONSTR += "            ";
                                JSONSTR += "\"z\":" + cell.z + ",\n";
                                JSONSTR += "            ";
                                if (Parame.exportCellWorldPosition)
                                {
                                    JSONSTR += "\"position\":" + cell.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                                }
                                else
                                {
                                    JSONSTR += "\"position\":" + cell.transform.localPosition.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                                }
                                JSONSTR += "            ";
                                JSONSTR += "\"scaling\":" + cell.transform.localScale.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                            }

                            int attrCount = cell.attrNames.Count;
                            attrCount = Math.Min(attrCount, cell.attrValues.Count);

                            // 属性名
                            {
                                JSONSTR += "            ";
                                JSONSTR += "\"attrNames\":[";

                                for (var j = 0; j < attrCount; j++)
                                {
                                    JSONSTR += "\"" + cell.attrNames[j] + "\"";

                                    if (j != attrCount - 1)
                                    {
                                        JSONSTR += ",";
                                    }
                                }

                                JSONSTR += "],\n";
                            }

                            // 属性值
                            {
                                JSONSTR += "            ";
                                JSONSTR += "\"attrValues\":[";

                                for (var j = 0; j < attrCount; j++)
                                {
                                    JSONSTR += "\"" + cell.attrValues[j] + "\"";

                                    if (j != attrCount - 1)
                                    {
                                        JSONSTR += ",";
                                    }
                                }

                                JSONSTR += "]\n";
                            }
                        }

                        JSONSTR += "        ";
                        JSONSTR += "}";
                    }
                    else
                    {
                        JSONSTR += "        ";
                        JSONSTR += "null";
                    }

                    if (i != count - 1)
                    {
                        JSONSTR += ",\n";
                    }
                    else
                    {
                        JSONSTR += "\n";
                    }
                }

                JSONSTR += "    ";
                JSONSTR += "]\n";
            }

            // 尾
            {
                JSONSTR += "}";
            }

            return JSONSTR;
        }

        public static string getHexGridDynaJSON_Release(HexGridDynamicComponent hexGridComponent, BackgroundGrid background)
        {
            string JSONSTR = "";

            // 首
            {
                JSONSTR += "{";
            }

            // 名称
            {
                if (background)
                {
                    JSONSTR += "";
                    JSONSTR += "\"isHex\":" + (background.isHex ? "true" : "false") + ",";
                    JSONSTR += "";
                    JSONSTR += "\"isRotate\":" + (background.isRotate ? "true" : "false") + ",";
                    JSONSTR += "";
                    JSONSTR += "\"cellSize\":" + (background.cellSize) + ",";
                }
                JSONSTR += "";
                JSONSTR += "\"name\":\"" + hexGridComponent.Name + "\",";
                JSONSTR += "";
                JSONSTR += "\"enableEdge\":" + (hexGridComponent.EnableCellEdge ? "true" : "false") + ",";
                JSONSTR += "";
                JSONSTR += "\"cellEdge\":" + hexGridComponent.CellEdgeWidth + ",";

                hexGridComponent.RefreshMinMax();

                JSONSTR += "";
                JSONSTR += "\"minX\":" + hexGridComponent.MinX + ",";
                JSONSTR += "";
                JSONSTR += "\"maxX\":" + hexGridComponent.MaxX + ",";
                JSONSTR += "";
                JSONSTR += "\"minZ\":" + hexGridComponent.MinZ + ",";
                JSONSTR += "";
                JSONSTR += "\"maxZ\":" + hexGridComponent.MaxZ + ",";
            }

            if (Parame.exportHexGridDynamicTerrain && Parame.tempTerrainData != null)
            {
                JSONSTR += "";
                JSONSTR += "\"terrainData\":[";

                int count = Parame.tempTerrainData.Length;
                for (int i = 0; i < count; i++)
                {
                    JSONSTR += Parame.tempTerrainData[i];

                    if (i < count - 1)
                    {
                        JSONSTR += ",";
                    }
                }

                JSONSTR += "],";
            }

            // 宽高,位置，尺寸
            {
                JSONSTR += "";
                JSONSTR += "\"position\":" + hexGridComponent.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",";
                JSONSTR += "";
                JSONSTR += "\"rotation\":" + hexGridComponent.transform.localRotation.eulerAngles.ToString().Replace('(', '[').Replace(')', ']') + ",";
            }

            // 网格单元数组
            {
                JSONSTR += "";
                JSONSTR += "\"cells\":[";

                List<HexCellDynamicComponent> cells = hexGridComponent.getHexCells();

                List<HexCellDynamicComponent> list = new List<HexCellDynamicComponent>();


                foreach (var temp in cells)
                {
                    if (temp.gameObject.activeInHierarchy)
                    {
                        list.Add(temp);
                    }
                }

                int count = list.Count;

                for (int i = 0; i < count; i++)
                {
                    HexCellDynamicComponent cell = list[i];
                    if (cell != null)
                    {
                        // 网格单元
                        //JSONSTR += cellJSON(cell);
                        JSONSTR += cellArray_Release(cell);
                    }
                    else
                    {
                        JSONSTR += "";
                        JSONSTR += "null";
                    }

                    if (i != count - 1)
                    {
                        JSONSTR += ",";
                    }
                    else
                    {
                        JSONSTR += "";
                    }
                }

                JSONSTR += "";
                JSONSTR += "]";
            }

            // 尾
            {
                JSONSTR += "}";
            }

            return JSONSTR;
        }

        private static string cellJSON_Release(HexCellDynamicComponent cell)
        {
            // 网格单元
            var JSONSTR = "";
            JSONSTR += "{";
            {

                // 坐标
                {
                    JSONSTR += "";
                    JSONSTR += "\"x\":" + cell.x + ",";
                    JSONSTR += "";
                    JSONSTR += "\"y\":" + cell.y + ",";
                    JSONSTR += "";
                    JSONSTR += "\"z\":" + cell.z + ",";

                    JSONSTR += "";
                    JSONSTR += "\"terrainID\":" + cell.terrainID + ",\n";
                    JSONSTR += "";
                    JSONSTR += "\"id\":" + cell.Name + ",\n";
                    JSONSTR += "";
                    JSONSTR += "\"cellType\":" + cell.cellType + ",\n";
                    JSONSTR += "";
                    JSONSTR += "\"size\":" + cell.size + ",\n";
                    JSONSTR += "";
                    if (Parame.exportCellWorldPosition)
                    {
                        JSONSTR += "\"position\":" + cell.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",";
                    }
                    else
                    {
                        JSONSTR += "\"position\":" + cell.transform.localPosition.ToString().Replace('(', '[').Replace(')', ']') + ",";
                    }
                    JSONSTR += "";
                    JSONSTR += "\"scaling\":" + cell.transform.localScale.ToString().Replace('(', '[').Replace(')', ']') + ",";
                }

                int attrCount = cell.attrNames.Count;
                attrCount = Math.Min(attrCount, cell.attrValues.Count);

                // 属性名
                {
                    JSONSTR += "";
                    JSONSTR += "\"attrNames\":[";

                    for (var j = 0; j < attrCount; j++)
                    {
                        JSONSTR += "\"" + cell.attrNames[j] + "\"";

                        if (j != attrCount - 1)
                        {
                            JSONSTR += ",";
                        }
                    }

                    JSONSTR += "],";
                }

                // 属性值
                {
                    JSONSTR += "";
                    JSONSTR += "\"attrValues\":[";

                    for (var j = 0; j < attrCount; j++)
                    {
                        JSONSTR += "\"" + cell.attrValues[j] + "\"";

                        if (j != attrCount - 1)
                        {
                            JSONSTR += ",";
                        }
                    }

                    JSONSTR += "]";
                }
            }

            JSONSTR += "";
            JSONSTR += "}";

            return JSONSTR;
        }

        private static string cellArray_Release(HexCellDynamicComponent cell)
        {
            // 网格单元
            var JSONSTR = "";
            JSONSTR += "[";
            {

                // 坐标
                {
                    JSONSTR += "";
                    JSONSTR += "" + cell.x + ",";
                    JSONSTR += "";
                    JSONSTR += "" + cell.y + ",";
                    JSONSTR += "";
                    JSONSTR += "" + cell.z + ",";

                    JSONSTR += "";
                    JSONSTR += "" + cell.terrainID + ",";
                    JSONSTR += "";
                    JSONSTR += "" + cell.Name + ",";
                    JSONSTR += "";
                    JSONSTR += "" + cell.cellType + ",";
                    JSONSTR += "";
                    JSONSTR += "" + cell.size + ",";
                    JSONSTR += "";
                    if (Parame.exportCellWorldPosition)
                    {
                        JSONSTR += "" + cell.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",";
                    }
                    else
                    {
                        JSONSTR += "" + cell.transform.localPosition.ToString().Replace('(', '[').Replace(')', ']') + ",";
                    }
                    JSONSTR += "";
                    JSONSTR += "" + cell.transform.localScale.ToString().Replace('(', '[').Replace(')', ']') + ",";
                }

                int attrCount = cell.attrNames.Count;
                attrCount = Math.Min(attrCount, cell.attrValues.Count);

                // 属性名
                {
                    JSONSTR += "";
                    JSONSTR += "[";

                    for (var j = 0; j < attrCount; j++)
                    {
                        JSONSTR += "\"" + cell.attrNames[j] + "\"";

                        if (j != attrCount - 1)
                        {
                            JSONSTR += ",";
                        }
                    }

                    JSONSTR += "],";
                }

                // 属性值
                {
                    JSONSTR += "";
                    JSONSTR += "[";

                    for (var j = 0; j < attrCount; j++)
                    {
                        JSONSTR += "\"" + cell.attrValues[j] + "\"";

                        if (j != attrCount - 1)
                        {
                            JSONSTR += ",";
                        }
                    }

                    JSONSTR += "]";
                }
            }

            JSONSTR += "";
            JSONSTR += "]";

            return JSONSTR;
        }

        public static string getHexGridDynaJSON(HexGridDynamicComponent hexGridComponent, BackgroundGrid background)
        {
            string JSONSTR = "";

            // 首
            {
                JSONSTR += "{\n";
            }

            // 名称
            {
                if (background)
                {
                    JSONSTR += "    ";
                    JSONSTR += "\"isHex\":" + (background.isHex ? "true" : "false") + ",\n";
                    JSONSTR += "    ";
                    JSONSTR += "\"isRotate\":" + (background.isRotate ? "true" : "false") + ",\n";
                    JSONSTR += "    ";
                    JSONSTR += "\"cellSize\":" + (background.cellSize) + ",\n";
                }
                JSONSTR += "    ";
                JSONSTR += "\"name\":\"" + hexGridComponent.Name + "\",\n";
                JSONSTR += "    ";
                JSONSTR += "\"enableEdge\":" + (hexGridComponent.EnableCellEdge ? "true" : "false") + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"cellEdge\":" + hexGridComponent.CellEdgeWidth + ",\n";

                hexGridComponent.RefreshMinMax();

                JSONSTR += "    ";
                JSONSTR += "\"minX\":" + hexGridComponent.MinX + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"maxX\":" + hexGridComponent.MaxX + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"minZ\":" + hexGridComponent.MinZ + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"maxZ\":" + hexGridComponent.MaxZ + ",\n";
            }

            if (Parame.exportHexGridDynamicTerrain && Parame.tempTerrainData != null)
            {
                JSONSTR += "    ";
                JSONSTR += "\"terrainData\":[";

                int count = Parame.tempTerrainData.Length;
                for (int i = 0; i < count; i++)
                {
                    JSONSTR += Parame.tempTerrainData[i];

                    if (i < count - 1)
                    {
                        JSONSTR += ",";
                    }
                }

                JSONSTR += "],\n";
            }

            // 宽高,位置，尺寸
            {
                JSONSTR += "    ";
                JSONSTR += "\"position\":" + hexGridComponent.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                JSONSTR += "    ";
                JSONSTR += "\"rotation\":" + hexGridComponent.transform.localRotation.eulerAngles.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
            }

            // 网格单元数组
            {
                JSONSTR += "    ";
                JSONSTR += "\"cells\":[\n";

                List<HexCellDynamicComponent> cells = hexGridComponent.getHexCells();

                List<HexCellDynamicComponent> list = new List<HexCellDynamicComponent>();


                foreach (var temp in cells)
                {
                    if (temp.gameObject.activeInHierarchy)
                    {
                        list.Add(temp);
                    }
                }

                int count = list.Count;

                for (int i = 0; i < count; i++)
                {
                    HexCellDynamicComponent cell = list[i];
                    if (cell != null)
                    {
                        // 网格单元
                        //JSONSTR += cellJSON(cell);
                        JSONSTR += cellArray(cell);
                    }
                    else
                    {
                        JSONSTR += "        ";
                        JSONSTR += "null";
                    }

                    if (i != count - 1)
                    {
                        JSONSTR += ",\n";
                    }
                    else
                    {
                        JSONSTR += "\n";
                    }
                }

                JSONSTR += "    ";
                JSONSTR += "]\n";
            }

            // 尾
            {
                JSONSTR += "}";
            }

            return JSONSTR;
        }

        private static string cellJSON(HexCellDynamicComponent cell)
        {
            // 网格单元
            var JSONSTR = "        ";
            JSONSTR += "{\n";
            {

                // 坐标
                {
                    JSONSTR += "            ";
                    JSONSTR += "\"x\":" + cell.x + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "\"y\":" + cell.y + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "\"z\":" + cell.z + ",\n";

                    JSONSTR += "            ";
                    JSONSTR += "\"terrainID\":" + cell.terrainID + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "\"id\":" + cell.Name + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "\"cellType\":" + cell.cellType + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "\"size\":" + cell.size + ",\n";
                    JSONSTR += "            ";
                    if (Parame.exportCellWorldPosition)
                    {
                        JSONSTR += "\"position\":" + cell.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                    }
                    else
                    {
                        JSONSTR += "\"position\":" + cell.transform.localPosition.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                    }
                    JSONSTR += "            ";
                    JSONSTR += "\"scaling\":" + cell.transform.localScale.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                }

                int attrCount = cell.attrNames.Count;
                attrCount = Math.Min(attrCount, cell.attrValues.Count);

                // 属性名
                {
                    JSONSTR += "            ";
                    JSONSTR += "\"attrNames\":[";

                    for (var j = 0; j < attrCount; j++)
                    {
                        JSONSTR += "\"" + cell.attrNames[j] + "\"";

                        if (j != attrCount - 1)
                        {
                            JSONSTR += ",";
                        }
                    }

                    JSONSTR += "],\n";
                }

                // 属性值
                {
                    JSONSTR += "            ";
                    JSONSTR += "\"attrValues\":[";

                    for (var j = 0; j < attrCount; j++)
                    {
                        JSONSTR += "\"" + cell.attrValues[j] + "\"";

                        if (j != attrCount - 1)
                        {
                            JSONSTR += ",";
                        }
                    }

                    JSONSTR += "]\n";
                }
            }

            JSONSTR += "        ";
            JSONSTR += "}";

            return JSONSTR;
        }

        private static string cellArray(HexCellDynamicComponent cell)
        {
            // 网格单元
            var JSONSTR = "        ";
            JSONSTR += "[\n";
            {

                // 坐标
                {
                    JSONSTR += "            ";
                    JSONSTR += "" + cell.x + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "" + cell.y + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "" + cell.z + ",\n";

                    JSONSTR += "            ";
                    JSONSTR += "" + cell.terrainID + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "" + cell.Name + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "" + cell.cellType + ",\n";
                    JSONSTR += "            ";
                    JSONSTR += "" + cell.size + ",\n";
                    JSONSTR += "            ";
                    if (Parame.exportCellWorldPosition)
                    {
                        JSONSTR += "" + cell.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                    }
                    else
                    {
                        JSONSTR += "" + cell.transform.localPosition.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                    }
                    JSONSTR += "            ";
                    JSONSTR += "" + cell.transform.localScale.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                }

                int attrCount = cell.attrNames.Count;
                attrCount = Math.Min(attrCount, cell.attrValues.Count);

                // 属性名
                {
                    JSONSTR += "            ";
                    JSONSTR += "[";

                    for (var j = 0; j < attrCount; j++)
                    {
                        JSONSTR += "\"" + cell.attrNames[j] + "\"";

                        if (j != attrCount - 1)
                        {
                            JSONSTR += ",";
                        }
                    }

                    JSONSTR += "],\n";
                }

                // 属性值
                {
                    JSONSTR += "            ";
                    JSONSTR += "[";

                    for (var j = 0; j < attrCount; j++)
                    {
                        JSONSTR += "\"" + cell.attrValues[j] + "\"";

                        if (j != attrCount - 1)
                        {
                            JSONSTR += ",";
                        }
                    }

                    JSONSTR += "]\n";
                }
            }

            JSONSTR += "        ";
            JSONSTR += "]";

            return JSONSTR;
        }

        public static string getHexGridDynaSimpleJSON(HexGridDynamicComponent hexGridComponent, BackgroundGrid background)
        {
            string JSONSTR = "";

            // 网格单元数组
            {
                JSONSTR += "[\n";

                List<HexCellDynamicComponent> cells = hexGridComponent.getHexCells();

                List<HexCellDynamicComponent> list = new List<HexCellDynamicComponent>();


                foreach (var temp in cells)
                {
                    if (temp.checkActive())
                    {
                        list.Add(temp);
                    }
                }

                int count = list.Count;

                for (int i = 0; i < count; i++)
                {
                    HexCellDynamicComponent cell = list[i];
                    if (cell != null)
                    {
                        JSONSTR += cell.size + ",";

                        JSONSTR += "\"" + cell.transform.name + "\",";

                        Vector3 vec = new Vector3(Mathf.RoundToInt(cell.transform.localScale.x), Mathf.RoundToInt(cell.transform.localScale.y), Mathf.RoundToInt(cell.transform.localScale.z));
                        JSONSTR += vec.ToString().Replace('(', '[').Replace(')', ']') + ",";

                        if (Parame.exportCellWorldPosition)
                        {
                            JSONSTR += "" + cell.transform.position.ToString().Replace('(', '[').Replace(')', ']');
                        }
                        else
                        {
                            JSONSTR += "" + cell.transform.localPosition.ToString().Replace('(', '[').Replace(')', ']');
                        }
                    }
                    else
                    {
                        JSONSTR += "        ";
                        JSONSTR += "null";
                    }

                    if (i != count - 1)
                    {
                        JSONSTR += ",\n";
                    }
                    else
                    {
                        JSONSTR += "\n";
                    }
                }

                JSONSTR += "]\n";
            }

            return JSONSTR;
        }

        public static void exportGridDynBin(GameObject root, HexMapComponent hexMapComponent)
        {

            HexGridDynamicComponent[] hexGrdiCompARR = root.GetComponentsInChildren<HexGridDynamicComponent>();

            List<HexGridDynamicComponent> hexGrdiCompList = new List<HexGridDynamicComponent>();
            foreach (var temp in hexGrdiCompARR)
            {
                if (temp.gameObject.activeInHierarchy)
                {
                    hexGrdiCompList.Add(temp);
                }
            }

            BackgroundGrid background = hexMapComponent.GetComponentInChildren<BackgroundGrid>();

            if (!Parame.exportSimpleCellData)
            {

            }
        }

        public static void exportGridTerrainBin(HexGridDynamicComponent hexGridDyn, Boolean isHex, Boolean isRotate)
        {
            List<HexCellDynamicComponent> temp = hexGridDyn.getHexCells();

            if (isHex || !isRotate)
            {
                return;
            }

            if (temp.Count > 0)
            {
                hexGridDyn.RefreshMinMax();

                var cellsParent = hexGridDyn.transform;
                if (hexGridDyn.GetComponentInChildren<CellListComponent>())
                {
                    cellsParent = hexGridDyn.GetComponentInChildren<CellListComponent>().transform;
                }

                int gridWidth = hexGridDyn.MaxX - hexGridDyn.MinX +1;
                int gridHeight = hexGridDyn.MaxZ - hexGridDyn.MinZ +1;

                int cellCount = 0;

                int perCellCount = 0;

                if (hexGridDyn.EnableCellEdge)
                {
                    perCellCount = 4;
                    cellCount = gridWidth * gridHeight * perCellCount;
                }
                else
                {
                    perCellCount = 1;
                    cellCount = gridWidth * gridHeight;
                }

                byte[] byteArr = new byte[cellCount];

                for (int i = 0; i < gridHeight; i++)
                {
                    for (int j = 0; j < gridWidth; j++)
                    {
                        int currCellIndex = (i * gridWidth + j) * perCellCount;
                        var listNear = HexCoordinates.FindNearCellIDList(j + hexGridDyn.MinX, 0, i + hexGridDyn.MinZ, Parame.RSquareType, isHex, isRotate);
                        var id = HexCoordinates.CellIDFromNearList(listNear);
                        var cell = cellsParent.FindChild(id);
                        HexCellDynamicComponent cellComp = null;
                        if (cell) cellComp = cell.GetComponent<HexCellDynamicComponent>();
                        if (cellComp) cellComp.terrainID = currCellIndex;
                        byteArr[currCellIndex] = cell ? (byte)(cellComp.terrain << 4) : (byte)0;

                        if (hexGridDyn.EnableCellEdge)
                        {
                            cell = null;
                            cellComp = null;
                            listNear = HexCoordinates.FindNearCellIDList(j + hexGridDyn.MinX, 0, i + hexGridDyn.MinZ, Parame.RSquareEdgeType1, isHex, isRotate);
                            id = HexCoordinates.CellIDFromNearList(listNear);
                            cell = cellsParent.FindChild(id);
                            if (cell) cellComp = cell.GetComponent<HexCellDynamicComponent>();
                            if (cellComp) cellComp.terrainID = currCellIndex + 1;
                            byteArr[currCellIndex + 1] = cell ? (byte)(cellComp.terrain << 4) : (byte)0;

                            cell = null;
                            cellComp = null;
                            listNear = HexCoordinates.FindNearCellIDList(j + hexGridDyn.MinX, 0, i + hexGridDyn.MinZ, Parame.RSquareEdgeType4, isHex, isRotate);
                            id = HexCoordinates.CellIDFromNearList(listNear);
                            cell = cellsParent.FindChild(id);
                            if (cell) cellComp = cell.GetComponent<HexCellDynamicComponent>();
                            if (cellComp) cellComp.terrainID = currCellIndex + 2;
                            byteArr[currCellIndex + 2] = cell ? (byte)(cellComp.terrain << 4) : (byte)0;

                            cell = null;
                            cellComp = null;
                            listNear = HexCoordinates.FindNearCellIDList(j + hexGridDyn.MinX, 0, i + hexGridDyn.MinZ, Parame.RSquarePointType1, isHex, isRotate);
                            id = HexCoordinates.CellIDFromNearList(listNear);
                            cell = cellsParent.FindChild(id);
                            if (cell) cellComp = cell.GetComponent<HexCellDynamicComponent>();
                            if (cellComp) cellComp.terrainID = currCellIndex + 3;
                            byteArr[currCellIndex + 3] = cellComp ? (byte)(cellComp.terrain << 4) : (byte)0;
                        }
                    }
                }

                Parame.exportFileName = Path.Combine(Parame.exportPath, hexGridDyn.Name + ".bin");

                var fs = File.Open(Parame.exportFileName, FileMode.Create);
                var jsonWriter = new BinaryWriter(fs);

                jsonWriter.Write(byteArr);

                jsonWriter.Close();
                fs.Close();

                Parame.tempTerrainData = byteArr;
            }
            else
            {

            }
        }
    }
}
