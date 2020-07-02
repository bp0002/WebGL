using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ModelTextureDetail
{
    /// <summary>
    /// 模型分析 - 三角形数据 - UV数据 - 贴图为 图片 还是 颜色
    /// </summary>
    public class ModelAnaly
    {
        public Mesh mesh;
        public Renderer renderer;
        public Material material;
        public Texture2D texture;
        public string assetPath;

        public Vector2[] newUVs;

        public List<int> useColorUVIndexList = new List<int>();
        public List<int> useRectUVIndexList = new List<int>();

        public List<int> RectUList = new List<int>();
        public List<int> RectVList = new List<int>();

        /// 再处理 - 三角形三个顶点使用的UV 可能会是 贴图UV的同时,只形成一个线段而不是三角形
        public List<TrianglrInfo> notTrueTriangles = new List<TrianglrInfo>();
        public List<TrianglrInfo> isTrueTriangles = new List<TrianglrInfo>();

        /// <summary>
        /// 使用有效最大区域的贴图纹理的三角形信息
        /// </summary>
        public List<TrianglrInfo> activeTriangles = new List<TrianglrInfo>();

        public List<TrianglrInfo> trianglesInfos = new List<TrianglrInfo>();
        /// <summary>
        /// 模型中的颜色贴图数据
        /// </summary>
        public List<ImageData> colorImageDatas = new List<ImageData>();
        public List<ImageData> textureImageDatas = new List<ImageData>();

        /// <summary>
        /// 模型中的图片贴图数据
        /// </summary>
        public List<ImageData> modelAdjacentImageDatas = new List<ImageData>();

        /// <summary>
        /// 源贴图
        /// </summary>
        public ImageData SrcImageData;

        public ModelAnaly(Mesh m, Material mat, Renderer mr)
        {
            this.mesh = m;
            this.material = mat;
            this.renderer = mr;

            int count = mesh.uv.Length;

            newUVs = new Vector2[count];

            this.InitSrcImageData();
            this.ananlyTriangles();
            //this.analyAdjacent();
        }

        public void InitSrcImageData()
        {
            texture = (Texture2D)material.mainTexture;
            assetPath = AssetDatabase.GetAssetPath(texture);

            SrcImageData = new ImageData(null);
            SrcImageData.SrcPath = assetPath;
            SrcImageData.dataWidth = texture.width;
            SrcImageData.dataHeight = texture.height;
            SrcImageData.data = new Color[texture.width * texture.height];
            SrcImageData.getPixelsFromTexture(ref texture, out SrcImageData.data);
        }

        /// <summary>
        /// 解析模型所有三角形数据 -
        /// </summary>
        public void ananlyTriangles()
        {
            int count = this.mesh.triangles.Length;

            int trianglesCount = count / 3;

            List<TrianglrInfo> tempTrueTriangles = new List<TrianglrInfo>();


            List<TrianglrInfo> waitSureColorOrRectInfos = new List<TrianglrInfo>();

            //////////////////////////////////////////////
            ///
            for (int i = 0; i < trianglesCount; i++)
            {
                TrianglrInfo triangleInfo = analyTriangle(this.mesh.triangles[i * 3 + 0], this.mesh.triangles[i * 3 + 1], this.mesh.triangles[i * 3 + 2]);

                if (triangleInfo.textureFlag == 0)
                {
                    if (
                        useRectUVIndexList.Contains(triangleInfo.uvInfo_0.uvIndex)
                        && useRectUVIndexList.Contains(triangleInfo.uvInfo_1.uvIndex)
                        && useRectUVIndexList.Contains(triangleInfo.uvInfo_2.uvIndex)
                        )
                    {
                        Debug.LogWarning("忽略三角形");
                    }
                    else
                    {

                        useRectUVIndexList.Add(triangleInfo.uvInfo_0.uvIndex);
                        useRectUVIndexList.Add(triangleInfo.uvInfo_1.uvIndex);
                        useRectUVIndexList.Add(triangleInfo.uvInfo_2.uvIndex);

                        activeTriangles.Add(triangleInfo);
                    }
                }
            }

            for (int i = 0; i < trianglesCount; i++)
            {
                TrianglrInfo triangleInfo = analyTriangle(this.mesh.triangles[i * 3 + 0], this.mesh.triangles[i * 3 + 1], this.mesh.triangles[i * 3 + 2]);

                if (triangleInfo.textureFlag == 1)
                {
                    if (
                        useColorUVIndexList.Contains(triangleInfo.uvInfo_0.uvIndex)
                        && useColorUVIndexList.Contains(triangleInfo.uvInfo_1.uvIndex)
                        && useColorUVIndexList.Contains(triangleInfo.uvInfo_2.uvIndex)
                    )
                    {
                        Debug.LogWarning("忽略三角形");
                    }
                    else if (
                        useRectUVIndexList.Contains(triangleInfo.uvInfo_0.uvIndex)
                        && useRectUVIndexList.Contains(triangleInfo.uvInfo_1.uvIndex)
                        && useRectUVIndexList.Contains(triangleInfo.uvInfo_2.uvIndex)
                    )
                    {
                        Debug.LogWarning("忽略三角形");
                    }
                    else
                    {
                        useColorUVIndexList.Add(triangleInfo.uvInfo_0.uvIndex);
                        useColorUVIndexList.Add(triangleInfo.uvInfo_1.uvIndex);
                        useColorUVIndexList.Add(triangleInfo.uvInfo_2.uvIndex);

                        activeTriangles.Add(triangleInfo);
                    }
                }
                else if (triangleInfo.textureFlag == 0)
                {
                    if (
                        useRectUVIndexList.Contains(triangleInfo.uvInfo_0.uvIndex)
                        && useRectUVIndexList.Contains(triangleInfo.uvInfo_1.uvIndex)
                        && useRectUVIndexList.Contains(triangleInfo.uvInfo_2.uvIndex)
                        )
                    {
                        Debug.LogWarning("忽略三角形");
                    }
                    else
                    {

                        useRectUVIndexList.Add(triangleInfo.uvInfo_0.uvIndex);
                        useRectUVIndexList.Add(triangleInfo.uvInfo_1.uvIndex);
                        useRectUVIndexList.Add(triangleInfo.uvInfo_2.uvIndex);

                        activeTriangles.Add(triangleInfo);
                    }
                }
                else
                {
                    waitSureColorOrRectInfos.Add(triangleInfo);
                }
            }

            foreach (var info in waitSureColorOrRectInfos)
            {
                if (
                    (
                    useColorUVIndexList.Contains(info.uvInfo_0.uvIndex)
                    && useColorUVIndexList.Contains(info.uvInfo_1.uvIndex)
                    && useColorUVIndexList.Contains(info.uvInfo_2.uvIndex)
                    )

                    || (
                        useRectUVIndexList.Contains(info.uvInfo_0.uvIndex)
                        && useRectUVIndexList.Contains(info.uvInfo_1.uvIndex)
                        && useRectUVIndexList.Contains(info.uvInfo_2.uvIndex)
                    )
                )
                {
                    Debug.LogWarning("忽略三角形 - 三点UV坐标已通过其他三角形获取");
                }
                else
                {
                    info.textureFlag = 0;
                    activeTriangles.Add(info);
                }
            }

            int minUVIndex = 0;
            int maxUVIndex = 0;

            foreach (var info in activeTriangles)
            {
                info.createImages(SrcImageData, SrcImageData.dataWidth, SrcImageData.dataHeight);

                minUVIndex = Math.Min(minUVIndex, info.uvInfo_0.uvIndex);
                minUVIndex = Math.Min(minUVIndex, info.uvInfo_1.uvIndex);
                minUVIndex = Math.Min(minUVIndex, info.uvInfo_2.uvIndex);

                maxUVIndex = Math.Max(maxUVIndex, info.uvInfo_0.uvIndex);
                maxUVIndex = Math.Max(maxUVIndex, info.uvInfo_1.uvIndex);
                maxUVIndex = Math.Max(maxUVIndex, info.uvInfo_2.uvIndex);

                if (!info.isColor)
                {
                    //if (triangleInfo.images[0].dataWidth == 0 || triangleInfo.images[0].dataHeight == 0)
                    //{
                    //    notTrueTriangles.Add(triangleInfo);
                    //}
                    //else
                    //{
                    //    isTrueTriangles.Add(triangleInfo);
                    //    tempTrueTriangles.Add(triangleInfo);
                    //}

                    foreach (var image in info.images)
                    {
                        if (!image.isColor)
                        {
                            textureImageDatas.Add(image);
                        }
                    }

                    RectUList.Add(info.uvInfo_0.u00);
                    RectUList.Add(info.uvInfo_1.u00);
                    RectUList.Add(info.uvInfo_2.u00);

                    RectVList.Add(info.uvInfo_0.v00);
                    RectVList.Add(info.uvInfo_1.v00);
                    RectVList.Add(info.uvInfo_2.v00);

                    //Debug.Log(info.uvInfo_0.u00 + " - " + info.uvInfo_0.v00);
                    //Debug.Log(info.uvInfo_1.u00 + " - " + info.uvInfo_1.v00);
                    //Debug.Log(info.uvInfo_2.u00 + " - " + info.uvInfo_2.v00);
                }
                else
                {

                    foreach (var image in info.images)
                    {
                        if (image.isColor)
                        {
                            colorImageDatas.Add(image);
                        }
                    }
                }
            }

            Debug.LogWarning("MinUVIndex - " + minUVIndex + " - MaxUVIndex - " + maxUVIndex + "- uv.Length - " + mesh.uv.Length);

            //Debug.Log(RectUList);
            //Debug.Log(RectVList);

            modelAdjacentImageDatas = combineImageDatas(textureImageDatas);

            //for (int i = 0; i < trianglesCount; i++)
            //{
            //    TrianglrInfo triangleInfo = analyTriangle(this.mesh.triangles[i * 3 + 0], this.mesh.triangles[i * 3 + 1], this.mesh.triangles[i * 3 + 2]);
            //    trianglesInfos[i] = triangleInfo;

            //    if (!triangleInfo.isColor)
            //    {
            //        if (triangleInfo.images[0].dataWidth == 0 || triangleInfo.images[0].dataHeight == 0)
            //        {
            //            notTrueTriangles.Add(triangleInfo);
            //        }
            //        else
            //        {
            //            isTrueTriangles.Add(triangleInfo);
            //            tempTrueTriangles.Add(triangleInfo);
            //        }
            //    }
            //    else
            //    {

            //        foreach (var image in triangleInfo.images)
            //        {
            //            if (image.isColor)
            //            {
            //                colorImageDatas.Add(image);
            //            }
            //        }
            //    }
            //}

            //int trueCount = isTrueTriangles.Count;
            //foreach (var tinfo in notTrueTriangles)
            //{
            //    for (int i = 0; i < trueCount; i++)
            //    {
            //        if (isTrueTriangles[i].analyTextureAdjacent(tinfo))
            //        {
            //            tinfo.images[0].changeParent(isTrueTriangles[i].images[0]);
            //            break;
            //        }
            //    }
            //}



            //for (int i = trueCount - 1; i >= 0; i--)
            //{

            //    for (int j = trueCount - 1; j >= 0; j--)
            //    {
            //        if (i != j)
            //        {
            //            TrianglrInfo iinfo = tempTrueTriangles[i];
            //            TrianglrInfo jinfo = tempTrueTriangles[j];

            //            if (iinfo != null && jinfo != null)
            //            {
            //                if (iinfo.checkTextureInclude(jinfo))
            //                {
            //                    jinfo.images[0].changeParent(iinfo.images[0]);
            //                    tempTrueTriangles[j] = null;
            //                }
            //            }
            //        }
            //    }

            //}

            //List<ImageData> tempImageDatas = new List<ImageData>();
            //for (int i = trueCount - 1; i >= 0; i--)
            //{
            //    TrianglrInfo iinfo = tempTrueTriangles[i];
            //    if (iinfo != null)
            //    {
            //        activeTriangles.Add(iinfo);
            //        tempImageDatas.Add(iinfo.images[0]);
            //    }
            //}
        }
        /// <summary>
        /// 解析指定三角形数据 - 获得UV数据,判断UV类型,转换像素信息
        /// </summary>
        /// <param name="v0"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public TrianglrInfo analyTriangle(int v0, int v1, int v2)
        {
            TrianglrInfo triangleInfo = new TrianglrInfo();

            triangleInfo.uvInfo_0 = new UVInfo();
            triangleInfo.uvInfo_0.u0 = this.mesh.uv[v0][0];
            triangleInfo.uvInfo_0.v0 = this.mesh.uv[v0][1];
            triangleInfo.uvInfo_0.u00 = (int)Math.Ceiling( this.mesh.uv[v0][0] * (SrcImageData.dataWidth)) - 1;
            triangleInfo.uvInfo_0.v00 = (int)Math.Ceiling( this.mesh.uv[v0][1] * (SrcImageData.dataHeight)) - 1;
            triangleInfo.uvInfo_0.uvIndex = v0;

            triangleInfo.uvInfo_1 = new UVInfo();
            triangleInfo.uvInfo_1.u0 = this.mesh.uv[v1][0];
            triangleInfo.uvInfo_1.v0 = this.mesh.uv[v1][1];
            triangleInfo.uvInfo_1.u00 = (int)Math.Ceiling(this.mesh.uv[v1][0] * (SrcImageData.dataWidth)) - 1;
            triangleInfo.uvInfo_1.v00 = (int)Math.Ceiling(this.mesh.uv[v1][1] * (SrcImageData.dataHeight)) - 1;
            triangleInfo.uvInfo_1.uvIndex = v1;

            triangleInfo.uvInfo_2 = new UVInfo();
            triangleInfo.uvInfo_2.u0 = this.mesh.uv[v2][0];
            triangleInfo.uvInfo_2.v0 = this.mesh.uv[v2][1];
            triangleInfo.uvInfo_2.u00 = (int)Math.Ceiling(this.mesh.uv[v2][0] * (SrcImageData.dataWidth)) - 1;
            triangleInfo.uvInfo_2.v00 = (int)Math.Ceiling(this.mesh.uv[v2][1] * (SrcImageData.dataHeight)) - 1;
            triangleInfo.uvInfo_2.uvIndex = v2;

            triangleInfo.analyDataType(SrcImageData, SrcImageData.dataWidth, SrcImageData.dataHeight);

            return triangleInfo;
        }

        /// <summary>
        /// 对解析出的三角形信息解析，获得相邻纹理信息,合并相邻纹理为一个
        /// @ 只检测 三角形纹理区域 3/2 点纹理坐标完全相同 的情况
        /// @ 没有检测 A 三角形纹理区域 包含 B 三角形纹理区域的情况
        /// </summary>
        public void analyAdjacent()
        {

            int count = trianglesInfos.Count;

            List<TrianglrInfo> tempTriangleInfos = new List<TrianglrInfo>();

            for (int i = 0; i < count; i++)
            {
                if (!trianglesInfos[i].isColor)
                {
                    tempTriangleInfos.Add(trianglesInfos[i]);
                }
            }

            int texTriangleCount = tempTriangleInfos.Count;

            for (int i = 0; i < texTriangleCount; i++)
            {
                TrianglrInfo temp = tempTriangleInfos[i];
                if (temp != null)
                {

                    List<TrianglrInfo> adjacentInfos = new List<TrianglrInfo>();

                    for (int j = 0; j < texTriangleCount; j++)
                    {
                        TrianglrInfo temp2 = tempTriangleInfos[j];
                        if (temp2 != null)
                        {
                            Boolean flag = temp2.analyTextureAdjacent(adjacentInfos);
                            if (flag)
                            {
                                adjacentInfos.Add(temp2);
                                tempTriangleInfos[j] = null;
                            }
                        }
                    }

                    ImageData adjacentImageData = analyCombintImageData(adjacentInfos);

                    foreach (var info in adjacentInfos)
                    {

                        foreach (var image in info.images)
                        {
                            image.changeParent(adjacentImageData);
                        }
                    }

                    modelAdjacentImageDatas.Add(adjacentImageData);
                }

            }
            
        }

        /// <summary>
        /// 合并相邻纹理为一个
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        public ImageData analyCombintImageData(List<TrianglrInfo> infos)
        {
            ImageData res = new ImageData(SrcImageData);

            int count = infos.Count;

            int x = infos[0].images[0].SrcX;
            int y = infos[0].images[0].SrcY;
            int x1 = infos[0].images[0].SrcX + infos[0].images[0].dataWidth;
            int y1 = infos[0].images[0].SrcY + infos[0].images[0].dataHeight;
            int w = infos[0].images[0].dataWidth;
            int h = infos[0].images[0].dataHeight;

            for (int i = 1; i < count; i++)
            {
                ImageData temp = infos[i].images[0];

                x = Math.Min(x, temp.SrcX);
                x1 = Math.Max(x1, temp.SrcX + temp.dataWidth);
                y = Math.Min(y, temp.SrcY);
                y1 = Math.Max(y1, temp.SrcY + temp.dataHeight);
            }

            w = x1 - x;
            h = y1 - y;

            res.SrcX = x;
            res.SrcY = y;
            res.dataWidth = w;
            res.dataHeight = h;
            res.SrcPath = infos[0].images[0].SrcPath;
            res.isColor = false;
            res.changeParent(SrcImageData);

            for (int i = 1; i < count; i++)
            {
                ImageData temp = infos[i].images[0];
                temp.changeParent(res);
            }

            return res;
        }

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
    }
}
