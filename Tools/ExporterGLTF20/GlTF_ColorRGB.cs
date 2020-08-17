#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class GlTF_ColorRGB : GlTF_Writer {
	Color color;
	public GlTF_ColorRGB (string n) { name = n; }
	public GlTF_ColorRGB (Color c) { color = c; }
	public GlTF_ColorRGB (string n, Color c) { name = n; color = c; }
	public override void Write ()
	{
		Indent();
		if (name != null && name.Length > 0)
			jsonWriter.Write ("\"" + name + "\": ");
		else
			jsonWriter.Write ("\"color\": [");

        color.r = Mathf.RoundToInt(color.r * 255) / 255.0f;
        color.g = Mathf.RoundToInt(color.g * 255) / 255.0f;
        color.b = Mathf.RoundToInt(color.b * 255) / 255.0f;

        jsonWriter.Write (color.r.ToString() + ", " + color.g.ToString() + ", " +color.b.ToString()+"]");
	}
}
#endif