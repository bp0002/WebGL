0. 容器节点销毁子节点时的泄漏
    4.0.0已修复
        # 20190101-0000
        * container.ts
            > .dispose()

1. 合并绘制/缓冲静态内容
    container -> useBitmapCache === true
            # 20190130-00
            - 节点添加 useBitmapCache 属性, 添加canvas, 添加 DT
            * control.ts
                > line 121 - 126 . CLASS
                > .dispose()
                > .createCacheCanvas()

        -> 显示：将该区域内容绘制到屏外canvas
            # 20190130-20   
            - 绘制区域重新计算
            * control.ts
                > ._processMeasuresCache()
                > ._measureCache()
            # 20190130-21
            * container.ts
                > ._draw()

        -> 事件：progressPick: 处理子节点起始 x,y 为0
            # 20190130-01
            - ADT 添加事件坐标记录, 节点响应事件时取ADT 保存的坐标，不再使用事件传递过程中使用的坐标
            * advancedDynamicTexture.ts : 
                > line 72 - 74 . CLASS
                > line 552 - 554 . _doPicking()
            * control.ts
                > ._processObservables()
            * container.ts
                > ._processPicking()

        -> 更新: 节点 isDirty === true 时, 调用 root.markAsDirty; 置 父节点 wasDirty, render 时重绘离屏canvas
            # 20190130-30
            - wasDirty, markAsDirty的变化
            * control.ts
                > _markAsDirty()

        -> 销毁: 节点dispose 时销毁
            # 20190130-11
            * control.ts
                > .dispose()

2. 缓冲帧, 多节点的同图片帧动画合并, 
    Image 节点目标帧 变化时
        -> 获取：目标帧canvas，不存在则创建
        -> 创建: 以图片url+帧序号 为检索标识，存放于 Image 静态属性Map中
        -> 绘制: 直接使用缓冲的帧绘制，避免裁剪
        -> 销毁: 暂不销毁
        # 20190130-111
        * image.ts
            > CLASS
            > ._draw()

3. 节点统计
    打印的数据结构
        # 20190130-1111
        * control.ts : 
            > IADTDebugData
            > CLASS 

    统计打印
        # 20190130-0000
        - 添加打印的开关
        * advancedDynamicTexture.ts : 
            > line 75 - 77 . CLASS
            > ._render()

    统计更新
        # 20190130-0001
        * advancedDynamicTexture.ts : 
            > ._render()

    节点数-节点绘制数
        # 20190130-0010
        * control.ts : 
            > .constructor()
            > ._draw()
            > .dispose()
        * line.ts
            > ._draw()
        * container.ts
            > ._draw()
        * inputText.ts
            > ._draw()

    图片节点数-图片绘制数
        # 20190130-0020
        * image.ts : 
            > .constructor()
            > ._draw()
            > .dispose()

    文本节点数-文本绘制数
        # 20190130-0030
        * textBlock.ts : 
            > .constructor()
            > ._draw()
            > .dispose()

4. 节点对象缓冲
    文本节点
        # 20190130-000010
        * textBlock.ts
            > .disposeCall()
            > .Create()
            > .Init()
    图片节点
        # 20190130-000020
        * image.ts
            > .disposeCall()
            > .Create()
            > .Init()

5. 节点 left top 变化时不触发 _wasDirty
    修改 _markAsDirty()
        # 20190214-0000
        * contol.ts
            > ._markAsDirty()
            > .left()
            > .top()

6. 修复容器节点 isEnable === false 不能屏蔽子节点事件捕获的 bug
    container 为使用 isEnable 属性
        # 20190218-0000
        > container.ts
            > ._processPicking()

7. 优化： 过滤不在容器显示范围内的子节点的绘制调用
    添加 
        # 20190222-0000
        > control.ts
            > .isLimited
            > .checkLimit()
            > ._processMeasures()

        > container.ts
            >   public useDisplayLimit: boolean = false;
                public displayLimitLeft: number = 0;
                public displayLimitTop: number = 0;
                public displayLimitWidth: number = 0;
                public displayLimitHeight: number = 0;
            >   ._draw()
        > image.ts
            >   ._draw()
        > textBlock.ts
            >   ._draw()

8. UI 绑定 mesh 的节点 更新
    计算目标位置时使用 mesh 所在scene
        # 20190301-0000
        > advancedDynamicTexture.ts
            > ._checkUpdate()

9. Image 节点创建时 屏蔽无效 url 的使用
    无效 url 仍然会导致 Image 节点的创建
        # 20190304-0000
        > image.ts
            > .constructor()
            > ._draw() 绘制前添加 domImage 检查

10. Image 节点九宫格属性
    添加九宫格功能
        # 20190304-0001
        > image.ts
            > .private _sliceWidths: string;
            > .private _source_sliceWidths: string;
            > .private _sliceLeft: number;
            > .private _sliceTop: number;
            > .private _sliceRight: number;
            > .private _sliceBottom: number;
            > .private _source_sliceLeft: number;
            > .private _source_sliceTop: number;
            > .private _source_sliceRight: number;
            > .private _source_sliceBottom: number;
            > .sliceWidths();
            > .sourceSliceWidths();
            > ._draw();
            > ._renderNinePatch();
            > .computeBorders();
            > .computeSlice();

11. ADT 扩展指定 2D canvas 渲染
    创建时传入 canvas
        # 20190313-0000
        > advancedDynamicTextuew.ts
            > .CreateOffscreenFullscreenUI()
            > .constructor()
    
11. ADT 扩展指定 2D canvas 渲染 模式
    > 作为 2D 纹理传入 3D 场景渲染
    > 作为独立 canvas 自行渲染，不使用为 2D 纹理，即不进行纹理更新操作
        # 20190313-0001
        > advancedDynamicTextuew.ts
            > ._checkUpdate()
            > RenderNormal
            > RenderOffscreen
            > piGUIRenderMode

13. ===============================
    GUI 3D 方案
        - control -> 使用 TransformNode / Mesh
        - 节点内容 -> 使用 Texture / ambintColor&diffuseColor / DynamicTexture&Text
    ===============================

14.  (重复使用空闲缓冲目标；优化canvas2D的API使用 .save 与 .restore ) ; 
    应该对 GPU 内存问题有缓建；
    另 项目中 .scene.ts 监听 resize 的地方 屏蔽( 减小对浏览器测试的影响 canvas 变化浏览器会重新产生对应GPU内存，调整大小时可看到GPU内存翻倍 )

15. 手机QQ input 
    # 20190610-0000
    > control.ts
        > .getRect