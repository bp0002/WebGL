using ModelTextureDetail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using System.Security.Cryptography;

namespace ModelTextureDetail
{
    public class AnalyTool
    {
        int nbSelectedObjects = 0;
        Transform[] sortTransforms;
        int sortTransformsCount;

        Texture2D resTex;

        List<Transform> analyTransforms;

        List<AnalyGameObject> analyGameObjectList;

        public List<Mesh> meshList ;
        public List<Vector2[]> newUVList ;
        public List<ModelAnaly> modelAnalies;

        public void analy()
        {
            resTex = null;

            meshList = new List<Mesh>();
            newUVList = new List<Vector2[]>();
            modelAnalies = new List<ModelAnaly>();
            analyGameObjectList = new List<AnalyGameObject>();

            List<Transform> transforms = initTransformList();

            analySelect(analyGameObjectList, modelAnalies);
        }

        public void reset()
        {
            foreach (var modelAnaly in modelAnalies)
            {
                modelAnaly.material.SetTexture("_MainTex", modelAnaly.texture);
            }
        }

        public void apply()
        {
            foreach (var modelAnaly in modelAnalies)
            {
                Material combineMaterial = new Material(Shader.Find("Pi/Simple"));
                combineMaterial.mainTexture = resTex;
                combineMaterial.name = "Combine";

                meshList.Add(modelAnaly.mesh);

                //modelAnaly.renderer.sharedMaterials[0] = combineMaterial;

                int triangleInfoCount = modelAnaly.activeTriangles.Count;
                for (int i = 0; i < triangleInfoCount; i++)
                {
                    TrianglrInfo info = modelAnaly.activeTriangles[i];
                    ImageData imageData;

                    if (info.isColor)
                    {
                        float srcU = 0;
                        float srcV = 0;
                        int index = 0;

                        srcU = info.uvInfo_0.u0;
                        srcV = info.uvInfo_0.v0;

                        index       = info.imageUids.IndexOf("" + srcU + "" + srcV);
                        imageData   = info.images[index];

                        imageData.computeNewInfo();

                        //modelAnaly.mesh.uv[info.uvInfo_0.uvIndex][0] = imageData.new_x / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_0.uvIndex][1] = imageData.new_y / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_0.uvIndex] = new Vector2((float)(imageData.new_x) / (float)resTex.width, (float)imageData.new_y / (float)resTex.height);

                        /////////////////////////////////////////////
                        srcU = info.uvInfo_1.u0;
                        srcV = info.uvInfo_1.v0;

                        index = info.imageUids.IndexOf("" + srcU + "" + srcV);
                        imageData = info.images[index];

                        imageData.computeNewInfo();

                        //modelAnaly.mesh.uv[info.uvInfo_1.uvIndex][0] = imageData.new_x / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_1.uvIndex][1] = imageData.new_y / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_1.uvIndex] = new Vector2((float)imageData.new_x / (float)resTex.width, (float)imageData.new_y / (float)resTex.height);

                        /////////////////////////////////////////////
                        srcU = info.uvInfo_2.u0;
                        srcV = info.uvInfo_2.v0;

                        index = info.imageUids.IndexOf("" + srcU + "" + srcV);
                        imageData = info.images[index];

                        imageData.computeNewInfo();

                        //modelAnaly.mesh.uv[info.uvInfo_2.uvIndex][0] = imageData.new_x / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_2.uvIndex][1] = imageData.new_y / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_2.uvIndex] = new Vector2((float)imageData.new_x / (float)resTex.width, (float)imageData.new_y / (float)resTex.height);

                    } else
                    {
                        float srcU = 0;
                        float srcV = 0;
                        int index = 0;

                        imageData = info.images[index];
                        imageData.computeNewInfo();

                        srcU = info.uvInfo_0.u0;
                        srcV = info.uvInfo_0.v0;

                        //modelAnaly.mesh.uv[info.uvInfo_0.uvIndex][0] = (imageData.new_x + info.uvInfo_0.u00 - imageData.SrcX) / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_0.uvIndex][1] = (imageData.new_y + info.uvInfo_0.v00 - imageData.SrcY) / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_0.uvIndex] = new Vector2((imageData.new_x + info.uvInfo_0.u00 - imageData.SrcX) / (float)resTex.width, (imageData.new_y + info.uvInfo_0.v00 - imageData.SrcY) / (float)resTex.height);

                        /////////////////////////////////////////////
                        srcU = info.uvInfo_1.u0;
                        srcV = info.uvInfo_1.v0;

                        //modelAnaly.mesh.uv[info.uvInfo_1.uvIndex][0] = (imageData.new_x + info.uvInfo_1.u00 - imageData.SrcX) / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_1.uvIndex][1] = (imageData.new_y + info.uvInfo_1.v00 - imageData.SrcY) / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_1.uvIndex] = new Vector2((imageData.new_x + info.uvInfo_1.u00 - imageData.SrcX) / (float)resTex.width, (imageData.new_y + info.uvInfo_1.v00 - imageData.SrcY) / (float)resTex.height);

                        /////////////////////////////////////////////
                        srcU = info.uvInfo_2.u0;
                        srcV = info.uvInfo_2.v0;

                        //modelAnaly.mesh.uv[info.uvInfo_2.uvIndex][0] = (imageData.new_x + info.uvInfo_1.u00 - imageData.SrcX) / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_2.uvIndex][1] = (imageData.new_y + info.uvInfo_1.v00 - imageData.SrcY) / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_2.uvIndex] = new Vector2((imageData.new_x + info.uvInfo_2.u00 - imageData.SrcX) / (float)resTex.width, (imageData.new_y + info.uvInfo_2.v00 - imageData.SrcY) / (float)resTex.height);
                    }
                }
            }
        }

        public void combine()
        {

            foreach (var modelAnaly in modelAnalies)
            {
                Material combineMaterial = new Material(Shader.Find("Pi/Simple"));
                combineMaterial.mainTexture = resTex;
                combineMaterial.name = "Combine";

                //modelAnaly.renderer.sharedMaterials[0] = combineMaterial;

                //modelAnaly.material.mainTexture = resTex;

                int triangleInfoCount = modelAnaly.activeTriangles.Count;
                for (int i = 0; i < triangleInfoCount; i++)
                {
                    TrianglrInfo info = modelAnaly.activeTriangles[i];
                    ImageData imageData;

                    if (info.isColor)
                    {
                        float srcU = 0;
                        float srcV = 0;
                        int index = 0;

                        srcU = info.uvInfo_0.u0;
                        srcV = info.uvInfo_0.v0;

                        index = info.imageUids.IndexOf("" + srcU + "" + srcV);
                        imageData = info.images[index];

                        imageData.computeNewInfo();

                        //modelAnaly.mesh.uv[info.uvInfo_0.uvIndex][0] = imageData.new_x / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_0.uvIndex][1] = imageData.new_y / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_0.uvIndex] = new Vector2(imageData.new_x / (float)resTex.width, imageData.new_y / (float)resTex.height);

                        /////////////////////////////////////////////
                        srcU = info.uvInfo_1.u0;
                        srcV = info.uvInfo_1.v0;

                        index = info.imageUids.IndexOf("" + srcU + "" + srcV);
                        imageData = info.images[index];

                        imageData.computeNewInfo();

                        //modelAnaly.mesh.uv[info.uvInfo_1.uvIndex][0] = imageData.new_x / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_1.uvIndex][1] = imageData.new_y / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_1.uvIndex] = new Vector2(imageData.new_x / (float)resTex.width, imageData.new_y / (float)resTex.height);

                        /////////////////////////////////////////////
                        srcU = info.uvInfo_2.u0;
                        srcV = info.uvInfo_2.v0;

                        index = info.imageUids.IndexOf("" + srcU + "" + srcV);
                        imageData = info.images[index];

                        imageData.computeNewInfo();

                        //modelAnaly.mesh.uv[info.uvInfo_2.uvIndex][0] = imageData.new_x / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_2.uvIndex][1] = imageData.new_y / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_2.uvIndex] = new Vector2(imageData.new_x / (float)resTex.width, imageData.new_y / (float)resTex.height);

                    }
                    else
                    {
                        float srcU = 0;
                        float srcV = 0;
                        int index = 0;

                        imageData = info.images[index];
                        imageData.computeNewInfo();

                        srcU = info.uvInfo_0.u0;
                        srcV = info.uvInfo_0.v0;

                        //modelAnaly.mesh.uv[info.uvInfo_0.uvIndex][0] = (imageData.new_x + info.uvInfo_0.u00 - imageData.SrcX) / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_0.uvIndex][1] = (imageData.new_y + info.uvInfo_0.v00 - imageData.SrcY) / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_0.uvIndex] = new Vector2((imageData.new_x + info.uvInfo_0.u00 - imageData.SrcX) / (float)resTex.width, (imageData.new_y + info.uvInfo_0.v00 - imageData.SrcY) / (float)resTex.height);

                        /////////////////////////////////////////////
                        srcU = info.uvInfo_1.u0;
                        srcV = info.uvInfo_1.v0;

                        //modelAnaly.mesh.uv[info.uvInfo_1.uvIndex][0] = (imageData.new_x + info.uvInfo_1.u00 - imageData.SrcX) / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_1.uvIndex][1] = (imageData.new_y + info.uvInfo_1.v00 - imageData.SrcY) / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_1.uvIndex] = new Vector2((imageData.new_x + info.uvInfo_1.u00 - imageData.SrcX) / (float)resTex.width, (imageData.new_y + info.uvInfo_1.v00 - imageData.SrcY) / (float)resTex.height);

                        /////////////////////////////////////////////
                        srcU = info.uvInfo_2.u0;
                        srcV = info.uvInfo_2.v0;

                        //modelAnaly.mesh.uv[info.uvInfo_2.uvIndex][0] = (imageData.new_x + info.uvInfo_1.u00 - imageData.SrcX) / (float)resTex.width;
                        //modelAnaly.mesh.uv[info.uvInfo_2.uvIndex][1] = (imageData.new_y + info.uvInfo_1.v00 - imageData.SrcY) / (float)resTex.height;
                        modelAnaly.newUVs[info.uvInfo_2.uvIndex] = new Vector2((imageData.new_x + info.uvInfo_2.u00 - imageData.SrcX) / (float)resTex.width, (imageData.new_y + info.uvInfo_2.v00 - imageData.SrcY) / (float)resTex.height);
                    }
                }
            }
            

        }

        public void createTree()
        {

        }

        public void updateGameObjects()
        {
            Material material = new Material(Shader.Find("Pi/Shadow"));
            material.mainTexture = resTex;

            AssetDatabase.CreateAsset(material, "Assets/CombineMat.mat");

            foreach (var ago in analyGameObjectList)
            {
                ago.newTexture = resTex;
                ago.createData(material);
            }
        }

        /// <summary>
        /// Transform排序，深度遍历
        /// </summary>
        /// <param name="trs"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private List<Transform> sortTrs(Transform[] trs, List<AnalyGameObject> saveAGOList)
        {
            AnalyGameObject rootAGO = new AnalyGameObject(null);

            List<Transform> saveList = new List<Transform>();

            for (var i = 0; i < trs.Length; i++)
            {
                saveList.Add(trs[i]);

                AnalyGameObject ago = new AnalyGameObject(trs[i]);
                ago.changeParent(rootAGO);
                saveAGOList.Add(ago);

                if (trs[i].childCount > 0)
                {
                    sortTrs(trs[i], saveList, ago, saveAGOList);
                }
            }

            return saveList;
        }

        /// <summary>
        /// Transform 深度遍历
        /// </summary>
        /// <param name="trs"></param>
	    private void sortTrs(Transform parentTR, List<Transform> saveList, AnalyGameObject parentAGO, List<AnalyGameObject> saveAGOList)
        {
            for (var i = 0; i < parentTR.childCount; i++)
            {
                Transform t = parentTR.GetChild(i);

                saveList.Add(t);

                AnalyGameObject ago = new AnalyGameObject(t);
                ago.changeParent(parentAGO);
                saveAGOList.Add(ago);

                if (t.childCount > 0)
                {
                    sortTrs(t, saveList, ago, saveAGOList);
                }
            }
        }

        /// <summary>
        /// 收集场景中的 Transform 并排序
        /// </summary>
        /// <returns></returns>
        private List<Transform> initTransformList()
        {

            /// 首先，收集场景中的对并且排序，添加到列表中
            Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel);

            /// 排序
            analyTransforms = sortTrs(transforms, analyGameObjectList);

            //analyTransforms = new List<Transform>(transforms);

            /// 选择的节点数量
		    nbSelectedObjects = analyTransforms.Count;

            return analyTransforms;
        }

        /// <summary>
        /// 尝试获取节点上 Mesh 
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
	    private Mesh GetMesh(Transform tr)
        {
            var mr = GetRenderer(tr);
            Mesh m = null;

            if (mr != null && mr.enabled)
            {
                var t = mr.GetType();
                if (t == typeof(MeshRenderer))
                {
                    MeshFilter mf = tr.GetComponent<MeshFilter>();
                    if (!mf)
                    {
                        Debug.Log("The gameObject " + tr.name + " will be exported as Transform (object has no MeshFilter component attached)");
                        return m;
                    }

                    m = mf.sharedMesh;
                }
                else if (t == typeof(SkinnedMeshRenderer))
                {
                    SkinnedMeshRenderer smr = mr as SkinnedMeshRenderer;
                    m = smr.sharedMesh;
                }
            }

            return m;
        }
        /// <summary>
        /// 获取 MeshRenderer / SkinnedMeshRenderer 面板数据
        /// </summary>
        /// <param name="tr">目标节点</param>
        /// <returns></returns>
	    private Renderer GetRenderer(Transform tr)
        {
            Renderer mr = tr.GetComponent<MeshRenderer>();
            if (mr == null)
            {
                mr = tr.GetComponent<SkinnedMeshRenderer>();
            }
            return mr;
        }

        private void analySelect(List<AnalyGameObject> selects, List<ModelAnaly> modelAnalys)
        {

            int nbDisabledObjects = 0;

            foreach (AnalyGameObject ago in selects)
            {
                Transform tr = ago.srcTransform;

                /// /如果有没勾选(非活动状态)的node不会导出，最后会提示有多少个
			    if (tr.gameObject.activeInHierarchy == false)
                {
                    nbDisabledObjects++;//如果有没勾选的node不会导出，最后会提示有多少个
                    continue;
                }

                Mesh mesh = GetMesh(tr);


                var mr = GetRenderer(tr);

                if (mr != null && mesh != null)
                {
                    var sharedMaterials = mr.sharedMaterials;

                    var subMeshCount = mesh.subMeshCount;
                    Material material;
                    /// subMesh 数量为 1 的情况
                    for (var i = 0; i < subMeshCount; ++i)
                    {

                        if (i < sharedMaterials.Length)
                        {
                            material = sharedMaterials[i];

                            ModelAnaly modelAnaly = analyModel(mesh, material, mr);

                            ago.setModelAnaly(modelAnaly);

                            modelAnalys.Add(modelAnaly);
                        }
                    }
                }
            }

            /// 
            combineModels(modelAnalys);

        }

        private ModelAnaly analyModel(Mesh mesh, Material mat, Renderer mr)
        {
            ModelAnaly modelAnaly = new ModelAnaly(mesh, mat, mr);
            return modelAnaly;
        }


        /// <summary>
        /// 此处合并多个模型的 贴图数据 - 默认多个模型间用到的 贴图纹理不会有关联, 仅考虑贴图可能相同的情况
        /// @ 贴图相同的情况 - 源文件相同 且 贴图区域 信息相同 (即贴图纹理信息 uid)
        /// @ 贴图相同的情况 - 颜色相同 (即贴图纹理信息 uid)
        /// </summary>
        private void combineModels(List<ModelAnaly> modelAnalys)
        {
            /// 
            List<ImageData> imageDatasAllModels = new List<ImageData>();
            List<string> imageDatasUidAllModels = new List<string>();

            List<ImageDataWithPath> imageDataWithPathList = new List<ImageDataWithPath>();
            List<string> tempPathList = new List<string>();

            foreach (var modelAnaly in modelAnalys)
            {
                foreach (var imageData in modelAnaly.modelAdjacentImageDatas)
                {
                    //string uid = imageData.getUid();
                    //if (!imageDatasUidAllModels.Contains(uid))
                    //{
                    //    ImageData newImageData  = new ImageData(imageData.SrcImage);
                    //    newImageData.SrcPath    = imageData.SrcPath;
                    //    newImageData.SrcX       = imageData.SrcX;
                    //    newImageData.SrcY       = imageData.SrcY;
                    //    newImageData.dataWidth  = imageData.dataWidth;
                    //    newImageData.dataHeight = imageData.dataHeight;
                    //    newImageData.isColor    = imageData.isColor;

                    //    imageDatasAllModels.Add(newImageData);
                    //    imageDatasUidAllModels.Add(uid);

                    //    imageData.changeParent(newImageData);
                    //} else
                    //{
                    //    int index = imageDatasUidAllModels.IndexOf(uid);
                    //    imageData.changeParent(imageDatasAllModels[index]);
                    //}

                    string path = imageData.SrcPath;
                    if (!tempPathList.Contains(path))
                    {
                        ImageDataWithPath imageDataWithPath = new ImageDataWithPath(path);

                        imageDataWithPathList.Add(imageDataWithPath);
                        tempPathList.Add(path);

                        imageDataWithPath.add(imageData);

                    } else
                    {
                        int index = tempPathList.IndexOf(path);
                        ImageDataWithPath imageDataWithPath = imageDataWithPathList[index];

                        imageDataWithPath.add(imageData);
                    }

                }

                foreach (var imageData in modelAnaly.colorImageDatas)
                {
                    string uid = imageData.getUid();
                    if (!imageDatasUidAllModels.Contains(uid))
                    {
                        ImageData newImageData  = new ImageData(imageData.SrcImage);
                        newImageData.SrcX       = imageData.SrcX;
                        newImageData.SrcY       = imageData.SrcY;
                        newImageData.dataWidth  = imageData.dataWidth;
                        newImageData.dataHeight = imageData.dataHeight;
                        newImageData.isColor    = imageData.isColor;

                        imageDatasAllModels.Add(newImageData);
                        imageDatasUidAllModels.Add(uid);

                        imageData.changeParent(newImageData);
                    }
                    else
                    {
                        int index = imageDatasUidAllModels.IndexOf(uid);
                        imageData.changeParent(imageDatasAllModels[index]);
                    }
                }
            }

            foreach (var temp in imageDataWithPathList)
            {
                List<ImageData> tempList = temp.combineImageDatas();

                foreach(var iData in tempList)
                {
                    imageDatasAllModels.Add(iData);
                }
            }

            analyCombintImageDataForModels(imageDatasAllModels, imageDatasUidAllModels);
        }

        /// <summary>
        /// 合并纹理为一个
        /// </summary>
        /// <param name="infos"></param>
        /// <returns></returns>
        private ImageData analyCombintImageDataForModels(List<ImageData> imageDatas, List<string> uidList)
        {
            ImageData res = new ImageData(null);
            res.SrcX        = 0;
            res.SrcX        = 0;
            res.dataWidth   = 2048;
            res.dataHeight  = 2048;
            res.isColor     = false;

            int count = imageDatas.Count;

            resTex = Texture2D.whiteTexture;
            Texture2D[] textures = imageDatasToTextures(imageDatas);

            Rect[] rects = resTex.PackTextures(textures, 2, 2048);

            /// 保存为文件
            byte[] bytes = resTex.EncodeToJPG();
            //string texName = byteToMd5(bytes);
            string texName = System.DateTime.Now.ToString().Replace("/", "-").Replace(":", "-").Replace(" ", "_");

            resTex.name = texName;
            resTex.alphaIsTransparency = false;

            var dirPath = Application.dataPath + "/CombineTextures/";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(dirPath + texName + ".jpg", bytes);

            //AssetDatabase.CreateAsset(resTex, "Assets/" + texName + ".png");

            //FileUtil.CopyFileOrDirectory(dirPath + texName + ".png", "Assets/" + texName + ".png");

            resTex.Apply();

            AssetDatabase.Refresh();

            res.dataWidth   = resTex.width;
            res.dataHeight  = resTex.height;

            for (int i = 0; i < count; i++)
            {
                Rect block = rects[i];

                int index = i;
                ImageData imageData = imageDatas[index];

                if (imageData != null)
                {
                    ImageData newImageData  = new ImageData(null);

                    if (imageData.isColor)
                    {
                        newImageData.SrcX       = (int)Math.Round(block.x * (res.dataWidth) - 1) + ImageData.colorDataSize / 2;
                        newImageData.SrcY       = (int)Math.Round(block.y * (res.dataHeight) - 1) + ImageData.colorDataSize / 2;
                        newImageData.dataWidth  = imageData.dataWidth;
                        newImageData.dataHeight = imageData.dataHeight;
                    }
                    else
                    {
                        newImageData.SrcX       = (int)Math.Round(block.x * (res.dataWidth - 1));
                        newImageData.SrcY       = (int)Math.Round(block.y * (res.dataHeight - 1));
                        newImageData.dataWidth  = imageData.dataWidth;
                        newImageData.dataHeight = imageData.dataHeight;
                    }

                    newImageData.changeParent(res);
                    imageData.changeParent(newImageData);
                }
            }

            return res;
        }

        /// <summary>
        /// 创建离散纹理列表
        /// </summary>
        /// <param name="colorImageDatas"></param>
        private Texture2D[] imageDatasToTextures(List<ImageData> imageDatas)
        {
            int count = imageDatas.Count;

            Texture2D[] textures = new Texture2D[count];

            for (int i = 0; i < count; i++)
            {
                ImageData imageData = imageDatas[i];
                Texture2D texture = new Texture2D(imageData.getTextureWidth(), imageData.getTextureHeight());

                texture.SetPixels(imageData.getTextureColorData());

                textures[i] = texture;
            }

            return textures;
        }

        public static string byteToMd5(byte[] data)
        {
            MD5 md5Hash = MD5.Create();
            byte[] md5bt = md5Hash.ComputeHash(data);
            
            return System.Convert.ToBase64String(md5bt);
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