using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;

namespace ModelTextureDetail
{
    public class ImageData
    {
        /// <summary>
        /// 根据 颜色 / 图片区域数据+图片所属源图片 计算的唯一标识
        /// </summary>
        private string uid;
        
        /// <summary>
        /// 所属源图片的路径
        /// </summary>
        public string SrcPath;

        /// <summary>
        /// 所属源图片的路径
        /// </summary>
        public readonly ImageData SrcImage;

        /// <summary>
        /// 作为 parent 的内容
        /// </summary>
        public ImageData parent = null;

        public Boolean isColor;

        private Color color;

        //public float r;
        //public float g;
        //public float b;
        //public float a;

        /// <summary>
        /// 在parent的x坐标
        /// </summary>
        public int x = 0;
        /// <summary>
        /// 在parent的y坐标
        /// </summary>
        public int y = 0;

        /// <summary>
        /// 在最终图片的x坐标
        /// </summary>
        public int new_x;
        /// <summary>
        /// 在最终图片的y坐标
        /// </summary>
        public int new_y;

        /// <summary>
        /// 图片贴图数据
        /// </summary>
        public Color[] data;
        /// <summary>
        /// 目标uv用到的图片贴图像素宽度
        /// </summary>
        public int dataWidth;
        /// <summary>
        /// 目标uv用到的图片贴图像素高度
        /// </summary>
        public int dataHeight;

        /// <summary>
        /// 在parent的x坐标
        /// </summary>
        public int SrcX;
        /// <summary>
        /// 在parent的y坐标
        /// </summary>
        public int SrcY;
        public const int colorDataSize = 15;

        public ImageData(ImageData imageData)
        {
            SrcImage = imageData;
            if (imageData != null)
            {
                SrcPath = imageData.SrcPath;
            }
        }

        public void changeParent(ImageData imageData)
        {
            parent = imageData;
        }

        public void computeNewInfo()
        {
            ImageData temp = this;
            ImageData tempParent = temp.parent;

            new_x = 0; // temp.SrcX - tempParent.SrcX;
            new_y = 0; // temp.SrcY - tempParent.SrcY;

            while (tempParent != null)
            {

                if (tempParent.SrcImage != null)
                {
                    new_x += temp.SrcX - tempParent.SrcX;
                    new_y += temp.SrcY - tempParent.SrcY;
                } else
                {
                    new_x += tempParent.SrcX;
                    new_y += tempParent.SrcY;
                }

                temp = tempParent;
                tempParent = temp.parent;
            }
        }

        //public void colorToImage()
        //{
        //    data = new Color[dataWidth * dataHeight];

        //    for (int i = 0; i < dataHeight; i++)
        //    {
        //        for (int j = 0; j < dataWidth; j++)
        //        {
        //            Color color = new Color(r, g, b, a);
        //            data[i * dataWidth + j] = color;
        //        }
        //    }
        //}

        public string getUid()
        {
            string uid = "";
            if (isColor)
            {
                getSrcColor();

                uid = "-" + color.r + "-" + color.g + "-" + color.b + "-" + color.a;
            }
            else
            {
                uid = "" + SrcPath + "-" + SrcX + "-" + SrcY;
            }
            return uid;
        }

        /// <summary>
        /// 从源文件获取颜色数据
        /// </summary>
        /// <param name="srcImageData"></param>
        public Color getSrcColor()
        {
            if (isColor)
            {
                int index = SrcY * SrcImage.dataWidth + SrcX;
                if (index >= SrcImage.data.Length)
                {
                    Debug.Log("");
                }
                color = SrcImage.data[index];
            }

            return color;
        }

        ///// <summary>
        ///// 从源文件拷贝目标数据
        ///// </summary>
        ///// <param name="srcImageData"></param>
        //public void getSrcData(ImageData srcImageData)
        //{
        //    data = new Color[dataWidth * dataHeight];

        //    for (int i = 0; i < dataHeight; i++)
        //    {
        //        for (int j = 0; j < dataWidth; j++)
        //        {
        //            Color color = srcImageData.data[(SrcY + i) * srcImageData.dataWidth + (SrcX + j)];
        //            data[i * dataWidth + j] = color;
        //        }
        //    }
        //}

        public int getTextureWidth()
        {
            int width = 0;
            if (isColor)
            {
                width = colorDataSize;
            } else
            {
                width = dataWidth;
            }

            if (width == 0)
            {
                Debug.LogWarning("width = 0");
            }

            return width;
        }
        public int getTextureHeight()
        {
            int height = 0;
            if (isColor)
            {
                height = colorDataSize;
            }
            else
            {
                height = dataHeight;
            }

            if (height == 0)
            {
                Debug.LogWarning("Height = 0");
            }

            return height;
        }

        public Color[] getTextureColorData()
        {
            int width   = getTextureWidth();
            int height  = getTextureHeight();

            Color[] colors = new Color[width * height];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (isColor)
                    {
                        getSrcColor();
                        colors[i * width + j] = new Color(color.r, color.g, color.b, color.a);
                    }
                    else
                    {
                        int index = (SrcY + i) * SrcImage.dataWidth + (SrcX + j);
                        if (index >= SrcImage.data.Length)
                        {
                            Debug.LogWarning("getTextureColorData Error");
                        }
                        Color color = SrcImage.data[index];
                        colors[i * width + j] = color;
                    }

                }
            }

            return colors;
        }

        /// <summary>
        /// 检测当前 贴图区域是否可以和 目标区域合并，如果可以则拓展当前区域
        /// </summary>
        /// <param name="imageData"></param>
        /// <returns></returns>
        public Boolean checkNeedCombine(ImageData imageData)
        {
            Boolean flag = false;

            int minRight    = Math.Min(SrcX + dataWidth, imageData.SrcX + imageData.dataWidth);
            int maxLeft     = Math.Max(SrcX, imageData.SrcX);
            int minBottom   = Math.Min(SrcY + dataHeight, imageData.SrcY + imageData.dataHeight);
            int maxTop      = Math.Max(SrcY, imageData.SrcY);

            int maxRight    = Math.Max(SrcX + dataWidth, imageData.SrcX + imageData.dataWidth);
            int minLeft     = Math.Min(SrcX, imageData.SrcX);
            int maxBottom   = Math.Max(SrcY + dataHeight, imageData.SrcY + imageData.dataHeight);
            int minTop      = Math.Min(SrcY, imageData.SrcY);

            if (minRight >= maxLeft && minBottom >= maxTop)
            {
                flag = true;
                SrcX = minLeft;
                SrcY = minTop;
                dataWidth = maxRight - minLeft;
                dataHeight = maxBottom - minTop;
                if (minLeft > 1024 || minTop > 1024 || minLeft + dataWidth > 1024 || minTop + dataHeight > 1024)
                {
                    Debug.LogWarning("checkNeedCombine Error");
                }
            }

            return flag;
        }

        public bool getPixelsFromTexture(ref Texture2D texture, out Color[] pixels)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            //Make texture readable
            TextureImporter im = AssetImporter.GetAtPath(path) as TextureImporter;
            if (!im)
            {
                pixels = new Color[1];
                return false;
            }
            bool readable = im.isReadable;
#if UNITY_5_4
		TextureImporterFormat format = im.textureFormat;
#else
            TextureImporterCompression format = im.textureCompression;
#endif
            TextureImporterType type = im.textureType;
            bool isConvertedBump = im.convertToNormalmap;

            if (!readable)
                im.isReadable = true;
#if UNITY_5_4
		if (type != TextureImporterType.Image)
			im.textureType = TextureImporterType.Image;
		im.textureFormat = TextureImporterFormat.ARGB32;
#else
            if (type != TextureImporterType.Default)
                im.textureType = TextureImporterType.Default;

            im.textureCompression = TextureImporterCompression.Uncompressed;
#endif
            im.SaveAndReimport();

            pixels = texture.GetPixels();

            if (!readable)
                im.isReadable = false;
#if UNITY_5_4
		if (type != TextureImporterType.Image)
			im.textureType = type;
#else
            if (type != TextureImporterType.Default)
                im.textureType = type;
#endif
            if (isConvertedBump)
                im.convertToNormalmap = true;

#if UNITY_5_4
		im.textureFormat = format;
#else
            im.textureCompression = format;
#endif

            im.SaveAndReimport();

            return true;
        }
    }
}
