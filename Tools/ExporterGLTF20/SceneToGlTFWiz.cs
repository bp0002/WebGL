/***************************************************************************
GlamExport
 - Unity3D Scriptable Wizard to export Hierarchy or Project objects as glTF


****************************************************************************/
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Reflection;
using Ionic.Zip;
using System.Security.Cryptography;
using Assets.ExporterGLTF20;

public enum IMAGETYPE
{
	GRAYSCALE,
	RGB,
	RGBA,
	RGBA_OPAQUE,
	R,
	G,
	B,
	A,
	G_INVERT,
	NORMAL_MAP,
	IGNORE
}

public class SceneToGlTFWiz : MonoBehaviour
{
	public int jpgQuality           = 75;

    private GlTF_Writer writer;

    int nbSelectedObjects           = 0;
	string savedPath                = "";
    /// GlTF_Node rootNode;

    static bool done                = true;
    /// Transform排序，深度遍历
    private Transform[] temp;
    /// Transform排序，深度遍历
    private List<Transform> tempTRList = new List<Transform>();
    private int tc;

    private static float[] Vector3toAray(Vector3 v)
    {
        float[] arr = new float[3];
        arr[0] = v.x;
        arr[1] = v.y;
        arr[2] = v.z;
        return arr;
    }

    private static void parseUnityCamera(Transform tr)
	{
		if (tr.GetComponent<Camera>().orthographic)
		{
			GlTF_Orthographic cam = new GlTF_Orthographic();
			cam.type = "orthographic";
			cam.size = tr.GetComponent<Camera> ().orthographicSize;
			cam.zfar = tr.GetComponent<Camera>().farClipPlane;
			cam.znear = tr.GetComponent<Camera>().nearClipPlane;
			cam.name = GlTF_Writer.cleanNonAlphanumeric(tr.name);
			//cam.xmag =tr.GetComponent<Camera>().;
			GlTF_Writer.cameras.Add(cam);
		}
		else
		{
			GlTF_Perspective cam = new GlTF_Perspective();
			cam.type = "perspective";
			cam.zfar = tr.GetComponent<Camera>().farClipPlane;
			cam.znear = tr.GetComponent<Camera>().nearClipPlane;
			cam.aspect_ratio = tr.GetComponent<Camera>().aspect;
			cam.yfov = tr.GetComponent<Camera>().fieldOfView;
			cam.name = GlTF_Writer.cleanNonAlphanumeric(tr.name);
			GlTF_Writer.cameras.Add(cam);
		}
	}

    private int getNbSelectedObjects()
    {
        return nbSelectedObjects;
    }

    private bool isDone()
	{
		return done;
	}

    private void resetParser()
	{
		done = false;
	}

    /// <summary>
    /// 粒子系统
    /// </summary>
    /// <param name="tr"></param>
    private void parseUnityParticle(Transform tr){
		GlTF_Particle par = new GlTF_Particle();
		Renderer mr = tr.GetComponent<Renderer>();
		var diffuseTextureIndex = -1;


        par.initParticleSystem(tr);

        if (mr != null) {
			var sm = mr.sharedMaterials;
				var mat = sm[0];
			if (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)
			{
				var textureValue = new GlTF_Material.DictValue();
				textureValue.name = "diffuseTexture";
				diffuseTextureIndex = processTextureForParticle((Texture2D)mat.GetTexture("_MainTex"), IMAGETYPE.RGBA );
				textureValue.intValue.Add("index", diffuseTextureIndex);
				textureValue.intValue.Add("texCoord", 0);

                par.setTextureIndex(diffuseTextureIndex);

                Vector2 offset = mat.GetTextureOffset("_MainTex");
                if (offset != null && offset.x != 0 && offset.y != 0)
                {
                    par.setTextureOffset("[" + offset.x + "," + offset.y + "]");
                    //textureValue.stringValue.Add("offset", "[" + offset.x + "," + offset.y + "]");
                }

                Vector2 scale = mat.GetTextureScale("_MainTex");
                if (scale != null && scale.x != 1 && scale.y != 1 && scale.x != 0 && scale.y != 0)
                {
                    par.setTextureScale("[" + scale.x + "," + scale.y + "]");
                    //textureValue.stringValue.Add("scale", "[" + scale.x + "," + scale.y + "]");
                }
            }

            int srcBlend = (int)mat.GetFloat("_SrcBlend");
            int dstBlend = (int)mat.GetFloat("_DstBlend");
            par.setAlphaMode(AlphaMode.format_particle(srcBlend, dstBlend));
        }

        GlTF_Writer.particles.Add(par);
	}

    /// <summary>
    /// 灯光，点 ，聚光灯，平行光
    /// </summary>
    /// <param name="tr"></param>
    private static void parseUnityLight(Transform tr, ExportOption opt)
	{
		var trL = tr.GetComponent<Light> ();
		switch (trL.type)
		{
		    case LightType.Point:
				GlTF_PointLight pl = new GlTF_PointLight ();
				pl.intensity = trL.intensity;
				pl.range = trL.range;
				pl.color = new GlTF_ColorRGB(trL.color);
				pl.name = GlTF_Writer.cleanNonAlphanumeric(tr.name);
				GlTF_Writer.lights.Add(pl);
				break;
			case LightType.Spot:
				GlTF_SpotLight sl = new GlTF_SpotLight ();
				sl.intensity = trL.intensity;
				sl.innerConeAngle = trL.spotAngle;
				sl.range = trL.range;
				sl.outerConeAngle = trL.spotAngle;
				sl.color = new GlTF_ColorRGB(trL.color);
				sl.name = GlTF_Writer.cleanNonAlphanumeric(tr.name);
				GlTF_Writer.lights.Add(sl);
				break;
			case LightType.Directional:
                var direction = new Vector3(0, 0, 1);
                var transformedDirection = trL.gameObject.transform.TransformDirection(direction);

                GlTF_DirectionalLight dl = new GlTF_DirectionalLight();
				dl.intensity    = trL.intensity;
				dl.color        = new GlTF_ColorRGB(trL.color);
				dl.name         = GlTF_Writer.cleanNonAlphanumeric(tr.name);
                // dl.direction    = "[0,0,-1]"; // 使用标准扩展 不需要方向位置
                // dl.position     = "[0,0,0]";
                if (opt.mShadow && trL.shadows != LightShadows.None)
                {
                    dl.shadowGenerator = new LightShadowGenerator();
                    dl.shadowGenerator.bias = trL.shadowBias / 10.0f;
                    dl.shadowGenerator.mapSize = 256 + 256 * QualitySettings.GetQualityLevel();
                }
                GlTF_Writer.lights.Add(dl);
				break;
			case LightType.Area:
				GlTF_AmbientLight al = new GlTF_AmbientLight();
				al.color = new GlTF_ColorRGB(trL.color);
				al.name = GlTF_Writer.cleanNonAlphanumeric(tr.name);
				GlTF_Writer.lights.Add(al);
				break;
		}

	}

    /// <summary>
    /// Transform排序，深度遍历
    /// </summary>
    /// <param name="trs"></param>
    /// <param name="c"></param>
    /// <returns></returns>
	private Transform[] sortTrs(Transform[] trs, int c){
        tempTRList.Clear();
        temp = new Transform[c];
		tc = 0;

		for(var i = 0;i<trs.Length;i++){
			temp[tc] = trs[i];

            tc++;

            tempTRList.Add(trs[i]);

            if (trs[i].childCount > 0){
				sortTrs(trs[i]);
			}
		}

        //return temp;
        return tempTRList.ToArray();
    }

    /// <summary>
    /// Transform 深度遍历
    /// </summary>
    /// <param name="trs"></param>
	private void sortTrs(Transform trs)
    {
		for(var i = 0 ; i < trs.childCount; i++){
			Transform t = trs.GetChild (i);
			temp[tc] = t;

			tc++;

            tempTRList.Add(t);

            if (t.childCount > 0){
				sortTrs(t);
			}
		}
	}
    
    /// <summary>
    /// 处理天空盒
    /// </summary>
    /// <param name="path"></param>
    /// <param name="presetAsset"></param>
    /// <param name="exportPBRMaterials"></param>
    /// <param name="exportAnimation"></param>
    /// <param name="doConvertImages"></param>
    private void analySkyBox(ExportOption option)
    {

        int skyCount        = 0;
        var haveSkybox      = RenderSettings.skybox;
        string[] skyNames   = new string[6];

        /// 天空盒
        if (haveSkybox)
        {
            Texture2D nz;
            if (RenderSettings.skybox.HasProperty("_FrontTex"))
            {
                nz = RenderSettings.skybox.GetTexture("_FrontTex") as Texture2D;
                skyNames[0] = "nz";
                skyCount++;
                GlTF_Writer.PiskyTexture.Add(nz);
            }

            Texture2D pz;
            if (RenderSettings.skybox.HasProperty("_BackTex"))
            {
                pz = RenderSettings.skybox.GetTexture("_BackTex") as Texture2D;
                skyNames[1] = "pz";
                skyCount++;
                GlTF_Writer.PiskyTexture.Add(pz);
            }

            Texture2D nx;
            if (RenderSettings.skybox.HasProperty("_LeftTex"))
            {
                nx = RenderSettings.skybox.GetTexture("_LeftTex") as Texture2D;
                skyNames[2] = "nx";
                skyCount++;
                GlTF_Writer.PiskyTexture.Add(nx);
            }

            Texture2D px;
            if (RenderSettings.skybox.HasProperty("_RightTex"))
            {
                px = RenderSettings.skybox.GetTexture("_RightTex") as Texture2D;
                skyNames[3] = "px";
                skyCount++;
                GlTF_Writer.PiskyTexture.Add(px);
            }

            Texture2D ny;
            if (RenderSettings.skybox.HasProperty("_UpTex"))
            {
                ny = RenderSettings.skybox.GetTexture("_UpTex") as Texture2D;
                skyNames[4] = "ny";
                skyCount++;
                GlTF_Writer.PiskyTexture.Add(ny);
            }

            Texture2D py;
            if (RenderSettings.skybox.HasProperty("_DownTex"))
            {
                py = RenderSettings.skybox.GetTexture("_DownTex") as Texture2D;
                skyNames[5] = "py";
                skyCount++;
                GlTF_Writer.PiskyTexture.Add(py);
            }
        }

        /// path = toGlTFname(path);
        /// 传进来的路径
        savedPath = Path.GetDirectoryName(option.mFileName);
        if (skyCount > 0 && skyCount < 6)
        {
            Debug.Log("处理 天空盒 失败 - 天空盒必须要有6张图 \n");
            throw new Exception("天空盒必须要有6张图");
        }
        else if (skyCount == 6)
        {
            GlTF_Writer.PiExportSky = true;

            for (var i = 0; i < GlTF_Writer.PiskyTexture.Count; i++)
            {
                convertTexture(GlTF_Writer.PiskyTexture[i], option.mFileName, savedPath, skyNames[i]);
            }
        }

        Debug.Log("处理 天空盒 - 面数 " + skyCount + "\n");
    }

    /// <summary>
    /// 处理坐标系
    /// </summary>
    private bool analyCoordinateSys(ExportOption option)
    {
        GlTF_Writer.exportedFiles.Clear();

        if (option.convertRightHanded)
        {
            GlTF_Writer.convertRightHanded = true;
        } else
        {
            GlTF_Writer.convertRightHanded = false;
        }

        Debug.Log("处理坐标系 是否转换为右手坐标系 - " + option.convertRightHanded);

        return option.convertRightHanded;
    }

    /// <summary>
    /// 处理根节点
    /// </summary>
    private GlTF_Node analyRootNode()
    {
        /// Create rootNode
        GlTF_Node rootNode = new GlTF_Node();
        rootNode.id = "UnityGlTF_root";
        rootNode.name = "UnityGlTF_root";

        // GlTF_Writer.nodes.Add(rootNode);
        // GlTF_Writer.nodeNames.Add(rootNode.name);
        // GlTF_Writer.rootNodes.Add(rootNode);
        
        Debug.Log("处理根节点 " + rootNode.name + "\n");

        return rootNode;
    }

    /// <summary>
    /// 收集场景中的 Transform 并排序
    /// </summary>
    /// <returns></returns>
    private List<Transform> initTransformList(Transform[] rootNodes)
    {
        Transform[] transforms;
        if (rootNodes == null)
        {
            /// 首先，收集场景中的对并且排序，添加到列表中
            transforms = Selection.GetTransforms(SelectionMode.TopLevel);
        } 
        else
        {
            transforms = rootNodes;
        }

        /// 排序
        transforms = sortTrs(transforms, transforms.Length);

        List<Transform> trs = new List<Transform>(transforms);

        /// 选择的节点数量
		nbSelectedObjects = trs.Count;

        return trs;
    }

    /// <summary>
    /// 收集场景中的 Transform 并排序
    /// </summary>
    /// <returns></returns>
    private List<Transform> initTransformList()
    {
        Transform[] transforms;

        /// 首先，收集场景中的对并且排序，添加到列表中
        transforms = Selection.GetTransforms(SelectionMode.TopLevel);

        /// 排序
        transforms = sortTrs(transforms, Selection.GetTransforms(SelectionMode.Deep).Length);

        List<Transform> trs = new List<Transform>(transforms);

        /// 选择的节点数量
		nbSelectedObjects = trs.Count;

        return trs;
    }
    /// <summary>
    /// 解析节点列表
    /// </summary>
    /// <param name="trs"></param>
    /// <param name="rootNode"></param>
    /// <param name="path"></param>
    /// <param name="exportAnimation"></param>
    /// <param name="debugRightHandedScale"></param>
    /// <param name="mExportNormal"></param>
    private void analyTransforms(List<Transform> trs, GlTF_Node rootNode, ExportOption option)
    {
        int nbDisabledObjects = 0;

        foreach (Transform tr in trs)
        {
            analyPiComponent(tr);

            /// /如果有没勾选(非活动状态)的node不会导出，最后会提示有多少个
			if (tr.gameObject.activeInHierarchy == false)
            {
                nbDisabledObjects++;//如果有没勾选的node不会导出，最后会提示有多少个
                continue;
            }

            /// 初始化node
            GlTF_Node node  = new GlTF_Node();
            node.id         = GlTF_Node.GetNameFromObject(tr);
            node.name       = GlTF_Writer.cleanNonAlphanumeric(tr.name);

            GlTF_Writer.EveryMeshAndAnimInit();

            analyLight(tr, node, option);

            analyCamera(tr, node);

            analyParticleSystem(tr, node);

            analyMesh(tr, node, option);

            analyAnimator(tr, node, option.mFileName);

            analyAnimation(trs, tr, node, option);

            analyAnimCount(node);

            analyTree(trs, tr, rootNode, node, option);

            analySkinnedMeshRenderer(trs, tr, node, option);

            foreach (Transform t in tr.transform)
            {
                if (t.gameObject.activeInHierarchy)
                    node.childrenNames.Add(GlTF_Node.GetNameFromObject(t));
            }

            GlTF_Writer.nodeNames.Add(node.id);
            GlTF_Writer.nodes.Add(node);
        }

        if (nbDisabledObjects > 0)
        {
            Debug.Log(nbDisabledObjects + " disabled object ignored during export");
        }
    }

    /// <summary>
    /// 判断是否导出 RTPL
    /// </summary>
    /// <param name="tr"></param>
    private void analyPiComponent(Transform tr)
    {
        ///// PiComponent 和 RTPL
        //if (tr.GetComponent<PiComponent>())
        //{
        //    var needRtpl = tr.GetComponent<PiComponent>();
        //    if (needRtpl.isRtpl)
        //    {
        //        writer.exportRtpl = true;
        //    }
        //}

        Debug.Log("isRtpl ? - " + writer.exportRtpl);
    }

    /// <summary>
    /// 解析 相机节点
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="node"></param>
    private void analyCamera(Transform tr, GlTF_Node node)
    {
        if (tr.GetComponent<Camera>() != null)
        {
            parseUnityCamera(tr);
            node.cameraName = GlTF_Writer.cameras.Count - 1;
        }

        Debug.Log("解析相机 - " + node.cameraName);
    }

    /// <summary>
    /// 解析光照节点
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="node"></param>
    private void analyLight(Transform tr, GlTF_Node node, ExportOption opt)
    {
        if (tr.GetComponent<Light>() != null)
        {
            parseUnityLight(tr, opt);
            node.lightName  = GlTF_Writer.cleanNonAlphanumeric(tr.name);
            node.lightIndex = GlTF_Writer.lights.Count - 1;
        }

        Debug.Log("解析灯光 - " + node.lightName);
    }

    /// <summary>
    /// 解析粒子节点
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="node"></param>
    private void analyParticleSystem(Transform tr, GlTF_Node node)
    {
        if (tr.GetComponent<ParticleSystem>() != null)
        {
            parseUnityParticle(tr);
            node.particleIndex = GlTF_Writer.particles.Count - 1;
        }

        Debug.Log("解析粒子 - " + node.particleIndex);
    }

    /// <summary>
    /// 解析 SkinnedMesh
    /// </summary>
    /// <param name="trs"></param>
    /// <param name="tr"></param>
    /// <param name="node"></param>
    /// <param name="path"></param>
    /// <param name="exportAnimation"></param>
    private void analySkinnedMeshRenderer(List<Transform> trs, Transform tr, GlTF_Node node, ExportOption option)
    {
        /// 骨骼的包围盒  用于点击
        SkinnedMeshRenderer skinMesh = tr.GetComponent<SkinnedMeshRenderer>();
        if (skinMesh != null)
        {
            node.center     = skinMesh.localBounds.center;
            node.extents    = skinMesh.localBounds.extents;
            node.boneName   = skinMesh.rootBone.name;
        }

        Debug.Log("解析 骨骼的包围盒 - " + node.boneName);

        GlTF_Accessor invBindMatrixAccessor = null;
        if (option.mAnimation && skinMesh != null && skinMesh.enabled && checkSkinValidity(skinMesh, trs) && skinMesh.rootBone != null)
        {
            GlTF_Skin skin = new GlTF_Skin();

            skin.name = GlTF_Writer.cleanNonAlphanumeric(skinMesh.rootBone.name) + "_skeleton_" + GlTF_Writer.cleanNonAlphanumeric(node.name) + tr.GetInstanceID();

            // Create invBindMatrices accessor
            invBindMatrixAccessor = new GlTF_Accessor(skin.name + "invBindMatrices", GlTF_Accessor.Type.MAT4, GlTF_Accessor.ComponentType.FLOAT);
            invBindMatrixAccessor.PibufferView = GlTF_Pi_BufferView.Pimat4BufferView;
            GlTF_Writer.accessors.Add(invBindMatrixAccessor);

            // Generate skin data
            skin.Populate(tr, ref invBindMatrixAccessor, GlTF_Writer.accessors.Count - 1, option.mFileName);
            GlTF_Writer.skins.Add(skin);
            node.skinIndex = GlTF_Writer.skins.IndexOf(skin);

            Debug.Log("解析 骨骼 - " + skin.name);
        }
    }

    /// <summary>
    /// 解析 普通 Mesh 包围盒
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="node"></param>
    private void analyBoxCollider(Transform tr, GlTF_Node node)
    {
        /// mesh的包围盒  用于点击
        var mbc = tr.GetComponent<BoxCollider>();
        if (mbc != null && mbc.enabled)
        {
            node.center = mbc.center;
            node.extents = mbc.size;
            node.meshBox = true;

            Debug.Log("解析 普通 Mesh 包围盒 - " + node.name);
        }
    }

    private void analyMesh(Transform tr, GlTF_Node node, ExportOption option)
    {

        Debug.Log("开始处理 Mesh " + tr.name + "\n");

        Mesh m = GetMesh(tr);
        if (m != null)
        {
            GlTF_Mesh mesh = new GlTF_Mesh();
            //if (tr.GetComponent<PiComponent>())
            //{
            //    var needRtpl = tr.GetComponent<PiComponent>();
            //    if (needRtpl.isRtpl)
            //    {
            //        mesh.matToRtpl = true;
            //    }
            //}

            /// 清洁非字母数字
            mesh.name = GlTF_Writer.cleanNonAlphanumeric(GlTF_Mesh.GetNameFromObject(m));

            GlTF_Accessor positionAccessor  = createPositionAccessor(m);
            GlTF_Accessor normalAccessor    = createNormalAccessor(m, option.mNormal);
            GlTF_Accessor uv0Accessor       = createUV0Accessor(m);
            GlTF_Accessor uv1Accessor       = createUV1Accessor(m);
            GlTF_Accessor colorAccessor     = createColorAccessor(m);
            GlTF_Accessor jointAccessor     = createJointAccessor(m, option.mAnimation);
            GlTF_Accessor weightAccessor    = createWeightAccessor(m, option.mAnimation);
            GlTF_Accessor tangentAccessor   = createTangentAccessor(m, option.mAnimation);

            //string positionMd5 = GlTF_Md5.getMd5 (m.vertices);

            var mr              = GetRenderer(tr);
            var sharedMaterials = mr.sharedMaterials;

            var subMeshCount    = m.subMeshCount;
            for (var i = 0; i < subMeshCount; ++i)
            {
                GlTF_Accessor indexAccessor     = createIndexAccessor(m, i);
                GlTF_Primitive primitive        = new GlTF_Primitive();
                GlTF_Attributes attributes      = new GlTF_Attributes();
                
                attributes.positionAccessor     = positionAccessor;
                attributes.normalAccessor       = normalAccessor;
                attributes.colorAccessor        = colorAccessor;
                attributes.texCoord0Accessor    = uv0Accessor;
                attributes.texCoord1Accessor    = uv1Accessor;
                //attributes.texCoord2Accessor  = uv2Accessor;
                //attributes.texCoord3Accessor  = uv3Accessor;
                attributes.jointAccessor        = jointAccessor;
                attributes.weightAccessor       = weightAccessor;
                //attributes.tangentAccessor    = tangentAccessor;

                primitive.name                  = GlTF_Primitive.GetNameFromObject(m, i);
                primitive.index                 = i;
                primitive.indices               = indexAccessor;
                primitive.attributes            = attributes;
                
                if (i < sharedMaterials.Length)
                {
                    var mat     = sharedMaterials[i];
                    var matName = getMaterialName(mat, mesh.matToRtpl);
                    matName = tr.name + matName;

                    if (GlTF_Writer.materialNames.Contains(matName))
                    {
                        primitive.materialIndex = GlTF_Writer.materialNames.IndexOf(matName); // THIS INDIRECTION CAN BE REMOVED!
                    }
                    else
                    {
                        GlTF_Material material  = new GlTF_Material();

                        material.name           = GlTF_Writer.cleanNonAlphanumeric(matName);
                        primitive.materialIndex = GlTF_Writer.materials.Count;

                        /// 添加材质信息
                        GlTF_Writer.materialNames.Add(matName);
                        GlTF_Writer.materials.Add(material);

                        /// technique
                        var shader              = mat.shader;
                        /// create program
                        GlTF_Program program    = new GlTF_Program();
                        program.name            = GlTF_Program.GetNameFromObject(shader);

                        var techName            = GlTF_Technique.GetNameFromObject(shader);

                        if (mesh.matToRtpl)
                        {
                            techName            = techName + "PiRtpl";
                            material.matToRtpl  = true;
                        }

                        if (GlTF_Writer.techniqueNames.Contains(techName))
                        {
                            material.instanceTechniqueIndex = GlTF_Writer.techniqueNames.IndexOf(techName);// THIS INDIRECTION CAN BE REMOVED!
                        }
                        else
                        {
                            GlTF_Technique tech     = new GlTF_Technique();
                            tech.name               = techName;
                            tech.program            = program.name;

                            /// 收集 Shader 所需参数
                            positionParameterAndAttribute(tech);
                            normalParameterAndAttribute(tech, normalAccessor);
                            uv0ParameterAndAttribute(tech, uv0Accessor);
                            uv1ParameterAndAttribute(tech, uv1Accessor);

                            tech.AddDefaultUniforms();

                            /// Populate technique with shader data 使用着色器数据填充技术
                            GlTF_Writer.techniqueNames.Add(techName);
                            GlTF_Writer.techniques.Add(tech);

                            /// 处理 Shader 所用参数
                            foreach (var attr in tech.attributes)
                            {
                                program.attributes.Add(attr.name);
                            }

                            GlTF_Writer.programs.Add(program);
                        }

                        /// material 数据收集
                        /// unityToPBRMaterial(mat, ref material);
                        if (mat.shader.name == "Standard")
                        {
                            unityToPBRMaterial(mat, ref material);
                        }
                        else
                        {
                            if (mat.shader.name == "Pi/PBR")
                            {
                                unityToPiMaterial_PBR(mat, mr, option, ref material);
                            }
                            else if (mat.shader.name == "Pi/PBRMetallicRoughness")
                            {
                                unityToPiMaterial_PBRMetallicRoughness(mat, mr, option, ref material);
                            }
                            else if (mat.shader.name == "Pi/PBRSpecularGlossiness")
                            {
                                unityToPiMaterial_PBRSpecularGlossiness(mat, mr, option, ref material);
                            }
                            else
                            {
                                unityToPiMaterial(mat, mr, option, ref material);
                            }
                        }
                        mesh.alphaIndex = material.renderQueue;
                    }
                }

                mesh.primitives.Add(primitive);
            }

            /// If gameobject having SkinnedMeshRenderer component has been transformed,
            /// the mesh would need to be baked here.如果已经变换了具有蒙皮网格渲染器组件的游戏对象，则需要在此处对网格进行烘焙。
            mesh.Populate(m, option.mFileName);

            /// mesh的包围盒  用于点击
            var mbc = tr.GetComponent<BoxCollider>();
            if (mbc != null && mbc.enabled)
            {
                mesh.center = mbc.center;
                mesh.extents = mbc.size;
                mesh.meshBox = true;

                Debug.Log("解析 普通 Mesh 包围盒 - " + node.name);
            }


            analyBoxCollider(tr, node);

            GlTF_Writer.meshes.Add(mesh);

            node.meshIndex = GlTF_Writer.meshes.IndexOf(mesh);
        }

        Debug.Log("结束处理 Mesh " + tr.name + "\n");
    }

    private void analySubMeshes(Mesh[] submeshes)
    {

    }

    private void analySubMesh(Mesh submesh)
    {

    }

    private void analyAnimator(Transform tr, GlTF_Node node, string path)
    {

        Debug.Log("开始解析 Animator - " + tr.name);

        Animator a = tr.GetComponent<Animator>();
        if (a != null)
        {
            GlTF_Writer.EveryMeshAndAnimInit();
            byte[] byteArr = new byte[0];
            AnimationClip[] clips = AnimationUtility.GetAnimationClips(tr.gameObject);
            for (int i = 0; i < clips.Length; i++)
            {
                //FIXME It seems not good to generate one animation per animator.每个animator生成一个动画似乎并不好。
                GlTF_Animation anim = new GlTF_Animation(GlTF_Writer.cleanNonAlphanumeric(clips[i].name));
                byteArr = GlTF_Md5.combine(byteArr, anim.Populate(clips[i], tr, path, GlTF_Writer.bakeAnimation));
                if (anim.exportAnim.Count > 0)
                {
                    GlTF_Writer.animations.Add(anim);
                    GlTF_Writer.oneAnimator.Add(anim);
                }
            }


            if (clips.Length > 0)
            {
                Debug.Log("收集 Animator 数据 - ");

                //1个模型有多个动画只导出一个动画文件
                string aniMd5 = GlTF_Md5.byteToMd5(byteArr);
                GlTF_Buffer oneAnim = new GlTF_Buffer();

                GlTF_Writer.AniPifloatBufferView.byteOffset = 0;
                GlTF_Writer.Pivec1BufferViewAnim.byteOffset = GlTF_Writer.AniPifloatBufferView.byteOffset + GlTF_Writer.AniPifloatBufferView.byteLength;
                GlTF_Writer.Pivec3BufferViewAnim.byteOffset = GlTF_Writer.Pivec1BufferViewAnim.byteOffset + GlTF_Writer.Pivec1BufferViewAnim.byteLength;
                GlTF_Writer.Pivec4BufferViewAnim.byteOffset = GlTF_Writer.Pivec3BufferViewAnim.byteOffset + GlTF_Writer.Pivec3BufferViewAnim.byteLength;

                string filePath = Path.Combine(Path.GetDirectoryName(path), aniMd5 + ".anim.bin");
                Stream PibinFile = File.Open(filePath, FileMode.Create);
                oneAnim.uri = aniMd5 + ".anim.bin";

                Debug.Log("写入 - " + oneAnim.uri);

                GlTF_Writer.animations.ForEach(v => {
                    if (!v.haveName)
                    {
                        v.md5Name = aniMd5;
                        v.haveName = !v.haveName;
                        v.fPath = filePath;
                    }
                });

                GlTF_Writer.AniPifloatBufferView.PimemoryStream.WriteTo(PibinFile);
                GlTF_Writer.Pivec1BufferViewAnim.PimemoryStream.WriteTo(PibinFile);
                GlTF_Writer.Pivec3BufferViewAnim.PimemoryStream.WriteTo(PibinFile);
                GlTF_Writer.Pivec4BufferViewAnim.PimemoryStream.WriteTo(PibinFile);

                oneAnim.BLength = PibinFile.Length;
                oneAnim.BufferviewIndex = GlTF_Writer.buffers.Count;
                PibinFile.Flush();
                PibinFile.Close();
                GlTF_Writer.buffers.Add(oneAnim);
                GlTF_Writer.oneAnimator.ForEach(v => v.bufferIndex = oneAnim.BufferviewIndex);

                Debug.Log("保存 - " + oneAnim.uri);
            }
        }

        Debug.Log("结束解析 Animator - " + tr.name);
    }

    private void analyAnimation(List<Transform> trs, Transform tr, GlTF_Node node, ExportOption option)
    {

        Animation animation = tr.GetComponent<Animation>();

        if (animation != null)
        {

            Debug.Log("开始解析 Animation - " + tr.name);

            AnimationClip clip  = animation.clip;

            //FIXME It seems not good to generate one animation per animator.
            GlTF_Animation anim = new GlTF_Animation(GlTF_Writer.cleanNonAlphanumeric(animation.name));

            anim.Populate(clip, tr, option.mFileName, GlTF_Writer.bakeAnimation);

            if (anim.channels.Count > 0)
            {
                GlTF_Writer.animations.Add(anim);
            }

            Debug.Log("结束解析 Animation - " + tr.name);
        }
    }

    private void analyAnimCount(GlTF_Node node)
    {
        GlTF_Anim_Count trAnimCount = GlTF_Writer.animCounts.Find(x => x.trName == node.id);
        if (trAnimCount != null)
        {
            node.animCount = trAnimCount.animCount;
        }
    }

    private void analyTree(List<Transform> trs, Transform tr, GlTF_Node rootNode, GlTF_Node node, ExportOption opt)
    {
        // Parse transform
        if (tr.parent == null)
        {
            Matrix4x4 mat = Matrix4x4.identity;
            if (opt.convertRightHanded)
                mat.m22 = -1;
            mat = mat * Matrix4x4.TRS(tr.localPosition, tr.localRotation, tr.localScale);
            node.matrix = new GlTF_Matrix(mat);
        }
        // Use good transform if parent object is not in selection
        else if (!trs.Contains(tr.parent))
        {
            node.hasParent = false;
            Matrix4x4 mat = Matrix4x4.identity;
            if (opt.convertRightHanded)
                mat.m22 = -1;
            mat = mat * tr.localToWorldMatrix;
            node.matrix = new GlTF_Matrix(mat);
        }
        else
        {
            node.hasParent = true;
            if (tr.localPosition != Vector3.zero)
                node.translation = new GlTF_Translation(tr.localPosition);
            if (tr.localScale != Vector3.one)
                node.scale = new GlTF_Scale(tr.localScale);
            if (tr.localRotation != Quaternion.identity)
                node.rotation = new GlTF_Rotation(tr.localRotation);
        }

        if (!node.hasParent)
        {

            // GlTF_Writer.nodes.Add(node);
            // GlTF_Writer.nodeNames.Add(node.name);
            GlTF_Writer.rootNodes.Add(node);
            // rootNode.childrenNames.Add(node.id);
        }
    }
    
    private GlTF_Accessor createPositionAccessor(Mesh m)
    {
        GlTF_Accessor positionAccessor = new GlTF_Accessor(
                                           GlTF_Accessor.GetNameFromObject(m, "position"),
                                           GlTF_Accessor.Type.VEC3,
                                           GlTF_Accessor.ComponentType.FLOAT
                                       );
        positionAccessor.PibufferView = GlTF_Writer.Pivec3BufferView;
        GlTF_Writer.accessors.Add(positionAccessor);

        return positionAccessor;
    }

    private GlTF_Accessor createNormalAccessor(Mesh m, bool mExportNormal)
    {
        GlTF_Accessor normalAccessor = null;
        if (m.normals.Length > 0 && mExportNormal)
        {
            normalAccessor              = new GlTF_Accessor(
                                            GlTF_Accessor.GetNameFromObject(m, "normal"), 
                                            GlTF_Accessor.Type.VEC3, 
                                            GlTF_Accessor.ComponentType.FLOAT
                                        );
            normalAccessor.PibufferView = GlTF_Writer.Pivec3BufferView;
            GlTF_Writer.accessors.Add(normalAccessor);
        }

        return normalAccessor;
    }

    private GlTF_Accessor createColorAccessor(Mesh m)
    {
        GlTF_Accessor colorAccessor = null;

        if (m.colors.Length > 0)
        {
            colorAccessor = new GlTF_Accessor(
                                GlTF_Accessor.GetNameFromObject(m, "color"),
                                GlTF_Accessor.Type.VEC4,
                                GlTF_Accessor.ComponentType.FLOAT
                            );
            colorAccessor.PibufferView = GlTF_Writer.Pivec4BufferView;
            GlTF_Writer.accessors.Add(colorAccessor);
        }

        return colorAccessor;
    }

    private GlTF_Accessor createUV0Accessor(Mesh m)
    {

        GlTF_Accessor uv0Accessor = null;

        if (m.uv.Length > 0)
        {
            uv0Accessor = new GlTF_Accessor(
                                GlTF_Accessor.GetNameFromObject(m, "uv0"),
                                GlTF_Accessor.Type.VEC2,
                                GlTF_Accessor.ComponentType.FLOAT
                            );
            uv0Accessor.PibufferView = GlTF_Writer.Pivec2BufferView;
            GlTF_Writer.accessors.Add(uv0Accessor);
        }

        return uv0Accessor;
    }

    private GlTF_Accessor createUV1Accessor(Mesh m)
    {
        GlTF_Accessor uv1Accessor = null;

        if (m.uv2.Length > 0)
        {
            uv1Accessor = new GlTF_Accessor(
                                GlTF_Accessor.GetNameFromObject(m, "uv1"),
                                GlTF_Accessor.Type.VEC2,
                                GlTF_Accessor.ComponentType.FLOAT
                            );
            uv1Accessor.PibufferView = GlTF_Writer.Pivec2BufferView;
            GlTF_Writer.accessors.Add(uv1Accessor);
        }

        return uv1Accessor;
    }

    private GlTF_Accessor createJointAccessor(Mesh m, bool exportAnimation)
    {
        GlTF_Accessor jointAccessor = null;

        if (exportAnimation && m.boneWeights.Length > 0)
        {
            jointAccessor = new GlTF_Accessor(
                                GlTF_Accessor.GetNameFromObject(m, "joints"),
                                GlTF_Accessor.Type.VEC4,
                                GlTF_Accessor.ComponentType.USHORT
                            );
            jointAccessor.PibufferView = GlTF_Writer.Pivec4UshortBufferView;

            GlTF_Writer.accessors.Add(jointAccessor);
        }

        return jointAccessor;
    }

    private GlTF_Accessor createWeightAccessor(Mesh m, bool exportAnimation)
    {
        GlTF_Accessor weightAccessor = null;

        if (exportAnimation && m.boneWeights.Length > 0)
        {
            weightAccessor = new GlTF_Accessor(
                                GlTF_Accessor.GetNameFromObject(m, "weights"),
                                GlTF_Accessor.Type.VEC4,
                                GlTF_Accessor.ComponentType.FLOAT
                            );
            weightAccessor.PibufferView = GlTF_Writer.Pivec4BufferView;
            GlTF_Writer.accessors.Add(weightAccessor);
        }

        return weightAccessor;
    }

    private GlTF_Accessor createTangentAccessor(Mesh m, bool exportAnimation)
    {
        GlTF_Accessor tangentAccessor = null;

        if (m.tangents.Length > 0)
        {
            //tangentAccessor = new GlTF_Accessor(GlTF_Accessor.GetNameFromObject(m, "tangents"), GlTF_Accessor.Type.VEC4, GlTF_Accessor.ComponentType.FLOAT);
            //tangentAccessor.PibufferView = GlTF_Writer.Pivec4BufferView;

            //GlTF_Writer.accessors.Add(tangentAccessor);
        }

        return tangentAccessor;
    }

    private GlTF_Accessor createIndexAccessor(Mesh m, int i)
    {
        GlTF_Accessor indexAccessor = new GlTF_Accessor(
                                            GlTF_Accessor.GetNameFromObject(m, "indices_" + i),
                                            GlTF_Accessor.Type.SCALAR,
                                            GlTF_Accessor.ComponentType.USHORT
                                        );
        indexAccessor.PibufferView = GlTF_Writer.PiushortBufferView;
        GlTF_Writer.accessors.Add(indexAccessor);

        return indexAccessor;
    }

    private void positionParameterAndAttribute(GlTF_Technique tech)
    {
        GlTF_Technique.Parameter tParam = new GlTF_Technique.Parameter();

        tParam.name = "position";
        tParam.type = GlTF_Technique.Type.FLOAT_VEC3;
        tParam.semantic = GlTF_Technique.Semantic.POSITION;

        tech.parameters.Add(tParam);

        GlTF_Technique.Attribute tAttr = new GlTF_Technique.Attribute();

        tAttr.name = "a_position";
        tAttr.param = tParam.name;

        tech.attributes.Add(tAttr);
    }

    private void normalParameterAndAttribute(GlTF_Technique tech, GlTF_Accessor accessor)
    {
        if (accessor != null)
        {
            GlTF_Technique.Parameter tParam = new GlTF_Technique.Parameter();
            tParam.name = "normal";
            tParam.type = GlTF_Technique.Type.FLOAT_VEC3;
            tParam.semantic = GlTF_Technique.Semantic.NORMAL;

            tech.parameters.Add(tParam);

            GlTF_Technique.Attribute tAttr = new GlTF_Technique.Attribute();
            tAttr.name = "a_normal";
            tAttr.param = tParam.name;

            tech.attributes.Add(tAttr);
        }
    }

    private void uv0ParameterAndAttribute(GlTF_Technique tech, GlTF_Accessor accessor)
    {
        if (accessor != null)
        {
            GlTF_Technique.Parameter tParam = new GlTF_Technique.Parameter();
            tParam.name = "texcoord0";
            tParam.type = GlTF_Technique.Type.FLOAT_VEC2;
            tParam.semantic = GlTF_Technique.Semantic.TEXCOORD_0;

            tech.parameters.Add(tParam);

            GlTF_Technique.Attribute tAttr = new GlTF_Technique.Attribute();
            tAttr.name = "a_texcoord0";
            tAttr.param = tParam.name;

            tech.attributes.Add(tAttr);
        }
    }

    private void uv1ParameterAndAttribute(GlTF_Technique tech, GlTF_Accessor accessor)
    {
        if (accessor != null)
        {
            GlTF_Technique.Parameter tParam = new GlTF_Technique.Parameter();
            tParam.name = "texcoord1";
            tParam.type = GlTF_Technique.Type.FLOAT_VEC2;
            tParam.semantic = GlTF_Technique.Semantic.TEXCOORD_1;

            tech.parameters.Add(tParam);

            GlTF_Technique.Attribute tAttr = new GlTF_Technique.Attribute();
            tAttr.name = "a_texcoord1";
            tAttr.param = tParam.name;

            tech.attributes.Add(tAttr);
        }
    }

    private string getMaterialName(Material mat, bool matToRtpl)
    {
        var matName = GlTF_Material.GetNameFromObject(mat);

        if (matToRtpl)
        {
            matName = GlTF_Material.GetNameFromObject(mat) + "PiRtpl";
        }

        return matName;
    }

    /// <summary>
    /// 查找 Transform 列表中的 Bone
    /// </summary>
    /// <param name="trs"></param>
    /// <returns></returns>
    private List<Transform> findBones(List<Transform> trs)
    {
        List<Transform> bones = new List<Transform>();
        foreach (Transform tr in trs)
        {
            /// 忽略未激活的节点
			if (!tr.gameObject.activeSelf)
                continue;

            SkinnedMeshRenderer skin = tr.GetComponent<SkinnedMeshRenderer>();
            if (skin)
            {
                foreach (Transform bone in skin.bones)
                {
                    bones.Add(bone);
                }
            }
        }

        Debug.Log("findBones : " + bones.Count);

        return bones;
    }

    /// <summary>
    /// 导出接口
    /// </summary>
    /// <param name="path"></param>
    /// <param name="presetAsset"></param>
    /// <param name="exportPBRMaterials"></param>
    /// <param name="exportAnimation"></param>
    /// <param name="doConvertImages"></param>
    /// <returns></returns>
    private IEnumerator Export2(ExportOption option, Transform[] rootNodes)
    {
        /// 初始化 GlTF_Writer
		GlTF_Writer.combineMesh = option.mCombineMesh;
        writer = new GlTF_Writer();
        writer.Init();
        Debug.Log("初始化 GlTF_Writer\n");

        /// 加入自己的版本号
		writer.extraString.Add("exporterVersion", GlTF_Writer.exporterVersion);
        Debug.Log("加入自己的版本号 " + GlTF_Writer.exporterVersion + "\n");

        done = false;

        GlTF_Node rootNode = analyRootNode();

        analySkyBox(option);

        bool debugRightHandedScale = analyCoordinateSys(option);

        /// 用于跟踪骨骼的临时列表
        Dictionary<string, GlTF_Skin> parsedSkins = new Dictionary<string, GlTF_Skin>();
        parsedSkins.Clear();

        List<Transform> trs     = initTransformList(rootNodes);
        List<Transform> bones   = findBones(trs);

        GlTF_Writer.geometryList.Clear();

        analyTransforms(trs, rootNode, option);

        string rPath = option.mFileName;

        if (writer.exportRtpl)
        {
            rPath = rPath.Substring(0, rPath.Length - 4) + "rtpl";
        }

        writer.OpenFiles(rPath);
        try
        {
            writer.Write();
        } catch
        {
            Debug.LogError("Error");
        }
        finally
        {
        }

        writer.CloseFiles();

        Debug.Log("Scene has been exported to " + option.mFileName + " \n");

        done = true;

        yield return true;
    }

    /// <summary>
    /// 导出接口
    /// </summary>
    /// <param name="path"></param>
    /// <param name="presetAsset"></param>
    /// <param name="exportPBRMaterials"></param>
    /// <param name="exportAnimation"></param>
    /// <param name="doConvertImages"></param>
    /// <returns></returns>
    private IEnumerator Export2(ExportOption option)
    {
        /// 初始化 GlTF_Writer
		GlTF_Writer.combineMesh = option.mCombineMesh;
        writer = new GlTF_Writer();
        writer.Init();
        Debug.Log("初始化 GlTF_Writer\n");

        /// 加入自己的版本号
		writer.extraString.Add("exporterVersion", GlTF_Writer.exporterVersion);
        Debug.Log("加入自己的版本号 " + GlTF_Writer.exporterVersion + "\n");

        done = false;

        GlTF_Node rootNode = analyRootNode();

        analySkyBox(option);

        bool debugRightHandedScale = analyCoordinateSys(option);

        /// 用于跟踪骨骼的临时列表
        Dictionary<string, GlTF_Skin> parsedSkins = new Dictionary<string, GlTF_Skin>();
        parsedSkins.Clear();

        List<Transform> trs = initTransformList();
        List<Transform> bones = findBones(trs);

        GlTF_Writer.geometryList.Clear();

        analyTransforms(trs, rootNode, option);

        string rPath = option.mFileName;

        if (writer.exportRtpl)
        {
            rPath = rPath.Substring(0, rPath.Length - 4) + "rtpl";
        }

        writer.OpenFiles(rPath);
        try
        {
            writer.Write();
        }
        catch
        {
            Debug.LogError("Error");
        }
        finally
        {
        }

        writer.CloseFiles();

        Debug.Log("Scene has been exported to " + option.mFileName + " \n");

        done = true;

        yield return true;
    }

    /// <summary>
    /// Check if all the bones referenced by the skin are in the selection
    /// 检查 skin 引用的所有 骨骼 是否都已选中
    /// </summary>
    /// <param name="skin"></param>
    /// <param name="selection"></param>
    /// <returns></returns>
    private bool checkSkinValidity(SkinnedMeshRenderer skin, List<Transform> selection)
	{
		string unselected = "";

		foreach(Transform t in skin.bones)
		{
			if (!selection.Contains(t))
			{
				unselected = unselected + "\n" + t.name;
			}
		}

		if(unselected.Length > 0)
		{
			Debug.LogError("Error while exportin skin for " + skin.name + " (skipping skinning export).\nClick for more details:\n \nThe following bones are used but are not selected" + unselected + "\n");
			return false;
		}

		return true;
	}

    /// <summary>
    /// 命名转换 空格 转 下划线
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
	private string toGlTFname(string name)
	{
		// remove spaces and illegal chars, replace with underscores
		string correctString = name.Replace(" ", "_");
		// make sure it doesn't start with a number
		return correctString;
	}

    /// <summary>
    /// 检查 实例继承关系
    /// </summary>
    /// <param name="t">子类</param>
    /// <param name="baseT">基类</param>
    /// <returns></returns>
	private bool isInheritedFrom(Type t, Type baseT)
	{
		if (t == baseT)
			return true;

		t = t.BaseType;
		while (t != null && t != typeof(System.Object))
		{
			if (t == baseT)
				return true;
			t = t.BaseType;
		}

		return false;
	}

    /// <summary>
    /// 获取 MeshRenderer / SkinnedMeshRenderer 面板数据
    /// </summary>
    /// <param name="tr">目标节点</param>
    /// <returns></returns>
	private Renderer GetRenderer(Transform tr)
	{
		Renderer mr = tr.GetComponent<MeshRenderer>();
		if (mr == null) {
			mr = tr.GetComponent<SkinnedMeshRenderer>();
		}
		return mr;
	}

    /// <summary>
    /// 标准处理颜色 - 颜色值处理在 0.0 ~ 1.0
    /// </summary>
    /// <param name="c"></param>
	private void clampColor(ref Color c)
	{
		c.r = c.r > 1.0f ? 1.0f : c.r;
		c.g = c.g > 1.0f ? 1.0f : c.g;
		c.b = c.b > 1.0f ? 1.0f : c.b;
		//c.a = c.a > 1.0f ? 1.0f : c.a;
	}

    /// <summary>
    /// 尝试获取节点上 Mesh 
    /// </summary>
    /// <param name="tr"></param>
    /// <returns></returns>
	private Mesh GetMesh(Transform tr)
	{
		var mr  = GetRenderer(tr);
		Mesh m  = null;

		if (mr != null && mr.enabled)
		{
			var t = mr.GetType();
			if (t == typeof(MeshRenderer))
			{
				MeshFilter mf = tr.GetComponent<MeshFilter>();
				if(!mf)
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
    /// 是否为半透明目标
    /// </summary>
    /// <param name="mat">材质</param>
    /// <param name="material">GLTF 材质描述</param>
    /// <returns>是否为半透明目标</returns>
    private bool handleTransparency(ref Material mat, ref GlTF_Material material)
	{
        /// _Mode 属性
		if (mat.HasProperty("_Mode") && mat.GetFloat("_Mode") != 0)
		{
			string mode         = mat.GetFloat("_Mode") == 1 ? "MASK" : "BLEND";

			GlTF_Material.StringValue alphaMode = new GlTF_Material.StringValue();
			alphaMode.name      = "alphaMode";
			alphaMode.value     = mode;

			material.values.Add(alphaMode);

            /// _Cutoff 属性
			if (mat.HasProperty("_Cutoff")){
				GlTF_Material.FloatValue alphaCutoff = new GlTF_Material.FloatValue();
				alphaCutoff.name    = "alphaCutoff";
				alphaCutoff.value   = mat.GetFloat("_Cutoff");
				material.values.Add(alphaCutoff);
			}

            if (mat.GetFloat("_Mode") == 1)
            {
                material.hasAlpha = true;
            }

			return true;
		}

		return false;
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="texture"></param>
    /// <param name="colors"></param>
    /// <param name="outputChannel"></param>
    /// <param name="inputChannel"></param>
	private void addTexturePixels(ref Texture2D texture, ref Color[] colors, IMAGETYPE outputChannel, IMAGETYPE inputChannel = IMAGETYPE.R)
	{
		int height = texture.height;
		int width = texture.width;
		Color[] inputColors = new Color[texture.width * texture.height];
		if (!texture || !getPixelsFromTexture(ref texture, out inputColors))
			return;

		if(height * width != colors.Length)
		{
			Debug.Log("Issue with texture dimensions");
			return;
		}

		if(inputChannel != IMAGETYPE.R && inputChannel != IMAGETYPE.A)
		{
			Debug.Log("Incorrect input channel (only 'R' and 'A' supported)");
		}

		for (int i = 0; i < height; ++i)
		{
			for (int j = 0; j < width; ++j)
			{
				int index = i * width + j;
				int newIndex = (height - i - 1) * width + j;
				Color c = outputChannel == IMAGETYPE.RGB ? inputColors[newIndex] : colors[index];
				float inputValue = inputChannel == IMAGETYPE.R ? inputColors[newIndex].r : inputColors[newIndex].a;

				if(outputChannel == IMAGETYPE.R)
				{
					c.r = inputValue;
				}
				else if(outputChannel == IMAGETYPE.G)
				{
					c.g = inputValue;
				}
				else if(outputChannel == IMAGETYPE.B)
				{
					c.b = inputValue;
				}
				else if(outputChannel == IMAGETYPE.G_INVERT)
				{
					c.g = 1.0f - inputValue;
				}

				colors[index] = c;
			}
		}

	}

    /// <summary>
    /// 创建咬合金属粗糙度纹理
    /// </summary>
    /// <param name="occlusion"></param>
    /// <param name="metallicRoughness"></param>
    /// <returns></returns>
	private int createOcclusionMetallicRoughnessTexture(ref Texture2D occlusion, ref Texture2D metallicRoughness)
	{
		string texName = "";
		int width = -1;
		int height = -1;
		string assetPath = "";
		if(occlusion)
		{
			texName = texName + GlTF_Texture.GetNameFromObject(occlusion);
			assetPath = AssetDatabase.GetAssetPath(occlusion);
			width = occlusion.width;
			height = occlusion.height;
		}
		else
		{
			texName = texName + "_";
		}

		if (metallicRoughness)
		{
			texName = texName + GlTF_Texture.GetNameFromObject(metallicRoughness);
			assetPath = AssetDatabase.GetAssetPath(metallicRoughness);
			width = metallicRoughness.width;
			height = metallicRoughness.height;
		}
		else
		{
			texName = texName + "_";
		}

		if (!GlTF_Writer.textureNames.Contains(texName))
		{
			// Create texture
			GlTF_Texture texture = new GlTF_Texture();
			texture.name = texName;

			// Export image
			GlTF_Image img = new GlTF_Image();
			img.name = texName;
			//img.uri =

			// Let's consider that the three textures have the same resolution
			Color[] outputColors = new Color[width * height];
			for (int i = 0; i < outputColors.Length; ++i)
				outputColors[i] = new Color(1.0f, 1.0f, 1.0f);

			if (occlusion)
				addTexturePixels(ref occlusion, ref outputColors, IMAGETYPE.R);
			if (metallicRoughness)
			{
				addTexturePixels(ref metallicRoughness, ref outputColors, IMAGETYPE.B);
				addTexturePixels(ref metallicRoughness, ref outputColors, IMAGETYPE.G_INVERT, IMAGETYPE.A);
			}

			Texture2D newtex = new Texture2D(width, height);
			newtex.SetPixels(outputColors);
			newtex.Apply();

			string pathInArchive = ""; //Path.GetDirectoryName(assetPath);
            string exportDir = Path.Combine(savedPath, pathInArchive);

			if (!Directory.Exists(exportDir))
				Directory.CreateDirectory(exportDir);

			string outputFilename = Path.GetFileNameWithoutExtension(assetPath) + "_converted_metalRoughness.jpg";
            string exportPath = exportDir + "/" + outputFilename;  // relative path inside the .zip
            File.WriteAllBytes(exportPath, newtex.EncodeToJPG(jpgQuality));

			if (!GlTF_Writer.exportedFiles.ContainsKey(exportPath))
				GlTF_Writer.exportedFiles.Add(exportPath, pathInArchive);
			else
				Debug.LogError("Texture '" + newtex.name + "' already exists");

			//img.uri = pathInArchive + "/" + outputFilename;
            img.uri = outputFilename;

            texture.source = GlTF_Writer.imageNames.Count;
			GlTF_Writer.imageNames.Add(img.name);
			GlTF_Writer.images.Add(img);

			// Add sampler
			GlTF_Sampler sampler;
			var samplerName = GlTF_Sampler.GetNameFromObject(metallicRoughness);
			if (!GlTF_Writer.samplerNames.Contains(samplerName))
			{
				sampler = new GlTF_Sampler(metallicRoughness);
				sampler.name = samplerName;
				GlTF_Writer.samplers.Add(sampler);
				GlTF_Writer.samplerNames.Add(samplerName);
			}

			GlTF_Writer.textures.Add(texture);
			GlTF_Writer.textureNames.Add(texName);
		}

		return GlTF_Writer.textureNames.IndexOf(texName);

	}

    /// <summary>
    /// Get or create texture object, image and sampler
    /// 获取或创建纹理对象，图像和采样器
    /// </summary>
    /// <param name="t"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    private int processTexture(Texture2D t, IMAGETYPE format)
	{
		var texName = GlTF_Texture.GetNameFromObject(t);
		if (AssetDatabase.GetAssetPath(t).Length == 0)
		{
			Debug.LogWarning("Texture " + t.name + " cannot be found in assets");
			return -1;
		}

		if (!GlTF_Writer.textureNames.Contains(texName))
		{
			string assetPath = AssetDatabase.GetAssetPath(t);

			// Create texture
			GlTF_Texture texture    = new GlTF_Texture();
			texture.name            = texName;

			// Export image
			GlTF_Image img  = new GlTF_Image();
			img.name        = GlTF_Image.GetNameFromObject(t);
			img.uri         = convertTexture(ref t, assetPath, savedPath, format);

			texture.source  = GlTF_Writer.imageNames.Count;

			GlTF_Writer.imageNames.Add(img.name);
			GlTF_Writer.images.Add(img);

			// Add sampler
			GlTF_Sampler sampler;
			var samplerName = GlTF_Sampler.GetNameFromObject(t);

			if (!GlTF_Writer.samplerNames.Contains (samplerName)) {

				sampler         = new GlTF_Sampler (t);
				sampler.name    = samplerName;

				GlTF_Writer.samplers.Add (sampler);
				GlTF_Writer.samplerNames.Add (samplerName);

				texture.samplerIndex = GlTF_Writer.samplers.Count - 1;

			} else {

				texture.samplerIndex = GlTF_Writer.samplerNames.IndexOf (samplerName);

			}

			GlTF_Writer.textures.Add(texture);
			GlTF_Writer.textureNames.Add(texName);
		}

		return GlTF_Writer.textureNames.IndexOf(texName);
    }

    private int processTextureForParticle(Texture2D t, IMAGETYPE format)
    {
        if (AssetDatabase.GetAssetPath(t).Length == 0)
        {
            Debug.LogWarning("Texture " + t.name + " cannot be found in assets");
            return -1;
        }

        var texSrcName = GlTF_Image.GetNameFromObject(t);
        var texParticleName = GlTF_Texture.GetNameFromObject(t) + "_particle";
        var samplerName = GlTF_Sampler.GetNameFromObject(t);

        if (!GlTF_Writer.imageNames.Contains(texSrcName))
        {
            string assetPath = AssetDatabase.GetAssetPath(t);

            // Export image
            GlTF_Image img = new GlTF_Image();
            img.name    = texSrcName;
            img.uri     = convertTexture(ref t, assetPath, savedPath, format);

            GlTF_Writer.images.Add(img);
            GlTF_Writer.imageNames.Add(img.name);
        }

        if (!GlTF_Writer.textureNames.Contains(texParticleName))
        {

            // Create texture
            GlTF_Texture texture = new GlTF_Texture();
            texture.name = texParticleName;
            texture.invertY = true;
            texture.source = GlTF_Writer.imageNames.IndexOf(texSrcName);

            // Add sampler
            GlTF_Sampler sampler;

            if (!GlTF_Writer.samplerNames.Contains(samplerName))
            {

                sampler = new GlTF_Sampler(t);
                sampler.name = samplerName;

                GlTF_Writer.samplers.Add(sampler);
                GlTF_Writer.samplerNames.Add(samplerName);

                texture.samplerIndex = GlTF_Writer.samplers.Count - 1;

            }
            else
            {

                texture.samplerIndex = GlTF_Writer.samplerNames.IndexOf(samplerName);

            }

            GlTF_Writer.textures.Add(texture);
            GlTF_Writer.textureNames.Add(texParticleName);
        }

        return GlTF_Writer.textureNames.IndexOf(texParticleName);
    }

    /// <summary>
    /// Get or create texture object, image and sampler
    /// 获取或创建纹理对象，图像和采样器
    /// </summary>
    /// <param name="t"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    private int processTextureAsLightmap(Texture2D t, ExportOption option, IMAGETYPE format)
    {

        const int JpegQuality = 100;
        const float LightmapBrightness = 8.0f;
        const float LightmapContrast = 1.1f;

        Color32[] c = t.GetPixels32();

        Texture2D tf = new Texture2D(t.width, t.height, TextureFormat.ARGB32, false);

        for (int j = 0; j < c.Length; j++)
        {
            // 非线性校正             
            float af = c[j].a / 255f;
            float rf = c[j].r / 255f;
            float gf = c[j].g / 255f;
            float bf = c[j].b / 255f;

            float ur = Mathf.Pow(rf * af, LightmapContrast) * 255f * LightmapBrightness;
            float ug = Mathf.Pow(gf * af, LightmapContrast) * 255f * LightmapBrightness;
            float ub = Mathf.Pow(bf * af, LightmapContrast) * 255f * LightmapBrightness;

            // 只导出 0 - option.mMaxLightmapMultiple 倍的亮度范围
            ur = Mathf.Clamp(ur / option.mMaxLightmapMultiple, 0, 255);
            ug = Mathf.Clamp(ug / option.mMaxLightmapMultiple, 0, 255);
            ub = Mathf.Clamp(ub / option.mMaxLightmapMultiple, 0, 255);

            c[j].r = Convert.ToByte(ur);
            c[j].g = Convert.ToByte(ug);
            c[j].b = Convert.ToByte(ub);
            c[j].a = 255;
        }

        tf.SetPixels32(c);

        var texName = GlTF_Texture.GetNameFromObject(t);
        if (AssetDatabase.GetAssetPath(t).Length == 0)
        {
            Debug.LogWarning("Texture " + t.name + " cannot be found in assets");
            return -1;
        }

        if (!GlTF_Writer.textureNames.Contains(texName))
        {
            string assetPath = AssetDatabase.GetAssetPath(t);

            // Create texture
            GlTF_Texture texture = new GlTF_Texture();
            texture.name = texName;

            // Export image
            GlTF_Image img = new GlTF_Image();
            img.name = GlTF_Image.GetNameFromObject(t);


            img.uri = convertTextureAsLightmap(ref tf, assetPath, savedPath, format);

            texture.source = GlTF_Writer.imageNames.Count;

            GlTF_Writer.imageNames.Add(img.name);
            GlTF_Writer.images.Add(img);

            // Add sampler
            GlTF_Sampler sampler;
            var samplerName = GlTF_Sampler.GetNameFromObject(tf);

            if (!GlTF_Writer.samplerNames.Contains(samplerName))
            {

                sampler = new GlTF_Sampler(tf);
                sampler.name = samplerName;

                GlTF_Writer.samplers.Add(sampler);
                GlTF_Writer.samplerNames.Add(samplerName);

                texture.samplerIndex = GlTF_Writer.samplers.Count - 1;

            }
            else
            {

                texture.samplerIndex = GlTF_Writer.samplerNames.IndexOf(samplerName);

            }

            GlTF_Writer.textures.Add(texture);
            GlTF_Writer.textureNames.Add(texName);
        }

        return GlTF_Writer.textureNames.IndexOf(texName);
    }

    /// <summary>
    /// Get or create texture object, image and sampler
    /// 获取或创建纹理对象，图像和采样器
    /// </summary>
    /// <param name="t"></param>
    /// <param name="matToRtpl"></param>
    /// <returns></returns>
    private int processTexture(Texture2D t,  bool matToRtpl)
	{
		var texName = GlTF_Texture.GetNameFromObject(t) + "PiRtpl";
		if (AssetDatabase.GetAssetPath(t).Length == 0)
		{
			Debug.LogWarning("Texture " + t.name + " cannot be found in assets");
			return -1;
		}

		// string assetPath = AssetDatabase.GetAssetPath(t);

		// Create texture
		GlTF_Texture texture    = new GlTF_Texture();
		texture.name            = texName;

		// Export image
		GlTF_Image img  = new GlTF_Image();
		img.name        = GlTF_Image.GetNameFromObject(t)+ "PiRtpl";

		if (matToRtpl) {
			img.uri = "{{it.img}}";
		} 

		texture.source = GlTF_Writer.imageNames.Count;

		GlTF_Writer.imageNames.Add(img.name);
		GlTF_Writer.images.Add(img);

		// Add sampler
		GlTF_Sampler sampler;
		var samplerName = GlTF_Sampler.GetNameFromObject(t) + "PiRtpl";

		if (!GlTF_Writer.samplerNames.Contains(samplerName))
		{
			sampler         = new GlTF_Sampler(t);
			sampler.name    = samplerName;

			GlTF_Writer.samplers.Add(sampler);
			GlTF_Writer.samplerNames.Add(samplerName);
		}

		GlTF_Writer.textures.Add(texture);
		GlTF_Writer.textureNames.Add(texName);

		return GlTF_Writer.textureNames.IndexOf(texName);
	}

    /// <summary>
    /// Get or create texture object, image and sampler
    /// 获取或创建纹理对象，图像和采样器
    /// </summary>
    /// <param name="t"></param>
    /// <param name="matToRtpl"></param>
    /// <returns></returns>
    private int processTextureAsLightmap(Texture2D t, bool matToRtpl)
    {
        var texName = GlTF_Texture.GetNameFromObject(t) + "PiRtpl";
        if (AssetDatabase.GetAssetPath(t).Length == 0)
        {
            Debug.LogWarning("Texture " + t.name + " cannot be found in assets");
            return -1;
        }

        // string assetPath = AssetDatabase.GetAssetPath(t);

        // Create texture
        GlTF_Texture texture = new GlTF_Texture();
        texture.name = texName;

        // Export image
        GlTF_Image img = new GlTF_Image();
        img.name = GlTF_Image.GetNameFromObject(t) + "PiRtpl";

        if (matToRtpl)
        {
            img.uri = "{{it.img}}";
        }

        texture.source = GlTF_Writer.imageNames.Count;

        GlTF_Writer.imageNames.Add(img.name);
        GlTF_Writer.images.Add(img);

        // Add sampler
        GlTF_Sampler sampler;
        var samplerName = GlTF_Sampler.GetNameFromObject(t) + "PiRtpl";

        if (!GlTF_Writer.samplerNames.Contains(samplerName))
        {
            sampler = new GlTF_Sampler(t);
            sampler.name = samplerName;

            GlTF_Writer.samplers.Add(sampler);
            GlTF_Writer.samplerNames.Add(samplerName);
        }

        GlTF_Writer.textures.Add(texture);
        GlTF_Writer.textureNames.Add(texName);

        return GlTF_Writer.textureNames.IndexOf(texName);
    }

    /// <summary>
    /// unity 材质 转换 PiMaterial
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="material"></param>
	private void unityToPiMaterial(Material mat, Renderer renderer, ExportOption option, ref GlTF_Material material)
    {
        Debug.Log("unityToPiMaterial begin =============");
        //Debug.Log("" + mat.HasProperty("_UseAlphaFromDiffuseTexture"));
        //Debug.Log("" + mat.HasProperty("_DiffuseTextureHasAlpha"));
        //Debug.Log("" + mat.GetInt("_UseAlphaFromDiffuseTexture"));
        //Debug.Log("" + mat.GetInt("_DiffuseTextureHasAlpha"));

        Debug.Log("" + mat.mainTextureScale.x);

        //Check transparency
        //bool hasTransparency = handleTransparency(ref mat, ref material);
        //cull
        material.shaderName = mat.shader.name;
        if (mat.shader.name == "PiShader" || mat.shader.name.IndexOf("Pi/") >= 0 || mat.shader.name.IndexOf("Mobile/") >= 0)
        {
            if (
                mat.shader.name == "Pi/Shadow"
                || mat.shader.name == "Pi/TransparentUseLight"
            )
            {
                material.disableLighting = false;
            }
            material.shaderName = "PiShader";
        }
        if (mat.shader.name == "Pi/Particle" || mat.shader.name == "Myshader/Particle" || mat.shader.name == "Pi/Myshader/Particle")
        {
            material.shaderName = "Myshader/Particle";
        }

        if (mat.HasProperty("_Cull") && mat.GetFloat("_Cull") != 2)
        {
            material.cull = mat.GetFloat("_Cull") == 1 ? "front" : "off";
        }

        if (mat.HasProperty("_Hue"))
        {
            //hue,s,b
            material.HSB[0] = mat.GetFloat("_Hue");
            material.HSB[1] = mat.GetFloat("_Saturation");
            material.HSB[2] = mat.GetFloat("_Brightness");
        }

        if (mat.HasProperty("_SrcBlend"))
        {
            material.srcBlend = mat.GetFloat("_SrcBlend");
        }

        if (mat.HasProperty("_DstBlend"))
        {
            material.dstBlend = mat.GetFloat("_DstBlend");
        }

        //useAlphaFromDiffuseTexture
        if (mat.HasProperty("_UseAlphaFromDiffuseTexture"))
        {
            material.useAlphaFromDiffuseTexture = mat.GetInt("_UseAlphaFromDiffuseTexture") == 1 ? true : false;
        }
        //useAlphaFromDiffuseTexture
        if (mat.shader.name == "Pi/TransparentUseLight")
        {
            material.useAlphaFromDiffuseTexture = true;
        }

        if (material.useAlphaFromDiffuseTexture)
        {
            material.hasAlpha = true;
        }

        //Parse diffuse channel texture and color
        if (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)
        {
            var textureValue = new GlTF_Material.DictValue();
            textureValue.name = "diffuseTexture";

            int diffuseTextureIndex = -1;
            if (material.matToRtpl)
            {
                diffuseTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), true);
            }
            else
            {
                diffuseTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), IMAGETYPE.RGBA_OPAQUE);
            }


            //TODO 导出rtpl要改这个index
            textureValue.intValue.Add("index", diffuseTextureIndex);
            textureValue.intValue.Add("texCoord", 0);
            if (
                mat.HasProperty("_Mode") 
                || mat.HasProperty("_UseAlphaFromDiffuseTexture")
                || (mat.shader.name == "Pi/TransparentUseLight")
            )
            {
                if (
                    (mat.GetFloat("_Mode") == 1 && mat.HasProperty("_Mode")) 
                    || (mat.HasProperty("_UseAlphaFromDiffuseTexture") && mat.GetInt("_UseAlphaFromDiffuseTexture") == 1)
                    || (mat.shader.name == "Pi/TransparentUseLight")
                )
                {
                    textureValue.stringValue.Add("hasAlpha", "true");
                }
            }

            Vector2 offset = mat.GetTextureOffset("_MainTex");
            if (offset != null && offset.x != 0 && offset.y != 0)
            {
                textureValue.stringValue.Add("offset", "[" + offset.x + "," + offset.y + "]");
            }

            Vector2 scale = mat.GetTextureScale("_MainTex");
            if (scale != null && scale.x != 0 && scale.y != 0 && scale.x != 1 && scale.y != 1)
            {
                textureValue.stringValue.Add("scale", "[" + scale.x + "," + scale.y + "]");
            }

            material.pbrValues.Add(textureValue);
        }

        if (mat.HasProperty("_TintColor"))
        {
            var colorValue = new GlTF_Material.ColorValue();
            colorValue.name = "diffuseColor";
            Color c = mat.GetColor("_TintColor");
            clampColor(ref c);
            colorValue.color = c;
            material.pbrValues.Add(colorValue);
            material.diffuseColor = c;
            material.alpha = c.a;
        }

        //Parse lightmap channel texture
        //if (mat.HasProperty("_BakeLightMapTex") && mat.GetTexture("_BakeLightMapTex") != null)
        //{

        if (renderer.lightmapIndex >= 0 && renderer.lightmapIndex != 255 && LightmapSettings.lightmaps.Length > renderer.lightmapIndex)
        {
            var textureValue = new GlTF_Material.DictValue();
            textureValue.name = "lightmapTexture";

            var lightmap = LightmapSettings.lightmaps[renderer.lightmapIndex];

            int lightmapTextureIndex = -1;
            if (material.matToRtpl)
            {
                lightmapTextureIndex = processTextureAsLightmap(lightmap.lightmapColor, true);
            }
            else
            {
                lightmapTextureIndex = processTextureAsLightmap(lightmap.lightmapColor, option, IMAGETYPE.RGBA_OPAQUE);
            }
            

            Vector4 scaleOffset = renderer.lightmapScaleOffset; 
            if (scaleOffset != null && scaleOffset.x != 0 && scaleOffset.y != 0 && scaleOffset.x != 1 && scaleOffset.y != 1)
            {
                textureValue.stringValue.Add("scale", "[" + scaleOffset.x + "," + scaleOffset.y + "]");
            }
            
            if (scaleOffset != null && scaleOffset.z != 0 && scaleOffset.w != 0)
            {
                textureValue.stringValue.Add("offset", "[" + scaleOffset.z + "," + scaleOffset.w + "]");
            }

            //TODO 导出rtpl要改这个index
            textureValue.intValue.Add("index", lightmapTextureIndex);
            textureValue.intValue.Add("texCoord", 1);
            // 光照图强度
            textureValue.floatValue.Add("level", option.mMaxLightmapMultiple);
            material.pbrValues.Add(textureValue);
        }
        //}

        ////Parse lightmap channel texture
        //if (mat.HasProperty("_CustomLightMapTex") && mat.GetTexture("_CustomLightMapTex") != null && mat.HasProperty("_UseCustomLightMap") && mat.GetInt("_UseCustomLightMap") == 1)
        //{
        //    var textureValue = new GlTF_Material.DictValue();
        //    textureValue.name = "lightmapTexture";
        //
        //    int lightmapTextureIndex = -1;
        //    if (material.matToRtpl)
        //    {
        //        lightmapTextureIndex = processTextureAsLightmap((Texture2D)mat.GetTexture("_CustomLightMapTex"), true);
        //    }
        //    else
        //    {
        //        lightmapTextureIndex = processTextureAsLightmap((Texture2D)mat.GetTexture("_CustomLightMapTex"), IMAGETYPE.RGBA_OPAQUE);
        //    }
        //
        //    Vector2 offset = mat.GetTextureOffset("_CustomLightMapTex");
        //    if (offset != null && offset.x != 0 && offset.y != 0)
        //    {
        //        textureValue.stringValue.Add("offset", "[" + offset.x + "," + offset.y + "]");
        //    }
        //
        //    Vector2 scale = mat.GetTextureScale("_CustomLightMapTex");
        //    if (scale != null && scale.x != 0 && scale.y != 0)
        //    {
        //        textureValue.stringValue.Add("scale", "[" + scale.x + "," + scale.y + "]");
        //    }
        //
        //    //TODO 导出rtpl要改这个index
        //    textureValue.intValue.Add("index", lightmapTextureIndex);
        //    textureValue.intValue.Add("texCoord", 1);
        //    material.pbrValues.Add(textureValue);
        //}

        material.renderQueue = mat.renderQueue;

        if (mat.HasProperty("_UseLightmapAsShadowmap"))
        {
            material.useLightmapAsShadowmap = mat.GetInt("_UseLightmapAsShadowmap") == 1;
        }
    }

    private void unityToPiMaterial_PBRMetallicRoughness(Material mat, Renderer renderer, ExportOption option, ref GlTF_Material material)
    {

        Debug.Log("unityToPiMaterial_PBRMetallicRoughness begin =============");

        Debug.Log("" + mat.mainTextureScale.x);
        
        material.shaderName = "PBRMetallicRoughness";

        if (mat.HasProperty("_Cull") && mat.GetFloat("_Cull") != 2)
        {
            material.cull = mat.GetFloat("_Cull") == 1 ? "front" : "off";
        }
        
        if (mat.HasProperty("_Metallic"))
        {
            material.metailValue = mat.GetFloat("_Metallic");
        }

        if (mat.HasProperty("_Glossiness"))
        {
            material.metailRoughnessValue = mat.GetFloat("_Glossiness");
        }

        // Parse diffuse channel texture and color
        if (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)
        {
            var textureValue = new GlTF_Material.DictValue();
            textureValue.name = "baseTexture";

            int albedoTextureIndex = -1;
            if (material.matToRtpl)
            {
                albedoTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), true);
            }
            else
            {
                albedoTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), IMAGETYPE.RGBA_OPAQUE);
            }


            // TODO 导出rtpl要改这个index
            textureValue.intValue.Add("index", albedoTextureIndex);
            textureValue.intValue.Add("texCoord", 0);

            Vector2 offset = mat.GetTextureOffset("_MainTex");
            if (offset != null && offset.x != 0 && offset.y != 0)
            {
                textureValue.stringValue.Add("offset", "[" + offset.x + "," + offset.y + "]");
            }

            Vector2 scale = mat.GetTextureScale("_MainTex");
            if (scale != null && scale.x != 0 && scale.y != 0 && scale.x != 1 && scale.y != 1)
            {
                textureValue.stringValue.Add("scale", "[" + scale.x + "," + scale.y + "]");
            }

            material.pbrValues.Add(textureValue);
        }

        if (mat.HasProperty("_Color"))
        {
            var colorValue  = new GlTF_Material.ColorValue();
            colorValue.name = "baseColor";

            Color c = mat.GetColor("_Color");
            clampColor(ref c);
            colorValue.color = c;
            material.pbrValues.Add(colorValue);

            material.metailBaseColor    = c;
        }

        material.renderQueue = mat.renderQueue;
    }

    private void unityToPiMaterial_PBRSpecularGlossiness(Material mat, Renderer renderer, ExportOption option, ref GlTF_Material material)
    {

        Debug.Log("unityToPiMaterial_PBRSpecularGlossiness begin =============");

        Debug.Log("" + mat.mainTextureScale.x);

        material.shaderName = "PBRSpecularGlossiness";

        if (mat.HasProperty("_Cull") && mat.GetFloat("_Cull") != 2)
        {
            material.cull = mat.GetFloat("_Cull") == 1 ? "front" : "off";
        }

        if (mat.HasProperty("_Metallic"))
        {
            material.pbrMetailValue = mat.GetFloat("_Metallic");
        }

        if (mat.HasProperty("_Glossiness"))
        {
            material.pbrRoughnessValue = mat.GetFloat("_Glossiness");
        }

        // Parse diffuse channel texture and color
        if (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)
        {
            var textureValue = new GlTF_Material.DictValue();
            textureValue.name = "diffuseTexture";

            int albedoTextureIndex = -1;
            if (material.matToRtpl)
            {
                albedoTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), true);
            }
            else
            {
                albedoTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), IMAGETYPE.RGBA_OPAQUE);
            }


            // TODO 导出rtpl要改这个index
            textureValue.intValue.Add("index", albedoTextureIndex);
            textureValue.intValue.Add("texCoord", 0);

            Vector2 offset = mat.GetTextureOffset("_MainTex");
            if (offset != null && offset.x != 0 && offset.y != 0)
            {
                textureValue.stringValue.Add("offset", "[" + offset.x + "," + offset.y + "]");
            }

            Vector2 scale = mat.GetTextureScale("_MainTex");
            if (scale != null && scale.x != 0 && scale.y != 0 && scale.x != 1 && scale.y != 1)
            {
                textureValue.stringValue.Add("scale", "[" + scale.x + "," + scale.y + "]");
            }

            material.pbrValues.Add(textureValue);
        }

        if (mat.HasProperty("_Color"))
        {
            var colorValue = new GlTF_Material.ColorValue();
            colorValue.name = "diffuseColor";

            Color c = mat.GetColor("_Color");
            clampColor(ref c);
            colorValue.color = c;
            material.pbrValues.Add(colorValue);

            material.specularDiffuseColor = c;
        }

        if (mat.HasProperty("_SpecColor"))
        {
            var colorValue = new GlTF_Material.ColorValue();
            colorValue.name = "specularColor";

            Color c = mat.GetColor("_SpecColor");
            clampColor(ref c);
            colorValue.color = c;
            material.pbrValues.Add(colorValue);

            material.specularColor = c;
        }

        material.renderQueue = mat.renderQueue;
    }

    private void unityToPiMaterial_PBR(Material mat, Renderer renderer, ExportOption option, ref GlTF_Material material)
    {

        Debug.Log("unityToPiMaterial_PBR begin =============");

        Debug.Log("" + mat.mainTextureScale.x);

        material.shaderName = "PBR";

        if (mat.HasProperty("_Cull") && mat.GetFloat("_Cull") != 2)
        {
            material.cull = mat.GetFloat("_Cull") == 1 ? "front" : "off";
        }

        if (mat.HasProperty("_Metallic"))
        {
            material.pbrMetailValue = mat.GetFloat("_Metallic");
        }

        if (mat.HasProperty("_Glossiness"))
        {
            material.pbrRoughnessValue = mat.GetFloat("_Glossiness");
        }

        // Parse diffuse channel texture and color
        if (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)
        {
            var textureValue = new GlTF_Material.DictValue();
            textureValue.name = "albedoTexture";

            int albedoTextureIndex = -1;
            if (material.matToRtpl)
            {
                albedoTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), true);
            }
            else
            {
                albedoTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), IMAGETYPE.RGBA_OPAQUE);
            }


            // TODO 导出rtpl要改这个index
            textureValue.intValue.Add("index", albedoTextureIndex);
            textureValue.intValue.Add("texCoord", 0);

            Vector2 offset = mat.GetTextureOffset("_MainTex");
            if (offset != null && offset.x != 0 && offset.y != 0)
            {
                textureValue.stringValue.Add("offset", "[" + offset.x + "," + offset.y + "]");
            }

            Vector2 scale = mat.GetTextureScale("_MainTex");
            if (scale != null && scale.x != 0 && scale.y != 0 && scale.x != 1 && scale.y != 1)
            {
                textureValue.stringValue.Add("scale", "[" + scale.x + "," + scale.y + "]");
            }

            material.pbrValues.Add(textureValue);
        }

        if (mat.HasProperty("_ReflectionColor"))
        {
            var colorValue = new GlTF_Material.ColorValue();
            colorValue.name = "reflectivityColor";

            Color c = mat.GetColor("_ReflectionColor");
            clampColor(ref c);
            colorValue.color = c;
            material.pbrValues.Add(colorValue);

            material.pbrReflectionColor = c;
        }

        if (mat.HasProperty("_Color"))
        {
            var colorValue = new GlTF_Material.ColorValue();
            colorValue.name = "albedoColor";

            Color c = mat.GetColor("_Color");
            clampColor(ref c);
            colorValue.color = c;
            material.pbrValues.Add(colorValue);

            material.pbrAlbedoColor = c;
        }

        material.renderQueue = mat.renderQueue;
    }

    /// <summary>
    /// Unity 材质 转换 glTF PBR
    /// </summary>
    /// <param name="mat"></param>
    /// <param name="material"></param>
    private void unityToPBRMaterial(Material mat, ref GlTF_Material material)
	{
		bool isMaterialPBR  = true;
		bool isMetal        = true;
		bool hasPBRMap      = false;

		if (!mat.shader.name.Contains("Standard"))
		{
			Debug.Log("Material " + mat.shader + " is not fully supported");
			isMaterialPBR = false;
		}
		else
		{
			// Is metal workflow used
			isMetal             = mat.shader.name == "Standard";
            material.isMetal    = isMetal;

            GlTF_Writer.hasSpecularMaterials    = GlTF_Writer.hasSpecularMaterials || !isMetal;

			// Is smoothness defined by diffuse texture or PBR texture' alpha?
			if (mat.GetFloat("_SmoothnessTextureChannel") != 0)
				Debug.Log("Smoothness uses diffuse's alpha channel. Unsupported for now");

			hasPBRMap = (!isMetal && mat.GetTexture("_SpecGlossMap") != null || isMetal && mat.GetTexture("_MetallicGlossMap") != null);
		}

		//Check transparency
		bool hasTransparency = handleTransparency(ref mat, ref material);

		//Parse diffuse channel texture and color
		if (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)
		{
			var textureValue        = new GlTF_Material.DictValue();
			textureValue.name       = isMetal ? "baseColorTexture" : "diffuseTexture";

			int diffuseTextureIndex = processTexture((Texture2D)mat.GetTexture("_MainTex"), hasTransparency ? IMAGETYPE.RGBA : IMAGETYPE.RGBA_OPAQUE);

			textureValue.intValue.Add("index", diffuseTextureIndex);
			textureValue.intValue.Add("texCoord", 0);

			material.pbrValues.Add(textureValue);
		}

		if (mat.HasProperty("_Color"))
		{
			var colorValue          = new GlTF_Material.ColorValue();
			colorValue.name         = isMetal ? "baseColorFactor" : "diffuseFactor";

			Color c                 = mat.GetColor("_Color");

			clampColor(ref c);
			colorValue.color        = c;

			material.pbrValues.Add(colorValue);
		}

		//Parse PBR textures
		if (isMaterialPBR)
		{
			if (isMetal)
			{
				if (hasPBRMap) // No metallic factor if texture
				{
					var textureValue    = new GlTF_Material.DictValue();
					textureValue.name   = "metallicRoughnessTexture";

					Texture2D metallicRoughnessTexture = (Texture2D)mat.GetTexture("_MetallicGlossMap");
					Texture2D occlusion = (Texture2D)mat.GetTexture("_OcclusionMap");

					int metalRoughTextureIndex = createOcclusionMetallicRoughnessTexture (ref occlusion, ref metallicRoughnessTexture);

					textureValue.intValue.Add("index", metalRoughTextureIndex);
					textureValue.intValue.Add("texCoord", 0);

					material.pbrValues.Add(textureValue);
				}

				var metallicFactor      = new GlTF_Material.FloatValue();
				metallicFactor.name     = "metallicFactor";
				metallicFactor.value    = hasPBRMap ? 1.0f : mat.GetFloat("_Metallic");

				material.pbrValues.Add(metallicFactor);

				//Roughness factor
				var roughnessFactor     = new GlTF_Material.FloatValue();
				roughnessFactor.name    = "roughnessFactor";
				roughnessFactor.value   = hasPBRMap ? 1.0f : 1 - mat.GetFloat("_Glossiness"); // gloss scale is not supported for now(property _GlossMapScale)

				material.pbrValues.Add(roughnessFactor);
			}
			else
			{
				if (hasPBRMap) // No metallic factor if texture
				{
					var textureValue    = new GlTF_Material.DictValue();
					textureValue.name   = "specularGlossinessTexture";

					int specGlossTextureIndex = processTexture((Texture2D)mat.GetTexture("_SpecGlossMap"), IMAGETYPE.RGBA);

					textureValue.intValue.Add("index", specGlossTextureIndex);
					textureValue.intValue.Add("texCoord", 0);

					material.pbrValues.Add(textureValue);
				}

				var specularFactor      = new GlTF_Material.ColorValue();
				specularFactor.name     = "specularFactor";
				specularFactor.color    = hasPBRMap ? Color.white : mat.GetColor("_SpecColor"); // gloss scale is not supported for now(property _GlossMapScale)
				specularFactor.isRGB    = true;

				material.pbrValues.Add(specularFactor);

				var glossinessFactor    = new GlTF_Material.FloatValue();
				glossinessFactor.name   = "glossinessFactor";
				glossinessFactor.value  = hasPBRMap ? 1.0f : mat.GetFloat("_Glossiness"); // gloss scale is not supported for now(property _GlossMapScale)

                material.pbrValues.Add(glossinessFactor);
			}
		}

		//BumpMap
		if (mat.HasProperty("_BumpMap") && mat.GetTexture("_BumpMap") != null)
		{
			Texture2D bumpTexture = mat.GetTexture("_BumpMap") as Texture2D;
			// Check if it's a normal or a bump map
			TextureImporter im  = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(bumpTexture)) as TextureImporter;
			bool isBumpMap      = im.convertToNormalmap;

			if(isBumpMap)
			{
				Debug.LogWarning("Unsupported texture " + bumpTexture + " (normal maps generated from grayscale are not supported)");
			}
			else
			{
				var textureValue        = new GlTF_Material.DictValue();
				textureValue.name       = "normalTexture";

				int bumpTextureIndex    = processTexture(bumpTexture, IMAGETYPE.NORMAL_MAP);

				textureValue.intValue.Add("index", bumpTextureIndex);
				textureValue.intValue.Add("texCoord", 0);
				textureValue.floatValue.Add("scale", mat.GetFloat("_BumpScale"));

				material.values.Add(textureValue);
			}
		}

		//Emissive
		if (mat.HasProperty("_EmissionMap") && mat.GetTexture("_EmissionMap") != null)
		{
			Texture2D emissiveTexture = mat.GetTexture("_EmissionMap") as Texture2D;

			var textureValue    = new GlTF_Material.DictValue();
			textureValue.name   = "emissiveTexture";

			int emissiveTextureIndex = processTexture(emissiveTexture, IMAGETYPE.RGB);

			textureValue.intValue.Add("index", emissiveTextureIndex);
			textureValue.intValue.Add("texCoord", 0);

			material.values.Add(textureValue);
		}

		var emissiveFactor      = new GlTF_Material.ColorValue();
		emissiveFactor.name     = "emissiveFactor";
		emissiveFactor.isRGB    = true;
		emissiveFactor.color    = mat.GetColor("_EmissionColor");
		material.values.Add(emissiveFactor);

		//Occlusion (kept as separated channel for specular workflow, but merged in R channel for metallic workflow)
		if (mat.HasProperty("_OcclusionMap") && mat.GetTexture("_OcclusionMap") != null)
		{
			Texture2D occlusionTexture = mat.GetTexture("_OcclusionMap") as Texture2D;

			var textureValue    = new GlTF_Material.DictValue();
			textureValue.name   = "occlusionTexture";

			int occlusionTextureIndex   = processTexture(occlusionTexture, IMAGETYPE.RGB);

			textureValue.intValue.Add("index", occlusionTextureIndex);
			textureValue.intValue.Add("texCoord", 0);
			textureValue.floatValue.Add("strength", mat.GetFloat("_OcclusionStrength"));

			material.values.Add(textureValue);
		}

		// Unity materials are single sided by default Unity材质默认为单面
		GlTF_Material.BoolValue doubleSided = new GlTF_Material.BoolValue();
		doubleSided.name        = "doubleSided";
		doubleSided.value       = false;

		material.values.Add(doubleSided);
	}

	private bool getPixelsFromTexture(ref Texture2D texture, out Color[] pixels)
	{
        string path = AssetDatabase.GetAssetPath(texture);
        //Make texture readable
        TextureImporter im = AssetImporter.GetAtPath(path) as TextureImporter;
		if(!im)
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

	// Flip all images on Y and
	private string convertTextureAsLightmap(ref Texture2D inputTexture, string pathInProject, string exportDirectory, IMAGETYPE format)
	{
		int height = inputTexture.height;
		int width = inputTexture.width;
		Color[] textureColors = new Color[inputTexture.height * inputTexture.width];
        //if(!getPixelsFromTexture(ref inputTexture, out textureColors))
        //{
        //	Debug.Log("Failed to convert texture " + inputTexture.name + " (unsupported type or format)");
        //	return "";
        //}

        Color[] newTextureColors = new Color[inputTexture.height * inputTexture.width];

        Color[] c = inputTexture.GetPixels();

        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                newTextureColors[i * width + j] = c[(height - i - 1) * width + j];
                //if (format == IMAGETYPE.RGBA_OPAQUE)
                //newTextureColors[i * width + j].a = 1.0f;
            }
        }
        
		string pi_name = GlTF_Md5.getMd5(newTextureColors);
		Texture2D newtex = new Texture2D(inputTexture.width, inputTexture.height);
		newtex.SetPixels(newTextureColors);
		newtex.Apply();

		string pathInArchive = ""; //Path.GetDirectoryName(pathInProject);
        string exportDir = Path.Combine(exportDirectory, pathInArchive);

		if (!Directory.Exists(exportDir))
			Directory.CreateDirectory(exportDir);
		bool imgFormat = false;
		for(int i = 0;i < newTextureColors.Length; i++){
			if(newTextureColors[i].a < 1){
				imgFormat = true;
				break;
			}
		}
		string outputFilename = pi_name + (imgFormat? ".png" : ".jpg");
		string exportPath = exportDir + "/" + outputFilename;  // relative path inside the .zip
		//string pathInGltfFile = pathInArchive + "/" + outputFilename;
        string pathInGltfFile = outputFilename;
        File.WriteAllBytes(exportPath, (imgFormat ? newtex.EncodeToPNG() : newtex.EncodeToJPG(jpgQuality)));

		if (!GlTF_Writer.exportedFiles.ContainsKey(exportPath))
			GlTF_Writer.exportedFiles.Add(exportPath, pathInArchive);
		else
			Debug.LogError("Texture '" + inputTexture + "' already exists");

		return pathInGltfFile;
    }

    // Flip all images on Y and
    private string convertTexture(ref Texture2D inputTexture, string pathInProject, string exportDirectory, IMAGETYPE format)
    {
        int height = inputTexture.height;
        int width = inputTexture.width;
        Color[] textureColors = new Color[inputTexture.height * inputTexture.width];
        if (!getPixelsFromTexture(ref inputTexture, out textureColors))
        {
            Debug.Log("Failed to convert texture " + inputTexture.name + " (unsupported type or format)");
            return "";
        }
        Color[] newTextureColors = new Color[inputTexture.height * inputTexture.width];

        for (int i = 0; i < height; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                newTextureColors[i * width + j] = textureColors[(height - i - 1) * width + j];
                //if (format == IMAGETYPE.RGBA_OPAQUE)
                //newTextureColors[i * width + j].a = 1.0f;
            }
        }
        string pi_name = GlTF_Md5.getMd5(newTextureColors);
        Texture2D newtex = new Texture2D(inputTexture.width, inputTexture.height);
        newtex.SetPixels(newTextureColors);
        newtex.Apply();

        string pathInArchive = ""; //Path.GetDirectoryName(pathInProject);
        string exportDir = Path.Combine(exportDirectory, pathInArchive);

        if (!Directory.Exists(exportDir))
            Directory.CreateDirectory(exportDir);

        bool imgFormat = false;
        for (int i = 0; i < newTextureColors.Length; i++)
        {
            if (newTextureColors[i].a < 1)
            {
                imgFormat = true;
                break;
            }
        }

        string outputFilename = pi_name + (imgFormat ? ".png" : ".jpg");
        string exportPath = exportDir + "/" + outputFilename;  // relative path inside the .zip
        //string pathInGltfFile = pathInArchive + "/" + outputFilename;
        string pathInGltfFile = outputFilename;
        File.WriteAllBytes(exportPath, (imgFormat ? newtex.EncodeToPNG() : newtex.EncodeToJPG(jpgQuality)));

        if (!GlTF_Writer.exportedFiles.ContainsKey(exportPath))
            GlTF_Writer.exportedFiles.Add(exportPath, pathInArchive);
        else
            Debug.LogError("Texture '" + inputTexture + "' already exists");

        return pathInGltfFile;
    }

    /// <summary>
    /// 反转纹理
    /// </summary>
    /// <param name="inputTexture"></param>
    /// <param name="pathInProject"></param>
    /// <param name="exportDirectory"></param>
    /// <param name="name"></param>
    /// <returns></returns>
	public string convertTexture( Texture2D inputTexture, string pathInProject,string exportDirectory , string name)
	{
		int height = inputTexture.height;
		int width = inputTexture.width;
		Color[] textureColors = new Color[inputTexture.height * inputTexture.width];
		if(!getPixelsFromTexture(ref inputTexture, out textureColors))
		{
			Debug.Log("Failed to convert texture " + inputTexture.name + " (unsupported type or format)");
			return "";
		}
		Color[] newTextureColors = new Color[inputTexture.height * inputTexture.width];

		for (int i = 0; i < height; ++i)
		{
			for (int j = 0; j < width; ++j)
			{
				newTextureColors[i * width + j] = textureColors[(height - i - 1) * width + j];
			}
		}

		Texture2D newtex = new Texture2D(inputTexture.width, inputTexture.height);

		newtex.SetPixels(newTextureColors);
		newtex.Apply();

		string pathInArchive = ""; //Path.GetDirectoryName(pathInProject);
        string exportDir = exportDirectory + "/skybox";

		if (!Directory.Exists(exportDir))
			Directory.CreateDirectory(exportDir);

		
		string outputFilename ="sky_" + name + ".jpg";

		string exportPath = exportDir + "/" + outputFilename;  // relative path inside the .zip
		//string pathInGltfFile = pathInArchive + "/" + outputFilename;
        string pathInGltfFile = outputFilename;
        File.WriteAllBytes(exportPath,  newtex.EncodeToJPG(jpgQuality));

		if (!GlTF_Writer.exportedFiles.ContainsKey(exportPath))
			GlTF_Writer.exportedFiles.Add(exportPath, pathInArchive);
		else
			Debug.LogError("Texture '" + inputTexture + "' already exists");

		return pathInGltfFile;
	}

    public void ExportCoroutine(ExportOption exportOption)
    {
        StartCoroutine(Export2(exportOption));
    }

    public void ExportCoroutine(ExportOption exportOption, Transform[] rootNodes)
    {
        StartCoroutine(Export2(exportOption, rootNodes));
    }
}
#endif