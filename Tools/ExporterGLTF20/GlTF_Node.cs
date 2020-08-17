#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlTF_Node : GlTF_Writer {
	public Vector3 center;
	public Vector3 extents;
	public string boneName;
	public bool meshBox;

	public int animCount = 0;
	public int cameraName = -1;
	//Pi_
	public int lightIndex =-1;
	public bool hasParent = false;
	public List<string> childrenNames = new List<string>();

	public string lightName;
	public List<string>bufferViewNames = new List<string>();
	public List<string>indexNames = new List<string>();
	public List<string>accessorNames = new List<string>();
	public int meshIndex = -1;
	public GlTF_Matrix matrix;
	//	public GlTF_Mesh mesh;
	public GlTF_Rotation rotation;
	public GlTF_Scale scale;
	public GlTF_Translation translation;
	public int skinIndex = -1;
	public float particleIndex = -1;

	public static string GetNameFromObject(Object o)
	{
		return "node_" + GlTF_Writer.GetNameFromObject(o, true);
	}

	public override void Write ()
	{
		Indent();
		jsonWriter.Write ("{\n");
		IndentIn();
		Indent();
		CommaNL();


        if (lightIndex != -1 || particleIndex != -1)
        {
            var l = id.IndexOf(",-,");
            if (l >= 0)
                jsonWriter.Write("\"name\": \"" + id.Remove(l, id.Length - l).Replace("node_", "") + "\"");
            else
                jsonWriter.Write("\"name\": \"" + id.Replace("node_", "") + "\"");

            if (childrenNames != null && childrenNames.Count > 0)
            {
                CommaNL();
                Indent(); jsonWriter.Write("\"children\": [\n");
                IndentIn();
                foreach (string ch in childrenNames)
                {
                    CommaNL();
                    Indent(); jsonWriter.Write(GlTF_Writer.nodeNames.IndexOf(ch));
                }
                jsonWriter.WriteLine();
                IndentOut();
                Indent(); jsonWriter.Write("]");
            }

            CommaNL();

            Indent(); jsonWriter.Write ("\"extensions\": { \n" );
			IndentIn();
			CommaNL();
			if(lightIndex != -1){
				Indent();	jsonWriter.Write ("\"KHR_lights_punctual\": {\n" );
				IndentIn();
				Indent();	jsonWriter.Write ("\"light\": "+ lightIndex +"\n");
				IndentOut();
				Indent();	jsonWriter.Write ("}\n"); 
			}
			if(particleIndex != -1){
				Indent();	jsonWriter.Write ("\"PI_particle\": {\n" );
				IndentIn();
				Indent();	jsonWriter.Write ("\"particles\": "+ particleIndex +"\n");
				IndentOut();
				Indent();	jsonWriter.Write ("}\n"); 
			}
			IndentOut(); 
			Indent();jsonWriter.Write ("}");

			writeMatrix ();
            jsonWriter.Write("\n");

            IndentOut();
			Indent();jsonWriter.Write ("}");
		}
        else
        {
            var l = id.IndexOf(",-,");
            if (l >= 0)
                jsonWriter.Write("\"name\": \"" + id.Remove(l, id.Length - l).Replace("node_", "") + "\"");
            else
                jsonWriter.Write("\"name\": \"" + id.Replace("node_", "") + "\"");

            if (cameraName >= 0)
			{
				CommaNL();
				Indent();
				jsonWriter.Write ("\"camera\":"+cameraName);
			}
			else if (lightName != null)
			{
				CommaNL();
				Indent();
				jsonWriter.Write ("\"light\": \""+lightName+"\"");
			}
			else if (meshIndex != -1)
			{
				CommaNL();
				Indent();
				jsonWriter.Write ("\"mesh\": " + meshIndex);
			}

			if (animCount > 0)
			{
				CommaNL();
				Indent();
				jsonWriter.Write ("\"animCount\":"+animCount);
			}

			//skeletonbox
			if(boneName != null){
				CommaNL();
				Indent();	jsonWriter.Write ("\"boundingbox\":{\n" );
				IndentIn ();
				Indent ();	jsonWriter.Write ("\"center\": [" + center.x.ToString() + ", " + center.y.ToString() + ", " + center.z.ToString()+"],\n");
				Indent ();	jsonWriter.Write ("\"extents\": [" + extents.x*2 + ", " + extents.y*2 + ", " + extents.z*2 +"],\n");
				Indent ();	jsonWriter.Write ("\"boneName\":\"" + boneName + "\"\n");
				IndentOut();
				Indent();	jsonWriter.Write ("}");
			}else if(meshBox){
				CommaNL();
				Indent();	jsonWriter.Write ("\"boundingbox\":{\n" );
				IndentIn ();
				Indent ();	jsonWriter.Write ("\"center\": [" + center.x.ToString() + ", " + center.y.ToString() + ", " + -center.z+"],\n");
				Indent ();	jsonWriter.Write ("\"extents\": [" + extents.x + ", " + extents.y + ", " + extents.z +"]\n");
				IndentOut();
				Indent();	jsonWriter.Write ("}");
			}

			if (childrenNames != null && childrenNames.Count > 0)
			{
				CommaNL();
				Indent();	jsonWriter.Write ("\"children\": [\n");
				IndentIn();
				foreach (string ch in childrenNames)
				{
					CommaNL();
					Indent();		jsonWriter.Write (GlTF_Writer.nodeNames.IndexOf(ch));
				}
				jsonWriter.WriteLine();
				IndentOut();
				Indent();	jsonWriter.Write ("]");
			}

			writeMatrix ();

			if (skinIndex > -1)
			{
				CommaNL();
				Indent(); jsonWriter.Write("\"skin\": " + skinIndex + "\n");
			}

			IndentOut();
			Indent();		jsonWriter.Write ("}");
		}
	}
	public void writeMatrix(){
		if (matrix != null)
		{
			CommaNL();
			matrix.Write();
		}
		else
		{
			if (translation != null && (translation.items[0] != 0f || translation.items[1] != 0f || translation.items[2] != 0f))
			{
				CommaNL();
				translation.Write();
			}
			if (scale != null && (scale.items[0] != 1f || scale.items[1] != 1f || scale.items[2] != 1f))
			{
				CommaNL();
				scale.Write();
			}
			if (rotation != null && (rotation.items[0] != 0f || rotation.items[1] != 0f || rotation.items[2] != 0f || rotation.items[3] != 0f))
			{
				CommaNL();
				rotation.Write();
			}
		}
	}
}
#endif