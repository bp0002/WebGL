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

        Parame.EditEdge = EditorGUILayout.ToggleLeft("编辑 边", Parame.EditEdge);
        Parame.EditPoint = EditorGUILayout.ToggleLeft("编辑 点", Parame.EditPoint);

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
        
        Parame.exportHexGridDynamicTerrain = EditorGUILayout.ToggleLeft(Parame.exportHexGridDynamicTerrainDesc, Parame.exportHexGridDynamicTerrain);

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

                try
                {
                    checkRayHitMouseDrag(sceneView);
                }
                catch
                {

                }
                //if (checkRayHitMouseDrag(sceneView))
                //{
                //    GUIUtility.hotControl = controlId;
                //}
            }
            else if (getSelectHexCellDynamicTemplate() != null)
            {
                GUIUtility.hotControl = controlId;

                try
                {
                    checkRayHitDownDynamic(sceneView);
                }
                catch
                {

                }
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
                try
                {
                    checkRayHitDownDynamic(sceneView);
                }
                catch
                {

                }
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
        EditorExport.doExport();
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

            var gridDyn = templateCellDyn.transform.parent.parent.gameObject;

            var hexGridDyn = templateCellDyn.GetComponentInParent<HexGridDynamicComponent>();

            var cellListComp = gridDyn.GetComponentInChildren<CellListComponent>();

            /// 处理 distance
            if (templateCellDyn.cellSize != backgroundGrid.cellSize)
            {
                Parame.distance = 0;
            }

            if (hexGridDyn.EnableCellEdge)
            {
                Parame.distance = 0;
            }

            if (hexGridDyn.EnableCellEdge && backgroundGrid.cellSize != templateCellDyn.cellSize)
            {
                Debug.LogWarning(Parame.edgeWaring);
                return false;
            }

            Parame.hitInfoFlag = false;
            List<CustomRayInfo> list = backgroundGrid.rayCast(ray);

            if (list.Count > 0)
            {
                flag = true;

                if (Parame.hitInfoFlag)
                {
                    var hitLocalBackgroundPos = Parame.hitInfo.point;

                    //var grid = hexCellComp.gameObject.transform.parent.GetComponent<HexGridComponent>();

                    foreach (var info in list)
                    {
                        byte cellType = 0;
                        var hitLocalCellPos = hitLocalBackgroundPos - info.getPos();

                        if (hexGridDyn.EnableCellEdge && backgroundGrid.cellSize == templateCellDyn.cellSize)
                        {
                            cellType = HexCoordinates.CheckHitCellType(hitLocalCellPos, hexGridDyn.CellEdgeWidth, backgroundGrid.cellSize, backgroundGrid.isHex, backgroundGrid.isRotate);
                            if (backgroundGrid.isHex)
                            {

                            }
                            else
                            {
                                if (!Parame.EditPoint && ((cellType > 20 && cellType < 30) || cellType > 40 ) )
                                {
                                    continue;
                                }
                                if (!Parame.EditEdge && ((cellType > 10 && cellType < 20) || (cellType > 30 && cellType < 40)))
                                {
                                    continue;
                                }
                                if ((Parame.EditPoint || Parame.EditEdge) && cellType < 10)
                                {
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if (backgroundGrid.isHex)
                            {
                                if (backgroundGrid.isRotate)
                                {
                                    cellType = Parame.RHexType;
                                }
                                else
                                {
                                    cellType = Parame.HexType;
                                }
                            }
                            else
                            {
                                if (backgroundGrid.isRotate)
                                {
                                    cellType = Parame.RSquareType;
                                }
                                else
                                {
                                    cellType = Parame.SquareType;
                                }
                            }
                        }

                        if (cellType > 10)
                        {
                            List<int> specialNear = HexCoordinates.FindNearCellIDList(info.ix, info.iy, info.iz, cellType, backgroundGrid.isHex, backgroundGrid.isRotate);
                            string specialName = HexCoordinates.CellIDFromNearList(specialNear);
                            
                            if ((!cellListComp || !cellListComp.transform.FindChild(specialName)) && !gridDyn.transform.FindChild(specialName))
                            {
                                hexGridDyn.UpdateMinMax(specialNear);
                                hexGridDyn.CreateCellDynamic(specialNear[0], specialNear[1], specialNear[2], backgroundGrid.isHex, backgroundGrid.isRotate, backgroundGrid.cellSize, info.getPos() - backgroundGrid.transform.position, templateCellDyn, cellType, specialName);
                            }
                        }
                        else
                        {
                            List<int> specialNear = HexCoordinates.FindNearCellIDList(info.ix, info.iy, info.iz, cellType, backgroundGrid.isHex, backgroundGrid.isRotate);
                            string specialName = HexCoordinates.CellIDFromNearList(specialNear);

                            if ((!cellListComp || !cellListComp.transform.FindChild(specialName)) && !gridDyn.transform.FindChild(specialName))
                            {
                                hexGridDyn.UpdateMinMax(specialNear);
                                hexGridDyn.CreateCellDynamic(specialNear[0], specialNear[1], specialNear[2], backgroundGrid.isHex, backgroundGrid.isRotate, backgroundGrid.cellSize, info.getPos() - backgroundGrid.transform.position, templateCellDyn, cellType, specialName);
                            }
                        }

                    }
                }

            }

            Parame.hitInfoFlag = false;
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
    
}
