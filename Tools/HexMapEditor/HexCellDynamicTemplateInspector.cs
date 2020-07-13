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

            var template = target as HexCellDynamicTemplate;

            var hexMap = template.GetComponentInParent<HexMapComponent>();
            var hexGrid = template.GetComponentInParent<HexGridDynamicComponent>();
            var background = hexMap.GetComponentInChildren<BackgroundGrid>();

            var flag = true;

            if (hexGrid.CellEdgeWidth != 0.0f && background.cellSize != template.cellSize)
            {
                EditorGUILayout.HelpBox(Parame.edgeWaring, MessageType.Error);
                flag = false;
            }

            if (flag)
            {
                if (GUILayout.Button("更新 Cells"))
                {
                    
                    var cells = hexGrid.GetComponentsInChildren<HexCellDynamicComponent>();

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
}
