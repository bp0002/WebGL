# 已支持的功能
* 一般导出
    + 模型
    + 动画
        + TRS 动画
        + 相机动画
        + UV动画
        + 骨骼动画
    + 骨骼
    + 模型法线
* three 使用的 粒子材质模型
* 光照图
* 方向光
* 点光源
* 相机
    + 正交
    + 透视
* Unity 粒子系统

# 注意点
* 导出使用左手坐标系,导入也使用左手坐标系,则 Unity 中节点的 TRS 属性可以直接用于运行时的场景节点

# Shader 窗口i/
* Pi/Particle
    + 基本粒子 / GPU粒子
    + 可修改 UV
    + 可修改 Alpha 混合
    ![](/uploads/front_library/images/m_550b35cf341b741e29c53a18b9dd3420_r.png)
* Pi/Mobile
    + Diffuse/BumpedDiffuse
    + 即 Mobile/Diffuse 与 Mobile/BumpedDiffuse
    + 并不能做导出用,仅做效果对比用
* Pi/Simple
    + 直接渲染图片效果
        + JPG
        + png
    + 光照图
        + 使用烘培结果
        + 直接指定光照图
![](/uploads/front_library/images/m_18cdb23b4944fa40d0a8bc05b7c4b651_r.png)

* Pi/MyShader/Partice
    + 即原three的shader: Myshader/Particle
    + PS: 为减少修改,原 Myshader/Particle 可不修改，导出结果与 Pi/Partice 相同
* Pi/Shadow
    + 实时阴影
    + 导出时需要选择导出法线

# 导出窗口
![](/uploads/front_library/images/m_81517ae30eaa7b0da447fc09c583d8c0_r.png)