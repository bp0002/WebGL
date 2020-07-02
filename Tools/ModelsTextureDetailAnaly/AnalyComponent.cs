using ModelTextureDetail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ModelTextureDetail
{
    public class AnalyComponent : MonoBehaviour
    {
        public AnalyTool analyTool = new AnalyTool();
        public void analy()
        {
            analyTool.analy();
            analyTool.combine();
            analyTool.updateGameObjects();
        }
    }
}
