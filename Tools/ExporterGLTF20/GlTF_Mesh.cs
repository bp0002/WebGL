#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

/**
 * 将mesh的attribute和indice写入.mesh.bin文件，解析mesh到accessor
 */
public class GlTF_Mesh : GlTF_Writer {
    /// <summary>
    /// 材质是否转 RTPL
    /// </summary>
	public bool matToRtpl = false;

    /// <summary>
    /// 图元列表
    /// </summary>
	public List<GlTF_Primitive> primitives;

    public float alphaIndex = 0.0f;

    /// <summary>
    /// MD5 标识
    /// </summary>
	public string meshMd5;
    
    /// <summary>
    /// 文件路径
    /// </summary>
	public string filePath;

    /// <summary>
    /// 文件流
    /// </summary>
	public static Stream PibinFile;

    public Vector3 center;
    public Vector3 extents;
    public bool meshBox = false;

    /// <summary>
    /// geometry 数据
    /// </summary>
	public GlTF_Geometry geometry = new GlTF_Geometry();

    /// <summary>
    /// 获得 Mesh 命名 “mesh_xx”
    /// </summary>
    /// <param name="o"></param>
    /// <returns></returns>
	public static string GetNameFromObject(Object o)
	{
		return "mesh_" + GlTF_Writer.GetNameFromObject(o);
	}

    /// <summary>
    /// 构造函数
    /// </summary>
    public GlTF_Mesh()
    {
        primitives = new List<GlTF_Primitive>();
    }

    /// <summary>
    /// 填充 Mesh 数据
    /// </summary>
    /// <param name="m"></param>
    /// <param name="path"></param>
	public void Populate (Mesh m, string path)
	{
        /// 属性数据
		byte[] attrByte         = primitives[0].attributes.getAttrByte(m, geometry);
        /// 图元顶点序号数组
		byte[][] indiceByte     = new byte[primitives.Count][];

        /// 收集 indice 数据到 indiceByte[][]
		foreach (GlTF_Primitive p in primitives)
		{
			if(!combineMesh){
				p.indices.PibufferView.initPi();
			}

            /// 获取目标 图元 顶点序号数据
			int[] indice = m.GetTriangles(p.index);
			int[] resArr = new int[ indice.Length + 1];
			int[] defArr = new int[1]{p.materialIndex};

            /// 顶点序号数据
			indice.CopyTo(resArr, 0);
            /// 图元的材质序号
			defArr.CopyTo(resArr, indice.Length);

			indiceByte[p.index] = GlTF_Md5.getByte(resArr);
		}

		meshMd5             = GlTF_Md5.byteToMd5( GlTF_Md5.combine(attrByte, GlTF_Md5.combine(indiceByte)));
		geometry.GeoHash    = meshMd5;
		GlTF_Geometry geo   = geometryList.Find (x => x.GeoHash == meshMd5);

        /// 没有网格信息
		if (geo != null )
        {
			primitives = geo.primitives;
			return;
		}

        /// 合并 - 修改MD5
		if (combineMesh) {
			meshMd5 = Path.GetFileNameWithoutExtension(path) + ".mesh.bin";
		}

		if (primitives.Count > 0)
		{
			/// only populate first attributes because the data are shared between primitives仅填充第一个属性，因为数据在基元之间共享
			primitives[0].attributes.Populate(m, meshMd5);
		}

		foreach (GlTF_Primitive p in primitives)
		{
			p.Populate (m, meshMd5);
		}

		geometry.primitives     = primitives;

		geometryList.Add(geometry);

		if (combineMesh) {
			return;
		}

		GlTF_Buffer bin = GlTF_Writer.buffers.Find(buffer => buffer.uri.Contains(meshMd5));
		if (bin == null) {
			GlTF_Buffer a = new GlTF_Buffer ();

			PifloatBufferView.byteOffset        = 0;
			Pivec2BufferView.byteOffset         = PifloatBufferView.byteOffset      + PifloatBufferView.byteLength;
			Pivec3BufferView.byteOffset         = Pivec2BufferView.byteOffset       + Pivec2BufferView.byteLength;
			Pivec4BufferView.byteOffset         = Pivec3BufferView.byteOffset       + Pivec3BufferView.byteLength;
			Pivec4UshortBufferView.byteOffset   = Pivec4BufferView.byteOffset       + Pivec4BufferView.byteLength;
			PiushortBufferView.byteOffset       = Pivec4UshortBufferView.byteOffset + Pivec4UshortBufferView.byteLength;

			filePath    = Path.Combine(Path.GetDirectoryName(path), meshMd5 + ".mesh.bin");
			PibinFile   = File.Open(filePath, FileMode.Create);
			a.uri       = meshMd5 + ".mesh.bin";

			//floatBufferView.PimemoryStream.WriteTo(PibinFile);
			Pivec2BufferView.PimemoryStream.WriteTo(PibinFile);
			Pivec3BufferView.PimemoryStream.WriteTo(PibinFile);
			Pivec4BufferView.PimemoryStream.WriteTo(PibinFile);

			if (m.boneWeights.Length > 0)
            {
				Pivec4UshortBufferView.PimemoryStream.WriteTo(PibinFile);
			}

			PiushortBufferView.PimemoryStream.WriteTo(PibinFile);

			a.BLength           = PibinFile.Length;
			a.BufferviewIndex   = buffers.Count;

			PibinFile.Flush();
			PibinFile.Close();

			buffers.Add(a);
		}
	}

	public override void Write ()
	{
		Indent();	jsonWriter.Write ("{\n");
		IndentIn();
		Indent();	jsonWriter.Write ("\"name\": \"" + name.Replace("mesh_","") + "\",\n");
		Indent();	jsonWriter.Write ("\"geometry\": \"" + meshMd5 + "\",\n");
        Indent();   jsonWriter.Write ("\"alphaIndex\": " + alphaIndex + ",\n");
        if (meshBox)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"boundingbox\":{\n");
            IndentIn();
            Indent(); jsonWriter.Write("\"center\": [" + center.x.ToString() + ", " + center.y.ToString() + ", " + center.z.ToString() + "],\n");
            Indent(); jsonWriter.Write("\"extents\": [" + (extents.x / 2.0f) + ", " + (extents.y / 2.0f) + ", " + (extents.z / 2.0f) + "]\n");
            IndentOut();
            Indent(); jsonWriter.Write("},\n");
        }
        Indent();	jsonWriter.Write ("\"primitives\": [\n");
		IndentIn();
		foreach (GlTF_Primitive p in primitives)
		{
			CommaNL();
			Indent();	jsonWriter.Write ("{\n");
			p.Write ();
			Indent();	jsonWriter.Write ("}");
		}
		jsonWriter.WriteLine();
		IndentOut();
		Indent();	jsonWriter.Write ("]\n");
		IndentOut();
		Indent();	jsonWriter.Write ("}");
	}
}

public class GlTF_Geometry:GlTF_Writer{
	public string GeoHash;
	public List<GlTF_Primitive> primitives;
}
#endif