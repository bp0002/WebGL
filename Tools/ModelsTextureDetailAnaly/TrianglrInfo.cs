using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModelTextureDetail
{
    public class TrianglrInfo
    {
        /// <summary>
        /// 三个点的UV数据
        /// </summary>
        public UVInfo uvInfo_0;
        public UVInfo uvInfo_1;
        public UVInfo uvInfo_2;

        public Boolean isColor;

        public int textureFlag = 0;

        public List<ImageData> images = new List<ImageData>();
        public List<string> imageUids = new List<string>();

        /// <summary>
        /// 作为图片数据时 图片数据在源图片的范围
        /// </summary>
        public int textureSrcX;
        public int textureSrcY;
        public int textureSrcWidth;
        public int textureSrcHeight;

        public void analyDataType(ImageData SrcImageData, int srcWidth, int srcHeight)
        {
            textureFlag = checkIsColor();
        }

        public void createImages(ImageData SrcImageData, int srcWidth, int srcHeight)
        {
            if (textureFlag == 1)
            {
                isColor = true;

                float srcU = 0;
                float srcV = 0;

                srcU = uvInfo_0.u0;
                srcV = uvInfo_0.v0;

                if (!imageUids.Contains("" + srcU + "" + srcV))
                {
                    ImageData image = new ImageData(SrcImageData);
                    image.isColor = true;

                    image.SrcX = (int)Math.Floor((srcWidth - 1) * srcU);
                    image.SrcY = (int)Math.Floor((srcWidth - 1) * srcV);
                    image.dataWidth = 0;
                    image.dataHeight = 0;

                    images.Add(image);

                    imageUids.Add("" + srcU + "" + srcV);
                }

                srcU = uvInfo_1.u0;
                srcV = uvInfo_1.v0;

                if (!imageUids.Contains("" + srcU + "" + srcV))
                {
                    ImageData image = new ImageData(SrcImageData);
                    image.isColor = true;

                    image.SrcX = (int)Math.Floor((srcWidth - 1) * srcU);
                    image.SrcY = (int)Math.Floor((srcWidth - 1) * srcV);
                    image.dataWidth = 0;
                    image.dataHeight = 0;

                    images.Add(image);

                    imageUids.Add("" + srcU + "" + srcV);
                }

                srcU = uvInfo_2.u0;
                srcV = uvInfo_2.v0;

                if (!imageUids.Contains("" + srcU + "" + srcV))
                {
                    ImageData image = new ImageData(SrcImageData);
                    image.isColor = true;

                    image.SrcX = (int)Math.Floor((srcWidth - 1) * srcU);
                    image.SrcY = (int)Math.Floor((srcWidth - 1) * srcV);
                    image.dataWidth = 0;
                    image.dataHeight = 0;

                    images.Add(image);

                    imageUids.Add("" + srcU + "" + srcV);
                }
            }
            else
            {
                if (textureFlag == 0)
                {
                    isColor = false;

                    textureSrcX = (int)Math.Ceiling((srcWidth) * Math.Min(Math.Min(uvInfo_0.u0, uvInfo_1.u0), uvInfo_2.u0)) - 1;
                    textureSrcY = (int)Math.Ceiling((srcHeight) * Math.Min(Math.Min(uvInfo_0.v0, uvInfo_1.v0), uvInfo_2.v0)) - 1;
                    textureSrcWidth = (int)Math.Ceiling((srcWidth) * Math.Max(Math.Max(uvInfo_0.u0, uvInfo_1.u0), uvInfo_2.u0)) - textureSrcX ;
                    textureSrcHeight = (int)Math.Ceiling((srcHeight) * Math.Max(Math.Max(uvInfo_0.v0, uvInfo_1.v0), uvInfo_2.v0)) - textureSrcY ;

                    if (textureSrcHeight == 0)
                    {
                        Debug.LogWarning("textureSrcHeight = 0");
                    }
                    if (textureSrcWidth == 0)
                    {
                        Debug.LogWarning("textureSrcWidth = 0");
                    }

                    if (textureSrcWidth > 0 && textureSrcHeight > 0)
                    {
                        ImageData image = new ImageData(SrcImageData);
                        image.isColor = false;
                        image.SrcX = textureSrcX;
                        image.SrcY = textureSrcY;
                        //image.dataWidth = Math.Max(textureSrcWidth, 1);
                        //image.dataHeight = Math.Max(textureSrcHeight, 1);
                        image.dataWidth = textureSrcWidth;
                        image.dataHeight = textureSrcHeight;

                        images.Add(image);
                    }
                    else
                    {
                        Debug.LogWarning("忽略三角形数据");
                    }
                }
                else
                {
                    Debug.LogWarning("忽略三角形数据 - 三个点 UV 没有垂直三角形的关系");
                }
            }
        }

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

        public Boolean floatEquqal(float a, float b)
        {
            float dim = a - b;
            float s = (float) (1.0 / 2048.0);
            Boolean flag = Math.Abs(dim) < s;
            return flag;
        }

        public Boolean analyTextureAdjacent(TrianglrInfo info1)
        {
            Boolean flag = false;

            int counter = 0;

            if (info1.hasSameUVInfo(uvInfo_0))
            {
                counter++;
            }
            if (info1.hasSameUVInfo(uvInfo_1))
            {
                counter++;
            }
            if (info1.hasSameUVInfo(uvInfo_2))
            {
                counter++;
            }

            if (counter >= 2)
            {
                flag = true;
            }

            return flag;
        }

        //public Boolean checkTextureInclude(TrianglrInfo info1)
        //{
        //    Boolean flag = false;

        //    if (textureSrcX <= info1.textureSrcX
        //        && textureSrcY <= info1.textureSrcY
        //        && textureSrcWidth >= info1.textureSrcWidth
        //        && textureSrcHeight >= info1.textureSrcHeight
        //    )
        //    {
        //        flag = true;
        //    }

        //    return flag;
        //}

        public Boolean analyTextureAdjacent(List<TrianglrInfo> infos)
        {
            Boolean flag = false;

            int counter = infos.Count;

            if (counter > 0)
            {
                for (int i = 0; i < counter; i++)
                {
                    flag = analyTextureAdjacent(infos[i]);
                    if (flag)
                    {
                        break;
                    }
                }
            } else
            {
                flag = true;
            }

            return flag;
        }

        public Boolean hasSameUVInfo(UVInfo uvInfo)
        {
            Boolean flag = false;

            if (floatEquqal(uvInfo_0.u0, uvInfo.u0) && floatEquqal(uvInfo_0.v0, uvInfo.v0))
            {
                flag = true;
            }
            else if (floatEquqal(uvInfo_1.u0, uvInfo.u0) && floatEquqal(uvInfo_1.v0, uvInfo.v0))
            {
                flag = true;
            }
            else if (floatEquqal(uvInfo_2.u0, uvInfo.u0) && floatEquqal(uvInfo_2.v0, uvInfo.v0))
            {
                flag = true;
            }

            return flag;
        }

        public int hasSameUVInfoCount(UVInfo uvInfo)
        {
            int flag = 0;

            if (floatEquqal(uvInfo_0.u0, uvInfo.u0) && floatEquqal(uvInfo_0.v0, uvInfo.v0))
            {
                flag ++;
            }

            if (floatEquqal(uvInfo_1.u0, uvInfo.u0) && floatEquqal(uvInfo_1.v0, uvInfo.v0))
            {
                flag ++;
            }

            if (floatEquqal(uvInfo_2.u0, uvInfo.u0) && floatEquqal(uvInfo_2.v0, uvInfo.v0))
            {
                flag ++;
            }

            return flag;
        }

    }
}
