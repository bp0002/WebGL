using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HexMapEditor
{
    public class HexCellDynamicTemplate : MonoBehaviour
    {
        public List<string> attrNames = new List<string>();
        public List<string> attrValues = new List<string>();
        public Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public int cellSize = 1;

        public byte terrain = 0;

        public Material mat;
    }
}
