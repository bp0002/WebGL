using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HexMapEditor
{
    public class HexMapComponent : MonoBehaviour
    {
        public string Name = "HexMap";

        [Range(0.0f, 1.0f)]
        public float GridCellEdgeWidth = 0.0f;

        public Boolean GridEnableEdge = false;
    }
}
