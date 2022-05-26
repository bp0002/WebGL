# Unity 烘培光照 与 实时光照混合

## 光照图设置

* 使Unity工作于 RGBM 模式
  * 项目设置-> Player -> 光照图质量为 Normal
  * 光照图烘培界面 选择 高质量结果

## 光照类型

* 只有 Realtime 或 Baked

## 拓展:

### 实时光照控制

#### 实时光照 的 光照图模式可设置

* 为目标灯光添加 LightLightmapMode 组件
  * 修改 LightmapMode 为 LIGHTMAP_SHADOWONLY
    * 实时渲染时该光照作用于有烘培光照图的目标时，仅计算阴影