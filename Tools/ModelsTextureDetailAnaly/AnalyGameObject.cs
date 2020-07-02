using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ModelTextureDetail
{
    public class AnalyGameObject
    {
        public readonly Transform srcTransform;
        public readonly Mesh srcMesh;


        public GameObject newGameObject;
        public Transform newTransform;
        public Mesh newMesh;

        private AnalyGameObject parent;

        public Boolean isEmpty;

        public ModelAnaly modelAnaly;

        public Texture2D newTexture;

        public AnalyGameObject(Transform tr)
        {
            srcTransform = tr;

            newGameObject = new GameObject();

            if (tr == null)
            {
                newGameObject.name = "CombineRoot";
                newTransform = newGameObject.transform;
            } else
            {
                newGameObject.name = tr.name;
                newTransform = newGameObject.transform;
                newTransform.localPosition = new Vector3(tr.position.x, tr.position.y, tr.position.z);
                newTransform.localRotation = new Quaternion(tr.rotation.x, tr.rotation.y, tr.rotation.z, tr.rotation.w);
                newTransform.localScale = new Vector3(tr.localScale.x, tr.localScale.y, tr.localScale.z);

                newGameObject.SetActive(tr.gameObject.activeInHierarchy);
            }
        }

        public void changeParent(AnalyGameObject ago)
        {
            parent = ago;
            newTransform.parent = ago.newTransform;
        }

        public void setModelAnaly(ModelAnaly ma)
        {
            modelAnaly = ma;
        }

        /// <summary>
        /// 创建 MeshFilter
        /// 创建 MeshRenderer
        /// 创建 Material
        /// </summary>
        public void createData(Material material)
        {
            if (modelAnaly != null)
            {

                var filter = newGameObject.AddComponent<MeshFilter>();
                filter.sharedMesh = new Mesh();
                filter.sharedMesh.name = newGameObject.name + "_analy";
                filter.sharedMesh.SetVertices(new List<Vector3>(modelAnaly.mesh.vertices));
                filter.sharedMesh.SetNormals(new List<Vector3>(modelAnaly.mesh.normals));
                filter.sharedMesh.SetUVs(0, new List<Vector2>( modelAnaly.newUVs ) );
                filter.sharedMesh.SetTriangles(modelAnaly.mesh.triangles, 0);

                var renderer = newGameObject.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;

                //UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab("Assets/" + newGameObject.name + "_analy" + ".prefab");
                //PrefabUtility.ReplacePrefab(newGameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);

                UnityEngine.Object prefab = PrefabUtility.CreatePrefab("Assets/" + newGameObject.name + "_analy" + ".prefab", newGameObject);
            }
        }
    }
}
