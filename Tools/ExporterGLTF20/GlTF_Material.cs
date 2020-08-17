#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.ExporterGLTF20;

public class GlTF_Material : GlTF_Writer {
	public bool matToRtpl = false;

    public bool useLightmapAsShadowmap = false;
    public bool useAlphaFromDiffuseTexture = false;
	public bool hasAlpha = false;
	public string cull = "back";
	public float[] HSB = new float[]{0,0,0};
	public float alpha = 1;
	public Color diffuseColor = new Color(0,0,0,0);
	public float renderQueue = 0;
	public string shaderName;
	public float srcBlend = -1;
	public float dstBlend = -1;
    public bool disableLighting = true;

    public float metailValue = 0.0f;
    public Color metailBaseColor = new Color(0, 0, 0, 0);
    public float metailRoughnessValue = 0.0f;

    public Color specularColor = new Color(0, 0, 0, 0);
    public Color specularDiffuseColor = new Color(0, 0, 0, 0);
    public float specularGlossinessValue = 0.0f;

    public Color pbrAlbedoColor = new Color(0, 0, 0, 0);
    public float pbrMetailValue = 0.0f;
    public float pbrRoughnessValue = 0.0f;
    public Color pbrReflectionColor = new Color(0, 0, 0, 0);

    public class Value : GlTF_Writer {
	}

	public class ColorValue : Value {
		public Color color;
		public bool isRGB = false;

		public override void Write()
		{
			jsonWriter.Write ("\"" + name + "\": [");
			jsonWriter.Write (color.r.ToString() + ", " + color.g.ToString() + ", " +color.b.ToString() + (isRGB ? "" : ", " + color.a.ToString()));
			jsonWriter.Write ("]");
		}
	}

	public class VectorValue : Value {
		public Vector4 vector;

		public override void Write()
		{
			jsonWriter.Write ("\"" + name + "\": [");
			jsonWriter.Write (vector.x.ToString() + ", " + vector.y.ToString() + ", " + vector.z.ToString() + ", " + vector.w.ToString());
			jsonWriter.Write ("]");
		}
	}

	public class FloatValue : Value {
		public float value;

		public override void Write()
		{
			jsonWriter.Write("\"" + name + "\": " + value);
		}
	}

	public class IntValue : Value
	{
		public int value;

		public override void Write()
		{
			jsonWriter.Write("\"" + name + "\": " + value);
		}
	}

	public class BoolValue : Value
	{
		public bool value;

		public override void Write()
		{
			jsonWriter.Write("\"" + name + "\": " + (value ? "true" : "false"));
		}
	}

	public class StringValue : Value {
		public string value;

		public override void Write()
		{
			jsonWriter.Write ("\"" + name + "\": \"" + value +"\"");
		}
	}

	public class DictValue: Value
	{
		public Dictionary<string, int> intValue;
		public Dictionary<string, float> floatValue;
		public Dictionary<string, string> stringValue;
		public DictValue()
		{
			intValue = new Dictionary<string, int>();
			floatValue = new Dictionary<string, float>();
			stringValue = new Dictionary<string, string>();
		}
		public override void Write()
		{
			jsonWriter.Write("\"" + name + "\" : {\n");
			IndentIn();

			foreach (string key in intValue.Keys)
			{
				CommaNL();
				Indent(); jsonWriter.Write("\"" + key + "\" : " + intValue[key]);
			}
			foreach (string key in floatValue.Keys)
			{
				CommaNL();
				Indent(); jsonWriter.Write("\"" + key + "\" : " + floatValue[key]);
			}
			foreach (string key in stringValue.Keys)
			{
				CommaNL();
				Indent(); jsonWriter.Write("\"" + key + "\" : " + stringValue[key]);
			}
			jsonWriter.Write("\n");
			IndentOut();
			Indent(); jsonWriter.Write("}");
		}
	}

	public int instanceTechniqueIndex;
	public bool isMetal = false;
	public float shininess;
	public List<Value> values = new List<Value>();
	public List<Value> pbrValues = new List<Value>();

	public static string GetNameFromObject(Object o)
	{
		return "material_" + GlTF_Writer.GetNameFromObject(o, true);
	}

	public override void Write()
	{
		Indent(); jsonWriter.Write("{\n");
		IndentIn();
		writeExtras();
		if (isMetal)
		{
			Indent(); jsonWriter.Write("\"pbrMetallicRoughness\": {\n");
		}
		else
		{
			Indent(); jsonWriter.Write("\"extensions\": {\n");
			IndentIn();

			//Indent(); jsonWriter.Write("\"KHR_materials_pbrSpecularGlossiness\": {\n");
			Indent(); jsonWriter.Write("\"PI_material\": {\n");
		}
		IndentIn();
		if(shaderName != null){
			CommaNL();
			Indent(); jsonWriter.Write("\"type\" :\"" + shaderName + "\"");
		}

		if(srcBlend != -1 && dstBlend != -1){
            if (shaderName == "Myshader/Particle")
            {
                CommaNL();
                Indent(); jsonWriter.Write("\"blend\" :[" + srcBlend + "," + dstBlend + "]");
            } else if (shaderName == "PiShader")
            {
                CommaNL();
                Indent(); jsonWriter.Write("\"alphaMode\" :" + AlphaMode.format((int)srcBlend, (int)dstBlend));
            }

        }

        if (shaderName == "PBR")
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"metallic\" :" + pbrMetailValue);
            CommaNL();
            Indent(); jsonWriter.Write("\"roughness\" :" + (1.0f - pbrRoughnessValue));
        }

        if (shaderName == "PBRMetallicRoughness")
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"metallic\" :" + metailValue);
            CommaNL();
            Indent(); jsonWriter.Write("\"roughness\" :" + (1.0f - pbrRoughnessValue));
        }

        if (shaderName == "PBRSpecularGlossiness")
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"glossiness\" :" + specularGlossinessValue);
        }

        if (shaderName == "PiShader")
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"disableLighting\" :" + (disableLighting ? "true" : "false"));
        }

        if (useLightmapAsShadowmap)
        {
            CommaNL();
            Indent(); jsonWriter.Write("\"" + "useLightmapAsShadowmap" + "\" : " + "true");
        }

        if (useAlphaFromDiffuseTexture){
			CommaNL();
			Indent(); jsonWriter.Write("\"" + "useAlphaFromDiffuseTexture" + "\" : " + "true");
		}

		if(cull != "back"){
			CommaNL();
			Indent(); jsonWriter.Write ("\"cull\": \"" + cull + "\"");
		}

		if(alpha != 1){
			CommaNL();
			Indent(); jsonWriter.Write("\"" + "alpha" + "\" : " + alpha);
		}
		if(renderQueue != 0){
			CommaNL();
			Indent(); jsonWriter.Write("\"" + "renderQueue" + "\" : " + renderQueue);
		}

        //TODO
        /*if(HSB[0] !=0 || HSB[1] !=0 || HSB[2] !=0 ){
			CommaNL();
			Indent(); jsonWriter.Write("\"HSB\": [ ");
			jsonWriter.Write (HSB[0]*360+180 + ", " + HSB[1]*100 + ", " + HSB[2]*100);
			jsonWriter.Write(" ]");
		}*/

        foreach (var v in values)
        {
            CommaNL();
            Indent(); v.Write();
        }
        foreach (var v in pbrValues)
		{
			CommaNL();
			Indent(); v.Write();
		}
		if (!isMetal)
		{
			IndentOut();
			Indent(); jsonWriter.Write("}");
		}

		jsonWriter.Write("\n");
		IndentOut();
		Indent(); jsonWriter.Write("},\n");

		// write common values
		//foreach (var v in values)
		//{
			//CommaNL();
			//Indent(); v.Write();
		//}
		CommaNL();
		Indent();		jsonWriter.Write ("\"name\": \"" + name + "\"\n");
		IndentOut();
		Indent();		jsonWriter.Write ("}");

	}

}
#endif