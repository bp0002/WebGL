#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class GlTF_PointLight : GlTF_Light {
	public float constantAttenuation = 1f;
	public float linearAttenuation = 0f;
	public float quadraticAttenuation = 0f;

	public GlTF_PointLight () { type = "point"; }

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
		Indent();	jsonWriter.Write ("\"intensity\":"+ intensity);
		CommaNL();
		Indent();	jsonWriter.Write ("\"range\":"+ range +"\n");
		IndentOut();
		Indent();	jsonWriter.Write ("}");
		IndentOut();
	}
}
#endif