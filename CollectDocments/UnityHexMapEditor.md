# Unity 网格地图编辑器

## 需求

* 在编辑模式下
* 生成背景网格辅助编辑
* 生成 正方形或六边形单元格 组成的网格
* 单个单元格可添加自定义数据 (键值对)
* 单个单元格可调整位置
* 单个单元格可调整大小
* 单个单元格可调整渲染颜色
* 可设置单元格数据模板
* 可通过鼠标拖动快速应用模板数据到生成单元格
* 可获取选中的两个单元格在网格系统中的距离
* 可快速生成目标单元格处指定距离场范围内所有单元格

## 设计

* 节点树
```
    编辑根节点 - HexMap
        + 单元格模板数据列表父节点
            + 单元格模板数据节点
            + ...
        + 网格节点
            + 单元格
            + ...
        + 网格节点2...
```

## 要点

### 参考

* https://gameinstitute.qq.com/community/detail/123579
* https://catlikecoding.com/unity/tutorials/hex-map/part-1/

### 为 编辑节点 附加组件

* 创建对应组件类 继承 MonoBehaviour
```
    public class HexGridComponent : MonoBehaviour {

    }
```
* 为对应节点应用 写好的组件
    + 选中节点
    + 在 Inspector 面板 > 'Add Component' > Scripts > 目标组件

* 上面实现的组件附加到节点后 public 修饰的 实现会在其面板中显示出来，并可修改(如果非只读)


### 为自定义节点组件增加交互

* 为对应组件实现 CustomEditor
```
    [CustomEditor(typeof (HexGridComponent))]
    public class HexGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            /// 将组件上的 public 属性显示出来
            DrawDefaultInspector();

            /// 额外增加交互元素...
            /// GUILayout.Button("Create")
        }

    }
```

### 编辑时组件上需要的一些 数据需要保存更新

* 在面板节点被选中时,其组件和组件对应Editor响应 OnEnable 事件,适合用来做更新
```
    [CustomEditor(typeof (HexMapComponent))]
    public class HexMapEditor : Editor
    {
        private HexMapComponent hexMap;
        private GameObject gameObject;
        private GameObject template;

        private void OnEnable()
        {
            /// ....
        }
    }
```

### 正方形模型数据构建

* 点 和 面 - 以标准大小构建
```
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
```

### 六边形模型数据构建

* 点 和 面 - 以标准大小构建
```
            innerRadius = outerRadius * 0.866025404f;

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(HexMetrics.innerRadius, 0, HexMetrics.outerRadius * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(HexMetrics.innerRadius, 0, -HexMetrics.outerRadius * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, HexMetrics.outerRadius));
            HexMetrics.HexPlaneVertex.Add(new Vector3(HexMetrics.innerRadius, 0, HexMetrics.outerRadius * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(-HexMetrics.innerRadius, 0, HexMetrics.outerRadius * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, HexMetrics.outerRadius));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(-HexMetrics.innerRadius, 0, -HexMetrics.outerRadius * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(-HexMetrics.innerRadius, 0, HexMetrics.outerRadius * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, -HexMetrics.outerRadius));
            HexMetrics.HexPlaneVertex.Add(new Vector3(-HexMetrics.innerRadius, 0, -HexMetrics.outerRadius * 0.5f));

            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, 0));
            HexMetrics.HexPlaneVertex.Add(new Vector3(HexMetrics.innerRadius, 0, -HexMetrics.outerRadius * 0.5f));
            HexMetrics.HexPlaneVertex.Add(new Vector3(0, 0, -HexMetrics.outerRadius));

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
```

### 需要可操作 Scene 视图内容的界面

* 创建 编辑界面 - EditorWindow
```
[CanEditMultipleObjects]
public class EditorPanel : EditorWindow
{
    [MenuItem("Tools/HexMapEditor")]
    public static void ShowWindow()
    {
        var window = GetWindow<EditorPanel>();
        window.Show();
    }
}
```

* 增加交互元素
```
    private void OnGUI()
    {
        ///  ...
    }
```

* 接入/卸载 Scene 视图内事件响应 - 同时消耗掉鼠标拖动事件
```
    void OnSceneGUI(SceneView sceneView)
    {
        int controlId = GUIUtility.GetControlID(FocusType.Passive);
        Event.current.GetTypeForControl(controlId);

        if (Event.current.type == EventType.MouseDrag)
        {
            if (checkRayHitMouseDrag(sceneView))
            {
                /// 消耗掉鼠标拖动事件
                GUIUtility.hotControl = controlId;
            }
        }

        if (Event.current.type == EventType.MouseDown)
        {
            if (getSelectHexCellTemplate() != null)
            {
                /// 消耗掉鼠标事件
                GUIUtility.hotControl = controlId;

                checkRayHitMouseDrag(sceneView);
                //if (checkRayHitMouseDrag(sceneView))
                //{
                //    GUIUtility.hotControl = controlId;
                //}
            }
        }

        // 刷新界面
        sceneView.Repaint();
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

    private void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= this.OnSceneGUI;
    }
```

### Scene 视图内命中检测

* 转换事件坐标
```

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
```

* 生成射线

```
        Ray ray = sceneView.camera.ScreenPointToRay(mousePosition);
```

* 检测
```
    float num = float.PositiveInfinity;
    if (RXLookingGlass.IntersectRayMesh(ray, sharedMesh, meshFilter.transform.localToWorldMatrix, out hit)
         && hit.distance < num)
    {
        
    }
```

* 第三方工具 RXLookingGlass - https://github.com/slipster216/VertexPaint/blob/master/Editor/RxLookingGlass.cs
```
using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.Linq;
using System.Reflection;

[InitializeOnLoad]
public class RXLookingGlass
{
    public static Type type_HandleUtility;
    protected static MethodInfo meth_IntersectRayMesh;

    static RXLookingGlass()
    {
        var editorTypes = typeof(Editor).Assembly.GetTypes();

        type_HandleUtility = editorTypes.FirstOrDefault(t => t.Name == "HandleUtility");
        meth_IntersectRayMesh = type_HandleUtility.GetMethod("IntersectRayMesh", (BindingFlags.Static | BindingFlags.NonPublic));
    }

    public static bool IntersectRayMesh(Ray ray, MeshFilter meshFilter, out RaycastHit hit)
    {
        return IntersectRayMesh(ray, meshFilter.mesh, meshFilter.transform.localToWorldMatrix, out hit);
    }

    public static bool IntersectRayMesh(Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit)
    {
        var parameters = new object[] { ray, mesh, matrix, null };
        bool result = (bool)meth_IntersectRayMesh.Invoke(null, parameters);
        hit = (RaycastHit)parameters[3];
        return result;
    }
}
```

### 距离场功能

* 按网格系统计算距离
* 按网格系统查询指定距离的相关单元格