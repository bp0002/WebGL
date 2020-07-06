using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    [CustomEditor(typeof(BackgroundGrid))]
    public class BackgroundGridInspector : Editor
    {
        private string Desc = "BackgroundGrid - 网格地图的背景,显示对应正方形或六边形网格背景，已限制操作.";
        private string HexDesc = "isHex: 是否为六边形网格, ‘否’ 则为正方形";
        private string RotateDesc = "isRotate: 是否旋转,  ‘是’ 则多边形一顶点向上, '否' 则一条边向上";
        private string SizeDesc = "cellSize: 六边形对角线长度(边长的2倍), 正方形边长";
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(Desc, MessageType.Info);
            EditorGUILayout.HelpBox(HexDesc, MessageType.Info);
            EditorGUILayout.HelpBox(RotateDesc, MessageType.Info);
            EditorGUILayout.HelpBox(SizeDesc, MessageType.Info);

            DrawDefaultInspector();

            var _target = target as BackgroundGrid;
            if (GUILayout.Button("Create"))
            {
                _target.Create();
            }
        }
    }
}
