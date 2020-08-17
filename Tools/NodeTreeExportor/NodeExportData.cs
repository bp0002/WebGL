using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NodeTreeExportor
{
    public class NodeExportData
    {
        public readonly string tname;
        public readonly int index;
        public readonly string[] keys = null;
        public readonly string[] values = null;
        public readonly Vector3 lPos;
        public readonly Vector3 wPos;
        public readonly Vector3 lRot;
        public readonly Vector3 wRot;
        public readonly Vector3 lScl;
        public readonly Vector3 wScl;
        public readonly List<int> children = new List<int>();
        public NodeExportData(Transform tr, int i)
        {
            if (tr == null)
            {
                this.lPos = new Vector3(0, 0, 0);
                this.lRot = new Vector3(0, 0, 0);
                this.lScl = new Vector3(1, 1, 1);

                this.wPos = new Vector3(0, 0, 0);
                this.wRot = new Vector3(0, 0, 0);
                this.wScl = new Vector3(1, 1, 1);

                this.tname = "__root__";
            }
            else
            {
                this.lPos = tr.localPosition;
                this.lRot = tr.localRotation.eulerAngles;
                this.lScl = tr.localScale;

                this.wPos = tr.position;
                this.wRot = tr.rotation.eulerAngles;
                this.wScl = tr.lossyScale;

                this.tname = tr.name;

                NodeCustomData nd = tr.GetComponent<NodeCustomData>();
                if (nd != null)
                {
                    this.keys = nd.keys;
                    this.values = nd.values;
                }
            }
            this.index = i;
        }

        public string toString()
        {
            var str = "{";

            str += "\"name\":\"" + tname + "\",";
            str += "\"lPos\":" + lPos.ToString().Replace('(', '[').Replace(')', ']') + ",";

            str += "\"lRot\":[" + Mathf.RoundToInt(lRot.x * Mathf.PI / 180.0f * 10000) / 10000 + "," + Mathf.RoundToInt(lRot.y * Mathf.PI / 180.0f * 10000) / 10000 + "," + Mathf.RoundToInt(lRot.z * Mathf.PI / 180.0f * 10000) / 10000 + "],";

            str += "\"lScl\":" + lScl.ToString().Replace('(', '[').Replace(')', ']') + ",";
            str += "\"wPos\":" + wPos.ToString().Replace('(', '[').Replace(')', ']') + ",";

            str += "\"wRot\":[" + Mathf.RoundToInt(wRot.x * Mathf.PI / 180.0f * 10000) / 10000 + "," + Mathf.RoundToInt(wRot.y * Mathf.PI / 180.0f * 10000) / 10000 + "," + Mathf.RoundToInt(wRot.z * Mathf.PI / 180.0f * 10000) / 10000 + "],";

            str += "\"wScl\":" + wScl.ToString().Replace('(', '[').Replace(')', ']') + ",";

            str += "\"children\":[";
            int count = children.Count;
            for (int i = 0; i < count; i++)
            {
                str += children[i];

                if (i < count - 1)
                {
                    str += ",";
                }
            }
            str += "],";


            str += "\"keys\":[";
            if (keys != null)
            {
                count = keys.Length;
                for (int i = 0; i < count; i++)
                {
                    str += "\"" + keys[i] + "\"";

                    if (i < count - 1)
                    {
                        str += ",";
                    }
                }
            }
            str += "],";


            str += "\"values\":[";
            if (keys != null)
            {
                count = values.Length;
                for (int i = 0; i < count; i++)
                {
                    str += "\"" + values[i] + "\"";

                    if (i < count - 1)
                    {
                        str += ",";
                    }
                }
            }
            str += "],";

            str += "\"index\":" + index;

            str += "}";

            return str;
        }
    }
}
