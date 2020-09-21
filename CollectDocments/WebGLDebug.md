# WebGL Debug

## https://docs.google.com/presentation/d/12AGAUmElB0oOBgbEEBfhABkIMCL3CUX7kdAPLuwZ964/htmlpresent

### > 没有渲染

* 常见情况：没有输出，也没有错误报告给控制台
    + OpenGL错误阻止绘图
    + “相机”指向错误的方向/模型在视野外
    + 上载数据时忘记绑定纹理或缓冲区
    + 忘记使用正确的着色器程序
    + 忘记将顶点属性启用为数组
    + 忘记使用正确的纹理
    + 忘了OpenGL ES规则: 纹理大小的非2的幂
    + JavaScript中的错误导致“undefined”在出错的地方传递到WebGL

### > 调试

* 当您面对黑屏时
    + 检查WebGL错误
    + 从良好的基础开始
    + 反复添加代码以实现目标
* 调试Shader
    + 删除功能，当心顶点属性不被使用
    + 在您要识别的区域中输出恒定的颜色
* 使用库和工具对问题进行分类
    + webgl-debug.js
        - http://www.khronos.org/webgl/wiki/Debugging
        - 帮助模拟丢失的上下文事件，使您的应用更强大
    + WebGL Inspector

### > 上下文的丢失

* 需要准备WebGL应用程序以处理上下文的丢失
    + 移动设备上的电源事件
    + 其他内容会强制重置GPU
    + 浏览器在“背景”选项卡上放置上下文
    + 浏览器由于资源不足而丢弃上下文
    + webgl-debug.js
        - 帮助模拟丢失的上下文事件，使您的应用更强大
