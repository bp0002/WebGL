using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelTextureDetail
{
    /// <summary>
    /// 每个顶点的 UV 数据转换
    /// </summary>
    public class UVInfo
    {
        public int uvIndex;
        public float u0;
        public float v0;

        public int u00;
        public int v00;

        public Boolean isColor;

        /// <summary>
        /// 转换后合并后的新uv
        /// </summary>
        public float u1;
        /// <summary>
        /// 转换后合并后的新uv
        /// </summary>
        public float v1;
    }
}
