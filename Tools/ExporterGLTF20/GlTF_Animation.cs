#if UNITY_EDITOR
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.IO.Compression;
using Ionic.Zip;


public class GlTF_Anim : GlTF_Writer{
	public int attribute;
	public int targetNode;
	public int interpolation = 1;
    /// <summary>
    /// time 动画关键帧 时间 Accessor
    /// </summary>
    public GlTF_Accessor timeAcc;
    /// <summary>
    /// value 动画关键帧 属性值 Accessor
    /// </summary>
    public GlTF_Accessor valAcc;
}

public class GlTF_Anim_Count : GlTF_Writer{
    /// <summary>
    /// 目标节点名称
    /// </summary>
	public string trName;
    /// <summary>
    /// 动画数量
    /// </summary>
	public int animCount = 0;

	public GlTF_Anim_Count (string n) {
		trName = n;
		animCount = 1;
	}
}

public class GlTF_PiAnim : GlTF_Writer{
    /// <summary>
    /// 
    /// </summary>
	public float[] arr;

	public GlTF_PiAnim(float[] arr){
		float[] b = new float[arr.Length];
		for(var i = 0; i < arr.Length;i++){
			b [i] = arr [i];
		}
		arr = b;
	}
}

public class GlTF_Animation : GlTF_Writer
{
    public string fPath;
    public string md5Name;
    public bool haveName                        = false;
	public List<GlTF_Channel> channels          = new List<GlTF_Channel>();
	public List<GlTF_AnimSampler> animSamplers  = new List<GlTF_AnimSampler>();
	public List<float[]> GlTF_PiAnims           = new List<float[]>();
	public List<string> GlTF_PiAnimNode         = new List<string>();
	public List<GlTF_Anim> exportAnim           = new List<GlTF_Anim>();
	public string targetNode;
	public GameObject item;
	public string filePath;
	public static Stream PibinFile;
	private GameObject GameObj;
	private AnimationClip Clip;
	private Material mat;
	private string texName;
	public Transform parent;
	public float[] u        = null;
	public float[] v        = null;
	public float[] uTil     = null;
	public float[] vTil     = null;
	public float[] cApha    = null;
	public float[] ortSize  = null;
	public string targetId;
	private bool isLinear;
	private GlTF_Anim newAnim1;
    public List<GLTF_AnimationEvent> animationEventList = new List<GLTF_AnimationEvent>();

    public class KeysInfo
	{
        /// <summary>
        /// 是否都是有线段组成的
        /// </summary>
		public bool isStraight;
        /// <summary>
        /// inTangent 和 outTangent 是否都相等
        /// </summary>
        public bool isInAndOutTangentSame;
		public EditorCurveBinding binding; 
	}

    /// <summary>
    /// 旋转类型
    /// </summary>
	public enum ROTATION_TYPE
	{
        /// <summary>
        /// 未知
        /// </summary>
		UNKNOWN,
        /// <summary>
        /// 四元数
        /// </summary>
		QUATERNION,
        /// <summary>
        /// 欧拉角
        /// </summary>
		EULER
	};

    public enum ANIME_TYPES
    {
        translation, rotation, scale, uScale, vScale, uOffset, vOffset, alpha, color, ort, per, piIsActive, rotation_eular
    }

	public GlTF_Animation (string n) {
		name = n;
	}

	public enum AnimPrototype{
		translation,
		scale,
		rotation,
		uv,
		color,
		ort,
		per
	}

    /// <summary>
    /// 目标曲线
    /// </summary>
	private struct TargetCurveSet
	{
		public string name;

		public AnimationCurve[] translationCurves;
		public AnimationCurve[] rotationCurves;
		//Additional curve types
		public AnimationCurve[] scaleCurves;
		public AnimationCurve[] colorCurves;
		public AnimationCurve uOffset;
		public AnimationCurve vOffset;
		public AnimationCurve uTiling;
		public AnimationCurve vTiling;
		public AnimationCurve alpha;
		public AnimationCurve orthographic;
		public AnimationCurve perspective;
        public AnimationCurve piIsActive;

        public ROTATION_TYPE rotationType;

		public void Init()
		{
			translationCurves   = new AnimationCurve[3];
			rotationCurves      = new AnimationCurve[4];
			scaleCurves         = new AnimationCurve[3];
			colorCurves         = new AnimationCurve[3];
		}
	}

    /// <summary>
    /// 获取 MeshRenderer / SkinnedMeshRenderer
    /// </summary>
    /// <param name="tr"></param>
    /// <returns></returns>
	private Renderer GetRenderer(Transform tr)
	{
		Renderer mr = tr.GetComponent<MeshRenderer>();

		if (mr == null)
        {
			mr = tr.GetComponent<SkinnedMeshRenderer>();
		}

		return mr;
	}
    /// <summary>
    /// 动画目标材质的名称
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="name"></param>
    /// <returns></returns>
	private string getIndex(Transform tr, string name)
	{
        var meshRender = GetRenderer(tr);

        if (meshRender == null || meshRender.sharedMaterials == null)
        {
			return "";
		}

		if (meshRender.sharedMaterials.Length > 1)
        {
			throw new Exception("多维材质不支持uv动画！！");
		}

		mat = meshRender.sharedMaterials[0];
		if (mat.HasProperty("_MainTex") && mat.GetTexture("_MainTex") != null)
		{
			texName = GlTF_Texture.GetNameFromObject((Texture2D)mat.GetTexture("_MainTex"));
			return texName;
		}

		return "";
	}
    /// <summary>
    /// 获取指定名称的子节点
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="name"></param>
	private  void getChild(Transform tr, string name)
	{
		if (tr.childCount == 0)
        {
			if (tr.name == name)
            {
				item = tr.gameObject;
			}
		}
        else
        {
			for (var i = 0; i < tr.childCount; i++)
			{
				var child = tr.GetChild(i);

				if (child.name == name)
                {
					item = child.gameObject;
				}

				if (item == null)
                {
					getChild(child, name, child.name + "/");
				}
                else
                {
					break;
				}
			}
		}

	}

	private  void getChild(Transform tr,string name,string path)
	{
		if (tr.childCount == 0)
        {
			return;
		}
		for (var i = 0; i < tr.childCount; i++)
		{
			var child = tr.GetChild(i);

			if (path + child.name == name)
            {
				item = child.gameObject;
			}

			if (item == null)
            {
				getChild (child, name, path + child.name + "/");
			}
            else
            {
				break;
			}
		}

	}
    
    /// <summary>
    /// 根据节点路径取到节点数组（从根节点到字节点的顺序）
    /// </summary>
    /// <param name="path"></param>
    /// <param name="curNode"></param>
    /// <returns></returns>
    public static GameObject[] GetFutureNode(string path, GameObject curNode)
	{
		if (path == "")
        { 
			return new GameObject[] { curNode };
		}

        var parentNode          = curNode;
        string[] paths          = path.Split('/');
        List<GameObject> objs   = new List<GameObject>();

		for (var i = 0; i < paths.Length; i++)
        {
			var node = FindNode(paths[i], parentNode);

			if (node == null)
            {
                //throw new Exception("找不到对应名字的节点");
                return null;
            }

			parentNode = node;

			objs.Add(node);
		}

		return objs.ToArray();
	}

    /// <summary>
    /// 判断 arr 内 数据是否不重复
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    public bool isRepeat(float[] arr)
	{
		float a = arr[0];

		for (var i = 0; i < arr.Length; i++) {
			if (arr[i] != a) {
				return true;
			}
		}

		return false;
	}

    /// <summary>
    /// 判断 arr 内 数据是否不重复
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    public bool isRepeat(Vector3[] arr)
	{
		Vector3 a = arr [0];

		for (var i = 0; i < arr.Length; i++) {
			if (arr[i][0] != a[0] || arr[i][1] != a[1] || arr[i][2] != a[2]) {
				return true;
			}
		}
		return false;
	}
    /// <summary>
    /// 填充一个数据
    /// </summary>
    /// <param name="clipName"></param>
    /// <param name="arr"></param>
    /// <returns></returns>
	public GlTF_Accessor populateItem(string clipName, float[] arr){
		GlTF_Accessor uAccessor = new GlTF_Accessor (targetId + "_OffsetAccessor_" + clipName, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);

		uAccessor.PibufferView          = GlTF_Writer.Pivec1BufferViewAnim;
		uAccessor.isAnimInBufferview    = false;

		if (!isLinear)
        {
			newAnim1.interpolation = 0;
		}

		uAccessor.Populate(arr);

		return uAccessor;
	}

	public GlTF_Accessor populateItem(string name, GlTF_Accessor.Type t1, GlTF_Accessor.ComponentType t2, GlTF_Pi_BufferView b){
		GlTF_Accessor translationAccessor = new GlTF_Accessor (targetId + "_TranslationAccessor_" + name, t1, t2);

		translationAccessor.PibufferView        = b;
		translationAccessor.isAnimInBufferview  = false;

		string[] kk = new string[]{ "STEP","LINEAR","PICUBICSPLINE","CUBICSPLINE"};

		return translationAccessor;
	}

    /// <summary>
    /// 填充目标节点动画数据
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="tr"></param>
    /// <param name="path"></param>
    /// <param name="bake"></param>
    /// <returns></returns>
	public byte[] Populate(AnimationClip clip, Transform tr, string path, bool bake = true)
	{
		parent = tr;

		Dictionary<string, TargetCurveSet> targetCurvesBinding = new Dictionary<string, TargetCurveSet>();

		collectClipCurves(clip, ref targetCurvesBinding, tr);

        // 20200814 - AnimationEvent 导出
        if (clip.events != null && (clip.events.Length > 0))
        {
            var eventCount = clip.events.Length;
            for (int i = 0; i < eventCount; i++)
            {
                var evet = clip.events[i];
                var animEvent = new GLTF_AnimationEvent(evet.stringParameter, evet.time);
                this.animationEventList.Add(animEvent);
            }
        }

		this.Clip       = clip;
		this.GameObj    = tr.gameObject;

		byte[] byteArr  = new byte[0];

		EditorCurveBinding[] binds = AnimationUtility.GetCurveBindings(clip);

		simplifyBinds(binds, tr.gameObject);

		targetNode = GlTF_Node.GetNameFromObject(tr);
		//Keyframe[] keys = AnimationUtility.GetEditorCurve(Clip, info[v?].bind).keys;
		// Bake animation for all animated nodes

		foreach (string target in targetCurvesBinding.Keys)
		{
			Transform targetTr = target.Length > 0 ? tr.Find(target) : tr;
			if (targetTr == null)
				continue;

			targetId = GlTF_Node.GetNameFromObject(targetTr);

			Keyframe[] keysPos  = null;
			Keyframe[] keysRot  = null;
            Keyframe[] keysEular = null;
            Keyframe[] keysSca  = null;
			Keyframe[] keysU    = null;
			Keyframe[] keysCol  = null;
			Keyframe[] keysOrt  = null;
			Keyframe[] keysPer  = null;
            Keyframe[] keysActive  = null;

            getChild (tr,targetCurvesBinding[target].name);

            /// 一个节点可能会绑定多种动画表现
			for (var i = 0; i < binds.Length; i++) {
				if(binds[i].path == target){
					if (binds[i].propertyName.Contains("m_LocalPosition"))
					{
						keysPos = AnimationUtility.GetEditorCurve (Clip, binds [i]).keys;
					}
					else if (binds[i].propertyName.Contains("m_LocalScale"))
					{
						keysSca = AnimationUtility.GetEditorCurve (Clip, binds [i]).keys;
					}
					else if (binds[i].propertyName.ToLower().Contains("localrotation"))
					{
						keysRot = AnimationUtility.GetEditorCurve (Clip, binds [i]).keys;
					}
					// Takes into account 'localEuler', 'localEulerAnglesBaked' and 'localEulerAnglesRaw'
					else if (binds[i].propertyName.ToLower().Contains("localeuler"))
                    {
                        keysEular = AnimationUtility.GetEditorCurve(Clip, binds[i]).keys;
                    }
                    else if(binds[i].propertyName.Contains("_MainTex_ST"))
                    {
						keysU = AnimationUtility.GetEditorCurve (Clip, binds [i]).keys;
					}
                    else if(binds[i].propertyName.Contains("_TintColor"))
                    {
						keysCol = AnimationUtility.GetEditorCurve (Clip, binds [i]).keys;
					}
                    else if(binds[i].propertyName.Contains("orthographic size"))
                    {
						keysOrt = AnimationUtility.GetEditorCurve (Clip, binds [i]).keys;
					}
                    else if(binds[i].propertyName.Contains("field of view"))
                    {
						keysPer = AnimationUtility.GetEditorCurve (Clip, binds [i]).keys;
                    }
                    else if (binds[i].propertyName.Contains("IsActive"))
                    {
                        keysActive = AnimationUtility.GetEditorCurve(Clip, binds[i]).keys;
                    }
                }
			}

			isLinear = true;
			if (keysPos != null)
            {
				byteArr = GlTF_Md5.combine (byteArr, bakeCurveSet2(targetId, clip.name, keysPos, targetCurvesBinding [target],"translation"));
			}

			if (keysSca != null)
            {
				byteArr = GlTF_Md5.combine (byteArr, bakeCurveSet2(targetId, clip.name, keysSca, targetCurvesBinding [target], "scale"));
			}


			if (keysRot != null)
            {
				byteArr = GlTF_Md5.combine (byteArr, bakeCurveSet2(targetId, clip.name, keysRot, targetCurvesBinding [target],"rotation"));
			}

            if (keysEular != null)
            {
                byteArr = GlTF_Md5.combine(byteArr, bakeCurveSet2(targetId, clip.name, keysEular, targetCurvesBinding[target], "eular"));
            }

            if (keysU != null)
            {
				if(IsStep(targetCurvesBinding [target].uOffset) && IsStep(targetCurvesBinding [target].vOffset) && IsStep(targetCurvesBinding [target].uTiling) && IsStep(targetCurvesBinding [target].vTiling)){
					isLinear = false;
				}
				byteArr = GlTF_Md5.combine (byteArr, bakeCurveSet2(targetId, clip.name, keysU, targetCurvesBinding [target],"uv"));
				isLinear = true;
			}

			if (keysCol != null)
            {
				if (IsStep(targetCurvesBinding [target].colorCurves) && IsStep(targetCurvesBinding [target].alpha))
                {
					isLinear = false;
				}
				byteArr = GlTF_Md5.combine (byteArr, bakeCurveSet2(targetId, clip.name, keysCol, targetCurvesBinding [target],"color"));
				isLinear = true;
			} 

			if (keysOrt != null)
            {
				if (float.IsInfinity(keysOrt[0].inTangent) && float.IsInfinity(keysOrt[0].outTangent) )
                {
					isLinear = false;
				}
				byteArr = GlTF_Md5.combine (byteArr, bakeCurveSet2(targetId, clip.name, keysOrt, targetCurvesBinding [target],"ort"));
				isLinear = true;
			}


			if (keysPer != null)
            {
				if (float.IsInfinity(keysPer[0].inTangent) && float.IsInfinity(keysPer[0].outTangent) )
                {
					isLinear = false;
				}
				byteArr = GlTF_Md5.combine (byteArr, bakeCurveSet2(targetId, clip.name, keysPer, targetCurvesBinding [target],"per"));
				isLinear = true;
            }

            if (keysActive != null)
            {
                if (float.IsInfinity(keysActive[0].inTangent) && float.IsInfinity(keysActive[0].outTangent))
                {
                    isLinear = false;
                }
                byteArr = GlTF_Md5.combine(byteArr, bakeCurveSet2(targetId, clip.name, keysActive, targetCurvesBinding[target], "piIsActive"));
                isLinear = true;
            }

        }
		return byteArr;
    }
    /// <summary>
    /// 动画曲线烘培 - 数据记录
    /// </summary>
    /// <param name="targetId">目标节点名称</param>
    /// <param name="name">动画名称</param>
    /// <param name="keyFrames">关键帧数据列表</param>
    /// <param name="curveSet">目标烘培曲线</param>
    /// <param name="type">动画操作的数据类型</param>
    /// <returns></returns>
	private byte[] bakeCurveSet2(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type)
    {

        /// 关键帧只有一帧
		if (keyFrames.Length == 1)
        {
            throw new Exception("动画请做2帧及以上！！");
        }

        float[] times = new float[keyFrames.Length];

        for (var i = 0; i < keyFrames.Length; i++)
        {
            times[i] = keyFrames[i].time;
        }

        /// Initialize accessors for current animation
        /// 动画时间 访问器
        GlTF_Accessor timeAccessor = getTimeAccessor(targetId, name, times);

        // Bake and populate animation data
        byte[] byteArr  = new byte[0];

        switch (type)
        {
            case "translation":
                {
                    byteArr = bakeCurveSetTranslation(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            case "scale":
                {
                    byteArr = bakeCurveSetScale(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            case "rotation":
                {
                    byteArr = bakeCurveSetRotation(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            case "eular":
                {
                    byteArr = bakeCurveSetRotationEular(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            case "uv":
                {
                    byteArr = bakeCurveSetUV(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            case "color":
                {
                    byteArr = bakeCurveSetColor(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            case "ort":
                {
                    byteArr = bakeCurveSetOrt(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            case "per":
                {
                    byteArr = bakeCurveSetPer(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            case "piIsActive":
                {
                    byteArr = bakeCurveSetIsActive(targetId, name, keyFrames, curveSet, type, timeAccessor, times);
                    break;
                }
            default:
                {
                    break;
                }
        }

        return byteArr;
    }

    /// <summary>
    /// 动画曲线烘培 - 数据记录
    /// </summary>
    /// <param name="targetId">目标节点名称</param>
    /// <param name="name">动画名称</param>
    /// <param name="keyFrames">关键帧数据列表</param>
    /// <param name="curveSet">目标烘培曲线</param>
    /// <param name="type">动画操作的数据类型</param>
    /// <returns></returns>
    private byte[] bakeCurveSet(string targetId, string name, Keyframe[] keys, TargetCurveSet curveSet, string type){
        // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
        newAnim1 = new GlTF_Anim();

        if (keys.Length == 1)
        {
            throw new Exception("动画请做2帧及以上！！");
        }
        // Bake and populate animation data
        float[] times = new float[keys.Length];
        Vector3[] prop1 = new Vector3[keys.Length];
        Vector4[] prop2 = new Vector4[keys.Length];
        byte[] byteArr = new byte[0];

        // Initialize accessors for current animation
        GlTF_Accessor timeAccessor = new GlTF_Accessor(targetId + "_TimeAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);
        timeAccessor.PibufferView = GlTF_Writer.AniPifloatBufferView;
        timeAccessor.isAnimInBufferview = false;
        int timeAccessorIndex = GlTF_Writer.accessors.Count;
        //accessors.Add (timeAccessor);

        for (var i = 0; i < keys.Length; i++)
        {
            times[i] = keys[i].time;
        }

        timeAccessor.Populate(times);
        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));

        newAnim1.timeAcc = timeAccessor;

        GlTF_Accessor valueAccessor;
        if (type == "translation" || type == "scale")
        {
            if (type == "translation")
            {
                newAnim1.attribute = 0;
                int iT = getInterpolation(curveSet.translationCurves);
                newAnim1.interpolation = iT;
                valueAccessor = populateItem(name, GlTF_Accessor.Type.VEC3, GlTF_Accessor.ComponentType.FLOAT, GlTF_Writer.Pivec3BufferViewAnim);
                prop1 = getProp1(curveSet.translationCurves, iT, times);
                valueAccessor.Populate(prop1);
            }
            else
            {
                newAnim1.attribute = 2;
                int iT = getInterpolation(curveSet.scaleCurves);
                newAnim1.interpolation = iT;
                valueAccessor = populateItem(name, GlTF_Accessor.Type.VEC3, GlTF_Accessor.ComponentType.FLOAT, GlTF_Writer.Pivec3BufferViewAnim);
                prop1 = getProp1(curveSet.scaleCurves, iT, times);
                valueAccessor.Populate(prop1, true);
            }
            byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(prop1));
            newAnim1.valAcc = valueAccessor;
            addAnimCount(targetId);
        }
        else if (type == "rotation")
        {
            newAnim1.attribute = 1;
            int iT = getInterpolation(curveSet.rotationCurves);
            newAnim1.interpolation = iT;
            valueAccessor = populateItem(name, GlTF_Accessor.Type.VEC4, GlTF_Accessor.ComponentType.FLOAT, GlTF_Writer.Pivec4BufferViewAnim);
            prop2 = getProp2(curveSet.rotationCurves, iT, times);
            valueAccessor.Populate(prop2, false);
            byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(prop2));
            newAnim1.valAcc = valueAccessor;
            addAnimCount(targetId);
        }
        else if (type == "uv")
        {
            u = new float[keys.Length];
            v = new float[keys.Length];
            uTil = new float[keys.Length];
            vTil = new float[keys.Length];
            //var index = getIndex (item.transform,curveSet.name);
            for (int i = 0; i < times.Length; ++i)
            {
                float currentTime = times[i];
                u[i] = curveSet.uOffset.Evaluate(currentTime);
                v[i] = curveSet.vOffset.Evaluate(currentTime);
                uTil[i] = curveSet.uTiling.Evaluate(currentTime);
                vTil[i] = curveSet.vTiling.Evaluate(currentTime);
            }

            var index = getIndex(item.transform, curveSet.name);
            // TODO 判断去重，需要加上in，out为0的条件。
            if (isRepeat(u) || u[0] != mat.mainTextureScale[0])
            {
                newAnim1 = new GlTF_Anim();
                newAnim1.timeAcc = timeAccessor;
                newAnim1.attribute = 3;
                valueAccessor = populateItem(name, u);
                byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(u));
                newAnim1.valAcc = valueAccessor;
                addAnimCount(targetId);
            }
            if (isRepeat(v) || v[0] != mat.mainTextureScale[1])
            {
                newAnim1 = new GlTF_Anim();
                newAnim1.timeAcc = timeAccessor;
                newAnim1.attribute = 4;
                valueAccessor = populateItem(name, v);
                byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(v));
                newAnim1.valAcc = valueAccessor;
                addAnimCount(targetId);
            }
            if (isRepeat(uTil) || uTil[0] != mat.mainTextureOffset[0])
            {
                newAnim1 = new GlTF_Anim();
                newAnim1.timeAcc = timeAccessor;
                newAnim1.attribute = 5;
                valueAccessor = populateItem(name, uTil);
                byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(uTil));

                newAnim1.valAcc = valueAccessor;
                addAnimCount(targetId);
            }
            if (isRepeat(vTil) || vTil[0] != mat.mainTextureOffset[1])
            {
                newAnim1 = new GlTF_Anim();
                newAnim1.timeAcc = timeAccessor;
                newAnim1.attribute = 6;
                valueAccessor = populateItem(name, vTil);
                byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(vTil));
                newAnim1.valAcc = valueAccessor;
                addAnimCount(targetId);
            }
        }
        else if (type == "color")
        {
            cApha = new float[keys.Length];
            getIndex(item.transform, curveSet.name);
            for (int i = 0; i < times.Length; ++i)
            {
                float currentTime = times[i];
                prop1[i] = new Vector3(curveSet.colorCurves[0].Evaluate(currentTime), curveSet.colorCurves[1].Evaluate(currentTime), curveSet.colorCurves[2].Evaluate(currentTime));
                cApha[i] = curveSet.alpha.Evaluate(currentTime);
            }
            //if(isRepeat(cApha)){

            newAnim1.attribute = 7;
            valueAccessor = new GlTF_Accessor(targetId + "_OffsetAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);
            valueAccessor.PibufferView = GlTF_Writer.Pivec1BufferViewAnim;
            valueAccessor.isAnimInBufferview = false;

            if (!isLinear)
            {
                newAnim1.interpolation = 0;
            }
            //accessors.Add (valueAccessor);
            newAnim1.valAcc = valueAccessor;
            valueAccessor.Populate(cApha);

            byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(cApha));
            addAnimCount(targetId);
            //}
            Color c = mat.GetColor("_TintColor");
            if (isRepeat(prop1) || prop1[0].x != c.r || prop1[0].y != c.g || prop1[0].z != c.b)
            {
                newAnim1 = new GlTF_Anim();
                newAnim1.timeAcc = timeAccessor;
                // TinColor
                valueAccessor = new GlTF_Accessor(targetId + "_colorAccessor_" + name, GlTF_Accessor.Type.VEC3, GlTF_Accessor.ComponentType.FLOAT);
                valueAccessor.PibufferView = GlTF_Writer.Pivec3BufferViewAnim;
                valueAccessor.isAnimInBufferview = false;
                if (!isLinear)
                {
                    newAnim1.interpolation = 0;
                }
                //accessors.Add(valueAccessor);

                valueAccessor.Populate(prop1, true);
                newAnim1.valAcc = valueAccessor;
                newAnim1.attribute = 8;

                addAnimCount(targetId);
            }
        }
        else if (type == "ort")
        {
            ortSize = new float[keys.Length];
            for (int i = 0; i < times.Length; ++i)
            {
                float currentTime = times[i];
                ortSize[i] = curveSet.orthographic.Evaluate(currentTime);
            }
            if (isRepeat(ortSize))
            {
                valueAccessor = new GlTF_Accessor(targetId + "_OrtAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);
                valueAccessor.PibufferView = GlTF_Writer.Pivec1BufferViewAnim;

                if (!isLinear)
                {
                    newAnim1.interpolation = 0;
                }
                //accessors.Add (valueAccessor);

                valueAccessor.Populate(ortSize);
                newAnim1.valAcc = valueAccessor;
                byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(ortSize));
                newAnim1.valAcc = valueAccessor;
                newAnim1.attribute = 9;

                addAnimCount(targetId);
            }
        }
        else if (type == "per")
        {
            ortSize = new float[keys.Length];
            for (int i = 0; i < times.Length; ++i)
            {
                float currentTime = times[i];
                ortSize[i] = curveSet.perspective.Evaluate(currentTime) / 180 * Mathf.PI;
            }
            if (isRepeat(ortSize))
            {
                valueAccessor = new GlTF_Accessor(targetId + "_PerAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);
                valueAccessor.PibufferView = GlTF_Writer.Pivec1BufferViewAnim;
                valueAccessor.isAnimInBufferview = false;
                if (!isLinear)
                {
                    newAnim1.interpolation = 0;
                }
                //accessors.Add (valueAccessor);

                valueAccessor.Populate(ortSize);
                byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(ortSize));
                newAnim1.valAcc = valueAccessor;
                newAnim1.attribute = 10;
                addAnimCount(targetId);
            }
        }


        return byteArr;
    }

    private byte[] bakeCurveSetTranslation(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {

        GlTF_Accessor valueAccessor;

        // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
        newAnim1 = new GlTF_Anim();

        /// Bake and populate animation data
        byte[] byteArr  = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));

        int iT = getInterpolation(curveSet.translationCurves);

        valueAccessor   = populateItem(name, GlTF_Accessor.Type.VEC3, GlTF_Accessor.ComponentType.FLOAT, GlTF_Writer.Pivec3BufferViewAnim);

        prop1           = getProp1(curveSet.translationCurves, iT, times);

        valueAccessor.Populate(prop1);

        var transBytes = GlTF_Md5.getByte(prop1);

        byteArr         = GlTF_Md5.combine(byteArr, transBytes);

        newAnim1.interpolation  = iT;
        newAnim1.attribute      = (int)ANIME_TYPES.translation;
        newAnim1.timeAcc        = timeAccessor;
        newAnim1.valAcc         = valueAccessor;

        addAnimCount(targetId);

        return byteArr;
    }

    private byte[] bakeCurveSetScale(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {
        GlTF_Accessor valueAccessor;

        // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
        newAnim1 = new GlTF_Anim();

        /// Bake and populate animation data
        byte[] byteArr  = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));


        int iT = getInterpolation(curveSet.scaleCurves);

        valueAccessor = populateItem(name, GlTF_Accessor.Type.VEC3, GlTF_Accessor.ComponentType.FLOAT, GlTF_Writer.Pivec3BufferViewAnim);

        prop1 = getProp1(curveSet.scaleCurves, iT, times);

        valueAccessor.Populate(prop1, false);

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(prop1));

        newAnim1.interpolation  = iT;
        newAnim1.attribute      = (int)ANIME_TYPES.scale;
        newAnim1.timeAcc        = timeAccessor;
        newAnim1.valAcc         = valueAccessor;

        addAnimCount(targetId);

        return byteArr;
    }

    private byte[] bakeCurveSetRotation(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {
        GlTF_Accessor valueAccessor;

        // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
        newAnim1 = new GlTF_Anim();

        /// Bake and populate animation data
        byte[] byteArr = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));
        
        int iT = getInterpolation(curveSet.rotationCurves);

        valueAccessor   = populateItem(name, GlTF_Accessor.Type.VEC4, GlTF_Accessor.ComponentType.FLOAT, GlTF_Writer.Pivec4BufferViewAnim);
        prop2           = getProp2(curveSet.rotationCurves, iT, times);
        byteArr         = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(prop2));

        valueAccessor.Populate(prop2, false);

        newAnim1.interpolation  = iT;
        newAnim1.attribute      = (int)ANIME_TYPES.rotation;
        newAnim1.timeAcc        = timeAccessor;
        newAnim1.valAcc         = valueAccessor;

        addAnimCount(targetId);

        return byteArr;
    }

    private byte[] bakeCurveSetRotationEular(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {
        GlTF_Accessor valueAccessor;

        // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
        newAnim1 = new GlTF_Anim();

        /// Bake and populate animation data
        byte[] byteArr = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));


        int iT          = getInterpolation(curveSet.rotationCurves);

        valueAccessor   = populateItem(name, GlTF_Accessor.Type.VEC3, GlTF_Accessor.ComponentType.FLOAT, GlTF_Writer.Pivec3BufferViewAnim);

        prop1           = getProp1_Eular(curveSet.rotationCurves, iT, times);

        valueAccessor.Populate(prop1, false);

        var propByte = GlTF_Md5.getByte(prop1);

        byteArr = GlTF_Md5.combine(byteArr, propByte);

        newAnim1.interpolation  = iT;
        newAnim1.attribute      = (int)ANIME_TYPES.rotation_eular;
        newAnim1.timeAcc        = timeAccessor;
        newAnim1.valAcc         = valueAccessor;

        addAnimCount(targetId);

        return byteArr;
    }

    private byte[] bakeCurveSetUV(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {
        GlTF_Accessor valueAccessor;

        /// Bake and populate animation data
        byte[] byteArr  = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));

        u       = new float[keyFrames.Length];
        v       = new float[keyFrames.Length];
        uTil    = new float[keyFrames.Length];
        vTil    = new float[keyFrames.Length];

        //var index = getIndex (item.transform,curveSet.name);
        for (int i = 0; i < times.Length; ++i)
        {
            float currentTime = times[i];
            u[i]        = curveSet.uOffset.Evaluate(currentTime);
            v[i]        = curveSet.vOffset.Evaluate(currentTime);
            uTil[i]     = curveSet.uTiling.Evaluate(currentTime);
            vTil[i]     = curveSet.vTiling.Evaluate(currentTime);
        }

        var index       = getIndex(item.transform, curveSet.name);

        // TODO 判断去重，需要加上in，out为0的条件。
        if (isRepeat(u) || u[0] != mat.mainTextureScale[0])
        {

            // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
            newAnim1 = new GlTF_Anim();

            valueAccessor       = populateItem(name, u);
            byteArr             = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(u));

            newAnim1.attribute  = (int)ANIME_TYPES.uScale;
            newAnim1.timeAcc    = timeAccessor;
            newAnim1.valAcc     = valueAccessor;

            addAnimCount(targetId);
        }
        if (isRepeat(v) || v[0] != mat.mainTextureScale[1])
        {

            // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
            newAnim1 = new GlTF_Anim();

            valueAccessor       = populateItem(name, v);
            byteArr             = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(v));

            newAnim1.attribute  = (int)ANIME_TYPES.vScale;
            newAnim1.timeAcc    = timeAccessor;
            newAnim1.valAcc     = valueAccessor;

            addAnimCount(targetId);
        }
        if (isRepeat(uTil) || uTil[0] != mat.mainTextureOffset[0])
        {

            // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
            newAnim1 = new GlTF_Anim();

            valueAccessor       = populateItem(name, uTil);
            byteArr             = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(uTil));

            newAnim1.attribute  = (int)ANIME_TYPES.uOffset;
            newAnim1.timeAcc    = timeAccessor;
            newAnim1.valAcc     = valueAccessor;

            addAnimCount(targetId);
        }
        if (isRepeat(vTil) || vTil[0] != mat.mainTextureOffset[1])
        {

            // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
            newAnim1 = new GlTF_Anim();

            valueAccessor       = populateItem(name, vTil);
            byteArr             = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(vTil));

            newAnim1.attribute  = (int)ANIME_TYPES.vOffset;
            newAnim1.timeAcc    = timeAccessor;
            newAnim1.valAcc     = valueAccessor;

            addAnimCount(targetId);
        }

        return byteArr;
    }

    private byte[] bakeCurveSetColor(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {
        GlTF_Accessor valueAccessor;

        /// Bake and populate animation data
        byte[] byteArr  = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));

        cApha   = new float[keyFrames.Length];

        getIndex(item.transform, curveSet.name);

        for (int i = 0; i < times.Length; ++i)
        {
            float currentTime = times[i];

            prop1[i]    = new Vector3(
                                curveSet.colorCurves[0].Evaluate(currentTime), 
                                curveSet.colorCurves[1].Evaluate(currentTime), 
                                curveSet.colorCurves[2].Evaluate(currentTime)
                            );
            cApha[i]    = curveSet.alpha.Evaluate(currentTime);
        }
        //if(isRepeat(cApha)){

        valueAccessor       = new GlTF_Accessor(targetId + "_OffsetAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);
        valueAccessor.PibufferView          = GlTF_Writer.Pivec1BufferViewAnim;
        valueAccessor.isAnimInBufferview = false;
        /// accessors.Add (valueAccessor);
        valueAccessor.Populate(cApha);

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(cApha));


        // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
        newAnim1 = new GlTF_Anim();

        if (!isLinear)
        {
            newAnim1.interpolation = 0;
        }
        newAnim1.attribute      = (int)ANIME_TYPES.alpha;
        newAnim1.timeAcc        = timeAccessor;
        newAnim1.valAcc         = valueAccessor;

        addAnimCount(targetId);
        //}

        Color c = mat.GetColor("_TintColor");
        if (isRepeat(prop1) || prop1[0].x != c.r || prop1[0].y != c.g || prop1[0].z != c.b)
        {
            newAnim1            = new GlTF_Anim();

            // TinColor
            valueAccessor       = new GlTF_Accessor(targetId + "_colorAccessor_" + name, GlTF_Accessor.Type.VEC3, GlTF_Accessor.ComponentType.FLOAT);
            valueAccessor.PibufferView          = GlTF_Writer.Pivec3BufferViewAnim;
            valueAccessor.isAnimInBufferview    = false;
            //accessors.Add(valueAccessor);
            valueAccessor.Populate(prop1, false);

            if (!isLinear)
            {
                newAnim1.interpolation = 0;
            }
            newAnim1.attribute      = (int)ANIME_TYPES.color;
            newAnim1.timeAcc        = timeAccessor;
            newAnim1.valAcc         = valueAccessor;

            addAnimCount(targetId);
        }

        return byteArr;
    }

    private byte[] bakeCurveSetOrt(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {
        GlTF_Accessor valueAccessor;

        /// Bake and populate animation data
        byte[] byteArr = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));
        
        ortSize = new float[keyFrames.Length];

        for (int i = 0; i < times.Length; ++i)
        {
            float currentTime   = times[i];
            ortSize[i]          = curveSet.orthographic.Evaluate(currentTime);
        }

        if (isRepeat(ortSize))
        {
            valueAccessor   = new GlTF_Accessor(targetId + "_OrtAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);
            valueAccessor.PibufferView  = GlTF_Writer.Pivec1BufferViewAnim;
            //accessors.Add (valueAccessor);
            valueAccessor.Populate(ortSize);

            byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(ortSize));

            // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
            newAnim1 = new GlTF_Anim();

            if (!isLinear)
            {
                newAnim1.interpolation = 0;
            }
            newAnim1.attribute      = (int)ANIME_TYPES.ort;
            newAnim1.timeAcc        = timeAccessor;
            newAnim1.valAcc         = valueAccessor;

            addAnimCount(targetId);
        }

        return byteArr;
    }

    private byte[] bakeCurveSetPer(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {
        GlTF_Accessor valueAccessor;

        /// Bake and populate animation data
        byte[] byteArr = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));

        ortSize = new float[keyFrames.Length];
        for (int i = 0; i < times.Length; ++i)
        {
            float currentTime   = times[i];
            ortSize[i]          = curveSet.perspective.Evaluate(currentTime) / 180 * Mathf.PI;
        }
        if (isRepeat(ortSize))
        {
            valueAccessor = new GlTF_Accessor(targetId + "_PerAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);
            valueAccessor.PibufferView          = GlTF_Writer.Pivec1BufferViewAnim;
            valueAccessor.isAnimInBufferview    = false;
            //accessors.Add (valueAccessor);
            valueAccessor.Populate(ortSize);

            byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(ortSize));

            // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
            newAnim1 = new GlTF_Anim();

            if (!isLinear)
            {
                newAnim1.interpolation = 0;
            }
            newAnim1.attribute  = (int)ANIME_TYPES.per;
            newAnim1.timeAcc    = timeAccessor;
            newAnim1.valAcc     = valueAccessor;

            addAnimCount(targetId);
        }

        return byteArr;
    }

    private byte[] bakeCurveSetIsActive(string targetId, string name, Keyframe[] keyFrames, TargetCurveSet curveSet, string type, GlTF_Accessor timeAccessor, float[] times)
    {
        GlTF_Accessor valueAccessor;

        /// Bake and populate animation data
        byte[] byteArr = new byte[0];
        Vector3[] prop1 = new Vector3[keyFrames.Length];
        Vector4[] prop2 = new Vector4[keyFrames.Length];

        byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(times));

        ortSize = new float[keyFrames.Length];
        for (int i = 0; i < times.Length; ++i)
        {
            float currentTime = times[i];
            ortSize[i] = curveSet.piIsActive.Evaluate(currentTime);
        }
        if (isRepeat(ortSize))
        {
            valueAccessor = new GlTF_Accessor(targetId + "_PerAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);
            valueAccessor.PibufferView = GlTF_Writer.Pivec1BufferViewAnim;
            valueAccessor.isAnimInBufferview = false;
            //accessors.Add (valueAccessor);
            valueAccessor.Populate(ortSize);

            byteArr = GlTF_Md5.combine(byteArr, GlTF_Md5.getByte(ortSize));

            // attribute,targetNode,timeAccessorOffset,timeAccessorCount,time.bufferView.byteOffset,time.bufferView.byteLength,valueOffset,valueCount,value.bufferView.byteOffset,value.bufferView.byteLength,interpolation
            newAnim1 = new GlTF_Anim();

            if (!isLinear)
            {
                newAnim1.interpolation = 0;
            }
            newAnim1.attribute = (int)ANIME_TYPES.piIsActive;
            newAnim1.timeAcc = timeAccessor;
            newAnim1.valAcc = valueAccessor;

            addAnimCount(targetId);
        }

        return byteArr;
    }

    private GlTF_Accessor getTimeAccessor(string targetId, string animName, float[] times)
    {
        // Initialize accessors for current animation
        /// 动画时间 访问器
        GlTF_Accessor timeAccessor = new GlTF_Accessor(targetId + "_TimeAccessor_" + name, GlTF_Accessor.Type.SCALAR, GlTF_Accessor.ComponentType.FLOAT);

        timeAccessor.PibufferView       = GlTF_Writer.AniPifloatBufferView;
        timeAccessor.isAnimInBufferview = false;

        /// int timeAccessorIndex       = GlTF_Writer.accessors.Count;
        /// accessors.Add (timeAccessor);

        timeAccessor.Populate(times);

        return timeAccessor;
    }

    private void collectClipCurves(AnimationClip clip, ref Dictionary<string, TargetCurveSet> targetCurves, Transform tr)
	{
		foreach (var binding in AnimationUtility.GetCurveBindings(clip))
		{
			GameObject[] objs = GetFutureNode(binding.path, parent.gameObject);

			if(objs == null || objs[0].activeInHierarchy == false){
				continue;
			}
			AnimationCurve curve = AnimationUtility.GetEditorCurve(clip, binding);

			if(curve.keys.Length == 1){
				throw new Exception ("动画请做2帧及以上！！" + tr.name + " - " + clip.name + "-" + binding.path + "-" + binding.propertyName);
			}
				
			if (!targetCurves.ContainsKey(binding.path))
			{
				TargetCurveSet curveSet = new TargetCurveSet();
				curveSet.Init();
				targetCurves.Add(binding.path, curveSet);
			}

			TargetCurveSet current = targetCurves[binding.path];
			if (binding.propertyName.Contains("m_LocalPosition"))
			{
				if (binding.propertyName.Contains(".x"))
					current.translationCurves[0] = curve;
				else if (binding.propertyName.Contains(".y"))
					current.translationCurves[1] = curve;
				else if (binding.propertyName.Contains(".z"))
					current.translationCurves[2] = curve;
			}
			else if (binding.propertyName.Contains("m_LocalScale"))
			{
				if (binding.propertyName.Contains(".x"))
					current.scaleCurves[0] = curve;
				else if (binding.propertyName.Contains(".y"))
					current.scaleCurves[1] = curve;
				else if (binding.propertyName.Contains(".z"))
					current.scaleCurves[2] = curve;
			}
			else if (binding.propertyName.ToLower().Contains("localrotation"))
			{
				current.rotationType = ROTATION_TYPE.QUATERNION;
				if (binding.propertyName.Contains(".x"))
					current.rotationCurves[0] = curve;
				else if (binding.propertyName.Contains(".y"))
					current.rotationCurves[1] = curve;
				else if (binding.propertyName.Contains(".z"))
					current.rotationCurves[2] = curve;
				else if (binding.propertyName.Contains(".w"))
					current.rotationCurves[3] = curve;
			}
			// Takes into account 'localEuler', 'localEulerAnglesBaked' and 'localEulerAnglesRaw'
			else if (binding.propertyName.ToLower().Contains("localeuler"))
			{
				current.rotationType = ROTATION_TYPE.EULER;
				if (binding.propertyName.Contains(".x"))
					current.rotationCurves[0] = curve;
				else if (binding.propertyName.Contains(".y"))
					current.rotationCurves[1] = curve;
				else if (binding.propertyName.Contains(".z"))
					current.rotationCurves[2] = curve;
			}
			if(binding.propertyName.Contains("MainTex_ST")){
				if (binding.propertyName.Contains(".x"))
					current.uOffset= curve;
				else if (binding.propertyName.Contains(".y"))
					current.vOffset = curve;
				else if (binding.propertyName.Contains(".z"))
					current.uTiling = curve;
				else if (binding.propertyName.Contains(".w"))
					current.vTiling = curve;
			}
			if(binding.propertyName.Contains("_TintColor")){
				if(current.colorCurves == null){
					current.colorCurves = new AnimationCurve[4];
				}
				if (binding.propertyName.Contains(".r"))
					current.colorCurves[0]= curve;
				else if (binding.propertyName.Contains(".g"))
					current.colorCurves[1] = curve;
				else if (binding.propertyName.Contains(".b"))
					current.colorCurves[2] = curve;
				else if (binding.propertyName.Contains(".a"))
					current.alpha = curve;
            }
            if (binding.propertyName.Contains("IsActive"))
            {
                current.piIsActive = curve;
            }
            if (binding.propertyName.Contains("orthographic size")){
				current.orthographic = curve;
			}

			if(binding.propertyName.Contains("field of view")){
				current.perspective = curve;
			}

			current.name = binding.path;
			targetCurves[binding.path] = current;
		}
	}

	public AnimationCurve createConstantCurve(float value, float endTime)
	{
		// No translation curves, adding them
		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(0, value);
		curve.AddKey(endTime, value);
		return curve;
	}
	public override void Write()
	{
		if (exportAnim.Count == 0)
			return;

		Indent();		jsonWriter.Write ("{\n");
		IndentIn();
		Indent();		jsonWriter.Write("\"name\": \"" + name + "\",\n");
		Indent();		jsonWriter.Write("\"nodeIndex\": \"" + GlTF_Writer.nodeNames.IndexOf (targetNode) + "\",\n");
		Indent();		jsonWriter.Write ("\"buffer\": " + bufferIndex + ",\n");


		/*float[] ind = new float[exportAnim.Count * 11];
		for(var j = 0;j < exportAnim.Count;j++){
			GlTF_Anim c = exportAnim [j];
			string n = GlTF_PiAnimNode [j];
			c.targetNode = GlTF_Writer.nodeNames.IndexOf (n);
			ind [j * 11] = exportAnim[j].attribute;
			ind [j * 11+1] = GlTF_Writer.nodeNames.IndexOf (n);
			ind [j * 11+2] = exportAnim[j].timeAcc.byteOffset;
			ind [j * 11+3] = exportAnim[j].timeAcc.count;
			ind [j * 11+4] = exportAnim[j].timeAcc.PibufferView.byteOffset;
			ind [j * 11+5] = exportAnim[j].timeAcc.PibufferView.byteLength;
			ind [j * 11+6] = exportAnim[j].valAcc.byteOffset;
			ind [j * 11+7] = exportAnim[j].valAcc.count;
			ind [j * 11+8] = exportAnim[j].valAcc.PibufferView.byteOffset;
			ind [j * 11+9] = exportAnim[j].valAcc.PibufferView.byteLength;
			ind [j * 11+10] = exportAnim[j].interpolation;
		}

		var x = GlTF_Writer.buffers.Find (v =>  v.uri == md5Name + ".anim.bin");
		if(x != null){
			var b = new GlTF_Pi_BufferView("floatBufferView", 0);
			b.Populate (ind);
			var c = x.BLength;
			Indent();		jsonWriter.Write ("\"start\": " + c+ ",\n");
			string filePath = Path.Combine(Path.GetDirectoryName(fPath), x.uri);
			Stream PibinFile = File.Open(filePath, FileMode.Open);
			PibinFile.Position = c;
			b.PimemoryStream.WriteTo(PibinFile);
			x.BLength = PibinFile.Length;
			Indent();		jsonWriter.Write ("\"length\": " + (x.BLength - c )+ ",\n");
			PibinFile.Flush();
			PibinFile.Close ();
		}*/

        if (this.animationEventList.Count > 0)
        {
            Indent(); jsonWriter.Write("\"events\": [\n");
            IndentIn();

            int count = this.animationEventList.Count;
            for (int i = 0; i < count; i++)
            {
                var animEvent = this.animationEventList[i];
                CommaNL();
                Indent(); jsonWriter.Write(animEvent.toString());
            }

            IndentOut();
            jsonWriter.Write("\n");
            Indent(); jsonWriter.Write("],\n");
        }

	
		Indent();       jsonWriter.Write ("\"PiChannel\": [\n");
		IndentIn();



		for (var j = 0;j < exportAnim.Count;j++)
		{
			GlTF_Anim c = exportAnim [j];
			string n = GlTF_PiAnimNode [j];
			c.targetNode = GlTF_Writer.nodeNames.IndexOf (n);
            if (c.targetNode == -1)
            {
                Debug.LogError("动画的目标节点未激活 或 不存在： " + n);
            }
			CommaNL();
			Indent();		jsonWriter.Write ("[");
			jsonWriter.Write ( c.attribute + ",");
			jsonWriter.Write ( c.targetNode + ",");
			jsonWriter.Write ( c.timeAcc.byteOffset + ",");
			jsonWriter.Write ( c.timeAcc.count + ",");
			jsonWriter.Write ( c.timeAcc.PibufferView.byteOffset + ",");
			jsonWriter.Write ( c.timeAcc.PibufferView.byteLength + ",");
			jsonWriter.Write ( c.valAcc.byteOffset + ",");
			jsonWriter.Write ( c.valAcc.count + ",");
			jsonWriter.Write ( c.valAcc.PibufferView.byteOffset + ",");
			jsonWriter.Write ( c.valAcc.PibufferView.byteLength + ",");
			jsonWriter.Write ( c.interpolation );

			jsonWriter.Write ("]");
		}

		/*for (var j = 0;j < GlTF_PiAnims.Count;j++)
		{
			float[] c = GlTF_PiAnims [j];
			string n = GlTF_PiAnimNode [j];
			c[1] = GlTF_Writer.nodeNames.IndexOf (n);
			CommaNL();
			Indent();		jsonWriter.Write ("[");

			for(var i = 0 ;i < c.Length;i++){
				jsonWriter.Write ( c[i]);
				if(i < c.Length - 1){
					jsonWriter.Write (",");
				}
			}
			jsonWriter.Write ("]");
		}*/
		IndentOut();
		jsonWriter.Write ("\n");
		Indent();		jsonWriter.Write ("]\n");

		/*Indent();		jsonWriter.Write ("\"channels\": [\n");
		foreach (GlTF_Channel c in channels)
		{
			CommaNL();
			c.Write ();
		}
		jsonWriter.WriteLine();
		Indent();		jsonWriter.Write ("],\n");

		Indent();		jsonWriter.Write ("\"samplers\": [\n");
		IndentIn();
		foreach (GlTF_AnimSampler s in animSamplers)
		{
			CommaNL();
			s.Write ();
		}
		IndentOut();
		jsonWriter.WriteLine();
		Indent();		jsonWriter.Write ("]\n");*/

		IndentOut();
		Indent();		jsonWriter.Write ("}");
	}
    
    /// <summary>
    /// 简化binds(去除binds中的虚拟体的绑定属性， 每帧数据相等的属性)
    /// </summary>
    /// <param name="binds"></param>
    /// <param name="curNode"></param>
    private void simplifyBinds(EditorCurveBinding[] binds, GameObject curNode)
	{
		for (var i = 0; i < binds.Length; i++) {
			var v = binds[i];
			//// 动画不能含有旋转
			//if (v.propertyName.IndexOf("localEulerAnglesRaw") >= 0)
			//{
			//	throw new Exception("动画不能含有旋转，请用Quaternion插值！");
			//}
		}
	}

    /// <summary>
    /// 帧数据 In / Out 是否相同
    /// </summary>
    /// <param name="keys"></param>
    /// <returns></returns>
	private bool IsInAndOutTangentSame(Keyframe[] keys)
	{
		bool result = true;
		for (var i = 0; i < keys.Length; ++i)
		{
			if ( float.IsInfinity(keys[i].inTangent) ||
				float.IsInfinity(keys[i].outTangent) ||
				!IsFloatSame(keys[i].inTangent, keys[i].outTangent))
			{
				result = false;
				break;
			}
		}
		return result;
	}

	private bool IsFloatSame(float a, float b)
	{
		return Mathf.Abs(a - b) <= 0.0001F;
	}

	private bool IsStraightLine(Keyframe[] keys)
	{
		bool result = true;
		for (var i = 1; i < keys.Length; ++i) 
		{
			// 如果这一帧的inTangent等于上一帧的outTangent，那么这是线段
			if (
				float.IsInfinity(keys[i].inTangent) ||
				float.IsInfinity(keys[i - 1].outTangent) ||
				!IsFloatSame(keys[i].inTangent, keys[i - 1].outTangent))
			{
				result = false;
				break;
			}
		}
		return result;
	}

    /// <summary>
    /// 根据名字找子节点
    /// </summary>
    /// <param name="name"></param>
    /// <param name="node"></param>
    /// <returns></returns>
	public static GameObject FindNode(string name, GameObject node)
	{
		if (name == "")
			throw new Exception("名字为空，无法寻找节点!");

		for (var i = 0; i < node.transform.childCount; i++)
		{
			var child = node.transform.GetChild(i);
			if (child.name == name)
				return child.gameObject;
		}
		return null;
	}
    
    /// <summary>
    /// 当只有一帧时添加到2帧
    /// </summary>
    /// <param name="arr"></param>
    /// <returns></returns>
    public static float[] addTwo(float[] arr)
	{
		if(arr.Length == 1){
			float[] re = new float[2];
			re [0] = arr [0];
			re [1] = arr [0];
			return re;
		}
		return arr;

	}

    /// <summary>
    /// 获得曲线类型
    /// </summary>
    /// <param name="curves"></param>
    /// <returns></returns>
    private int getInterpolation(AnimationCurve[] curves){
		if (IsStep (curves)) {
			return 0;
		} else if (IsStraightLine (curves)) {
			return 1;
		} else if (IsInAndOutTangentSame (curves)) {
			return 2;
		} else {
			return 3;
		}
    }


    /// <summary>
    /// step 判断
	///     1. 如果每个点的值一样 并且 每个点的 in和out都等于0 
	///     2. 如果存在两个值以上 并且 除了第一个点和最后一个点以外每个点的 in和out都等于无穷 
	/// 满足以上两个条件中的一个就可以认为是step
	/// 
    /// </summary>
    /// <param name="curves"></param>
    /// <returns></returns>
    private bool IsStep(AnimationCurve[] curves) {
		bool result = true;
		foreach (var curve in curves)
        {
            if (curve != null)
            {
                if (!IsStep(curve))
                {
                    result = false;
                    break;
                }
            }
		}
		return result;
	}

	private bool IsStep(AnimationCurve curve) {
		bool result = true;
		bool isSame = true;
		float oneValue = curve.keys [0].value;
		foreach (var key in curve.keys) {
			if (key.value != oneValue) {
				isSame = false;
				break;
			}
		}

		if(isSame){
			return true;
		}

		if (curve.keys.Length < 2) {
			return true;
		} else {
			for(var i = 1;i < curve.keys.Length;i++){
				if(!(float.IsInfinity (curve.keys [i-1].outTangent) || float.IsInfinity (curve.keys [i].inTangent))){
					return false;
				}
			}
		}

		return result;
	}


    /// <summary>
    /// 判断曲线是否都由线段组成
    /// </summary>
    /// <param name="curves"></param>
    /// <returns></returns>
    private bool IsStraightLine(AnimationCurve[] curves){
		bool result = true;
		foreach (var curve in curves) {

            if (curve != null)
            {
                // 注：可能有关键帧只有1帧，就是线段模式；

                for (int j = 1; j < curve.keys.Length; ++j)
                {
                    Keyframe pt1 = curve.keys[j - 1], pt2 = curve.keys[j];

                    // 注意：time相同，k是Infinity
                    float k = (pt2.value - pt1.value) / (pt2.time - pt1.time);
                    // 线段的判断： 第一个点的out和第二点的in，都要等于两点的斜率
                    bool isLine = IsFloatSame(k, pt1.outTangent) && IsFloatSame(k, pt2.inTangent);
                    if (!isLine)
                    {
                        result = false;
                        break;
                    }
                }
            }
		}

		return result;
    }

	/// <summary>
    /// 判断曲线的每个关键帧都是光滑的
	/// in，out都相等
    /// </summary>
    /// <param name="curves"></param>
    /// <returns></returns>
	private bool IsInAndOutTangentSame(AnimationCurve[] curves)
	{
		bool result = true;
		foreach (var curve in curves)
        {
            if (curve != null)
            {
                foreach (var key in curve.keys)
                {
                    if (!IsFloatSame(key.inTangent, key.outTangent))
                    {
                        result = false;
                        break;
                    }
                }
            }
		}
		return result;
	}

	private Vector3[] getProp1(AnimationCurve[] c, int iT,float[] times)
	{
		/*
		 * 为了适应babylon加载的顺序
		    数据结构
		    step和linear
			    [value,value,value...........]
		    平滑曲线
			    [inAndOut,value,inAndOut,value.............]
		    其他
			    [intangent,value,outangent, intangent,value,outangent, intangent,value,outangent...........................]
		*/
		Vector3[] prop1 = new Vector3[times.Length];
		if(iT == 0 || iT == 1){
			for (int i = 0; i < times.Length; ++i) {
				float currentTime = times [i];
				prop1 [i] = new Vector3 (c [0].Evaluate (currentTime), c [1].Evaluate (currentTime), c [2].Evaluate (currentTime));
			}
		}else if(iT == 2){
			prop1 = new Vector3[times.Length*2];
			for (int i = 0; i < times.Length; ++i) {
				float currentTime = times [i];
				prop1 [i*2] = new Vector3 ((float)c [0].keys[i].inTangent, (float)c [1].keys[i].inTangent, (float)c [2].keys[i].inTangent);
				prop1 [i*2 +1] = new Vector3 ((float)c [0].Evaluate (currentTime), (float)c [1].Evaluate (currentTime), (float)c [2].Evaluate (currentTime));
			}
		}else{
			prop1 = new Vector3[times.Length*3];
			for (int i = 0; i < times.Length; ++i) {
				float currentTime = times [i];
				prop1[i*3] = new Vector4(c[0].keys[i].inTangent, c[1].keys[i].inTangent, c[2].keys[i].inTangent);
				prop1[i*3+1] = new Vector4(c[0].Evaluate(currentTime), c[1].Evaluate(currentTime), c[2].Evaluate(currentTime));
				prop1[i*3+2] = new Vector4(c[0].keys[i].outTangent, c[1].keys[i].outTangent, c[2].keys[i].outTangent);
			}
		}
		return prop1;

    }
    private Vector3[] getProp1_Eular(AnimationCurve[] c, int iT, float[] times)
    {
        /*
		 * 为了适应babylon加载的顺序
		    数据结构
		    step和linear
			    [value,value,value...........]
		    平滑曲线
			    [inAndOut,value,inAndOut,value.............]
		    其他
			    [intangent,value,outangent, intangent,value,outangent, intangent,value,outangent...........................]
		*/
        float temp = Mathf.PI / 180.0f;
        Vector3[] prop1 = new Vector3[times.Length];
        if (iT == 0 || iT == 1)
        {
            for (int i = 0; i < times.Length; ++i)
            {
                float currentTime = times[i];
                prop1[i] = new Vector3(c[0].Evaluate(currentTime) * temp, c[1].Evaluate(currentTime) * temp, c[2].Evaluate(currentTime) * temp);
            }
        }
        else if (iT == 2)
        {
            prop1 = new Vector3[times.Length * 2];
            for (int i = 0; i < times.Length; ++i)
            {
                float currentTime = times[i];
                prop1[i * 2] = new Vector3((float)c[0].keys[i].inTangent * temp, (float)c[1].keys[i].inTangent * temp, (float)c[2].keys[i].inTangent * temp);
                prop1[i * 2 + 1] = new Vector3((float)c[0].Evaluate(currentTime) * temp, (float)c[1].Evaluate(currentTime) * temp, (float)c[2].Evaluate(currentTime) * temp);
            }
        }
        else
        {
            prop1 = new Vector3[times.Length * 3];
            for (int i = 0; i < times.Length; ++i)
            {
                float currentTime = times[i];
                prop1[i * 3] = new Vector4(c[0].keys[i].inTangent * temp, c[1].keys[i].inTangent * temp, c[2].keys[i].inTangent * temp);
                prop1[i * 3 + 1] = new Vector4(c[0].Evaluate(currentTime) * temp, c[1].Evaluate(currentTime) * temp, c[2].Evaluate(currentTime) * temp);
                prop1[i * 3 + 2] = new Vector4(c[0].keys[i].outTangent * temp, c[1].keys[i].outTangent * temp, c[2].keys[i].outTangent * temp);
            }
        }
        return prop1;

    }
    private Vector4[] getProp2(AnimationCurve[] c, int iT,float[] times)
	{
		Vector4[] prop1= new Vector4[times.Length];
		if(iT == 0 || iT == 1){
			for (int i = 0; i < times.Length; ++i) {
				float currentTime = times [i];
				prop1 [i] = new Vector4 (c [0].Evaluate (currentTime), c [1].Evaluate (currentTime), c [2].Evaluate (currentTime), c [3].Evaluate (currentTime));
			}
		}else if(iT == 2){
			prop1 = new Vector4[times.Length*2];
			for (int i = 0; i < times.Length; ++i) {
				float currentTime = times [i];
				prop1 [i*2] = new Vector4 (c [0].keys[i].inTangent, c [1].keys[i].inTangent, c [2].keys[i].inTangent, c [3].keys[i].inTangent);
				prop1 [i*2 +1] = new Vector4 (c [0].Evaluate (currentTime), c [1].Evaluate (currentTime), c [2].Evaluate (currentTime), c [3].Evaluate (currentTime));
			}
		}else{
			prop1 = new Vector4[times.Length*3];
			for (int i = 0; i < times.Length; ++i) {
				float currentTime = times [i];
				prop1[i*3] = new Vector4(c[0].keys[i].inTangent, c[1].keys[i].inTangent, c[2].keys[i].inTangent, c [3].keys[i].inTangent);
				prop1[i*3+1] = new Vector4(c[0].Evaluate(currentTime), c[1].Evaluate(currentTime), c[2].Evaluate(currentTime), c [3].Evaluate (currentTime));
				prop1[i*3+2] = new Vector4(c[0].keys[i].outTangent, c[1].keys[i].outTangent, c[2].keys[i].outTangent, c [3].keys[i].outTangent);
			}
		}

		return prop1;
	}

	public void addAnimCount (string targetId){
		GlTF_PiAnimNode.Add(targetId);
		exportAnim.Add(newAnim1);

		GlTF_Anim_Count trAnimCount = animCounts.Find (x => x.trName == targetId);

		if (trAnimCount == null) {
			trAnimCount = new GlTF_Anim_Count (targetId);
			animCounts.Add(trAnimCount);
		} else {
			trAnimCount.animCount++;
		}
	}
}
#endif