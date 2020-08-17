#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class GlTF_Buffer : GlTF_Writer
{
    /// <summary>
    /// buffer URI
    /// </summary>
    public string uri;
    /// <summary>
    /// Buffer 长度
    /// </summary>
    public long BLength;
    /// <summary>
    /// 所属 BufferViwer 序号
    /// </summary>
	public int BufferviewIndex;

	public override void Write ()
	{
		Indent();		jsonWriter.Write ("{\n");
		IndentIn();
		//var binName = binary ? "binary_glTF" : Path.GetFileNameWithoutExtension(GlTF_Writer.binFileName);
		Indent();		jsonWriter.Write ("\"byteLength\": " + BLength + ",\n");
		Indent();		jsonWriter.Write ("\"uri\": \"" + uri + "\"\n");

		IndentOut();
		Indent();		jsonWriter.Write ("}");
	}
}

/// <summary>
/// 
/// </summary>
public class GlTF_Writer {
    /// <summary>
    /// 是否导出 RTPL
    /// </summary>
	public bool exportRtpl = false;
    /// <summary>
    /// 是否合并Mesh
    /// </summary>
	public static bool combineMesh = false;

	public int bufferIndex = 0;
	public static FileStream fs;
	public static StreamWriter jsonWriter;
	public static BinaryWriter binWriter;
	public static Stream binFile;
	public static int indent = 0;
	public static string binFileName;
	public static bool binary;
	static bool[] firsts                                    = new bool[100];
    /// <summary>
    /// byte BufferView
    /// </summary>
	public static GlTF_Pi_BufferView PibyteBufferView       = new GlTF_Pi_BufferView("byteBufferView", 0);
    /// <summary>
    /// ushort BufferView
    /// </summary>
	public static GlTF_Pi_BufferView PiushortBufferView     = new GlTF_Pi_BufferView("ushortBufferView", 0, 34963);
    /// <summary>
    /// float BufferView
    /// </summary>
	public static GlTF_Pi_BufferView PifloatBufferView      = new GlTF_Pi_BufferView("floatBufferView", 0);
    /// <summary>
    /// float BufferView
    /// </summary>
	public static GlTF_Pi_BufferView AniPifloatBufferView = new GlTF_Pi_BufferView("anifloatBufferView", 0);
    /// <summary>
    /// vec2 BufferView
    /// </summary>
	public static GlTF_Pi_BufferView Pivec2BufferView       = new GlTF_Pi_BufferView("vec2BufferView", 8);
    /// <summary>
    /// vec3 BufferView
    /// </summary>
	public static GlTF_Pi_BufferView Pivec3BufferView       = new GlTF_Pi_BufferView("vec3BufferView", 12);
    /// <summary>
    /// vec4 BufferView
    /// </summary>
	public static GlTF_Pi_BufferView Pivec4BufferView       = new GlTF_Pi_BufferView("vec4BufferView", 16);
    /// <summary>
    /// vec4 Ushort BufferView
    /// </summary>
	public static GlTF_Pi_BufferView Pivec4UshortBufferView = new GlTF_Pi_BufferView("vec4UshortBufferView", 8);
    /// <summary>
    /// mat4 BufferView
    /// </summary>
	public static GlTF_Pi_BufferView Pimat4BufferView       = new GlTF_Pi_BufferView("mat4BufferView", 64);

	public static GlTF_Pi_BufferView Pivec1BufferViewAnim   = new GlTF_Pi_BufferView("vec1BufferViewAnim", 0);
	public static GlTF_Pi_BufferView Pivec3BufferViewAnim   = new GlTF_Pi_BufferView("vec3BufferViewAnim", 12);
	public static GlTF_Pi_BufferView Pivec4BufferViewAnim   = new GlTF_Pi_BufferView("vec4BufferViewAnim", 16);

	public static bool PiExportSky                          = false;
    public static List<Texture2D> PiskyTexture              = new List<Texture2D>();
    /// <summary>
    /// geometry 数据列表 - 未使用 (标准GLTF导出
    /// </summary>
    public static List<GlTF_Geometry> geometrys             = new List<GlTF_Geometry>();
    /// <summary>
    /// geometry 数据列表
    /// </summary>
	public static List<GlTF_Geometry> geometryList          = new List<GlTF_Geometry> ();
	public static List<GlTF_Anim_Count> animCounts          = new List<GlTF_Anim_Count> ();


	public static List<GlTF_Pi_BufferView> PibufferViews    = new List<GlTF_Pi_BufferView>();
	public static List<GlTF_Buffer> buffers                 = new List<GlTF_Buffer>();

	public static List<GlTF_Camera> cameras                 = new List<GlTF_Camera>();
	public static List<GlTF_Light> lights                   = new List<GlTF_Light>();
	public static List<GlTF_Particle> particles             = new List<GlTF_Particle>();
	public static List<GlTF_Mesh> meshes                    = new List<GlTF_Mesh>();
	public static List<GlTF_Accessor> accessors             = new List<GlTF_Accessor>();
	public static List<GlTF_Accessor> animaccessors         = new List<GlTF_Accessor>();
	public static List<GlTF_Animation> oneAnimator          = new List<GlTF_Animation>();
    public static Hashtable meshesHashMap                   = new Hashtable();

    /// <summary>
    /// node 名称列表
    /// </summary>
    public static List<string> nodeNames                    = new List<string>();

    /// <summary>
    /// node 列表
    /// </summary>
    public static List<GlTF_Node> nodes                     = new List<GlTF_Node>();

    /// <summary>
    /// material 名称列表
    /// </summary>
    public static List<string> materialNames                = new List<string>();
    /// <summary>
    /// material 列表
    /// </summary>
	public static List<GlTF_Material> materials             = new List<GlTF_Material>();
    /// <summary>
    /// sampler 名称列表
    /// </summary>
	public static List<string> samplerNames                 = new List<string>();
    /// <summary>
    /// sampler 列表
    /// </summary>
	public static List<GlTF_Sampler> samplers               = new List<GlTF_Sampler>();
    /// <summary>
    /// texture 名称列表
    /// </summary>
	public static List<string> textureNames                 = new List<string>();
    /// <summary>
    /// texture 列表
    /// </summary>
	public static List<GlTF_Texture> textures               = new List<GlTF_Texture>();
    /// <summary>
    /// image 名称列表
    /// </summary>
	public static List<string> imageNames                   = new List<string>();
    /// <summary>
    /// image 列表
    /// </summary>
	public static List<GlTF_Image> images                   = new List<GlTF_Image>();
    /// <summary>
    /// animation 列表
    /// </summary>
	public static List<GlTF_Animation> animations           = new List<GlTF_Animation>();
    /// <summary>
    /// technique 名称列表
    /// </summary>
	public static List<string> techniqueNames               = new List<string>();
    /// <summary>
    /// technique 列表
    /// </summary>
	public static List<GlTF_Technique> techniques           = new List<GlTF_Technique>();
    /// <summary>
    /// programe 列表
    /// </summary>
	public static List<GlTF_Program> programs               = new List<GlTF_Program>();
    /// <summary>
    /// shader 列表
    /// </summary>
	public static List<GlTF_Shader> shaders                 = new List<GlTF_Shader>();
    /// <summary>
    /// skin 列表
    /// </summary>
	public static List<GlTF_Skin> skins                     = new List<GlTF_Skin>();
    /// <summary>
    /// Root 节点列表
    /// </summary>
	public static List<GlTF_Node> rootNodes                 = new List<GlTF_Node>();

	// Keys are original file path, values correspond to the directory in the output zip file
	//键是原始文件路径，值对应于输出zip文件中的目录
	public static Dictionary<string, string> exportedFiles  = new Dictionary<string, string>();
	// Exporter specifics
    /// <summary>
    /// 是否烘培动画
    /// </summary>
	public static bool bakeAnimation;
    /// <summary>
    /// 是否导出PBR材质
    /// </summary>
	public static bool exportPBRMaterials;
    /// <summary>
    /// 是否有镜面材质
    /// </summary>
	public static bool hasSpecularMaterials = false;
    /// <summary>
    /// 是否使用右手坐标系
    /// </summary>
	public static bool convertRightHanded = false;
    /// <summary>
    /// 导出器版本
    /// </summary>
	public static string exporterVersion = "2.2.1";
    /// <summary>
    /// 非基本字符 字符集
    /// </summary>
	public static Regex rgx = new Regex("[^a-zA-Z0-9 -_.]");

    public string id;
    public string name; // name of this object
    
    static public string cleanNonAlphanumeric(string s)
	{
		return rgx.Replace(s, "");
	}
	static public string GetNameFromObject(Object o, bool useId = false)
	{
		var ret = cleanNonAlphanumeric(o.name);
		if (useId)
		{
			ret += ",-," + o.GetInstanceID();
		}
		return ret;
	}

	public void convertVector3LeftToRightHandedness(ref Vector3 vect)
	{
		vect.z = -vect.z;
	}

	public void convertVector4LeftToRightHandedness(ref Vector4 vect)
	{
		vect.z = -vect.z;
		vect.w = -vect.w;
	}

	public void convertQuatLeftToRightHandedness(ref Quaternion quat)
	{
		quat.w = -quat.w;
		quat.z = -quat.z;
	}

	// Decomposes a matrix, converts each component from left to right handed and
	// rebuilds a matrix
	// FIXME: there is probably a better way to do that. It doesn't work well with non uniform scales
	public void convertMatrixLeftToRightHandedness(ref Matrix4x4 mat)
	{
		Vector3 position = mat.GetColumn(3);
		convertVector3LeftToRightHandedness(ref position);
		Quaternion rotation = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
		convertQuatLeftToRightHandedness(ref rotation);

		Vector3 scale = new Vector3(mat.GetColumn(0).magnitude, mat.GetColumn(1).magnitude, mat.GetColumn(2).magnitude);
		float epsilon = 0.00001f;

		// Some issues can occurs with non uniform scales
		if(Mathf.Abs(scale.x - scale.y) > epsilon  || Mathf.Abs(scale.y - scale.z) > epsilon || Mathf.Abs(scale.x - scale.z) > epsilon)
		{
			Debug.LogWarning("A matrix with non uniform scale is being converted from left to right handed system. This code is not working correctly in this case");
		}

		// Handle negative scale component in matrix decomposition
		if (Matrix4x4.Determinant(mat) < 0)
		{
			Quaternion rot = Quaternion.LookRotation(mat.GetColumn(2), mat.GetColumn(1));
			Matrix4x4 corr = Matrix4x4.TRS(mat.GetColumn(3), rot, Vector3.one).inverse;
			Matrix4x4 extractedScale = corr * mat;
			scale = new Vector3(extractedScale.m00, extractedScale.m11, extractedScale.m22);
		}

		// convert transform values from left handed to right handed
		mat.SetTRS(position, rotation, scale);
	}
    /// <summary>
    /// 
    /// </summary>
	public static void EveryMeshAndAnimInit(){
		oneAnimator                     = new List<GlTF_Animation> ();
		PifloatBufferView               = new GlTF_Pi_BufferView("floatBufferView", 0);
        /// Animation
        AniPifloatBufferView            = new GlTF_Pi_BufferView("AnifloatBufferView", 0);
        PibyteBufferView                = new GlTF_Pi_BufferView("byteBufferView", 0);
		Pivec1BufferViewAnim            = new GlTF_Pi_BufferView("vec1BufferViewAnim", 0);
		Pivec3BufferViewAnim            = new GlTF_Pi_BufferView("vec3BufferViewAnim", 12);
		Pivec4BufferViewAnim            = new GlTF_Pi_BufferView("vec4BufferViewAnim", 16);

		if (!combineMesh)
        {
			PiushortBufferView          = new GlTF_Pi_BufferView("ushortBufferView", 0, 34963);
			Pivec2BufferView            = new GlTF_Pi_BufferView("vec2BufferView", 8);
			Pivec3BufferView            = new GlTF_Pi_BufferView("vec3BufferView", 12);
			Pivec4BufferView            = new GlTF_Pi_BufferView("vec4BufferView", 16);
			Pivec4UshortBufferView      = new GlTF_Pi_BufferView("vec4iBufferView", 8);
			PifloatBufferView           = new GlTF_Pi_BufferView("floatBufferView", 0);
			Pimat4BufferView            = new GlTF_Pi_BufferView("mat4BufferView", 64);

			Pivec2BufferView.target     = (int)GlTF_Pi_BufferView.TARGET.ARRAY;
			Pivec3BufferView.target     = (int)GlTF_Pi_BufferView.TARGET.ARRAY;
			Pivec4BufferView.target     = (int)GlTF_Pi_BufferView.TARGET.ARRAY;
			PiushortBufferView.target   = (int)GlTF_Pi_BufferView.TARGET.ELEMENT;
		}
	}

	public void Init()
	{
		firsts = new bool[100];

		PiushortBufferView          = new GlTF_Pi_BufferView("ushortBufferView", 0, 34963);
        AniPifloatBufferView        = new GlTF_Pi_BufferView("anifloatBufferView", 0);
        PifloatBufferView           = new GlTF_Pi_BufferView("floatBufferView", 0);
		Pivec2BufferView            = new GlTF_Pi_BufferView("vec2BufferView", 8);
		Pivec3BufferView            = new GlTF_Pi_BufferView("vec3BufferView", 12);
		Pivec4BufferView            = new GlTF_Pi_BufferView("vec4BufferView", 16);
		Pivec4UshortBufferView      = new GlTF_Pi_BufferView("vec4iBufferView", 8);
		Pimat4BufferView            = new GlTF_Pi_BufferView("mat4BufferView", 64);

		/// Animation
		PibyteBufferView            = new GlTF_Pi_BufferView("byteBufferView", 0);
		Pivec1BufferViewAnim        = new GlTF_Pi_BufferView("vec1BufferViewAnim", 0);
		Pivec3BufferViewAnim        = new GlTF_Pi_BufferView("vec3BufferViewAnim", 12);
		Pivec4BufferViewAnim        = new GlTF_Pi_BufferView("vec4BufferViewAnim", 16);

		Pivec2BufferView.target     = (int)GlTF_Pi_BufferView.TARGET.ARRAY;
		Pivec3BufferView.target     = (int)GlTF_Pi_BufferView.TARGET.ARRAY;
		Pivec4BufferView.target     = (int)GlTF_Pi_BufferView.TARGET.ARRAY;
		PiushortBufferView.target   = (int)GlTF_Pi_BufferView.TARGET.ELEMENT;

		animCounts                  = new List<GlTF_Anim_Count> ();
		PibufferViews               = new List<GlTF_Pi_BufferView>();
		buffers                     = new List<GlTF_Buffer>();
		cameras                     = new List<GlTF_Camera>();
		lights                      = new List<GlTF_Light>();
		particles                   = new List<GlTF_Particle>();
		meshes                      = new List<GlTF_Mesh>();
		meshesHashMap               = new Hashtable();
		accessors                   = new List<GlTF_Accessor>();
		animaccessors               = new List<GlTF_Accessor>();
		nodes                       = new List<GlTF_Node>();
		nodeNames                   = new List<string>();

		materialNames               = new List<string>();
		materials                   = new List<GlTF_Material>();

		PiskyTexture                = new List<Texture2D>();
		PiExportSky                 = false;
		samplerNames                = new List<string>();
		samplers                    = new List<GlTF_Sampler>();

		textureNames                = new List<string>();
		textures                    = new List<GlTF_Texture>();

		imageNames                  = new List<string>();
		images                      = new List<GlTF_Image>();
		animations                  = new List<GlTF_Animation>();

		techniqueNames              = new List<string>();
		techniques                  = new List<GlTF_Technique>();

		programs                    = new List<GlTF_Program>();
		shaders                     = new List<GlTF_Shader>();
		skins                       = new List<GlTF_Skin>();
		rootNodes                   = new List<GlTF_Node>();

		bakeAnimation               = true;
		hasSpecularMaterials        = false;
	}
    /// <summary>
    /// 实现缩进
    /// </summary>
	public void Indent() {
		for (int i = 0; i < indent; i++)
			jsonWriter.Write ("\t");
	}

    /// <summary>
    /// 增加缩进量
    /// </summary>
    public void IndentIn() {
		indent++;
        if (indent >= 0)
        {
            firsts[indent] = true;
        }
	}

    /// <summary>
    /// 减少缩进量
    /// </summary>
    public void IndentOut() {
		indent--;
	}

    /// <summary>
    /// 行首
    /// </summary>
    public void CommaStart() {
		firsts[indent] = false;
	}

    /// <summary>
    /// 换行
    /// </summary>
    public void CommaNL() {
        if (firsts.Length <= indent || indent < 0)
        {
            jsonWriter.Write(",\n");
        }
        else
        {
            if (!firsts[indent])
                jsonWriter.Write(",\n");

            firsts[indent] = false;
        }
	}

	// Extra data for objects
	public Dictionary<string, string> extraString   = new Dictionary<string, string>();
	public Dictionary<string, float> extraFloat     = new Dictionary<string, float>();
	public Dictionary<string, bool> extraBool       = new Dictionary<string, bool>();

    /// <summary>
    /// 打开文件写 - 
    /// </summary>
    public void OpenFiles (string filepath) {
		fs = File.Open(filepath, FileMode.Create);

        /// Value is an empty string since we want the file at the root of the .zip file
        /// 值是一个空字符串，因为我们希望文件位于.zip文件的根目录下
		exportedFiles.Add(filepath, "");

		jsonWriter = new StreamWriter (fs);

		if (combineMesh)
        {
            binFileName = Path.GetFileNameWithoutExtension(filepath) + ".mesh.bin";
            var binPath = Path.Combine(Path.GetDirectoryName(filepath), binFileName);

            exportedFiles.Add(binPath, "");  // Value is an empty string since we want the file at the root of the .zip file

            binFile = File.Open(binPath, FileMode.Create);
        }
	}
    /// <summary>
    /// 结束文件写 - 保存文件内容
    /// </summary>
	public void CloseFiles() {
		jsonWriter.Close ();
		fs.Close();

		if (combineMesh)
        {
            binFile.Close();
        }

	}

	public void writeExtras()
	{
		if (extraFloat.Count > 0 || extraString.Count > 0 || extraBool.Count > 0)
		{
			Indent(); jsonWriter.Write("\"extras\": {\n");
			    IndentIn();

			    foreach (var s in extraString)
			    {
				    CommaNL();
				    Indent(); jsonWriter.Write("\"" + s.Key + "\" : \"" + s.Value + "\"");
			    }

			    foreach (var s in extraFloat)
			    {
				    CommaNL();
				    Indent(); jsonWriter.Write("\"" + s.Key + "\" : " + s.Value + "");
			    }

			    foreach (var s in extraBool)
			    {
				    CommaNL();
				    Indent(); jsonWriter.Write("\"" + s.Key + "\" : " + (s.Value ? "true" : "false") + "");
			    }

			    IndentOut();
			jsonWriter.Write("\n");
			Indent(); jsonWriter.Write("},");
			jsonWriter.Write("\n");
		}
	}

	public virtual void Write () {
		if (combineMesh) {
			GlTF_Buffer bu = new GlTF_Buffer ();
			bu.uri = binFileName;

			PifloatBufferView.byteOffset        = 0;
			Pivec2BufferView.byteOffset         = PifloatBufferView.byteOffset + PifloatBufferView.byteLength;
			Pivec3BufferView.byteOffset         = Pivec2BufferView.byteOffset + Pivec2BufferView.byteLength;
			Pivec4BufferView.byteOffset         = Pivec3BufferView.byteOffset + Pivec3BufferView.byteLength;
			Pivec4UshortBufferView.byteOffset   = Pivec4BufferView.byteOffset + Pivec4BufferView.byteLength;
			PiushortBufferView.byteOffset       = Pivec4UshortBufferView.byteOffset + Pivec4UshortBufferView.byteLength;

			Pivec2BufferView.PimemoryStream.WriteTo (binFile);
			Pivec3BufferView.PimemoryStream.WriteTo (binFile);
			Pivec4BufferView.PimemoryStream.WriteTo (binFile);
			Pivec4UshortBufferView.PimemoryStream.WriteTo (binFile);
			PiushortBufferView.PimemoryStream.WriteTo (binFile);

			bu.BLength          = binFile.Length;
			bu.BufferviewIndex  = buffers.Count;

			binFile.Flush ();
			buffers.Add (bu);

		}
		jsonWriter.Write ("{\n");
		IndentIn();

		/// asset
		CommaNL();
		Indent();	jsonWriter.Write ("\"asset\": {\n");
		    IndentIn();
		    Indent();	jsonWriter.Write ("\"generator\": \"Unity "+ Application.unityVersion + "\",\n");

		    writeExtras();

		    Indent();	jsonWriter.Write ("\"version\": \"2.0\"\n");

		    IndentOut();
		Indent();	jsonWriter.Write ("}");


		if (animations.Count > 0)
		{
			CommaNL();
			Indent();	jsonWriter.Write ("\"animations\": [\n");
			IndentIn();
			foreach (GlTF_Animation a in animations)
			{
				CommaNL();
				a.Write ();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();	jsonWriter.Write ("]");
		}

		if (accessors != null && accessors.Count > 0)
		{
			CommaNL();
			Indent();	jsonWriter.Write ("\"accessors\": [\n");
			IndentIn();
			foreach (GlTF_Accessor a in accessors)
			{
				CommaNL();
				a.Write ();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();	jsonWriter.Write ("]");
		}

		if (!binary)
		{


				// FIX: Should support multiple buffers
				CommaNL();
				Indent();	jsonWriter.Write ("\"buffers\": [\n");
				IndentIn();
				foreach (GlTF_Buffer a in buffers)
				{
					CommaNL();
					a.Write ();
				}
				jsonWriter.WriteLine();

				IndentOut();
				Indent();	jsonWriter.Write ("]");
		}

			if (PibufferViews != null && PibufferViews.Count > 0)
			{
				CommaNL();
				Indent();	jsonWriter.Write ("\"bufferViews\": [\n");
				IndentIn();
				foreach (GlTF_Pi_BufferView bv in PibufferViews)
				{
					if (bv.byteLength > 0)
					{
						CommaNL();
						bv.Write ();
					}
				}
				jsonWriter.WriteLine();
				IndentOut();
				Indent();	jsonWriter.Write ("]");
			}

		if (cameras != null && cameras.Count > 0)
		{
			CommaNL();
			Indent();		jsonWriter.Write ("\"cameras\": [\n");
			IndentIn();
			foreach (GlTF_Camera c in cameras)
			{
				CommaNL();
				c.Write ();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();		jsonWriter.Write ("]");
		}

		if(hasSpecularMaterials)
		{
			CommaNL();
			Indent(); jsonWriter.Write("\"extensionsRequired\": [\n");
			IndentIn();
			Indent(); jsonWriter.Write("\"KHR_materials_pbrSpecularGlossiness\"\n");
			IndentOut();
			Indent(); jsonWriter.Write("]");
		}

		if(hasSpecularMaterials || binary)
		{
			CommaNL();
			Indent(); jsonWriter.Write("\"extensionsUsed\": [\n");
			IndentIn();
			if (hasSpecularMaterials)
			{
				Indent(); jsonWriter.Write("\"KHR_materials_pbrSpecularGlossiness\"\n");
			}
			if (binary)
			{
				Indent(); jsonWriter.Write("\"KHR_binary_glTF\"\n");
			}
			IndentOut();
			Indent(); jsonWriter.Write("]");
		}

		if (images.Count > 0)
		{
			CommaNL();
			Indent();	jsonWriter.Write ("\"images\": [\n");
			IndentIn();
			foreach (var i in images)
			{
				CommaNL();
				i.Write ();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();	jsonWriter.Write ("]");
		}

		if (materials.Count > 0)
		{
			CommaNL();
			Indent();	jsonWriter.Write ("\"materials\": [\n");
			IndentIn();
			foreach (GlTF_Material m in materials)
			{
				CommaNL();
				m.Write ();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();	jsonWriter.Write ("]");
		}

		if (meshes != null && meshes.Count > 0)
		{
			CommaNL();
			Indent();
			jsonWriter.Write ("\"meshes\": [\n");
			IndentIn();
			foreach (GlTF_Mesh m in meshes)
			{
				CommaNL();
				m.Write ();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();
			jsonWriter.Write ("]");
		}

		if (nodes != null && nodes.Count > 0)
		{
			CommaNL();
			Indent();			jsonWriter.Write ("\"nodes\": [\n");
			IndentIn();
			foreach (GlTF_Node n in nodes)
			{
				CommaNL();
				n.Write();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();			jsonWriter.Write ("]");
		}

		if ( lights.Count > 0 || particles.Count > 0)
		{
			CommaNL();
			Indent();			jsonWriter.Write ("\"extensions\": {\n");
			IndentIn();
			if(lights.Count > 0){
				CommaNL();
				Indent();				jsonWriter.Write ("\"KHR_lights_punctual\": {\n");
				IndentIn();
				Indent();					jsonWriter.Write ("\"lights\": [\n");
				foreach (GlTF_Light l in lights)
				{
					CommaNL();
					l.Write();
				}
				jsonWriter.WriteLine();
				Indent();					jsonWriter.Write ("]\n");
				IndentOut();
				Indent();				jsonWriter.Write ("}");
			}
			if(particles.Count > 0){
				CommaNL();
				Indent();				jsonWriter.Write ("\"PI_particle\": {\n");
				IndentIn();
				Indent();					jsonWriter.Write ("\"particles\": [\n");
				foreach (GlTF_Particle l in particles)
				{
					CommaNL();
					l.Write();
				}
				jsonWriter.WriteLine();
				Indent();					jsonWriter.Write ("]\n");
				IndentOut();
				Indent();				jsonWriter.Write ("}\n");
			}
			IndentOut();
			Indent();			jsonWriter.Write ("}");
		}

		if (samplers.Count > 0)
		{
			CommaNL();
			Indent();	jsonWriter.Write ("\"samplers\": [\n");
			IndentIn();
			foreach (GlTF_Sampler s in samplers)
			{
				CommaNL();
				s.Write ();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();	jsonWriter.Write ("]");
		}
		CommaNL();
		Indent();			jsonWriter.Write ("\"scenes\": [\n");
		IndentIn();
		Indent();			jsonWriter.Write ("{\n");
		IndentIn();
		CommaNL();
		Indent(); jsonWriter.Write("\"name\":\"defaultScene\",\n");
		Indent();			jsonWriter.Write ("\"nodes\": [\n");
		IndentIn();
		foreach (GlTF_Node n in rootNodes)
		{
			CommaNL();
			Indent();		jsonWriter.Write(nodes.IndexOf(n));
		}
		jsonWriter.WriteLine();
		IndentOut();
		Indent();			jsonWriter.Write ("],\n");

		Indent();			jsonWriter.Write ("\"extensions\": {\n");

		IndentIn ();
		Indent();			jsonWriter.Write ("\"PI_environment\": {\n");
		IndentIn ();
		CommaNL ();
		// Indent();				jsonWriter.Write ("\"color\": [" + RenderSettings.ambientSkyColor[0] + ","+RenderSettings.ambientSkyColor[1] + ","+RenderSettings.ambientSkyColor[2] + "]");
		if(RenderSettings.fog){
			Indent();			jsonWriter.Write ("\"fog\": {\n");
			IndentIn ();
			CommaNL ();
			Indent();			jsonWriter.Write ("\"mode\": \"" + RenderSettings.fogMode +"\"");
			CommaNL ();
			Indent();			jsonWriter.Write ("\"fogColor\": [" + RenderSettings.fogColor[0] + ","+RenderSettings.fogColor[1] + ","+RenderSettings.fogColor[2] + "]");
			CommaNL ();
			Indent();			jsonWriter.Write ("\"fogDensity\": " + RenderSettings.fogDensity);
			CommaNL ();
			Indent();			jsonWriter.Write ("\"fogStart\": " + RenderSettings.fogStartDistance);
			CommaNL ();
			Indent();			jsonWriter.Write ("\"fogEnd\": " + RenderSettings.fogEndDistance+"\n");
			IndentOut ();
			Indent();	jsonWriter.Write ("}" );
		}
		if(PiExportSky){
			Indent();				jsonWriter.Write ("\"skybox\": {\n");
			IndentIn ();
			CommaNL ();
			Indent();			jsonWriter.Write ("\"path\": \"skybox/sky\"");
			CommaNL ();
			Indent();			jsonWriter.Write ("\"size\": " + 100 + "\n");
			IndentOut ();
			Indent();	jsonWriter.Write ("}\n" );
		}

		IndentOut ();
		Indent();			jsonWriter.Write ("}\n");
		IndentOut ();
		Indent();		jsonWriter.Write ("}\n");
		IndentOut();
		Indent();			jsonWriter.Write ("}\n");
		IndentOut();
		Indent();			jsonWriter.Write ("],\n");

		Indent(); jsonWriter.Write("\"scene\": 0");

		if(skins.Count > 0)
		{
			CommaNL();
			Indent(); jsonWriter.Write("\"skins\": [\n");
			IndentIn();
			foreach(GlTF_Skin skin in skins)
			{
				CommaNL();
				skin.Write();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent(); jsonWriter.Write("]");
		}

		if (textures.Count > 0)
		{
			CommaNL();
			Indent();	jsonWriter.Write ("\"textures\": [\n");
			IndentIn();
			foreach (GlTF_Texture t in textures)
			{
				CommaNL();
				t.Write ();
			}
			jsonWriter.WriteLine();
			IndentOut();
			Indent();	jsonWriter.Write ("]");
		}

		IndentOut();
		jsonWriter.Write ("\n}");
		jsonWriter.Flush();
	
	}

    private void WriteAsset()
    {
        CommaNL();
        Indent(); jsonWriter.Write("\"asset\": {\n");
        IndentIn();
        Indent(); jsonWriter.Write("\"generator\": \"Unity " + Application.unityVersion + "\",\n");

        writeExtras();

        Indent(); jsonWriter.Write("\"version\": \"2.0\"\n");

        IndentOut();
        Indent(); jsonWriter.Write("}");
    }

    private void WriteAccessor()
    {
        if (accessors != null && accessors.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"accessors\": [\n");
            IndentIn();
            foreach (GlTF_Accessor a in accessors)
            {
                CommaNL();
                a.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteAnimation()
    {
        if (animations.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"animations\": [\n");
            IndentIn();
            foreach (GlTF_Animation a in animations)
            {
                CommaNL();
                a.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteLight()
    {
        if (lights.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"KHR_lights_punctual\": {\n");
            IndentIn();
            Indent(); jsonWriter.Write("\"lights\": [\n");
            foreach (GlTF_Light l in lights)
            {
                CommaNL();
                l.Write();
            }
            jsonWriter.WriteLine();
            Indent(); jsonWriter.Write("]\n");
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }
    }

    private void WriteCamera()
    {
        if (cameras != null && cameras.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"cameras\": [\n");
            IndentIn();
            foreach (GlTF_Camera c in cameras)
            {
                CommaNL();
                c.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteFOG()
    {
        if (RenderSettings.fog)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"fog\": {\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"mode\": \"" + RenderSettings.fogMode + "\"");
            CommaNL();
            Indent(); jsonWriter.Write("\"fogColor\": [" + RenderSettings.fogColor[0] + "," + RenderSettings.fogColor[1] + "," + RenderSettings.fogColor[2] + "]");
            CommaNL();
            Indent(); jsonWriter.Write("\"fogDensity\": " + RenderSettings.fogDensity / 10.0);
            CommaNL();
            Indent(); jsonWriter.Write("\"fogStart\": " + RenderSettings.fogStartDistance);
            CommaNL();
            Indent(); jsonWriter.Write("\"fogEnd\": " + RenderSettings.fogEndDistance + "\n");
            IndentOut();
            Indent(); jsonWriter.Write("}");
        }
    }

    private void WriteSkyBox()
    {
        if (PiExportSky)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"skybox\": {\n");
            IndentIn();
            CommaNL();
            Indent(); jsonWriter.Write("\"path\": \"skybox/sky\"");
            CommaNL();
            Indent(); jsonWriter.Write("\"size\": " + 100 + "\n");
            IndentOut();
            Indent(); jsonWriter.Write("}\n");
        }
    }

    private void WriteParticle()
    {
        if (particles.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"PI_particle\": {\n");
            IndentIn();
            Indent(); jsonWriter.Write("\"particles\": [\n");
            foreach (GlTF_Particle l in particles)
            {
                CommaNL();
                l.Write();
            }
            jsonWriter.WriteLine();
            Indent(); jsonWriter.Write("]\n");
            IndentOut();
            Indent(); jsonWriter.Write("}\n");
        }
    }

    private void WriteNode()
    {
        if (nodes != null && nodes.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"nodes\": [\n");
            IndentIn();
            foreach (GlTF_Node n in nodes)
            {
                CommaNL();
                n.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteMesh()
    {
        if (meshes != null && meshes.Count > 0)
        {
            CommaNL();
            Indent();
            jsonWriter.Write("\"meshes\": [\n");
            IndentIn();
            foreach (GlTF_Mesh m in meshes)
            {
                CommaNL();
                m.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent();
            jsonWriter.Write("]");
        }
    }

    private void WriteMaterial()
    {
        if (materials.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"materials\": [\n");
            IndentIn();
            foreach (GlTF_Material m in materials)
            {
                CommaNL();
                m.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteSampler()
    {
        if (samplers.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"samplers\": [\n");
            IndentIn();
            foreach (GlTF_Sampler s in samplers)
            {
                CommaNL();
                s.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteTexture()
    {
        if (textures.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"textures\": [\n");
            IndentIn();
            foreach (GlTF_Texture t in textures)
            {
                CommaNL();
                t.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteImage()
    {
        if (images.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"images\": [\n");
            IndentIn();
            foreach (var i in images)
            {
                CommaNL();
                i.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteSkin()
    {
        if (skins.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"skins\": [\n");
            IndentIn();
            foreach (GlTF_Skin skin in skins)
            {
                CommaNL();
                skin.Write();
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }

    private void WriteBuffer()
    {
        // FIX: Should support multiple buffers
        CommaNL();
        Indent(); jsonWriter.Write("\"buffers\": [\n");
        IndentIn();
        foreach (GlTF_Buffer a in buffers)
        {
            CommaNL();
            a.Write();
        }
        jsonWriter.WriteLine();

        IndentOut();
        Indent(); jsonWriter.Write("]");
    }

    private void WriteBufferView()
    {
        if (PibufferViews != null && PibufferViews.Count > 0)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"bufferViews\": [\n");
            IndentIn();
            foreach (GlTF_Pi_BufferView bv in PibufferViews)
            {
                if (bv.byteLength > 0)
                {
                    CommaNL();
                    bv.Write();
                }
            }
            jsonWriter.WriteLine();
            IndentOut();
            Indent(); jsonWriter.Write("]");
        }
    }
}

#endif