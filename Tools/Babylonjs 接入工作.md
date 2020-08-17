
# Babylon 接入工作

## 构建

* 能从Babylonjs的ts源码全部构建成大的js
* 对于不支持Promise的移动浏览器，还需要用Babel模拟
* 对于我们改动的引擎层的代码，全部用 PI_BEGIN, PI_END注释下

``` js

// PI_BEGIN

// 改动部分

// PI_END

```

## GUI，白鹏童鞋

* 预期：2周
* 场景2D
   + 如何跟着3D物件走
* GUI
   + 调研
      + 使用
         * 哪些组件
            + 新的组件的扩展
         * 数据绑定：如何实现
         * 事件，交互
         * 特效：babylon现有支持程度
            + 运动动画
            + 淡入淡出
         * 和原有项目的HTML功能
            + 列出babylon明显不支持的功能，讨论如何实现
         * Canvas2D 功能
            + 字体
            + 其他功能了解，注意结合项目，关注和项目有关的功能
      + 原理
         * Event 事件：冒泡，捕获
         * 渲染：到纹理 Texture
            + 脏更新 dirty
      + 性能测试
         * 对canvas2D 和 GUI 分别 进行性能测试
         * 大规模GUI空间的性能测试
         * 内容变化的GUI性能测试
   + 集成到项目中
      + 从paint方法入手
      + 不需要tpl，wcss，用代码组织类

## 场景 & Unity导入

* Engine 的参数
   + 抗锯齿 antialias
   + disableWebGL2Support 禁止webgl2
   + enableOfflineSupport = false 不需要IndexedDB
   + disableManifestCheck 不需要manifest检查
   + 其他选项，有个了解
* Scene
   + Octree 相机裁剪
   + Node
   + Camera
      * Free
      * 绑在某个物体的相机
      * 相机控制器
         + 相机轨迹
   + Mesh
   + Light
   + Fog
* gltf 2.0
   + 了解：格式
   + Unity 导出插件，代码：了解
   + babylon loader 代码：了解
   + 每个我们的功能的导出和导入实现

## 渲染 相关

* 公告牌 Billboard，精灵
   + 调研：babylon中关于公告牌的使用方式
* 粒子系统 particle
   + 调研 GPU粒子
      * 功能是否足够，和unity对比差在哪里，和美术确定下需求
      * 导出插件格式
      * webgl 1.0 和 低端手机 上 性能对比 
   + 确定美术能不能使用babylon的粒子系统编辑器
      * 如果一定要用unity，要确定unity的粒子属性哪些是babylon支持的
* 动画
   + 调研
      * 功能 & 使用：是否足够
         + 节点动画
            * 空间变动：translation，rotation，scale
            * 材质( ? )：uv，图片，alpha ...
         + Morph动画（顶点动画）
         + 骨骼动画
      * 导出插件是否都有相应的导出功能
   + 集成到项目中，项目开发
* 材质 & Shader
   + 参考SimpleMaterial，了解如何自定义材质
   + 另外写一个材质，GanChuMaterial
      * 雾
      * 简单光照
      * 两张纹理：一张正常纹理，一张光照图
      * 骨骼蒙皮 
      * 其他需要的功能
* 后处理 & Shader（优先级：低）
   + 调研：babylon的后处理支持的功能和使用

## 性能 & 优化

* 优先级最低，集成之后再处理