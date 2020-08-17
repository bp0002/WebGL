using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace NodeTreeExportor
{
    public class NodeTreeExportorExport
    {
        private static List<NodeExportData> tempList = new List<NodeExportData>();

        public static void doExport()
        {
            tempList.Clear();

            if (Selection.gameObjects.Length >= 1)
            {

                var JSONSTR = export();
                var fs = File.Open(Parame.exportPath + "/" + Parame.exportFileName + ".json", FileMode.Create);
                var jsonWriter = new StreamWriter(fs);

                jsonWriter.Write(JSONSTR);

                jsonWriter.Close();
                fs.Close();
            }
            else
            {
                Debug.LogWarning("请选择需要导出的节点");
            }
        }


        public static string export()
        {
            var tempRoots = Selection.gameObjects;

            var count = tempRoots.Length;

            var roots = new List<Transform>();

            var rootInfo = new NodeExportData(null, tempList.Count);
            tempList.Add(rootInfo);

            string JSONSTR = "[";

            for (int i = 0; i < count; i++)
            {
                var tr = tempRoots[i].transform;

                if (tr.gameObject.activeInHierarchy == true)
                {
                    var info = new NodeExportData(tr, tempList.Count);
                    tempList.Add(info);
                    roots.Add(tr);
                    rootInfo.children.Add(info.index);
                }
            }

            count = roots.Count;
            for (int i = 0; i < count; i++)
            {
                var tr = roots[i];

                var info = tempList[i];

                exportChildren(tr, info);
            }

            count = tempList.Count;

            for (int i = 0; i < count; i++)
            {
                JSONSTR += tempList[i].toString();

                if (i < count - 1)
                {
                    JSONSTR += ",";
                }
            }

            JSONSTR += "]";

            return JSONSTR;
        }

        private static void exportChildren(Transform parentTR, NodeExportData parentData)
        {
            var childCount = parentTR.childCount;

            for (int i = 0; i < childCount; i++)
            {
                var child = parentTR.GetChild(i);
                if (child.gameObject.activeInHierarchy == true)
                {
                    var info = new NodeExportData(child, tempList.Count);
                    tempList.Add(info);
                    parentData.children.Add(info.index);

                    exportChildren(child, info);
                }
            }
        }
    }
}
