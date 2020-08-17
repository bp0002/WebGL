#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using Assets.ExporterGLTF20;

public class GlTF_DirectionalLight : GlTF_Light {
	public GlTF_DirectionalLight () { type = "directional"; }
    public LightShadowGenerator shadowGenerator;
    public override void Write()
	{
		IndentIn();
		CommaNL();
		Indent();	jsonWriter.Write ("{\n");
		IndentIn();
		CommaNL();
		color.Write();
        if (shadowGenerator != null)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"shadowGenerator\": {");
            IndentIn();

            CommaNL();
            Indent(); jsonWriter.Write("\"bias\":" + shadowGenerator.bias);

            CommaNL();
            Indent(); jsonWriter.Write("\"mapSize\":" + shadowGenerator.mapSize);

            IndentOut();
            Indent(); jsonWriter.Write("}");
        }
        CommaNL();
        Indent(); jsonWriter.Write("\"name\":\"" + name + "\"");
        CommaNL();
		Indent();	jsonWriter.Write ("\"type\":\""+ type +"\"");
		CommaNL();
		Indent();	jsonWriter.Write ("\"intensity\":"+ intensity);
        // CommaNL();
        // Indent(); jsonWriter.Write("\"position\":" + position);
        // CommaNL();
        // Indent(); jsonWriter.Write("\"direction\":" + direction + "\n");
        IndentOut();
		Indent();	jsonWriter.Write ("}");
		IndentOut();
	}
}
#endif