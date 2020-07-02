using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HexMapEditor
{
    public class HexCellTemplate : MonoBehaviour
    {
        public List<string> attrNames = new List<string>();
        public List<string> attrValues = new List<string>();
        public Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
    }
}
