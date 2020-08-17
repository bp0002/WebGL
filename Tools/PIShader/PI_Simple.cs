using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Assets.shader;

namespace UnityEditor
{
	internal class PI_SimpleGUI : ShaderGUI
	{
	    public enum BlendMode
	    {
	        Opaque,
	        Cutout,
	        Transparent // Physically plausible transparency mode, implemented as alpha pre-multiply
	    }

		public BlendMode renderingType = 0;
        private Material material = null;

	    MaterialProperty blendMode = null;
	    MaterialEditor m_MaterialEditor;
		bool m_FirstTimeApply = true;
		MaterialProperty albedoMap = null;
		MaterialProperty albedoColor = null;
		MaterialProperty Hue = null;
		MaterialProperty Saturation = null;
		MaterialProperty Brightness = null;
		MaterialProperty Cull = null;
        MaterialProperty SrcBlend = null;
        MaterialProperty DstBlend = null;

        int RenderIndex = 0;

        MaterialProperty CustomLightMapTex = null;

        MaterialProperty LightMapStrength = null;

        Texture2D BakeLightMapTex   = null;
        Vector4 BakeLightMapTexST   = new Vector4(0, 0, 0, 0);
        bool UseBakeLightMap        = false;
        bool UseCustomLightMap      = false;

        bool UseAlphaFromDiffuseTexture = false;
        bool DiffuseTextureHasAlpha = false;
        bool UseLightmapAsShadowmap = false;

        bool IgnoreAreaLight        = true;
        bool IgnoreDirectionalLight = true;
        bool IgnorePointLight       = true;
        bool IgnoreSpotLight        = true;


        private static class Styles
		{
			public static GUIContent albedoText = new GUIContent("Albedo", "Albedo (RGB) and Transparency (A)");

			public static string renderingMode = "Rendering Mode";
			public static readonly string[] blendNames = Enum.GetNames(typeof(BlendMode));
		}

        private static List<LightmapTexRecord> lightMapList = new List<LightmapTexRecord>();
        private static int targetHashCode = 0;
        private static bool asTargetTex(LightmapTexRecord tg)
        {
            return tg.hash == targetHashCode;
        }


        public int index = 0;

        private Transform[] temp;
        private int tc;

        /// <summary>
        /// Transform排序，深度遍历
        /// </summary>
        /// <param name="trs"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private Transform[] sortTrs(Transform[] trs, int c)
        {
            temp = new Transform[c];
            tc = 0;

            for (var i = 0; i < trs.Length; i++)
            {
                temp[tc] = trs[i];

                tc++;

                if (trs[i].childCount > 0)
                {
                    sortTrs(trs[i]);
                }
            }

            return temp;
        }

        /// <summary>
        /// Transform 深度遍历
        /// </summary>
        /// <param name="trs"></param>
        private void sortTrs(Transform trs)
        {
            for (var i = 0; i < trs.childCount; i++)
            {
                Transform t = trs.GetChild(i);
                temp[tc] = t;

                tc++;

                if (t.childCount > 0)
                {
                    sortTrs(t);
                }
            }
        }

        /// <summary>
        /// 收集场景中的 Transform 并排序
        /// </summary>
        /// <returns></returns>
        private List<Transform> initTransformList()
        {
            GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            /// 首先，收集场景中的对并且排序，添加到列表中
            Transform[] transforms = Selection.GetTransforms(SelectionMode.TopLevel);
            /// 排序
            transforms = sortTrs(transforms, Selection.GetTransforms(SelectionMode.Deep).Length);

            List<Transform> trs = new List<Transform>(transforms);

            return trs;
        }

        private void DoLightmapFromScene()
        {
            List<Transform> transforms = initTransformList();
            GameObject[] gameObjects = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];

            if (transforms.Count == 0)
            {
                // Debug.Log("No gameobject! - Please add at least a gameobject to export");
                return;
            }
            
            foreach (var gameObject in transforms)
            {

                // Skinned meshes
                var meshRenderer = gameObject.GetComponent<MeshRenderer>();
                if (meshRenderer)
                {
                    //meshRenderer = gameObject.GetComponent<>();
                }
                if (meshRenderer)
                {
                    var material = meshRenderer.sharedMaterial;

                    if (meshRenderer != null && material && material.shader && material.shader.name == "PiShader"
                        && material.HasProperty("_UseCustomLightMap") && material.GetInt("_UseCustomLightMap") == 0)
                    {
                        if (meshRenderer.lightmapIndex >= 0)
                        {
                            targetHashCode = LightmapSettings.lightmaps[meshRenderer.lightmapIndex].GetHashCode();

                            int index = meshRenderer.lightmapIndex;

                            // http://answers.unity3d.com/questions/1114251/lightmappingcompleted-callback-occurs-before-light.html
                            string curScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
                            string[] parts = curScene.Split('/', '\\');
                            string sceneName = parts[parts.Length - 1].Split('.')[index];
                            string lightmapPath = Path.GetDirectoryName(curScene) + "/" + sceneName + "/";
                            string filepath = lightmapPath + "Lightmap-" + index + "_comp_light.exr";

                            TextureImporter texImporter = (TextureImporter)AssetImporter.GetAtPath(filepath);
                            if (texImporter != null)
                            {
                                if (!texImporter.isReadable)
                                {
                                    texImporter.isReadable = true;
                                    texImporter.SaveAndReimport();
                                }
                            }

                            //var tg = lightMapList.Find(asTargetTex);
                            //if (tg != null)
                            //{
                            //material.SetTexture("_BakeLightMapTex", tg.tex);
                            material.SetTexture("_BakeLightMapTex", LightmapSettings.lightmaps[meshRenderer.lightmapIndex].lightmapColor);
                            material.SetTextureScale("_BakeLightMapTex", new Vector2(meshRenderer.lightmapScaleOffset.x, meshRenderer.lightmapScaleOffset.y));
                            material.SetTextureOffset("_BakeLightMapTex", new Vector2(meshRenderer.lightmapScaleOffset.z, meshRenderer.lightmapScaleOffset.w));
                            //material.SetInt("_UseBakeLightMap", 1);
                            //}
                        }
                        else
                        {
                            material.SetInt("_UseBakeLightMap", 0);
                        }
                    }
                }

            }
        }

        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	    {
            material = materialEditor.target as Material;

            DoLightmapFromScene();

            FindProperties(properties);
	        m_MaterialEditor = materialEditor;

			// Use default labelWidth
			EditorGUIUtility.labelWidth = 0f;

			//Debug.Log("Hello");
			DoAlbedoArea(material);
            DoHue ();

            DoLightMap(material);

            //DoRenderIndex(material);

            // 从 material 初始化
            DoUseAlphaFromDiffuseTexture(material);
            DoIgnoreLights(material);


            if (m_FirstTimeApply)
			{
				Texture2D inputTexture = material.GetTexture ("_MainTex") as Texture2D;

                if (inputTexture)
                {
                    if (restore.textName.ContainsKey("" + material.GetHashCode()))
                    {
                        if (restore.textName[material.GetHashCode() + ""] == inputTexture.name)
                        {
                            m_FirstTimeApply = false;
                            return;
                        }
                        else
                        {
                            restore.textName.Add(material.GetHashCode() + "", inputTexture.name);
                        }
                    }
                    else
                    {
                        restore.textName.Add(material.GetHashCode() + "", inputTexture.name);
                    }

                    getRenderingMode(material);
                    m_FirstTimeApply = false;
                }

			}
		
	    }
		public  void FindProperties(MaterialProperty[] props){
			blendMode   = FindProperty("_Mode", props);
			albedoMap   = FindProperty("_MainTex", props);
			albedoColor = FindProperty("_TintColor", props);
			Hue         = FindProperty("_Hue", props);
			Saturation  = FindProperty("_Saturation", props);
			Brightness  = FindProperty("_Brightness", props);
			Cull        = FindProperty("_Cull", props);
            DstBlend    = FindProperty("_DstBlend", props);
            SrcBlend    = FindProperty("_SrcBlend", props);

            CustomLightMapTex = FindProperty("_CustomLightMapTex", props);
            LightMapStrength  = FindProperty("_LightMapStrength", props);

        }
        //      static void MaterialChanged(Material material) 
        //{
        //	SetupMaterialWithBlendMode(material, (BlendMode)material.GetFloat("_Mode"));
        //}

        //public static void SetupMaterialWithBlendMode(Material material, BlendMode blendMode)
        //{
        //    switch (blendMode)
        //    {
        //        case BlendMode.Opaque:
        //            material.SetOverrideTag("RenderType", "");
        //            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        //            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        //            material.SetInt("_ZWrite", 1);
        //            material.DisableKeyword("_ALPHATEST_ON");
        //            material.DisableKeyword("_ALPHABLEND_ON");
        //            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
        //            break;
        //        case BlendMode.Cutout:
        //            material.SetOverrideTag("RenderType", "TransparentCutout");
        //            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        //            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        //            material.SetInt("_ZWrite", 1);
        //            material.EnableKeyword("_ALPHATEST_ON");
        //            material.DisableKeyword("_ALPHABLEND_ON");
        //            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
        //            break;
        //        case BlendMode.Transparent:
        //            material.SetOverrideTag("RenderType", "Transparent");
        //            material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        //            material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        //            material.SetInt("_ZWrite", 0);
        //            material.DisableKeyword("_ALPHATEST_ON");
        //            material.EnableKeyword("_ALPHABLEND_ON");
        //            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        //            material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        //            break;
        //    }
        //}

        //void BlendModePopup()
        //{
        //	EditorGUI.showMixedValue = blendMode.hasMixedValue;
        //	var mode = (BlendMode)blendMode.floatValue;
        //	EditorGUILayout.Space();
        //	EditorGUI.BeginChangeCheck();
        //	mode = (BlendMode)EditorGUILayout.Popup(Styles.renderingMode, (int)mode, Styles.blendNames);
        //	if (EditorGUI.EndChangeCheck())
        //	{
        //		m_MaterialEditor.RegisterPropertyChangeUndo("Rendering Mode");
        //		blendMode.floatValue = (float)mode;
        //		foreach (var obj in blendMode.targets) {
        //			MaterialChanged((Material)obj);
        //		}
        //	}

        //	EditorGUI.showMixedValue = false;
        //}

        void DoAlbedoArea(Material material)
		{
            
            GUILayout.Label("DiffuseTexture", EditorStyles.boldLabel);

            //MaterialProperty[] a = new MaterialProperty[2];
            //a [0] = albedoMap;a [1] = albedoColor;
            EditorGUI.BeginChangeCheck();
			{
				//m_MaterialEditor.PropertiesDefaultGUI (a);
				m_MaterialEditor.TextureProperty (albedoMap,"Texture");
                m_MaterialEditor.ColorProperty (albedoColor,"TintColor");
            
            }
			//if (EditorGUI.EndChangeCheck())
			//{
			//	changeRenderingMode (material);
			//}
			EditorGUILayout.Space();
		}

        void DoLightMap(Material material)
        {
            EditorGUILayout.Space();

            GUILayout.Label("Light Map", EditorStyles.boldLabel);
            m_MaterialEditor.TextureProperty(CustomLightMapTex, "Custom LightMap Texture");
            
            UseCustomLightMap = material.GetInt("_UseCustomLightMap") == 1;
            UseCustomLightMap = EditorGUILayout.Toggle("UseCustomLightMap", UseCustomLightMap);
            material.SetInt("_UseCustomLightMap", UseCustomLightMap ? 1 : 0);

            UseLightmapAsShadowmap = material.GetInt("_UseLightmapAsShadowmap") == 1;
            UseLightmapAsShadowmap = EditorGUILayout.Toggle("UseLightmapAsShadowmap", UseLightmapAsShadowmap);
            material.SetInt("_UseLightmapAsShadowmap", UseLightmapAsShadowmap ? 1 : 0);

            UseBakeLightMap = material.GetInt("_UseBakeLightMap") == 1;
            UseBakeLightMap = EditorGUILayout.Toggle("UseBakeLightMap", UseBakeLightMap);
            material.SetInt("_UseBakeLightMap", UseBakeLightMap ? 1 : 0);
        }
        void DoRenderIndex(Material material)
        {
            RenderIndex = material.renderQueue;
            RenderIndex = EditorGUILayout.IntField(RenderIndex);
            material.renderQueue = RenderIndex;
        }

        Texture2D GetLightMapTextFromScene(int index)
        {
            const int JpegQuality = 100;
            const float LightmapBrightness = 8.0f;
            const float LightmapContrast = 1.1f;
#if UNITY_EDITOR
            // http://answers.unity3d.com/questions/1114251/lightmappingcompleted-callback-occurs-before-light.html
            string curScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
            string[] parts = curScene.Split('/', '\\');
            string sceneName = parts[parts.Length - 1].Split('.')[index];
            string lightmapPath = Path.GetDirectoryName(curScene) + "/" + sceneName + "/";
            string filepath = lightmapPath + "Lightmap-" + index + "_comp_light.exr";

            TextureImporter texImporter = (TextureImporter)AssetImporter.GetAtPath(filepath);
            if (!texImporter.isReadable)
            {
                texImporter.isReadable = true;
                texImporter.SaveAndReimport();
            }

            Texture2D ti = LightmapSettings.lightmaps[0].lightmapColor;

            Texture2D tf = new Texture2D(ti.width, ti.height, TextureFormat.ARGB32, false);
            Color32[] c = ti.GetPixels32();

            for (int j = 0; j < c.Length; j++)
            {
                float af = c[j].a / 255f;
                float rf = c[j].r / 255f;
                float gf = c[j].g / 255f;
                float bf = c[j].b / 255f;

                float ur = Mathf.Pow(rf * af, LightmapContrast) * 255f * LightmapBrightness;
                float ug = Mathf.Pow(gf * af, LightmapContrast) * 255f * LightmapBrightness;
                float ub = Mathf.Pow(bf * af, LightmapContrast) * 255f * LightmapBrightness;

                ur = Mathf.Clamp(ur / 1.3f / 2.0f, 0, 255);
                ug = Mathf.Clamp(ug / 1.3f / 2.0f, 0, 255);
                ub = Mathf.Clamp(ub / 1.3f / 2.0f, 0, 255);

                c[j].r = Convert.ToByte(ur);
                c[j].g = Convert.ToByte(ug);
                c[j].b = Convert.ToByte(ub);
                c[j].a = 255;
            }

            tf.SetPixels32(c);

            return tf;
#endif
        }

        void DoHue()
		{
			EditorGUILayout.Space();
			MaterialProperty[] a = new MaterialProperty[7]; 
			a [0] = Cull;
            a [1] = SrcBlend;
			a [2] = DstBlend;

            a [3] = Hue;
			a [4] = Saturation;
            a [5] = Brightness;
            a [6] = LightMapStrength;
            EditorGUI.BeginChangeCheck();
			m_MaterialEditor.PropertiesDefaultGUI(a); 
		}

        void DoUseAlphaFromDiffuseTexture(Material material)
        {

            GUILayout.Space(2);
            EditorGUILayout.Space();

            UseAlphaFromDiffuseTexture  = material.GetInt("_UseAlphaFromDiffuseTexture") == 1;
            DiffuseTextureHasAlpha      = material.GetInt("_DiffuseTextureHasAlpha") == 1;

            UseAlphaFromDiffuseTexture  = EditorGUILayout.Toggle("UseAlphaFromDiffuseTexture", UseAlphaFromDiffuseTexture);
            material.SetInt("_UseAlphaFromDiffuseTexture", UseAlphaFromDiffuseTexture ? 1 : 0);
            //Debug.Log(UseAlphaFromDiffuseTexture);

            DiffuseTextureHasAlpha      = EditorGUILayout.Toggle("DiffuseTextureHasAlpha", DiffuseTextureHasAlpha);
            material.SetInt("_DiffuseTextureHasAlpha", DiffuseTextureHasAlpha ? 1 : 0);
            //Debug.Log(UseAlphaFromDiffuseTexture);
            
        }

        void DoIgnoreLights(Material material)
        {
            EditorGUILayout.Space();

            IgnoreAreaLight         = material.GetInt("_IgnoreAreaLight") == 1;
            IgnoreDirectionalLight  = material.GetInt("_IgnoreDirectionalLight") == 1;
            IgnorePointLight        = material.GetInt("_IgnorePointLight") == 1;
            IgnoreSpotLight         = material.GetInt("_IgnoreSpotLight") == 1;

            IgnoreAreaLight         = EditorGUILayout.Toggle("IgnoreAreaLight", IgnoreAreaLight);
            material.SetInt("_IgnoreAreaLight", IgnoreAreaLight ? 1 : 0);

            IgnoreDirectionalLight  = EditorGUILayout.Toggle("IgnoreDirectionalLight", IgnoreDirectionalLight);
            material.SetInt("_IgnoreDirectionalLight", IgnoreDirectionalLight ? 1 : 0);

            IgnorePointLight        = EditorGUILayout.Toggle("IgnorePointLight", IgnorePointLight);
            material.SetInt("_IgnorePointLight", IgnorePointLight ? 1 : 0);

            IgnoreSpotLight         = EditorGUILayout.Toggle("IgnoreSpotLight", IgnoreSpotLight);
            material.SetInt("_IgnoreSpotLight", IgnoreSpotLight ? 1 : 0);

        }

        void getRenderingMode(Material material){
			Texture2D inputTexture = material.GetTexture ("_MainTex") as Texture2D;
				
			int height = inputTexture.height;
			int width = inputTexture.width;
			Color[] textureColors = new Color[inputTexture.height * inputTexture.width];
			List<float> alphaList = new List<float>();

			if(!getPixelsFromTexture(ref inputTexture, out textureColors))
			{
				Debug.Log("Failed to convert texture " + inputTexture.name + " (unsupported type or format)");
				return;
			}

			for (int i = 0; i < height; ++i)
			{
				for (int j = 0; j < width; ++j)
				{
					if (textureColors [(height - i - 1) * width + j].a <= 0.02) {
						if (!alphaList.Contains (0)) {
							alphaList.Add (0);
						}
					} else {
						if (!alphaList.Contains (textureColors [(height - i - 1) * width + j].a)) {
							alphaList.Add (textureColors [(height - i - 1) * width + j].a);
						}
					}
					if(alphaList.Count >= 3){
						break;
					}
				}
				if(alphaList.Count >= 3){
					break;
				}
			}
			material.SetInt("_UseAlphaFromDiffuseTexture", UseAlphaFromDiffuseTexture ? 1 : 0);

            //int DiffuseTextureHasAlpha = 0;

            if (alphaList.Count >= 3)
            {
                //material.SetInt("_UseAlphaFromDiffuseTexture", 1);
                //DiffuseTextureHasAlpha = 1;
                blendMode.floatValue = (float)2;
			} else if (alphaList.Count == 2 && alphaList.Contains (1)  && alphaList.Contains ( 0) )
            {
                //material.SetInt("_UseAlphaFromDiffuseTexture", 1);
                //DiffuseTextureHasAlpha = 1;
                blendMode.floatValue = (float)1;
			} else if (alphaList.Count == 2)
            {
                //material.SetInt("_UseAlphaFromDiffuseTexture", 1);
                //DiffuseTextureHasAlpha = 0;
                blendMode.floatValue = (float)2;
			} else if (alphaList.Count == 1 && alphaList.Contains (1) )
            {
                //DiffuseTextureHasAlpha = 0;
                blendMode.floatValue = (float)0;
			} else
            {
                //DiffuseTextureHasAlpha = 0;
                //material.SetInt("_UseAlphaFromDiffuseTexture", 1);
                blendMode.floatValue = (float)2;
			}

            //Debug.Log(UseAlphaFromDiffuseTexture + " - " + DiffuseTextureHasAlpha);
            //material.SetInt("_DiffuseTextureHasAlpha", DiffuseTextureHasAlpha);

            //SetupMaterialWithBlendMode (material, (BlendMode)blendMode.floatValue);
		}
		//void changeRenderingMode(Material material){
  //          Debug.Log("change");

		//	Material mat = albedoMap.targets[0] as Material;
		//	Texture2D inputTexture = mat.GetTexture ("_MainTex") as Texture2D;
		//	Color col = albedoColor.colorValue;

		//	if (restore.textName [material.GetHashCode () + ""] != null) {
		//		if (restore.textName [material.GetHashCode () + ""] != inputTexture.name) {
		//			restore.textName [material.GetHashCode () + ""] = inputTexture.name;
		//			getRenderingMode (material);
		//		} 
		//	} else {
		//		restore.textName.Add (material.GetHashCode () + "",inputTexture.name);
		//		getRenderingMode (material);
		//	}

		//	if (restore.srcAlpha.ContainsKey(material.GetHashCode () + "")) {
		//		if (restore.srcAlpha [material.GetHashCode () + ""] != col.a) {
		//			if (col.a < 1 && col.a >= 0) {
		//				blendMode.floatValue = (float)2;
		//				//SetupMaterialWithBlendMode (material, (BlendMode)blendMode.floatValue);
		//				restore.srcAlpha [material.GetHashCode () + ""] = col.a;
		//			} else if (col.a == 1) {
		//				getRenderingMode (material);
		//				restore.srcAlpha [material.GetHashCode () + ""] = col.a;
		//			} 
					
		//		}
		//	} else {
		//		restore.srcAlpha.Add (material.GetHashCode () + "",col.a);
		//		getRenderingMode (material);
		//	}
		//}

		private bool getPixelsFromTexture(ref Texture2D texture, out Color[] pixels)
		{
			//Make texture readable
			TextureImporter im = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(texture)) as TextureImporter;
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
	}
}

public class restore{
	public static  Dictionary<string, string> textName = new Dictionary<string, string>();
	public static Dictionary<string, float> srcAlpha =new Dictionary<string, float>();
}