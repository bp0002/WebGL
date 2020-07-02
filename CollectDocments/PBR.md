# PBR
* 基于物理的渲染 
    - 更准确地模拟现实世界中光的流动方式来渲染图形
    - 从真实世界的物体的照片中进行测量-研究和复制真实的物理值范围，以准确模拟反照率，光泽度，反射率和其他物理特性
* 重要点: 
    + 双向反射率分布函数和渲染方程的可行和快速近似
    + Feasible and quick approximations of the bidirectional reflectance distribution function and rendering equation are of mathematical importance in this field

# Surface PBR - 表面
* 依赖 双向反射率分布函数(BRDF)的简化模型
* 领域
    + 反射 (Reflection)
    + 扩散 (Diffusion)
    + 半透明和透明 (Translucency and transparency)
    + 节约能源 (Coservation of energy)
    + 金属性 (Metallicity)
    + 菲涅尔反射 (fresnel reflection)
    + 微表面散射 (Microsurface scattering)

# Volumes - 体
* 领域
    + 镜头相关/视角/景深效果
    + 焦散
    + 光散射
    + 参与媒体
    + 大气视觉
        - 天-夜晚循环
        - 海拔
        - 与太阳或月亮或其他轨道物体的角距离
        - 天空和天空条件，包括云层，降水和雾气或雾霾等气溶胶遮盖物

# BABYLON

## PBRMetallicRoughnessMaterial - 金属度粗糙度材质
* baseColor / baseTexture
    + 如果是金属材质
        - 法向入射（F0）时特定的反射率测量值
    + 如果是非金属
        - 表示材料的反射漫反射色(内容)
* metallic
    + 材料的金属标量值。也可以用于缩放金属纹理的金属度值
* roughness
    + 指定材料的粗糙度标量值。也可以用于缩放金属纹理的粗糙度值。
* metallicRoughnessTexture - 各通道是否使用是可选的，而不是必须的
    + 每像素的控制
    + B - 金属值
    + G - 粗糙度值
    + R - 环境光遮挡
* enviromentTexture
    + 环境纹理 - 在材质表面产生内容

## PBRSpecularGlossinessMaterial - 镜面光泽度材质
* diffuseColor / diffuseTexture
    + 漫反射内容
* specularColor
    + 镜面颜色 - 表现为各通道反射内容强度(反射率)
* glossiness
    + 光泽度 - 表现为反射内容清晰程度
* specularGlossinessTexture
    + 每像素的控制
    + RGB - 镜面反射内容
    + A - 光泽度
* environmentTexture
    + 环境纹理 - 在材质表面产生内容
    
## 灯光 / 环境纹理
* 可以不使用灯光,仅使用环境纹理来照亮场景
    + 环境纹理可以使用 HDR 就绪文件
    + 环境纹理可以使用 普通立方体纹理, 但无法获得HDR渲染
* 光的衰减: 与距离平方成反比

## 