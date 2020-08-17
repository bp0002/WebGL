using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.shader
{
    class LightmapTexRecord
    {
        public int hash;
        public Texture2D tex;
        public LightmapTexRecord(int hashCode, Texture2D texture2D)
        {
            hash = hashCode;
            tex = texture2D;
        }
    }
}
