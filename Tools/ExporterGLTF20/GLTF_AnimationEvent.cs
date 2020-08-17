using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GLTF_AnimationEvent
{
    public string key;
    public float progress;
    public GLTF_AnimationEvent(string _key, float _progress)
    {
        this.progress = _progress;
        this.key = _key;
    }
    public string toString()
    {
        return "[" + progress + ",\"" + key + "\"" + "]";
    }
}
