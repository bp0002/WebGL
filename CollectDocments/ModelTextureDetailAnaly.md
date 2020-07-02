# Unity 模型纹理细节分析工具

## 需求

* 当前模型只使用了纹理中的一小块区域
* 创建模型副本
* 创建只包含模型使用部分的纹理
* 模型副本使用的纹理没有多余内容
* 对多个模型进行处理，模型副本使用共同的纹理，该纹理为这多个模型实际使用的纹理内容的最小集合

## 限制

* 目标模型不能有UV动画 - 只能分析模型的静态纹理数据
* 某些情况模型三角面上的三个UV信息出现错误，只能用模型软件手动纠正

## 要点

### 关键数据
* 三角形
* 纹理信息数据
* 同模型内合并 纹理信息数据
* 多模型间合并 纹理信息数据

### 数据基础
* 由模型三角形面数据入手,需要顶点数据，uv数据
* 一个三角形对应的 UV 数据可生成一个 纹理信息数据 
* UV 浮点数据 转换为 纹理矩形整数数据
```

    textureSrcX = (int)Math.Ceiling((srcWidth) * Math.Min(Math.Min(uvInfo_0.u0, uvInfo_1.u0), uvInfo_2.u0)) - 1;
    textureSrcY = (int)Math.Ceiling((srcHeight) * Math.Min(Math.Min(uvInfo_0.v0, uvInfo_1.v0), uvInfo_2.v0)) - 1;
    textureSrcX = textureSrcX < 0 ? 0 : textureSrcX;
    textureSrcY = textureSrcY < 0 ? 0 : textureSrcY;
    textureSrcWidth = (int)Math.Ceiling((srcWidth) * Math.Max(Math.Max(uvInfo_0.u0, uvInfo_1.u0), uvInfo_2.u0)) - textureSrcX ;
    textureSrcHeight = (int)Math.Ceiling((srcHeight) * Math.Max(Math.Max(uvInfo_0.v0, uvInfo_1.v0), uvInfo_2.v0)) - textureSrcY ;

```

### 三角形对应的 UV 数据可能为 颜色数据 或者 图片内容数据

* 三角形UV类型检测 - 0: 矩形图片内容, 1: 纯颜色, 2: 待定
```
        private int checkIsColor()
        {
            int res = 0;

            int count = 0;

            if (floatEquqal(uvInfo_0.u0, uvInfo_1.u0) && floatEquqal(uvInfo_0.v0, uvInfo_1.v0))
            {
                count++;
            }

            if (floatEquqal(uvInfo_2.u0, uvInfo_1.u0) && floatEquqal(uvInfo_2.v0, uvInfo_1.v0))
            {
                count++;
            }

            if (floatEquqal(uvInfo_2.u0, uvInfo_0.u0) && floatEquqal(uvInfo_2.v0, uvInfo_0.v0))
            {
                count++;
            }

            if (count > 0)
            {
                res = 1;
            } else if (
                       !floatEquqal(uvInfo_0.u0, uvInfo_1.u0)
                    && !floatEquqal(uvInfo_0.u0, uvInfo_2.u0)
                    && !floatEquqal(uvInfo_1.u0, uvInfo_2.u0)
                    && !floatEquqal(uvInfo_0.v0, uvInfo_1.v0)
                    && !floatEquqal(uvInfo_0.v0, uvInfo_2.v0)
                    && !floatEquqal(uvInfo_1.v0, uvInfo_2.v0)
                )
            {
                res = 2;
            } else
            {
                res = 0;
            }

            return res;
        }
```

* 需要遍历一次模型所有三角形UV类型,首先收集 确认为 矩形纹理信息数据  和 纯颜色纹理信息数据  两种类型的三角形,再遍历剩余三角形
```
    如果 三角形对应的 三个UV 已经都被 纯颜色纹理信息数据  三角形确认 或者 都被 矩形纹理信息数据 确认
        true => 忽略该三角形
        false => 确认为 矩形图片内容
```

### 矩形纹理信息数据 合并

* 多个 矩形纹理信息数据 可能可以合并为 较少但较大区域的 多个 矩形纹理信息数据 
```
        public List<ImageData> combineImageDatas(List<ImageData> imageDatas)
        {
            List<ImageData> list = new List<ImageData>();

            int count = imageDatas.Count;

            for (int i = 0; i < count; i++)
            {
                ImageData iData = imageDatas[i];


                if (iData != null)
                {
                    Boolean preFlag = false;
                    int newCount = list.Count;
                    for (int n = 0; n < newCount; n++)
                    {
                        ImageData nData = list[n];

                        preFlag = nData.checkNeedCombine(iData);

                        if (preFlag)
                        {
                            iData.changeParent(nData);
                            imageDatas[i] = null;
                            break;
                        }
                    }

                    if (!preFlag)
                    {
                        ImageData newData = new ImageData(iData.SrcImage);
                        newData.SrcX = iData.SrcX;
                        newData.SrcY = iData.SrcY;
                        newData.dataWidth = iData.dataWidth;
                        newData.dataHeight = iData.dataHeight;
                        newData.isColor = iData.isColor;

                        list.Add(newData);

                        iData.changeParent(newData);
                        imageDatas[i] = null;

                        for (int j = 0; j < count; j++)
                        {
                            ImageData jData = imageDatas[j];
                            if (i != j && jData != null)
                            {
                                Boolean flag = newData.checkNeedCombine(jData);
                                if (flag)
                                {
                                    jData.changeParent(newData);
                                    imageDatas[j] = null;
                                }
                            }
                        }
                    }
                }

            }

            return list;
        }
```

### 生成新纹理

* 多个模型合并出的多个较大区域 矩形纹理信息数据 和 不重复的 纯颜色纹理信息数据 分别创建对应大小的纹理
* 创建大的空纹理，将这些小纹理合并到空纹理,会获得最终纹理及各小纹理在最终纹理的对应矩形信息
```
    Rect[] rects = resTex.PackTextures(textures, 2, 2048);
```

### 创建模型副本

* 依据源节点树结构，创建节点树副本
* 创建共同材质,应用新纹理
* 创建对应模型,拷贝顶点数据，拷贝三角形数据, 依据新矩形数据和源矩形数据计算和生成 UV 数据
* 应用共同材质