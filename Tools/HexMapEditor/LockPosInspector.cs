using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    [CustomEditor(typeof(LockPos))]
    public class LockPosInspector : Editor
    {
        LockPos obj;
        void Awake()
        {
            obj = target as LockPos;
        }
        void OnSceneGUI()
        {
            obj.transform.localPosition = new Vector3(0, 0, 0);
            obj.transform.localScale = new Vector3(1, 1, 1);
            obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
