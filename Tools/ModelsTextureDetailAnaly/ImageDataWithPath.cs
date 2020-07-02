using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelTextureDetail
{
    public class ImageDataWithPath
    {
        public readonly string SrcPath;
        public List<ImageData> imageDatas = new List<ImageData>();

        public ImageDataWithPath(string path)
        {
            SrcPath = path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageData"></param>
        public void add(ImageData imageData)
        {
            imageDatas.Add(imageData);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="imageDatas"></param>
        /// <returns></returns>
        public List<ImageData> combineImageDatas()
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
