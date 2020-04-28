# WebGL
* 操作顺序问题
+ 先声明使用哪个着色器程序, 再激活需要的属性
    - Android 9.0 多个渲染交叉时可能造成错误：WebGL: INVALID_OPERATION: drawElements: no buffer is bound to enabled attribute
    - 先禁用所有属性，再启用需要的属性
    - http://mjb.io/+/webgl-fixing-invalid_operation-drawarrays-attribs-not-setup-correctly/index.html
    - https://www.khronos.org/registry/webgl/specs/1.0/#6.5
```
    gl.useProgram(this.shader_program);
    gl.enableVertexAttribArray(this._a_position_loc);
    gl.enableVertexAttribArray(this._a_uv_loc);
```
+ 某些运行环境中需要确保 gl.getAttribLocation 获取的顶点属性标识为 0
