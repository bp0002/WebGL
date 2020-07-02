using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HexMapEditor
{
    public class CustomRayInfo
    {
        public float fx = 0.0f;
        public float fy = 0.0f;
        public float fz = 0.0f;

        public int ix = 0;
        public int iy = 0;
        public int iz = 0;

        public string getName()
        {
            return ix + "_" + iy + "_" + iz;
        }
        public Vector3 getPos()
        {
            return new Vector3(fx, fy, fz);
        }
    }              
}
