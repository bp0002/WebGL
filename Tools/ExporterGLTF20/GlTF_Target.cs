#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class GlTF_Target : GlTF_Writer {
	public string type;
	public int index;
	public new string id;
	public string path;
	public string mid;
	public string target;

	public GlTF_Target (string a){
		target = a;
	}

	public override void Write()
	{
		Indent();		jsonWriter.Write ("\"" + "target" + "\": {\n");
		IndentIn();
		/*if (type == "node") {
			
		} else if (type == "material") { 
			Indent ();
			jsonWriter.Write ("\"" + type + "\": " + GlTF_Writer.materialNames.IndexOf (mid) + ",\n");
		} else if (type == "camera") {
			Indent ();
			jsonWriter.Write ("\"" + type + "\": " + 0 + ",\n");
		} else {
			Indent ();
			jsonWriter.Write ("\"" + type + "\": " + GlTF_Writer.textureNames.IndexOf (id) + ",\n");
		}*/

		Indent ();
		jsonWriter.Write ("\"node\": " + GlTF_Writer.nodeNames.IndexOf (target) + ",\n");
		Indent();		jsonWriter.Write ("\"path\": \"" + path + "\"\n");
		IndentOut();
		Indent();		jsonWriter.Write ("}");
	}
}
#endif