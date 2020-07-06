using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    [CustomEditor(typeof (HexMapComponent))]
    public class HexMapEditor : Editor
    {
        public static string BackgroundDesc = "创建背景网格,用于辅助编辑.";
        public static string HexGridDesc = "(已淘汰)创建网格编辑的根节点,已预创建了单元格。";
        public static string HexGridDynamicDesc = "创建网格编辑的根节点,编辑时动态创建单元格。(编辑模式更灵活)";

        private HexMapComponent hexMap;
        private GameObject gameObject;
        private GameObject template;

        private void OnEnable()
        {

            if (hexMap == null && Selection.gameObjects.Length == 1)
            {
                //var gameObject = GameObject.Find(this.target.name);
                gameObject = Selection.gameObjects[0];
                if (gameObject == this.target)
                {
                    hexMap = gameObject.GetComponent<HexMapComponent>();
                }
            }

            //if (gameObject != null)
            //{
            //    var temp = gameObject.transform.FindChild("HexCellTemplates");
            //    if (temp == null)
            //    {
            //        GameObject go = new GameObject();
            //        go.name = "HexCellTemplates";
            //        go.transform.name = go.name;
            //        go.transform.parent = gameObject.transform;
            //        go.transform.hideFlags = HideFlags.NotEditable;
            //        go.AddComponent<HexCellTemplateList>();

            //        template = go;
            //    } else
            //    {
            //        template = temp.gameObject;
            //    }
            //}
        }

        private void OnDisable()
        {
            hexMap = null;
            gameObject = null;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Create Background Grid"))
            {
                createBackgroundGrid();
            }
            EditorGUILayout.HelpBox(BackgroundDesc, MessageType.Info);

            if (GUILayout.Button("Create New HexGrid"))
            {
                createHexGrid();
            }
            EditorGUILayout.HelpBox(HexGridDesc, MessageType.Info);

            //if (GUILayout.Button("Create New HexCell Template"))
            //{
            //    createHexCellTemplate();
            //}

            if (GUILayout.Button("Create New HexGridDynamic"))
            {
                createHexGridDynamic();
            }
            EditorGUILayout.HelpBox(HexGridDynamicDesc, MessageType.Info);
        }

        private void createBackgroundGrid()
        {
            BackgroundGrid backgroundGrid = gameObject.GetComponentInChildren<BackgroundGrid>();

            if (backgroundGrid == null)
            {
                GameObject go = new GameObject();
                go.name = "BackgroundGrid";
                go.transform.name = go.name;
                go.transform.parent = gameObject.transform;
                go.transform.localPosition = new Vector3(0, 0, 0);
                go.AddComponent<BackgroundGrid>();
                go.AddComponent<LockPos>();
            }
        }

        private void createHexGrid()
        {
            GameObject go = new GameObject();
            go.name = "HexGrid" + getChildHexGrids().Count;
            go.transform.name = go.name;
            go.transform.parent = gameObject.transform;
            go.AddComponent<HexGridComponent>();
            go.AddComponent<LockPos>();

            GameObject cellsParent = new GameObject();
            cellsParent.name = "CellsList";
            cellsParent.transform.name = cellsParent.name;
            cellsParent.transform.parent = go.transform;
            cellsParent.transform.localPosition = new Vector3(0, 0, 0);
            cellsParent.transform.localScale = new Vector3(1, 1, 1);
            cellsParent.transform.hideFlags = HideFlags.NotEditable;
            cellsParent.AddComponent<LockPos>();
            cellsParent.AddComponent<CellListComponent>();

            GameObject goTemplate = new GameObject();
            goTemplate.name = "HexCellTemplates";
            goTemplate.transform.name = goTemplate.name;
            goTemplate.transform.parent = go.transform;
            goTemplate.AddComponent<HexCellTemplateList>();
            goTemplate.transform.localPosition = new Vector3(0, 0, 0);
            goTemplate.transform.localScale = new Vector3(1, 1, 1);
            goTemplate.transform.hideFlags = HideFlags.NotEditable;
            goTemplate.AddComponent<LockPos>();
        }

        private void createHexGridDynamic()
        {
            GameObject go = new GameObject();
            go.name = "HexGridDynamic" + getChildHexGrids().Count;
            go.transform.name = go.name;
            go.transform.parent = gameObject.transform;
            go.AddComponent<HexGridDynamicComponent>();
            go.AddComponent<LockPos>();

            GameObject cellsParent = new GameObject();
            cellsParent.name = "CellsList";
            cellsParent.transform.name = cellsParent.name;
            cellsParent.transform.parent = go.transform;
            cellsParent.transform.localPosition = new Vector3(0, 0, 0);
            cellsParent.transform.localScale = new Vector3(1, 1, 1);
            cellsParent.transform.hideFlags = HideFlags.NotEditable;
            cellsParent.AddComponent<LockPos>();
            cellsParent.AddComponent<CellListComponent>();

            GameObject goTemplate = new GameObject();
            goTemplate.name = "HexCellDynamicTemplates";
            goTemplate.transform.name = goTemplate.name;
            goTemplate.transform.parent = go.transform;
            goTemplate.AddComponent<HexCellDynamicTemplateList>();
            goTemplate.transform.localPosition = new Vector3(0, 0, 0);
            goTemplate.transform.localScale = new Vector3(1, 1, 1);
            goTemplate.transform.hideFlags = HideFlags.NotEditable;
            goTemplate.AddComponent<LockPos>();
        }


        private List<GameObject> getChildHexGrids()
        {
            List<GameObject> list = new List<GameObject>();

            var arr = gameObject.GetComponentsInChildren<HexGridComponent>();

            foreach (var grid in arr)
            {
                list.Add(grid.gameObject);
            }

            return list;
        }

        //private void createHexCellTemplate()
        //{
        //    GameObject go = new GameObject();
        //    go.name = "HexGridTemplata" + getHexCellTemplates().Count;
        //    go.transform.name = go.name;
        //    go.transform.parent = template.transform;
        //    go.AddComponent<HexCellTemplate>();
        //}

        //private List<GameObject> getHexCellTemplates()
        //{
        //    List<GameObject> list = new List<GameObject>();

        //    var arr = template.GetComponentsInChildren<HexCellTemplate>();

        //    foreach (var grid in arr)
        //    {
        //        list.Add(grid.gameObject);
        //    }

        //    return list;
        //}
    }
}
