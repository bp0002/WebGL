using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    [CustomEditor(typeof(HexCellDynamicTemplate))]
    public class HexCellDynamicTemplateInspector : Editor
    {
        private string desc = "修改模板数据后，将数据更新到已依赖该模板创建的Cell";
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("更新 Cells"))
            {
                var template = target as HexCellDynamicTemplate;

                var grid = template.GetComponentInParent<HexGridDynamicComponent>();
                var cells = grid.GetComponentsInChildren<HexCellDynamicComponent>();

                foreach (var cell in cells)
                {
                    if (cell.TemplateName == template.name)
                    {
                        cell.UpdateFromTemplate(template);
                    }
                }
            }
            EditorGUILayout.HelpBox(desc, MessageType.Info);
        }
    }
}
