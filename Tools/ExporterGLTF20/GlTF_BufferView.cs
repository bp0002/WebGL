#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class GlTF_Pi_BufferView : GlTF_Writer  {

	public enum TARGET
	{
		ARRAY = 34962,
		ELEMENT = 34963
	}
    /// <summary>
    /// buffer 序号
    /// </summary>
	public int PibufferIndex;
    /// <summary>
    /// 数据长度
    /// </summary>
	public long byteLength;//": 25272,、
    /// <summary>
    /// 数据起点偏移量
    /// </summary>
	public long byteOffset;//": 0,

	public long byteStride;
    /// <summary>
    /// 数据目标类型
    /// </summary>
	public int target= -1;
    /// <summary>
    /// 数据名称
    /// </summary>
	public string PiName;

	//	public string target = "ARRAY_BUFFER";
	public int PicurrentOffset          = 0;
    /// <summary>
    /// 是否为二进制
    /// </summary>
	public bool bin                     = false;
    /// <summary>
    /// 数据内存操作流
    /// </summary>
	public MemoryStream PimemoryStream  = new MemoryStream();

	public GlTF_Pi_BufferView (string n, int s) {
        name = n;
        byteStride = s;
    }

	public GlTF_Pi_BufferView (string n, int s, int t) {
        name = n;
        byteStride = s;
        target = t;
    }

    /// <summary>
    /// 初始化 PI 相关数据
    /// </summary>
	public void initPi(){
		PicurrentOffset     = 0;
		PimemoryStream      = new MemoryStream();
	}
    /// <summary>
    /// 填充数据 - byte[]
    /// </summary>
    /// <param name="vs"></param>
    /// <param name="str"></param>
	public void Populate(byte[] vs, string str="")
	{
		if (str!="") PiName = str;

		for (int i = 0; i < vs.Length; i++)
		{
			float f = vs[i];

			PimemoryStream.Write(BitConverter.GetBytes(f), 0, BitConverter.GetBytes(f).Length);
			PicurrentOffset += 2;//TODO
		}

		byteLength = PicurrentOffset;
	}

    /// <summary>
    /// 填充数据 - int[] - 图元顶点序号数组
    /// </summary>
    /// <param name="vs"></param>
    /// <param name="flippedTriangle"></param>
    /// <param name="str"></param>
	public void Populate(int[] vs, bool flippedTriangle, string str="")
	{
		if (str!="") PiName = str;

		if (flippedTriangle)
		{
            /// 3 个 点 组成一个 图元 - 三角形
            /// 原始 0 - 1 - 2 -> 0 - 2 - 1  即反转三角形
			for (int i = 0; i < vs.Length; i += 3)
			{
				ushort u = (ushort)vs[i];
				PimemoryStream.Write(BitConverter.GetBytes(u), 0, BitConverter.GetBytes(u).Length);
				PicurrentOffset += 2;

				u = (ushort)vs[i+2];
				PimemoryStream.Write(BitConverter.GetBytes(u), 0, BitConverter.GetBytes(u).Length);
				PicurrentOffset += 2;

				u = (ushort)vs[i+1];
				PimemoryStream.Write(BitConverter.GetBytes(u), 0, BitConverter.GetBytes(u).Length);
				PicurrentOffset += 2;
			}
		}
		else
		{
			for (int i = 0; i < vs.Length; i++)
			{
				ushort u = (ushort)vs[i];
				PimemoryStream.Write(BitConverter.GetBytes(u), 0, BitConverter.GetBytes(u).Length);
				PicurrentOffset += 2;
			}
		}

        /// 数据对齐
		if (PicurrentOffset % 4 != 0){
			PimemoryStream.Write(BitConverter.GetBytes((ushort)0), 0, BitConverter.GetBytes((ushort)0).Length);
			PicurrentOffset += 2;
		}

		byteLength = PicurrentOffset;
	}

    /// <summary>
    /// 填充数据
    /// </summary>
    /// <param name="vs"></param>
    /// <param name="str"></param>
	public void PopulateShort(ushort vs,string str="")
	{
		if (str!="") PiName = str;

		ushort u = (ushort)vs;

		PimemoryStream.Write(BitConverter.GetBytes(u), 0, BitConverter.GetBytes(u).Length);
		PicurrentOffset += 2;
		byteLength      = PicurrentOffset;
	}

    /// <summary>
    /// 填充数据 - 顶点数据
    /// </summary>
    /// <param name="vs"></param>
    /// <param name="str"></param>
	public void Populate (float[] vs,string str="")
	{
		if (str!="") PiName = str;

		for (int i = 0; i < vs.Length; i++)
		{
			float f = vs[i];
			PimemoryStream.Write (BitConverter.GetBytes(f), 0, BitConverter.GetBytes(f).Length);
			PicurrentOffset += 4;
		}

		byteLength = PicurrentOffset;
	}

    /// <summary>
    /// 填充数据 - 
    /// </summary>
    /// <param name="v"></param>
    /// <param name="str"></param>
	public void Populate(uint v,string str="")
	{
		if (str!="") PiName = str;

		PimemoryStream.Write (BitConverter.GetBytes(v), 0, BitConverter.GetBytes(v).Length);

		PicurrentOffset += 4;
		byteLength      = PicurrentOffset;
	}

    /// <summary>
    /// 填充数据 - float - 顶点数据/Color数据/变换数据...
    /// </summary>
    /// <param name="v"></param>
    /// <param name="str"></param>
	public void Populate(float v,string str="")
	{
		if (str!="") PiName = str;

        var bytes = BitConverter.GetBytes(v);

        PimemoryStream.Write(bytes, 0, bytes.Length);

		PicurrentOffset += 4;
		byteLength      = PicurrentOffset;
	}

    /// <summary>
    /// 数据写入文件
    /// </summary>
	public override void Write ()
	{
		/*
		"bufferView_4642": {
			"buffer": "vc.bin",
			"byteLength": 630080,
			"byteOffset": 0,
			"target": "ARRAY_BUFFER"
		},
	    */

		Indent();		jsonWriter.Write ("{\n");
		IndentIn();

		//var binName = binary ? "binary_glTF" : Path.GetFileNameWithoutExtension(GlTF_Writer.binFileName);
		GlTF_Buffer bin = null;

		if(PiName != null){
			bin = GlTF_Writer.buffers.Find (buffer => buffer.uri.Contains( PiName));
		}

		if(bin != null){
			Indent();		jsonWriter.Write ("\"buffer\": " + bin.BufferviewIndex  +",\n");
		}else{
			Indent();		jsonWriter.Write ("\"buffer\": " + PibufferIndex  +",\n");
		}

		Indent();		jsonWriter.Write ("\"byteLength\": " + byteLength + ",\n");
		/*if ((int)target != (int)-1)
		{
			Indent(); jsonWriter.Write("\"target\": " + target + ",\n");
		}*/

		if (byteStride >= 4)
		{
			Indent(); jsonWriter.Write("\"byteStride\": " + byteStride + ",\n");
		}

		Indent();		jsonWriter.Write ("\"byteOffset\": " + byteOffset + "\n");

		IndentOut();
		Indent();		jsonWriter.Write ("}");
	}
}
#endif