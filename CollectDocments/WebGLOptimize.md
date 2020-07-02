# **<font color=#2299ff  >WebGL 优化</font>**

### **<font color=#2299ff  > > EMSC </font>**

#### **<font color=#66aaff > >> 基础</font>**

* https://emscripten.org/docs/optimizing/Optimizing-WebGL.html
* 由于WebGL需要进行额外的验证以确保Web安全，因此与本地OpenGL应用程序相比，运行WebGL应用程序在CPU方面的开销更高。
* 现实情况有很多GL硬件和驱动程序供应商，以及操作系统组合，因此很难生成特定的优化指南。
* 很少的有冲突的情况，对一个驱动程序的某些优化可能会导致另一GPU供应商的硬件性能下降，大多数情况下，这是由于特定功能不受特定硬件支持，导致驱动程序求助于仿真。
* 某些检测到的性能问题是由于浏览器及其使用的软件库的低效率或完全性能错误所致，并且与底层GL驱动程序或Web安全通常无关。

#### **<font color=#66aaff > >> GL模式</font>**

* 默认情况下，如果未选择特殊的与GL相关的链接器标志，则Emscripten面向WebGL 1 API。
    + 为了获得最佳性能，请改用VBO。
    + 如果您的应用程序以旧的桌面OpenGL API为目标，改用WebGL 1 / OpenGL ES 2。
    + 以OpenGL ES 3为目标时，如果需要从客户端内存进行渲染，或者需要使用glMapBuffer*()API。而WebGL 2核心没有这些功能。预期此仿真会影响性能，因此建议改用VBO。
    + 即使您的应用程序不需要任何WebGL 2 / OpenGL ES 3功能，也请考虑将其移植到WebGL 2上运行，因为WebGL 2中的JavaScript端性能已经过优化，不会产生临时垃圾。它可以使速度提高3-7％，并减少渲染时的潜在卡顿现象。要启用这些优化，使用链接器标志-s MAX_WEBGL_VERSION = 2进行构建，并确保在GL启动时创建WebGL 2上下文（如果使用EGL，则创建OpenGL ES 3上下文）。

#### **<font color=#66aaff > >> drawCall</font>**

* 在WebGL中，每个GL函数调用都具有一定的开销，即使看似简单并且几乎什么也不做。因为WebGL实现需要验证每个调用，因为基础的本机OpenGL规范不提供有关可以在Web上依赖的安全性的保证。
* 在asm.js / WebAssembly端，每个WebGL调用都会生成一个FFI转换（在asm.js上下文中执行代码与浏览器的本机C++上下文中执行代码之间的跳转），其开销比asm.js / WebAssembly内的常规函数​​调用略高。因此，在Web上，通常最好使CPU端性能尽量减少对WebGL的调用次数。
    + 优化渲染器和高级输入资源，以避免重复调用。如果需要，可以重构设计，以便渲染器能够更好地说明哪些状态更改是相关的，哪些状态更改是不需要的。
    + 如果高级渲染器能够保持GL调用流保持精简，则将产生最快的结果。
    + 将GL状态缓存在渲染器代码中，如果状态未更改，请避免进行冗余调用以多次设置相同状态。
        - 例如，某些引擎可能在每次绘制调用之前盲目地重新配置深度测试或alpha混合模式，或者为每个调用重置着色器程序。
    + 避免使用所有类型的渲染器模式，这些模式会在某些操作后将GL重置为某些特定的“基态”。应该仅更改从一个绘制调用转换为另一个绘制调用时所需的GL状态。
        - 例
        ```
        - glBindBuffer(GL_ARRAY_BUFFER, 0)
        - glUseProgram(0)
        - for(i in 0 -> max_attributes) glDisableVertexAttribArray(i);
        ```
    + 惰性状态缓存机制，仅在需要生效时才考虑设置GL状态。
        - 例
        ```
        // First draw
        glBindBuffer(...);
        glVertexAttribPointer(...);
        glActiveTexture(0);
        glBindTexture(GL_TEXTURE_2D, texture1);
        glActiveTexture(1);
        glBindTexture(GL_TEXTURE_2D, texture2);
        glDrawArrays(...);

        // Second draw (back-to-back)
        glBindBuffer(...);
        glVertexAttribPointer(...);
        glActiveTexture(0); // (*)
        glBindTexture(GL_TEXTURE_2D, texture1); // (*)
        glActiveTexture(1); // (*)
        glBindTexture(GL_TEXTURE_2D, texture2); // (*)
        glDrawArrays(...);

        标有星号的所有四个API调用都是多余的，但是简单的状态缓存不足以检测到这一点。
        惰性状态缓存机制将能够检测到这些类型的更改。
        由于其他原因，在渲染之前将惰性缓存技术应用于所有GL状态也会变得昂贵，如果渲染器已经很好地避免了重新提交冗余调用，则可能会浪费性能。
        适当数量的缓存可能需要一些调整才能找到平衡。
        一个好的经验法则是，首先通过高层设计固有地避免冗余状态调用的渲染器通常比那些严重依赖于低级状态缓存的渲染器更有效。
        ```

#### **<font color=#66aaff > >> 避免GPU-CPU同步点</font>**

* 有效使用GPU的最重要方面是确保在渲染期间CPU永远不需要阻塞GPU，反之亦然。这些类型的停顿会产生非常昂贵的CPU-GPU同步点，从而导致这两种资源的利用率很差。
    + 避免在渲染时创建新的GL资源
        - 需要在渲染时优化以下接口, 如果需要新资源，请在尝试使用它们进行渲染的前几帧创建并上传。
        ```
        glGen*()
        glCreate*()
        glGenTextures(), glGenBuffers(), glCreateShader() and so on
        ```
    + 请勿删除任何刚刚渲染过的GL资源。如果驱动程序检测到任何资源正在使用，函数glDelete*()可以引入完整的管道刷新。最好仅在加载时删除资源。(或许应该是加载新资源时进行旧资源删除)
    + 切勿在渲染时调用glGetError()或glCheckFramebufferStatus()。这些功能应仅限于在加载时检查，因为这两个功能都会产生完整的管道同步。
    + 不要在渲染时调用任何glGet*()API函数，而是在启动和加载时查询它们，并在渲染时引用缓存的结果。
    + 尝试避免在渲染时编译着色器，glCompileShader()和glLinkProgram()都可能非常慢。
    + 不要在渲染时调用glReadPixels(),这会将纹理内容复制回主存储器。如有必要，请改用WebGL 2 GL_PIXEL_PACK_BUFFER绑定目标。
        - 当PBO绑定到GL_PIXEL_PACK_BUFFER时，像素数据在GPU内存中的PBO，而不会下载到客户端的内存。
        - 任何读取像素的OpenGL操作都会从PBO中获取它们的数据，如glReadPixels，glGetTexImage和glGetCompressedTexImage。通常的操作会从FBO或纹理中抽取数据，并将它们读取客户端内存中。
        - PBO的主要优势
        ```
        可以通过DMA (Direct Memory Access) 快速地在显卡上传递像素数据，而不需要耗费CPU的时钟周期。
        另一个优势是它还具备异步DMA传输
        ```

#### **<font color=#66aaff > >> GPU驱动程序友好的内存访问行为</font>**

* 在CPU和GPU之间转移内存是GL性能问题的常见根源。
* 这是因为创建新的GL资源可能会很慢，并且如果数据尚未准备好，或者在能够用新版本覆盖之前仍需要旧版本的数据，则上载或下载数据可能会阻塞CPU。
    + 与包含属性的多个VBO相比，单个VBO中的交织顶点数据优先。
        - 顶点，uv，三角形 分别一个FBO 与 相互交织在一个 FBO 相比
        - 这样可以改善GPU顶点缓存的行为，并在设置顶点属性指针以进行渲染时避免了多次重复的glBindBuffer（）调用
    + 避免在运行时调用glBufferData()或glTexImage2D/3D() 来调整缓冲区或纹理内容的大小。
        - 在增加或减小动态VBO大小时，请使用std::vector样式的几何数组增长语义，以避免必须调整每个帧的大小。
    + 即使更新纹理或缓冲区的整个内容，在更新缓冲区纹理数据时，还是优选调用glBufferSubData()和glTexSubImage2D/3D()。
        - 如果缓冲区的大小会缩小，请不要急于重新创建存储，而只需忽略多余的大小即可。
    + 对于动态顶点缓冲区数据，请考虑每帧两倍或三倍大小的缓冲VBO，以避免更新使用的VBO。
        - GL_DYNAMIC 比 GL_STREAM 更优先使用

#### **<font color=#66aaff > >> 当GPU成为瓶颈时</font>**

* 确认不会发生CPU-GPU管道同步气泡，并且渲染仍然受GPU约束时
    + 在前向光照渲染器中可以多次实现几何图形的多个附加光照绘制通道，但是这种生成的GL API调用的数量可能太昂贵。这种情况下,应考虑在一次着色器遍历中计算多个光贡献,即使在某些对象不受某些光照影响的情况下,将在着色器中创建无操作算术运算。
    + 表现效果满足需求的情况下，请使用最低的片段着色器精度。不要期望GPU GLSL驱动程序会即时进行任何优化，应当在离线创作时预先优化着色器。这对于移动GPU驱动程序尤其重要。
    + 对渲染对象进行排序，首先依据目标FBO，然后按着色器程序，再以最大程度地减少所需的GL状态更改或二次绘制的条件，取决于程序是受CPU还是BPU绑定。当不再需要FBO的内容时，调用WebGL 2 glDiscardFramebuffer()。
    + 使用GPU探查器，或实施自定义片段着色器，以帮助剖析渲染场景中有多少透支(overdraw)(过度绘制)。
        - 大量透支(overdraw)不仅会产生额外的工作，而且渲染到相同显示内存块之间的顺序依赖关系会减慢并行渲染的速度。
        - 如果渲染启用了深度缓冲的3D场景，请考虑从前到后对场景进行排序，以最大程度地减少透支和每个像素填充带宽的冗余。
        - 如果在3D场景中使用非常复杂的片段着色器，请考虑进行深度预通过以将实际光栅化的颜色片段的数量减少到绝对最小值。(depth prepass)

#### **<font color=#66aaff > >> 优化加载时间和其他最佳实践</font>**

* 通常无法预期将使用哪些压缩纹理格式。
    + 纹理制作为多种压缩格式,在运行时只下载适当的一种，以最大程度地减少过多的下载。
    + 将纹理和其他资产缓存,避免在后续运行中重新下载。
* 考虑在下载其他资产时并行编译着色器。
    + 可以帮助隐藏着色器编译时间。
* 在下载大量资源之前，请在页面加载过程的测试用户浏览器对WebGL的支持。
    + 避免下载WebGL不支持的资源
* 如果WebGL初始化失败，请使用“ webglcontextcreationerror”回调检查WebGL上下文错误原因。
    + 浏览器可以在上下文创建错误处理程序中提供良好的诊断报告，以诊断根本原因。
* 请密切注意画布的可见大小（DOM元素的CSS像素大小）与画布上已初始化的WebGL上下文的物理渲染目标大小。
    + 确保这两个匹配，以便呈现1：1像素的内容。
* 使用failIfMajorPerformanceCaveat标志探测上下文创建，以检测何时在软件上进行渲染，并在这种情况下降低图形保真度。
* 确保所需的最少功能来初始化WebGL上下文。
    + https://www.khronos.org/registry/webgl/specs/1.0/#WEBGLCONTEXTATTRIBUTES
    + alpha
        - 不需要支持Alpha在HTML页面背景下混合画布的功能，应将其禁用
        - 如果该值为true，则绘图缓冲区具有alpha通道，用于执行OpenGL目标alpha操作并与页面合成。如果值为false，则没有alpha缓冲区可用。(输出的内容是否具有alpha通道信息)
    + depth
        - 如果该值为true，则绘图缓冲区的深度缓冲区至少为16位。如果该值为false，则没有深度缓冲区可用。
    + stencil
        - 如果该值为true，则绘图缓冲区具有至少8位的模板缓冲区。如果值为false，则没有模板缓冲区可用。
    + antialias
        - 如果该值为true并且实现支持抗锯齿，则绘图缓冲区将使用其技术选择（多样本/超样本）和质量执行抗锯齿。
    + premultipliedAlpha
        - 如果该值为true，则页面合成器将假定绘图缓冲区包含带有预乘alpha的颜色。
        - 如果alpha标志为false，则忽略此标志。
        （输出的内容是否预乘alpha）
    + preserveDrawingBuffer
        - 如果为false，则按照Drawing Buffer部分中的描述显示绘图缓冲区后，绘图缓冲区的内容将清除为其默认值。绘图缓冲区的所有元素（颜色，深度和模板）均被清除。如果值为true，则缓冲区不会被清除，并会保留其值，直到作者清除或覆盖。
        - 在某些硬件上，将prepareDrawingBuffer标志设置为true可能会对性能产生重大影响。
    + preferLowPowerToHighPerformance
        - 向实现提供提示，表明:如果可能，它会创建一个上下文，该上下文针对性能上的功耗进行了优化。例如，在具有多个GPU的硬件上，会使用其中一个功能不那么强大，但功耗也更低的GPU。实现时 可能选择,可能必须，也可能忽略此提示。(没多大用)
    + failIfMajorPerformanceCaveat
        - 如果该值为true，则如果实现确定创建的WebGL上下文的性能大大低于进行等效OpenGL调用的本机应用程序的性能，则上下文创建将失败。发生这种情况可能有多种原因，其中包括：
            - 如果已知用户的GPU驱动程序不稳定，则实现可能会切换到软件光栅化程序
            - 一个实现可能需要将帧缓冲区从GPU内存读回系统内存，然后再将其与页面的其余部分进行组合，从而显着降低性能。
        - 不需要高性能的应用程序应将此参数保留为默认值false。
        - 需要高性能的应用程序可以将此参数设置为true，并且如果上下文创建失败，则应用程序可能更喜欢使用后备渲染路径，例如2D canvas上下文。
        - 或者，应用程序可以在将参数设置为false的情况下重试WebGL上下文创建，但要使用降低保真度的渲染模式来提高性能。
* 避免使用任何 *glGetProcAddress（）API函数。 Emscripten提供了到所有GL API函数的静态链接，甚至对于所有WebGL扩展也是如此。
    + 仅提供*glGetProcAddress（）API是为了兼容性，以简化现有代码的移植
    + 通过调用动态获得的函数指针访问WebGL明显比直接调用函数要慢，
    + 由于额外的功能指针安全性验证，因此必须在asm.js / WebAssembly中进行动态调度
    + 由于Emscripten提供了所有静态链接的GL入口点，因此建议利用此优势以获得最佳性能。
* 始终使用requestAnimationFrame（）
    + 更稳定的帧率（与setInterval / setTimeout相比）
    + 隐藏时浏览器可以停止呈现
    + 如果显示许多标签页，浏览器会自动限制呈现

#### **<font color=#66aaff > >> 迁移到WebGL 2</font>**

* 与WebGL 1相比，新的WebGL 2 API提供了无成本的API优化，只需针对WebGL 2即可激活它们。
    + 加快速度的原因是，从JavaScript绑定的角度对WebGL 2 API进行了修改，现在可以使用WebGL，而不必分配会导致JS垃圾收集器压力的临时对象。
    + 这些新的入口点可以更好地与asm.js和WebAssembly应用程序配合使用，并使WebGL API的使用更加精简。
    + 实验
        - 将虚幻引擎4项目构建为针对WebGL 2的目标，而未进行其他引擎修改，则性能提高了7％。
    + 支持
        - Firefox 51 and Chrome 58
    + WebGL 1是基于OpenGL ES 2.0规范的，而WebGL 2是基于OpenGL ES 3.0规范的
    + 要从WebGL 1迁移到WebGL 2，请注意以下已知的向后不兼容列表
        - 在WebGL 2中，许多WebGL 1.0扩展已合并到核心WebGL 2 API中，并且当查询不同WebGL扩展列表时，这些扩展不再公告为存在。检测时同时检查扩展名和核心上下文版本号
            ```
            例如，ANGLE_instanced_arrays扩展提供了WebGL 1中实例渲染的存在，但这是WebGL 2的核心功能，因此不再在GL扩展列表中报告
            ```
        - 功能合并到核心时，用于调用该功能的特定功能名称已更改
            ```
            例如 glDrawBuffersEXT => glDrawBuffers
            ```
        - WebGL 2规范采用的WebGL 1扩展的完整列表为: （初始化WebGL2 / GLES 3.0上下文时，可以直接使用这些扩展而无需检查扩展是否存在）
            ```
            ANGLE_instanced_arrays
            EXT_blend_minmax
            EXT_color_buffer_half_float
            EXT_frag_depth
            EXT_sRGB
            EXT_shader_texture_lod
            OES_element_index_uint
            OES_standard_derivatives
            OES_texture_float
            OES_texture_half_float
            OES_texture_half_float_linear
            OES_vertex_array_object
            WEBGL_color_buffer_float
            WEBGL_depth_texture
            WEBGL_draw_buffers
            ```
    * WebGL 2引入了新的GLSL着色器语言格式
        - WebGL 1 着色器代码中使用 #version 100 的编译指示
        - WebGL 2 着色器代码中使用 #version 300 es  的编译指示
    * WebGL 2 / GLES 3.0中，可以继续使用WebGL 1 / GLES 2 #version 100着色器
        - WebGL 2具有向后突破的不兼容性，使用上述扩展名的#version 100着色器必须改写为#version 300 es格式。
    * 在WebGL 2 / GLES 3.0中，为扩展引入的纹理格式更改了许多纹理格式枚举。
        - 不再可以使用WebGL 1 / GLES 2扩展中所谓的未缩放纹理格式，而是必须将这些新尺寸的格式变体用于internalFormat字段。
        - 例如
            ```
            不是创建 
                format=GL_DEPTH_COMPONENT，
                type=GL_UNSIGNED_INT，
                internalFormat = GL_DEPTH_COMPONENT
            的纹理，而是需要在internalFormat字段中指定大小 
                format=GL_DEPTH_COMPONENT，
                type=GL_UNSIGNED_INT，
                internalFormat = GL_DEPTH_COMPONENT24
            ```
    *  WebGL 2 / GLES 3.0纹理格式的特殊陷阱
        - OES_texture_half_float 在核心WebGL 2 / GLES 3.0 中，半浮点（float16）纹理类型的枚举值更改了值
            ```
            WebGL1 / GLES 2中， GL_HALF_FLOAT_OES = 0x8d61
            WebGL2 / GLES 3.0中，GL_HALF_FLOAT = 0x140b
            ```
        - 其他纹理类型扩展通常包含在核心规范中，保留了所用枚举的值。
                
### Google Doc 

#### https://docs.google.com/presentation/d/12AGAUmElB0oOBgbEEBfhABkIMCL3CUX7kdAPLuwZ964/htmlpresent

#### > requestAnimationFrame

* 始终使用requestAnimationFrame（）
    + 更稳定的帧率（与setInterval / setTimeout相比）
    + 隐藏时浏览器可以停止呈现
    + 如果显示许多标签页，浏览器会自动限制呈现

#### get*/read*

* 避免 get*/read* 调用
    + 导致完全刷新/阻塞GPU
    + 通常会产生昂贵的副本/分配
    + 建议: 用Javascript自己缓存状态
    + 建议: 精细控制，只读必要内容

#### getError

* getError 的使用
    + 永远不要在分布产品中调用
    + 在任何地方都很费
    + 像Chrome这样的多进程渲染器可能会遭受很大的损失
    + 不要在开发之外使用webgl-debug.js

#### DrawCall

* 避免多余的电话
    + 最好的情况：通过JS处理(过滤)
    + 最坏的情况：将导致GPU阻塞（状态改变等
    + 使用WebGL Inspector查找多余的呼叫并确定可以在何处进行批处理

#### GL功能

* 禁用未使用的GL功能
    + 混合，alpha测试等
    + 不要过度改变状态

#### Link program

* 尽量减少 link programe
    + 着色器验证/翻译可能需要很长时间 在Windows上使用ANGLE更糟
    + 尽早/在加载时 创建/链接 programe
    + 平衡程序复杂度与 programe 数量

#### Buffer

* 不要更改渲染缓冲区，请更改帧缓冲区
    + 附加渲染缓冲区需要大量验证(请注意，这违反了iOS性能准则)

#### Depth Buffers and Draw Order

* 深度缓冲区按每像素深度自动对几何进行排序
    + 通常不需要在CPU上进行深度排序！
* 相对更优（不是免费的，但通常不是瓶颈）
* WebGL深度缓冲区通常为16位
    + 当心精度问题！
    + 几种在顶点着色器中修改z以防止z冲突/抖动的技巧 https://en.wikipedia.org/wiki/Z-fighting
    + 如果进行渲染到纹理/自定义帧缓冲区，请记住要附加新的深度缓冲区
    + 请记住将gl.DEPTH_BUFFER_BIT传递给gl.clear 清除速度很快，请忽略有关反转z的技巧
    + 深度传递(深度预处理)（无片段着色器）以实现更好的批处理

#### Sorting by State

* 绘制对象的排序
    + 目标帧缓冲区或上下文状态 混合，裁剪，深度等
    + 程序/缓冲区/纹理 （通常）需要刷新管道才能进行切换
    + Uniforms/Samplers 
* 提前对场景进行排序，如果可能，请保留为排序列表
    + 每一帧的对象层次结构遍历/排序可以获得批处理的收益
    + 可以轻松地对生成内容（模型/等）进行批处理（合并缓冲区/纹理/等）

#### Batching Textures 批处理纹理

* 标准纹理图集/ UV贴图
    + 可以减少服务器请求/加载时间
    + 具有更好的压缩
    + 许多绘制调用可以共享相同的纹理状态
* 注意事项
    + Mipmap！（请注意，这意味纹理大小必须为2的幂）
    + 在图集的条目之间添加边框像素做间隔（如果使用 filtering ）-否则会渗色
    + 保持大小合理（不允许8096x8096纹理！）
        - 如果是256x256，则可以在顶点等中使用BYTE tex坐标

#### Depth Pass

#### Draw Order Guarantees

#### Vertex Buffer Structure

#### Vertex Buffer Structure

#### Reusing Vertices with Index Buffers

#### Dynamic Buffers

#### Packing

#### Optimize Texture Sampling

#### Dependent Reads/Instructions

#### Cheating Fillrate Limitations

#### General Data Flow Rules

#### Throttling

#### Data Dependencies

#### Readback