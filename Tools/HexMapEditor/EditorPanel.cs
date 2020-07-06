using HexMapEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
public class EditorPanel : EditorWindow
{
    [MenuItem("Tools/HexMapEditor")]
    public static void ShowWindow()
    {
        var window = GetWindow<EditorPanel>();
        window.Show();
    }

    private void OnEnable()
    {
        HexMetrics.Init();
    }

    private void OnGUI()
    {
        //var tex = new Texture2D(64, 16);
        //tex = Texture2D.blackTexture;
        //GUI.Box(new Rect(0, 0, 64, 16), tex);

        GUILayout.BeginHorizontal();
        GUILayout.Label("输入要创建的 HexMap 名称:", EditorStyles.boldLabel);
        Parame.hexMapName = EditorGUILayout.TextField(Parame.hexMapName);
        if (GUILayout.Button("创建HexMap"))
        {
            CreateHexMap();
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(Parame.hexMapNameDesc, MessageType.Info);
        //GUILayout.Label("==============================================================", EditorStyles.boldLabel);

        //Parame.activeDown = EditorGUILayout.ToggleLeft("鼠标按下时响应", Parame.activeDown);
        //Parame.activeDrag = EditorGUILayout.ToggleLeft("鼠标按下拖动时响应", Parame.activeDrag);
        //Parame.activeUp = EditorGUILayout.ToggleLeft("鼠标弹起时时响应", Parame.activeUp);
        Parame.activeEditor = EditorGUILayout.ToggleLeft("启用编辑", Parame.activeEditor);

        EditorGUILayout.HelpBox(Parame.activeDesc, MessageType.Info);

        GUILayout.BeginHorizontal();
        GUILayout.Label("编辑 HexGridDynamic 的 正方形 时 锚点类型:");
        Parame.planIndex = EditorGUILayout.Popup(Parame.planIndex, Parame.planOptions);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("编辑 HexGridDynamic 的 菱形 时 锚点类型:");
        Parame.diamondIndex = EditorGUILayout.Popup(Parame.diamondIndex, Parame.diamondOptions);
        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox(Parame.OptionsDesc, MessageType.Info);

        //GUILayout.Label("==============================================================", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        GUILayout.Label("编辑 HexGridDynamic 的 正方形 时 距离场距离:");
        Parame.distance = EditorGUILayout.IntField(Parame.distance);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("编辑 HexGridDynamic 时 使用距离场创建多个单元格时，单元格生成规则:");
        Parame.distanceTypeIndex = EditorGUILayout.Popup(Parame.distanceTypeIndex, Parame.distanceTypes);
        GUILayout.EndHorizontal();

        
        checkHexCellDynamicDistance();

        GUILayout.BeginHorizontal();
        GUILayout.Label("编辑 HexGridDynamic 时 选中的两个单元的距离场距离:");
        GUILayout.Button("计算");
        Parame.realTimeDistance = EditorGUILayout.IntField(Parame.realTimeDistance);
        GUILayout.EndHorizontal();
        
        EditorGUILayout.HelpBox(Parame.distanceDesc, MessageType.Info);

        //GUILayout.Label("==============================================================", EditorStyles.boldLabel);

        // Export path
        GUILayout.Label("Export Path - # 选中要导出的 HexMap 节点 #", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        Parame.exportPath = EditorGUILayout.TextField(Parame.exportPath);
        if (GUILayout.Button("...", GUILayout.Width(48), GUILayout.Height(16)))
        {
            Parame.exportPath = EditorUtility.OpenFolderPanel("Export Path", Parame.exportPath, "");
        }
        GUILayout.EndHorizontal();

        Parame.exportHexGrid = EditorGUILayout.ToggleLeft("是否导出 HexGrid", Parame.exportHexGrid);
        Parame.exportHexGridDynamic = EditorGUILayout.ToggleLeft("是否导出 HexGridDynamic", Parame.exportHexGridDynamic);

        EditorGUILayout.HelpBox(Parame.exportHexGridDynamicDesc, MessageType.Info);

        Parame.exportCellWorldPosition = EditorGUILayout.ToggleLeft("是否导出 单元格 绝对位置", Parame.exportCellWorldPosition);

        EditorGUILayout.HelpBox(Parame.exportCellWorldPositionDesc, MessageType.Info);

        Parame.exportSimpleCellData = EditorGUILayout.ToggleLeft("导出 HexGridDynamic时 是否 仅导出 单元格数据", Parame.exportSimpleCellData);

        EditorGUILayout.HelpBox(Parame.exportSimpleCellDataDesc, MessageType.Info);

        if (GUILayout.Button(" 导出网格数据 ", GUILayout.Width(180), GUILayout.Height(24)))
        {
            if (checkExport())
            {
                doExport();
            }
        }
    }


    void OnSceneGUI(SceneView sceneView)
    {
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        Event.current.GetTypeForControl(controlId);

        if (Event.current.type == EventType.MouseDrag && Parame.activeEditor)
        {
            if (getSelectHexCellTemplate() != null)
            {
                GUIUtility.hotControl = controlId;

                checkRayHitMouseDrag(sceneView);
                //if (checkRayHitMouseDrag(sceneView))
                //{
                //    GUIUtility.hotControl = controlId;
                //}
            }
            else if (getSelectHexCellDynamicTemplate() != null)
            {
                GUIUtility.hotControl = controlId;

                checkRayHitDownDynamic(sceneView);
            }
        }

        if (Event.current.type == EventType.MouseDown && Parame.activeEditor)
        {
            if (getSelectHexCellTemplate() != null)
            {
                GUIUtility.hotControl = controlId;

                checkRayHitMouseDrag(sceneView);
                //if (checkRayHitMouseDrag(sceneView))
                //{
                //    GUIUtility.hotControl = controlId;
                //}
            } else if (getSelectHexCellDynamicTemplate() != null)
            {
                GUIUtility.hotControl = controlId;

                checkRayHitDownDynamic(sceneView);
            }
        }

        if (Event.current.type == EventType.MouseUp && Parame.activeUp)
        {
            if (getSelectHexCellTemplate() != null)
            {
                GUIUtility.hotControl = controlId;

                checkRayHitMouseDrag(sceneView);
            }
            else if (getSelectHexCellDynamicTemplate() != null)
            {
                GUIUtility.hotControl = controlId;

                checkRayHitDownDynamic(sceneView);
            }
        }

        // 刷新界面
        sceneView.Repaint();

        EditorUtility.SetDirty(sceneView);
    }

    private void CreateHexMap()
    {
        var gameObject = new GameObject();
        gameObject.name = Parame.hexMapName;
        gameObject.transform.name = Parame.hexMapName;
        gameObject.AddComponent<HexMapComponent>().Name = Parame.hexMapName;
        gameObject.transform.localPosition = new Vector3(0, 0, 0);
        gameObject.transform.localScale = new Vector3(1, 1, 1);
        gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);

    }

    private Boolean checkExport()
    {
        Boolean flag = false;
        if (Parame.exportPath != null && Parame.exportPath.Length > 0)
        {

            if (Directory.Exists(Parame.exportPath))
            {
                flag = true;
            }

        }

        return flag;
    }

    private Boolean checkSelectHexMap()
    {
        Boolean flag = false;

        if (Selection.gameObjects.Length == 1)
        {
            if (Selection.gameObjects[0].GetComponent<HexMapComponent>())
            {
                flag = true;
            }
        }

        return flag;
    }

    private void doExport()
    {
        GameObject root = null;
        HexMapComponent hexMapComponent = null;

        if (Selection.gameObjects.Length == 1)
        {
            root = Selection.gameObjects[0];
            hexMapComponent = root.GetComponent<HexMapComponent>();

            if (hexMapComponent)
            {

                Parame.exportFileName = Path.Combine(Parame.exportPath, hexMapComponent.Name + ".hexgrid");

                string JSONSTR = "";
                int count = 0;

                JSONSTR += "[\n";

                if (Parame.exportHexGridDynamic)
                {
                    string childJSON = exportGridsDynamic(root, hexMapComponent);
                    JSONSTR += childJSON;

                    count++;
                }

                if (Parame.exportHexGrid)
                {
                    if (count > 0)
                    {
                        JSONSTR += ",\n";
                    }
                    string childJSON = exportGrids(root, hexMapComponent);
                    JSONSTR += childJSON;

                    count++;
                }

                JSONSTR += "]\n";
                
                var fs = File.Open(Parame.exportFileName, FileMode.Create);
                var jsonWriter = new StreamWriter(fs);

                jsonWriter.Write(JSONSTR);

                jsonWriter.Close();
                fs.Close();
            }
        }
        else
        {
            Debug.LogWarning("请选择需要导出的 HexGridComponent 节点");
        }
    }

    private string exportGrids(GameObject root, HexMapComponent hexMapComponent)
    {
        HexGridComponent[] hexGrdiCompARR = root.GetComponentsInChildren<HexGridComponent>();

        List<HexGridComponent> hexGrdiCompList = new List<HexGridComponent>();
        foreach (var temp in hexGrdiCompARR)
        {
            if (temp.gameObject.activeInHierarchy)
            {
                hexGrdiCompList.Add(temp);
            }
        }

        string JSONSTR = "";

        BackgroundGrid background = hexMapComponent.GetComponentInChildren<BackgroundGrid>();

        JSONSTR += "{\n";
        JSONSTR += "\"name\":\"" + hexMapComponent.Name + "\",\n";
        JSONSTR += "\"position\":" + hexMapComponent.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
        JSONSTR += "\"rotation\":" + hexMapComponent.transform.rotation.eulerAngles.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
        JSONSTR += "\"grids\":[\n";

        int count = hexGrdiCompList.Count;

        for (int i = 0; i < count; i++)
        {
            var temp = hexGrdiCompList[i];

            JSONSTR += getHexGridJSON(temp, background);

            if (i < count - 1)
            {
                JSONSTR += ",\n";
            }
        }

        JSONSTR += "\n";

        JSONSTR += "]\n";
        JSONSTR += "}\n";
        Debug.LogWarning(JSONSTR);

        return JSONSTR;
    }


    private string exportGridsDynamic(GameObject root, HexMapComponent hexMapComponent)
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

        string JSONSTR = "";

        BackgroundGrid background = hexMapComponent.GetComponentInChildren<BackgroundGrid>();

        if (!Parame.exportSimpleCellData)
        {
            JSONSTR += "{\n";
            JSONSTR += "\"name\":\"" + hexMapComponent.Name + "\",\n";
            JSONSTR += "\"position\":" + hexMapComponent.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
            JSONSTR += "\"rotation\":" + hexMapComponent.transform.rotation.eulerAngles.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
            JSONSTR += "\"grids\":[\n";

            int count = hexGrdiCompList.Count;

            for (int i = 0; i < count; i++)
            {
                var temp = hexGrdiCompList[i];

                JSONSTR += getHexGridDynaJSON(temp, background);

                if (i < count - 1)
                {
                    JSONSTR += ",\n";
                }
            }

            JSONSTR += "\n";

            JSONSTR += "]\n";
            JSONSTR += "}\n";
        }
        else
        {
            int count = hexGrdiCompList.Count;

            for (int i = 0; i < count; i++)
            {
                var temp = hexGrdiCompList[i];

                JSONSTR += getHexGridDynaSimpleJSON(temp, background);

                if (i < count - 1)
                {
                    JSONSTR += ",\n";
                }
            }
        }

        Debug.LogWarning(JSONSTR);

        return JSONSTR;
    }

    private string getHexGridJSON(HexGridComponent hexGridComponent, BackgroundGrid background)
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

    private string getHexGridDynaJSON(HexGridDynamicComponent hexGridComponent, BackgroundGrid background)
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
                            JSONSTR += "\"size\":" + cell.size + ",\n";
                            JSONSTR += "            ";
                            if (Parame.exportCellWorldPosition)
                            {
                                JSONSTR += "\"position\":" + cell.transform.position.ToString().Replace('(', '[').Replace(')', ']') + ",\n";
                            } else
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

    private string getHexGridDynaSimpleJSON(HexGridDynamicComponent hexGridComponent, BackgroundGrid background)
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

    /// <summary>
    /// https://blog.csdn.net/akof1314/java/article/details/78639075
    /// </summary>
    /// <param name="sceneView"></param>
    /// <param name="mesh"></param>
    /// <returns></returns>
    private Boolean checkRayHitMouseDrag(SceneView sceneView)
    {
        Boolean flag = false;

        // 当前屏幕坐标，左上角是（0，0）右下角（camera.pixelWidth，camera.pixelHeight）
        Vector2 mousePosition = Event.current.mousePosition;

        // Retina 屏幕需要拉伸值
        float mult = 1;
#if UNITY_5_4_OR_NEWER
        mult = EditorGUIUtility.pixelsPerPoint;
#endif

        // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
        mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y * mult;
        mousePosition.x *= mult;

        
        HexCellTemplate templateCell = getSelectHexCellTemplate();

        if (templateCell != null)
        {

            Ray ray = sceneView.camera.ScreenPointToRay(mousePosition);

            HexCellComponent[] componentsInChildren = GameObject.FindObjectsOfType<HexCellComponent>();
            float num = float.PositiveInfinity;
            foreach (HexCellComponent hexCellComp in componentsInChildren)
            {
                MeshFilter meshFilter = hexCellComp.gameObject.GetComponent<MeshFilter>();
                Mesh sharedMesh = meshFilter.sharedMesh;
                RaycastHit hit;
                if (sharedMesh
                    && RXLookingGlass.IntersectRayMesh(ray, sharedMesh, meshFilter.transform.localToWorldMatrix, out hit)
                    && hit.distance < num)
                {
                    flag = true;

                    //Color color = new Color(0.5f, 0, 0, 0.5f);
                    //hexCellComp.UpdateColor(color);

                    hexCellComp.UpdateFromTemplate(templateCell);
                    break;
                }
            }
        }


        return flag;
    }


    /// <summary>
    /// https://blog.csdn.net/akof1314/java/article/details/78639075
    /// </summary>
    /// <param name="sceneView"></param>
    /// <param name="mesh"></param>
    /// <returns></returns>
    private Boolean checkRayHitDownDynamic(SceneView sceneView)
    {
        Boolean flag = false;

        // 当前屏幕坐标，左上角是（0，0）右下角（camera.pixelWidth，camera.pixelHeight）
        Vector2 mousePosition = Event.current.mousePosition;

        // Retina 屏幕需要拉伸值
        float mult = 1;
#if UNITY_5_4_OR_NEWER
        mult = EditorGUIUtility.pixelsPerPoint;
#endif

        // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
        mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y * mult;
        mousePosition.x *= mult;

        HexCellDynamicTemplate templateCellDyn = getSelectHexCellDynamicTemplate();
        if (templateCellDyn)
        {
            Ray ray = sceneView.camera.ScreenPointToRay(mousePosition);

            HexMapComponent hexMap = templateCellDyn.GetComponentInParent<HexMapComponent>();

            BackgroundGrid backgroundGrid = hexMap.gameObject.GetComponentInChildren<BackgroundGrid>();

            /// 处理 distance
            if (templateCellDyn.cellSize != backgroundGrid.cellSize)
            {
                Parame.distance = 0;
            }

            List<CustomRayInfo> list = backgroundGrid.rayCast(ray);

            if (list.Count > 0)
            {
                flag = true;

                //var grid = hexCellComp.gameObject.transform.parent.GetComponent<HexGridComponent>();

                var gridDyn = templateCellDyn.transform.parent.parent.gameObject;

                var cellListComp = gridDyn.GetComponentInChildren<CellListComponent>();

                foreach (var info in list)
                {
                    if (cellListComp)
                    {
                        if (cellListComp.transform.FindChild(info.getName()) || gridDyn.transform.FindChild(info.getName()))
                        {

                        }
                        else
                        {
                            templateCellDyn.GetComponentInParent<HexGridDynamicComponent>().CreateCellDynamic(info.ix, info.iy, info.iz, backgroundGrid.isHex, backgroundGrid.isRotate, backgroundGrid.cellSize, info.getPos(), templateCellDyn);
                        }
                    }
                    else
                    {
                        if (gridDyn.transform.FindChild(info.getName()))
                        {

                        }
                        else
                        {
                            templateCellDyn.GetComponentInParent<HexGridDynamicComponent>().CreateCellDynamic(info.ix, info.iy, info.iz, backgroundGrid.isHex, backgroundGrid.isRotate, backgroundGrid.cellSize, info.getPos(), templateCellDyn);
                        }
                    }
                }
            }

            //HexCellComponent[] componentsInChildren = GameObject.FindObjectsOfType<HexCellComponent>();
            //float num = float.PositiveInfinity;
            //foreach (HexCellComponent hexCellComp in componentsInChildren)
            //{
            //    MeshFilter meshFilter = hexCellComp.gameObject.GetComponent<MeshFilter>();
            //    Mesh sharedMesh = meshFilter.sharedMesh;
            //    RaycastHit hit;
            //    if (sharedMesh
            //        && RXLookingGlass.IntersectRayMesh(ray, sharedMesh, meshFilter.transform.localToWorldMatrix, out hit)
            //        && hit.distance < num)
            //    {
            //        flag = true;

            //        var grid = hexCellComp.gameObject.transform.parent.GetComponent<HexGridComponent>();

            //        var gridDyn = templateCellDyn.transform.parent.parent.gameObject;

            //        if (gridDyn.transform.FindChild(hexCellComp.name))
            //        {

            //        } else
            //        {
            //            templateCellDyn.GetComponentInParent<HexGridDynamicComponent>().CreateCellDynamic(hexCellComp.x, hexCellComp.y, hexCellComp.z, grid.cellSize, hexCellComp.transform.position, templateCellDyn);
            //        }

            //        break;
            //    }
            //}
        }


        return flag;
    }

    private HexCellTemplate getSelectHexCellTemplate()
    {
        HexCellTemplate templateCell = null;
        if (Selection.gameObjects.Length == 1)
        {
            var temp = Selection.gameObjects[0];
            templateCell = temp.GetComponent<HexCellTemplate>();
        }

        return templateCell;
    }

    private void checkHexCellDynamicDistance()
    {
        Parame.realTimeDistance = 0;
        if (Selection.gameObjects.Length == 2)
        {
            var dyn0 = Selection.gameObjects[0].GetComponent<HexCellDynamicComponent>();
            var dyn1 = Selection.gameObjects[1].GetComponent<HexCellDynamicComponent>();

            if (dyn0 != null && dyn1 != null)
            { 
                var hexMap = dyn0.transform.GetComponentInParent<HexMapComponent>();
                if (hexMap)
                {
                    var background = hexMap.GetComponentInChildren<BackgroundGrid>();
                    if (background && background.cellSize == dyn0.size && background.cellSize == dyn1.size)
                    {
                        if (background.isHex)
                        {
                            Parame.realTimeDistance = HexCoordinates.ComputeDistance(
                                HexCoordinates.FromIntPostionHex(dyn0.x, dyn0.y, dyn0.z, background.cellSize, background.isRotate),
                                HexCoordinates.FromIntPostionHex(dyn1.x, dyn1.y, dyn1.z, background.cellSize, background.isRotate)
                                );
                        } else
                        {
                            Parame.realTimeDistance = HexCoordinates.ComputeDistance(
                                HexCoordinates.FromIntPostionSquare(dyn0.x, dyn0.y, dyn0.z, background.cellSize, background.isRotate),
                                HexCoordinates.FromIntPostionSquare(dyn1.x, dyn1.y, dyn1.z, background.cellSize, background.isRotate)
                                );
                        }
                    }
                }
            }
        }
    }

    private HexCellDynamicTemplate getSelectHexCellDynamicTemplate()
    {
        HexCellDynamicTemplate templateCell = null;
        if (Selection.gameObjects.Length == 1)
        {
            var temp = Selection.gameObjects[0];
            templateCell = temp.GetComponent<HexCellDynamicTemplate>();
        }

        return templateCell;
    }

    private void OnFocus()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
        SceneView.onSceneGUIDelegate += this.OnSceneGUI;
        Repaint();
    }

    void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    //private void OnLostFocus()
    //{
    //    SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    //}

    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }

    //void OnSceneMouseOver(SceneView view)
    //{
    //    if (Event.current.type == EventType.MouseDrag)
    //    {
    //        Debug.LogWarning("ddddddddd");
    //    }

    //    if (Event.current.type == EventType.MouseDown)
    //    {
    //        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
    //        RaycastHit hit;

    //        var res = Physics.Raycast(ray, out hit);

    //        //And add switch Event.current.type for checking Mouse click and switch tiles
    //        if (res)
    //        {
    //            var hexCellComp = hit.transform.gameObject.GetComponent<HexCellComponent>();

    //            if (hexCellComp)
    //            {
    //                Debug.DrawRay(ray.origin, hit.transform.position, Color.blue, 5f);
    //            }
    //        }
    //        Debug.LogWarning("aaaaaaaaaaa");
    //    }

    //    if (Event.current.type == EventType.MouseMove)
    //    {
    //        Debug.LogWarning("mmmmmmmmm");
    //    }
    //}
}
