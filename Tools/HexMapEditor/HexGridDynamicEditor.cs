using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    [CustomEditor(typeof (HexGridDynamicComponent))]
    public class HexGridDynamicEditor : Editor
    {
        private string CellListDesc = "创建 CellList 节点，编辑生成的Cell会挂在该节点下.";
        private string TemplateDesc = "创建模板节点,模板节点的数据会应用与编辑出的Cell";
        private string RefreshDesc = "手动删除 Cell 节点后刷新 Min Max 统计";
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Selection.gameObjects.Length == 1)
            {
                var gameObject = Selection.gameObjects[0];
                
                if (gameObject != null)
                {

                    if (GUILayout.Button("Create Cells List Node"))
                    {
                        CellListComponent cellListComp = gameObject.GetComponentInChildren<CellListComponent>();
                        if (cellListComp == null)
                        {
                            GameObject cellsParent = new GameObject();
                            cellsParent.name = "CellsList";
                            cellsParent.transform.name = cellsParent.name;
                            cellsParent.transform.parent = gameObject.transform;
                            cellsParent.transform.localPosition = new Vector3(0, 0, 0);
                            cellsParent.transform.localScale = new Vector3(1, 1, 1);
                            cellsParent.transform.hideFlags = HideFlags.NotEditable;
                            cellsParent.AddComponent<LockPos>();
                            cellsParent.AddComponent<CellListComponent>();
                        }
                    }
                    EditorGUILayout.HelpBox(CellListDesc, MessageType.Info);

                    if (GUILayout.Button("Create Template"))
                    {
                        createTemplate();
                    }
                    EditorGUILayout.HelpBox(TemplateDesc, MessageType.Info);

                    if (GUILayout.Button("Refresh Min Max"))
                    {
                        gameObject.GetComponent<HexGridDynamicComponent>().RefreshMinMax();
                    }
                    EditorGUILayout.HelpBox(RefreshDesc, MessageType.Info);
                }
            }
        }

        private void createTemplate()
        {
            var gameObject = Selection.gameObjects[0];
            try
            {
                var templatesGO = gameObject.GetComponentsInChildren<HexCellDynamicTemplateList>()[0];

                if (templatesGO != null)
                {
                    GameObject goTemplate = new GameObject();
                    goTemplate.name = "HexCellDynamicTemplate" + templatesGO.transform.childCount;
                    goTemplate.transform.name = goTemplate.name;
                    goTemplate.transform.parent = templatesGO.transform;
                    goTemplate.AddComponent<HexCellDynamicTemplate>();
                }
            }
            finally
            {

            }
        }
    }
}
