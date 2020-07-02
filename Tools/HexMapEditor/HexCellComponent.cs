using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    public class HexCellCustomAttr
    {
        public string name;
        public string data;
    }
    public class HexCellComponent : MonoBehaviour
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

        private Color _color = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        private void OnEnable()
        {
            Debug.LogWarning("onenable");
        }

        private void OnDestroy()
        {
            var gridComp = gameObject.GetComponentInParent<HexGridComponent>();
            gridComp.RemoveCell(this);
        }

        private void OnMouseOver()
        {
            Debug.LogWarning("OnMouseOver");
        }

        public void initData(int i, int j, int k)
        {
            _x = i;
            _y = j;
            _z = k;

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

            var material = new Material(Shader.Find("Legacy Shaders/Transparent/Diffuse"));
            var renderer = go.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = material;
            _updateColor();

            //_prefab = PrefabUtility.CreatePrefab("Assets/" + _name + "_HexCell" + ".prefab", go);
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
        }

        public void UpdateColor(Color color)
        {
            _color.r = color.r;
            _color.g = color.g;
            _color.b = color.b;
            _color.a = color.a;
            _updateColor();
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
        }

        public void destory()
        {
            if (go == null)
            {
                go = transform.gameObject;
            }
            GameObject.DestroyImmediate(go);
        }

        /// <summary>
        /// 通过模板更新 数据
        /// </summary>
        /// <param name="template"></param>
        public void UpdateFromTemplate(HexCellTemplate template)
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

            UpdateColor(template.color);
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
    }

}
