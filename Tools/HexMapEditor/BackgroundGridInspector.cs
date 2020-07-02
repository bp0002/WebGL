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
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(Desc, MessageType.Info);

            DrawDefaultInspector();

            var _target = target as BackgroundGrid;
            if (GUILayout.Button("Create"))
            {
                _target.Create();
            }
        }
    }
}
