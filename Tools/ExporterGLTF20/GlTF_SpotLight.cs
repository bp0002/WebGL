#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class GlTF_SpotLight : GlTF_Light {
	public float constantAttenuation = 1f;
	public float fallOffAngle = 3.1415927f;
	public float fallOffExponent = 0f;
	public float linearAttenuation = 0f;
	public float quadraticAttenuation = 0f;

	public GlTF_SpotLight () { type = "spot"; }

	public override void Write()
	{
		IndentIn();
		CommaNL();
		Indent();	jsonWriter.Write ("{\n");
		IndentIn();
		CommaNL();
		color.Write();
		CommaNL();
		Indent();	jsonWriter.Write ("\"type\":\""+ type +"\"");
		CommaNL();
		Indent();	jsonWriter.Write ("\"spot\": { \n" );
		IndentIn();
		CommaNL();
		Indent();	jsonWriter.Write ("\"innerConeAngle\":"+ innerConeAngle/180 +"");
		CommaNL();
		Indent();	jsonWriter.Write ("\"outerConeAngle\":"+ innerConeAngle/180 +"");
		CommaNL();
		Indent();	jsonWriter.Write ("\"intensity\":"+ intensity +"\n");
		IndentOut();
		Indent();	jsonWriter.Write ("}");
		CommaNL();
		Indent();	jsonWriter.Write ("\"range\":"+ range +"\n");
		IndentOut();
		Indent();	jsonWriter.Write ("}");
		IndentOut();
	}
}
#endif