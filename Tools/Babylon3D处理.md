# ALPHA 混合
* Babylon 内部 ALPHA_PREMULTIPLIED 混合模式为
 - 
 - 设置了 Shader 中 PREMULTIPLYALPHA 宏，在最后 将 颜色RGB 乘上了 alpha
* 项目 - GUI 
 - 仅需要ALPHA混合过程处理
 - 因此拓展 PI_ALPHA_PREMULTIPLIED, 设置同 ALPHA_PREMULTIPLIED 但不设置宏