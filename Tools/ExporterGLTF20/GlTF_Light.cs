#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

public class GlTF_Light : GlTF_Writer {
	public GlTF_ColorRGB color;
	public string type;
	public float innerConeAngle;
	public float outerConeAngle;
	public float intensity = 1;
	public float range = 10;
    public string direction;
    public string position;
    //	public override void Write ()
    //	{
    //	}
}
#endif