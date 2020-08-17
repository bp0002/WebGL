#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class GlTF_Accessor : GlTF_Writer {
    /// <summary>
    /// 数据类型 - Accessor 类型 - 枚举;
    /// 数据类型的字符串。
    /// "SCALAR"表示标量值，
    /// "VEC3"表示3D向量，
    /// "MAT4"表示4x4矩阵
    /// </summary>
	public enum Type {
		SCALAR,
		VEC2,
		VEC3,
		VEC4,
		MAT4
	}

    /// <summary>
    /// 数据值类型 枚举;
    /// 引用的bufferView的数据类型;
    /// 数据分量的基础类型
    /// GL enum vals ": BYTE (5120), UNSIGNED_BYTE (5121), SHORT (5122), UNSIGNED_SHORT (5123), FLOAT (5126)
    /// </summary>
    public enum ComponentType {
		BYTE            = 5120,
		UNSIGNED_BYTE   = 5121,
		SHORT           = 5122,
		USHORT          = 5123,
		UNSIGNED_INT    = 5125,
		FLOAT           = 5126,
	}

    /// <summary>
    /// 动画是否放入GLTF_bufferviews
    /// </summary>
	public bool isAnimInBufferview = true;//
    /// <summary>
    /// 是否已填充数据
    /// </summary>
	public bool isPopulate;
    /// <summary>
    /// 数据 Hash
    /// </summary>
	public string HashStr;
    /// <summary>
    /// 访问器里 重载的 BufferView
    /// </summary>
	public GlTF_Pi_BufferView PibufferView;
    /// <summary>
    /// BufferView 序号
    /// </summary>
	public int bufferViewIndex;
    /// <summary>
    /// 数据起始点的偏移
    /// </summary>
	public long byteOffset; //": 0,
    /// <summary>
    /// 数据类型
    /// </summary>
	public ComponentType componentType; // GL enum vals ": BYTE (5120), UNSIGNED_BYTE (5121), SHORT (5122), UNSIGNED_SHORT (5123), FLOAT (5126)
    /// <summary>
    /// 数量
    /// </summary>
	public int count;//": 2399,
    /// <summary>
    /// 访问器类型
    /// </summary>
	public Type type = Type.SCALAR;

	public Vector2 scaleValues;
	public Vector2 offsetValues;

	Vector4 maxFloat;
	Vector4 minFloat;

	Matrix4x4 minMatrix = new Matrix4x4();
	Matrix4x4 maxMatrix = new Matrix4x4();

	int minInt;
	int maxInt;

    /// <summary>
    /// 构造1 - 默认 type(SCALAR) / componentType()
    /// </summary>
    /// <param name="n"></param>
    public GlTF_Accessor(string n) { 
		id          = n; 
		isPopulate  = false;
		HashStr     = "";
	}

    /// <summary>
    /// 构造2 - 指定 type / componentType
    /// </summary>
    /// <param name="n"></param>
    /// <param name="t"></param>
    /// <param name="c"></param>
    public GlTF_Accessor(string n, Type t, ComponentType c) {
		id              = n;
		type            = t;
		componentType   = c;
		isPopulate      = false;
		HashStr         = "";
	}

	public static string GetNameFromObject(Object o, string name)
	{
		return "accessor_" + name + "_"+ GlTF_Writer.GetNameFromObject(o, true);
	}

	void InitMinMaxInt()
	{
		maxInt  = int.MinValue;
		minInt  = int.MaxValue;
	}

	void InitMinMaxFloat()
	{
		float min   = float.MinValue;
		float max   = float.MaxValue;
		maxFloat    = new Vector4(min, min, min, min);
		minFloat    = new Vector4(max, max, max, max);
	}

	public void PopulateWithOffsetScale(Vector2[] v2s, bool flip,string str = "")
	{
		Vector2[] uv2 = v2s;
		for(int i=0;  i< uv2.Length; ++i)
		{
			float u = uv2[i][0] * scaleValues[0] + offsetValues[0];
			float v = uv2[i][1] * scaleValues[1] + offsetValues[1];
			uv2[i] = new Vector2(u, v);
		}
		Populate(uv2, flip);
	}

    /// <summary>
    /// 填充 - 三角形数据
    /// </summary>
    /// <param name="vs"></param> 顶点序号列表
    /// <param name="flippedTriangle"></param> 是否翻转三角形
    /// <param name="str"></param>
	public void Populate (int[] vs, bool flippedTriangle,string str = "")
	{
		if (isPopulate)
			return;

		isPopulate  = true;
		PibufferView.PibufferIndex = buffers.Count;

		if (type != Type.SCALAR)
			throw (new System.Exception());

        /// 写入前获取起点偏移
		byteOffset  = PibufferView.PicurrentOffset;
        /// 写入数据
		PibufferView.Populate(vs, flippedTriangle, str);

		if (isAnimInBufferview) {
			PibufferViews.Add(PibufferView);
		}

        /// 顶点数量
		count = vs.Length;

		if (count > 0)
		{
			InitMinMaxInt();
            /// 最大 / 最小 顶点序号
			for (int i = 0; i < count; ++i)
			{
				minInt = Mathf.Min(vs[i], minInt);
				maxInt = Mathf.Max(vs[i], maxInt);
			}
		}
	}

    /// <summary>
    /// 填充 - float 数组数据
    /// </summary>
    /// <param name="vs"></param> 顶点数据列表
    /// <param name="str"></param>
	public void Populate (float[] vs,string str = "")
	{
		if (isPopulate)
			return;

		isPopulate = true;

		if (type != Type.SCALAR)
			throw (new System.Exception());

        /// 写入前获取起点偏移
        byteOffset = PibufferView.PicurrentOffset;
        /// 设置当前 BufferView 序号
        PibufferView.PibufferIndex = buffers.Count;
        /// 写入 顶点数据
        PibufferView.Populate(vs, str);

		if (isAnimInBufferview)
        {
			PibufferViews.Add (PibufferView);
		}

		count = vs.Length;
		if (count > 0)
		{
			InitMinMaxFloat();
			for (int i = 0; i < count; ++i)
			{
				minFloat.x = Mathf.Min(vs[i], minFloat.x);
				maxFloat.x = Mathf.Max(vs[i], maxFloat.x);
			}
		}
	}

    /// <summary>
    /// 填充 - Vector2 数组数据 - UV0 / UV1
    /// </summary>
    /// <param name="v2s"></param>
    /// <param name="flip">是否反转 Y 值</param>
    /// <param name="str"></param>
	public void Populate (Vector2[] v2s, bool flip = false,string str = "")
	{
		if (isPopulate)
			return;

		isPopulate = true;

		if (type != Type.VEC2)
			throw (new System.Exception());

        /// Vector2 数量
        count = v2s.Length;
        /// 写入前获取 起点偏移
		byteOffset = PibufferView.PicurrentOffset;
        /// 设置当前 BufferView 序号
        PibufferView.PibufferIndex = buffers.Count;

		if (count > 0)
		{
			InitMinMaxFloat();
			if (flip)
			{
				for (int i = 0; i < v2s.Length; i++)
				{
                    /// 填入 UV - U
					PibufferView.Populate(v2s[i].x, str);

                    /// 反转Y方向数据
					float y = 1.0f - v2s[i].y;

                    /// 填入 UV - V
                    PibufferView.Populate(y, str);

					minFloat.x = Mathf.Min(v2s[i].x, minFloat.x);
					minFloat.y = Mathf.Min(y, minFloat.y);

					maxFloat.x = Mathf.Max(v2s[i].x, maxFloat.x);
					maxFloat.y = Mathf.Max(y, maxFloat.y);
				}
			}
            else
            {
				for (int i = 0; i < v2s.Length; i++)
				{
					PibufferView.Populate (v2s [i].x, str);
					PibufferView.Populate (v2s [i].y, str);

					minFloat.x = Mathf.Min(v2s[i].x, minFloat.x);
					minFloat.y = Mathf.Min(v2s[i].y, minFloat.y);
					maxFloat.x = Mathf.Max(v2s[i].x, maxFloat.x);
					maxFloat.y = Mathf.Max(v2s[i].y, maxFloat.y);
				}
			}
		}

		if (isAnimInBufferview)
        {
			PibufferViews.Add(PibufferView);
		}
	}

    /// <summary>
    /// 填充 - Vector3 数组数据 - 顶点数据/定位/旋转/缩放/粒子信息/target。。。
    /// </summary>
    /// <param name="v3s"></param>
    /// <param name="noConvert"></param>
    /// <param name="str"></param>
	public void Populate (Vector3[] v3s, bool doConvert = false, string str="")
	{
		if (isPopulate)
			return;

		isPopulate = true;

		if (type != Type.VEC3)
			throw (new System.Exception());
		
		if (PibufferView != null) {
            /// 获取起点偏移
			byteOffset                  = PibufferView.PicurrentOffset;
            /// 设置序号
			PibufferView.PibufferIndex  = buffers.Count;
		}

		count = v3s.Length;
		if (count > 0)
		{
			InitMinMaxFloat();

			for (int i = 0; i < v3s.Length; i++)
			{
                /// 坐标点 - 左手坐标系 转 右手坐标系
				if (doConvert)
                {
                    convertVector3LeftToRightHandedness(ref v3s[i]);
                }

                /// 填入数据 - 点数据
				PibufferView.Populate(v3s[i].x, str);
				PibufferView.Populate(v3s[i].y, str);
				PibufferView.Populate(v3s[i].z, str);

				minFloat.x = Mathf.Min(v3s[i].x, minFloat.x);
				minFloat.y = Mathf.Min(v3s[i].y, minFloat.y);
				minFloat.z = Mathf.Min(v3s[i].z, minFloat.z);

				maxFloat.x = Mathf.Max(v3s[i].x, maxFloat.x);
				maxFloat.y = Mathf.Max(v3s[i].y, maxFloat.y);
				maxFloat.z = Mathf.Max(v3s[i].z, maxFloat.z);
			}
		}

		if (isAnimInBufferview)
        {
			PibufferViews.Add (PibufferView);
		}
	}

    /// <summary>
    /// 填充数据 - Vector4 - - 四元旋转量
    /// </summary>
    /// <param name="v4s"></param>
    /// <param name="noConvert"></param>
    /// <param name="useUInt"></param>
    /// <param name="str"></param>
	public void PopulateShort(Vector4[] v4s, bool noConvert = true, bool useUInt = false,string str="")
	{
		if (isPopulate)
			return;

		isPopulate = true;

		if (type != Type.VEC4)
			throw (new System.Exception());

        count                       = v4s.Length;
        byteOffset                  = PibufferView.PicurrentOffset;
		PibufferView.PibufferIndex  = buffers.Count;

		if (count > 0)
		{
			InitMinMaxFloat();
			for (int i = 0; i < v4s.Length; i++)
			{
				PibufferView.PopulateShort ((ushort)v4s [i].x,str);
				PibufferView.PopulateShort ((ushort)v4s [i].y,str);
				PibufferView.PopulateShort ((ushort)v4s [i].z,str);
				PibufferView.PopulateShort ((ushort)v4s [i].w,str);

				minFloat.x = Mathf.Min(v4s[i].x, minFloat.x);
				minFloat.y = Mathf.Min(v4s[i].y, minFloat.y);
				minFloat.z = Mathf.Min(v4s[i].z, minFloat.z);
				minFloat.w = Mathf.Min(v4s[i].w, minFloat.w);

				maxFloat.x = Mathf.Max(v4s[i].x, maxFloat.x);
				maxFloat.y = Mathf.Max(v4s[i].y, maxFloat.y);
				maxFloat.z = Mathf.Max(v4s[i].z, maxFloat.z);
				maxFloat.w = Mathf.Max(v4s[i].w, maxFloat.w);
			}
		}

		if(isAnimInBufferview){
			PibufferViews.Add (PibufferView);
		}
	}

    /// <summary>
    /// Vector4 - 四元旋转量
    /// </summary>
    /// <param name="v4s"></param>
    /// <param name="noConvert"></param>
    /// <param name="useUInt"></param>
    /// <param name="str"></param>
	public void Populate (Vector4[] v4s, bool noConvert = true, bool useUInt = false, string str = "")
	{
		if (isPopulate)
			return;

		isPopulate = true;

		if (type != Type.VEC4)
			throw (new System.Exception());
		
		byteOffset = PibufferView.PicurrentOffset;
		PibufferView.PibufferIndex = buffers.Count;
		count = v4s.Length;

		if (count > 0)
		{
			InitMinMaxFloat();
			for (int i = 0; i < v4s.Length; i++)
			{
				if (convertRightHanded && !noConvert)
					convertVector4LeftToRightHandedness(ref v4s[i]);

				if (useUInt)
				{
					PibufferView.Populate ((uint)v4s [i].x, str);
					PibufferView.Populate ((uint)v4s [i].y, str);
					PibufferView.Populate ((uint)v4s [i].z, str);
					PibufferView.Populate ((uint)v4s [i].w, str);
				}
				else
				{
					PibufferView.Populate (v4s [i].x, str);
					PibufferView.Populate (v4s [i].y, str);
					PibufferView.Populate (v4s [i].z, str);
					PibufferView.Populate (v4s [i].w, str);
				}

				minFloat.x = Mathf.Min(v4s[i].x, minFloat.x);
				minFloat.y = Mathf.Min(v4s[i].y, minFloat.y);
				minFloat.z = Mathf.Min(v4s[i].z, minFloat.z);
				minFloat.w = Mathf.Min(v4s[i].w, minFloat.w);

				maxFloat.x = Mathf.Max(v4s[i].x, maxFloat.x);
				maxFloat.y = Mathf.Max(v4s[i].y, maxFloat.y);
				maxFloat.z = Mathf.Max(v4s[i].z, maxFloat.z);
				maxFloat.w = Mathf.Max(v4s[i].w, maxFloat.w);
			}
		}
		if(isAnimInBufferview){
			PibufferViews.Add (PibufferView);
		}
	}
    /// <summary>
    /// 填充颜色信息
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="str"></param>
	public void Populate(Color[] colors, string str="")
	{
		if (isPopulate)
			return;

		isPopulate = true;

		if (type != Type.VEC4)
			throw (new System.Exception());

        count                       = colors.Length;
        byteOffset                  = PibufferView.PicurrentOffset;
		PibufferView.PibufferIndex  = buffers.Count;

		if (count > 0)
		{
			InitMinMaxFloat();
			for (int i = 0; i < colors.Length; i++)
			{
				PibufferView.Populate (colors [i].r, str);
				PibufferView.Populate (colors [i].g, str);
				PibufferView.Populate (colors [i].b, str);
				PibufferView.Populate (colors [i].a, str);

				minFloat.x = Mathf.Min(colors[i].r, minFloat.x);
				minFloat.y = Mathf.Min(colors[i].g, minFloat.y);
				minFloat.z = Mathf.Min(colors[i].b, minFloat.z);
				minFloat.w = Mathf.Min(colors[i].a, minFloat.w);

				maxFloat.x = Mathf.Max(colors[i].r, maxFloat.x);
				maxFloat.y = Mathf.Max(colors[i].g, maxFloat.y);
				maxFloat.z = Mathf.Max(colors[i].b, maxFloat.z);
				maxFloat.w = Mathf.Max(colors[i].a, maxFloat.w);
			}
		}
		if (isAnimInBufferview)
        {
			PibufferViews.Add(PibufferView);
		}
	}
    /// <summary>
    /// 填充矩阵数据 - 变换信息
    /// </summary>
    /// <param name="matrices"></param>
    /// <param name="m"></param>
    /// <param name="str"></param>
	public void Populate(Matrix4x4[] matrices, Transform m, string str="")
	{
		if (isPopulate)
			return;

		isPopulate = true;

		if (type != Type.MAT4)
			throw (new System.Exception());

        count                       = matrices.Length;
        byteOffset                  = PibufferView.PicurrentOffset;
        PibufferView.PibufferIndex  = buffers.Count;

		if(count > 0)
		{
			for(int i = 0; i < matrices.Length; i++)
			{
				Matrix4x4 mat = matrices[i];

				// This code is buggy, don't use it for now.
				//if (convertRightHanded)
				//	convertMatrixLeftToRightHandedness(ref mat);
				for (int j = 0; j < 4; j++)
				{
					for(int k=0; k < 4; k++)
					{
						// Matrices in unity are column major
						// as for Gltf
						float value = mat[k, j];

						PibufferView.Populate(value, str);

						minMatrix[k, j] = Mathf.Min(value, minMatrix[k, j]);
						maxMatrix[k, j] = Mathf.Max(value, maxMatrix[k, j]);
					}
				}
			}
		}
		if (isAnimInBufferview) {
			PibufferViews.Add(PibufferView);
		}
	}

	void WriteMin()
	{
		if (componentType == ComponentType.FLOAT)
		{
			switch (type)
			{
				case Type.SCALAR:
					jsonWriter.Write (minFloat.x);
				    break;

				case Type.VEC2:
					jsonWriter.Write (minFloat.x + ", " + minFloat.y);
				    break;

				case Type.VEC3:
					jsonWriter.Write (minFloat.x + ", " + minFloat.y + ", " + minFloat.z);
				    break;

				case Type.VEC4:
					jsonWriter.Write (minFloat.x + ", " + minFloat.y + ", " + minFloat.z + ", " + minFloat.w);
				    break;

				case Type.MAT4:
					for (int i = 0; i < 15; ++i)
					{
						jsonWriter.Write(minMatrix[i] + ", ");
					}
					jsonWriter.Write(minMatrix[15]);
				    break;
			}
		}
		else if (componentType == ComponentType.USHORT || componentType == ComponentType.UNSIGNED_INT)
		{
			if (type == Type.SCALAR)
			{
				jsonWriter.Write(minInt);
			}
			else if (type == Type.VEC4)
			{
				jsonWriter.Write((int)minFloat.x + ", " + (int)minFloat.y + ", " + (int)minFloat.z + ", " + (int)minFloat.w);
			}
		}
	}

	void WriteMax()
	{
		if (componentType == ComponentType.FLOAT)
		{
			switch (type)
			{
			    case Type.SCALAR:
				    jsonWriter.Write (maxFloat.x);
				    break;

			    case Type.VEC2:
				    jsonWriter.Write (maxFloat.x + ", " + maxFloat.y);
				    break;

			    case Type.VEC3:
				    jsonWriter.Write (maxFloat.x + ", " + maxFloat.y + ", " + maxFloat.z);
				    break;

			    case Type.VEC4:
				    jsonWriter.Write (maxFloat.x + ", " + maxFloat.y + ", " + maxFloat.z + ", " + maxFloat.w);
				    break;

		        case Type.MAT4:
				    for(int i=0; i < 15; ++i)
				    {
					    jsonWriter.Write(maxMatrix[i] + ", ");
				    }
				    jsonWriter.Write(maxMatrix[15]);
				    break;
			}
		}
		else if (componentType == ComponentType.USHORT || componentType == ComponentType.UNSIGNED_INT)
		{
			if (type == Type.SCALAR)
			{
				jsonWriter.Write(maxInt);
			}
			else if(type == Type.VEC4)
			{
				jsonWriter.Write((int)maxFloat.x + ", " + (int)maxFloat.y + ", " + (int)maxFloat.z + ", " + (int)maxFloat.w);
			}
		}
	}

    /// <summary>
    /// Accessor 数据写
    /// </summary>
	public override void Write ()
	{
        /**
         * {
         *      "bufferView": 1,        // 引用了索引为 1 的 bufferView对象
         *      "byteOffset": 120,      // 多个accessor对象引用同一个bufferView对象的情况，byteOffset属性值指定了accessor所访问数据的开始位置
         *      "componentType": 5120,  // 5123(UNSIGNED_SHORT)
         *      "count": 100,           // 100 个 UNSIGNED_SHORT 的 SCALAR 数据
         *      "type": "SCALAR",       // type属性为"SCALAR"
         *      "byteStride": 4,        // bufferView对象所引用的数据可能以结构数组的形式进行存储，通过byteStride属性确定下一个引用数据的位置。byteStride属性用于指定两个数据元素之间的字节距离。
         * }
         */
        Indent();		jsonWriter.Write ("{\n");
		IndentIn();
		Indent();		jsonWriter.Write ("\"bufferView\": " + PibufferViews.IndexOf(PibufferView) +",\n");
		Indent();		jsonWriter.Write ("\"byteOffset\": " + byteOffset + ",\n");
		Indent();		jsonWriter.Write ("\"componentType\": " + (int)componentType + ",\n");
		Indent();		jsonWriter.Write ("\"count\": " + count + ",\n");

		/*Indent(); jsonWriter.Write("\"max\": [ ");
		WriteMax();
		jsonWriter.Write(" ],\n");
		Indent(); jsonWriter.Write("\"min\": [ ");
		WriteMin();
		jsonWriter.Write(" ],\n");*/

		Indent();		jsonWriter.Write ("\"type\": \"" + type + "\"\n");
		IndentOut();
		Indent();	jsonWriter.Write ("}");
	}
}
#endif