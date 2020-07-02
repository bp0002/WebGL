﻿using System;
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

                    if (GUILayout.Button("Create Template"))
                    {
                        createTemplate();
                    }
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
