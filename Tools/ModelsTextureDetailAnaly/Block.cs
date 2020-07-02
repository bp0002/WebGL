using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModelTextureDetail
{
    /// <summary>
    /// 装箱计算用的数据
    /// </summary>
    public class Block
    {
        public readonly int width;
        public readonly int height;
        public readonly string uid;
        public int x;
        public int y;
        public Block(string str, int w, int h)
        {
            this.width = w;
            this.height = h;
            this.uid = str;
        }
    }
}
