using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HexMapEditor
{
    public class CellExportBinData
    {
        public int localID;
        public byte cellType;
        public int posX;
        public int posY;
        public int posZ;
        /// <summary>
        /// 自定义数据的第一个数据 - 导出为Bin时只允许一个数据
        /// </summary>
        public int firstValue;

        CellExportBinData()
        {

        }
    }
}
