using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    class HexCellDynamicComponent : MonoBehaviour
    {

        private GameObject go;

        public List<string> attrNames = new List<string>();
        public List<string> attrValues = new List<string>();

        [SerializeField]
        private int _x;
        public int x
        {
            get { return _x; }
        }

        [SerializeField]
        private int _y;
        public int y
        {
            get { return _y; }
        }

        [SerializeField]
        private int _z;
        public int z
        {
            get { return _z; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        [SerializeField]
        private float _size;
        public float size
        {
            get { return _size; }
        }

        [SerializeField]
        private string _TemplateName;
        public string TemplateName
        {
            get { return _TemplateName; }
        }

        //[SerializeField]
        private UnityEngine.Object _prefab;
        //public UnityEngine.Object prefab
        //{
        //    get { return _prefab; }
        //}

        private Color _color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        private void OnEnable()
        {
            Debug.LogWarning("onenable");
        }

        private void OnMouseOver()
        {
            Debug.LogWarning("OnMouseOver");
        }

        public void initData(int i, int j, int k, HexCellDynamicTemplate template)
        {
            _x = i;
            _y = j;
            _z = k;
            _size = template.cellSize;
            _TemplateName = template.name;

            _name = x + "_" + y + "_" + z;

            go = transform.gameObject;
            go.name = _name;
            transform.name = _name;
        }

        public void CreatePlane()
        {
            go = transform.gameObject;

            go.name = _name;
            transform.name = _name;

            var filter = go.AddComponent<MeshFilter>();
            filter.sharedMesh = HexMetrics.GetPlanMesh();
            
            var renderer = go.AddComponent<MeshRenderer>();


            //_prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + _name + "_HexCell" + ".prefab");
            //PrefabUtility.ReplacePrefab(go, _prefab, ReplacePrefabOptions.ConnectToPrefab);

            //Undo.RecordObject(_prefab, go.name);
            //EditorUtility.SetDirty(_prefab);
        }

        public void UpdateHexPlane()
        {
            if (go == null)
            {
                go = transform.gameObject;
            }
            var filter = go.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = go.AddComponent<MeshFilter>();
            }
            filter.sharedMesh = HexMetrics.GetHexMesh();

            //Undo.RecordObject(_prefab, go.name);
            //EditorUtility.SetDirty(_prefab);

            //AssetDatabase.Refresh();
        }

        public void UpdatePlane()
        {
            if (go == null)
            {
                go = transform.gameObject;
            }
            var filter = go.GetComponent<MeshFilter>();
            if (filter == null)
            {
                filter = go.AddComponent<MeshFilter>();
            }
            filter.sharedMesh = HexMetrics.GetPlanMesh();

            //Undo.RecordObject(_prefab, go.name);
            //EditorUtility.SetDirty(_prefab);


            //AssetDatabase.Refresh();
        }

        public void UpdateColor(Color color)
        {
            _color.r = color.r;
            _color.g = color.g;
            _color.b = color.b;
            _color.a = color.a;
            _updateColor();
        }

        public void UpdateMaterial(Material mat)
        {
            if (go == null)
            {
                go = transform.gameObject;
            }

            go.GetComponent<MeshRenderer>().sharedMaterial = mat;

            //Undo.RecordObject(_prefab, go.name);
            //EditorUtility.SetDirty(_prefab);
        }

        private void _updateColor()
        {
            Color[] colorList = new Color[1];
            colorList[0] = _color;
            if (go == null)
            {
                go = transform.gameObject;
            }

            go.GetComponent<MeshRenderer>().sharedMaterial.SetColorArray("_Color", colorList);
            go.GetComponent<MeshRenderer>().sharedMaterial.color = _color;

            Undo.RecordObject(_prefab, go.name);
            EditorUtility.SetDirty(_prefab);
        }

        public void destory()
        {
            if (go == null)
            {
                go = transform.gameObject;
            }
            GameObject.DestroyImmediate(go);
        }


        public Boolean checkActive()
        {
            Boolean flag = true;

            if (!gameObject.activeInHierarchy)
            {
                flag = false;
            }

            var filter = gameObject.GetComponent<MeshFilter>();
            if (filter == null || filter.sharedMesh == null)
            {
                flag = false;
            }

            var render = gameObject.GetComponent<MeshRenderer>();
            if (render == null || render.sharedMaterial == null || render.sharedMaterial.color.a < 0.01f)
            {
                flag = false;
            }

            return flag;
        }

        /// <summary>
        /// 通过模板更新 数据
        /// </summary>
        /// <param name="template"></param>
        public void UpdateFromTemplate(HexCellDynamicTemplate template)
        {
            attrNames.Clear();
            attrValues.Clear();
            foreach (var str in template.attrNames)
            {
                attrNames.Add(str);
            }
            foreach (var value in template.attrValues)
            {
                attrValues.Add(value);
            }

            if (template.mat == null)
            {
                template.mat = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
                template.mat.renderQueue = 4000;

                AssetDatabase.CreateAsset(template.mat, "Assets/" + template.transform.name + ".mat");
            }

            template.mat.color = template.color;

            _size = template.cellSize;
            _TemplateName = template.name;

            go.transform.localScale = new Vector3(template.cellSize - 0.05f, template.cellSize - 0.05f, template.cellSize - 0.05f);

            //UpdateColor(template.color);
            UpdateMaterial(template.mat);
        }
    }
}
