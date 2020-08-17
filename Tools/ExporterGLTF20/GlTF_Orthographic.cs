#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class GlTF_Orthographic : GlTF_Camera {
	public float size;
	public float zfar;
	public float znear;
	public GlTF_Orthographic() { type = "orthographic"; }
	public override void Write ()
	{
		Indent();		jsonWriter.Write ("{\n");
		IndentIn();
		Indent();		jsonWriter.Write ("\"orthographic\": {\n");
		IndentIn();
		Indent();		jsonWriter.Write ("\"size\": "+size+",\n");
		Indent();		jsonWriter.Write ("\"zfar\": "+zfar.ToString()+",\n");
		Indent();		jsonWriter.Write ("\"znear\": "+znear.ToString()+",\n");
		Indent();		jsonWriter.Write("\"name\": \"" + name + "\"\n");
		IndentOut();
		Indent();		jsonWriter.Write ("},\n");
		Indent();		jsonWriter.Write ("\"type\": \"orthographic\"\n");
		IndentOut();
		Indent();		jsonWriter.Write ("}");
	}
}
#endif