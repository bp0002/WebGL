using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.ExporterGLTF20
{
    public class ExportOption
    {
        /// <summary>
        /// 导出的文件路径，名称
        /// </summary>
        public string mFileName = "";
        /// <summary>
        /// 导出文件所在目录
        /// </summary>
        public string mPrePath = "";
        /// <summary>
        /// 导出文件所在目录
        /// </summary>
        public string mPath = "";
        /// <summary>
        /// 是否导出 PBR
        /// </summary>
        public bool mPBR = true;
        /// <summary>
        /// 是否导出动画数据
        /// </summary>
        public bool mAnimation = true;
        /// <summary>
        /// 是否反转纹理
        /// </summary>
        public bool mConvertImage = true;
        /// <summary>
        /// 是否导出法线
        /// </summary>
        public bool mNormal = false;
        /// <summary>
        /// 是否合并网格 - 实际只是把各模型网格数据放同一文件，并不是合并网格
        /// </summary>
        public bool mCombineMesh = true;
        /// <summary>
        /// 最大光照图亮度倍数
        /// </summary>
        public float mMaxLightmapMultiple = 2.0f;
        /// <summary>
        /// 是否转换为右手坐标系 - Unity 为左手坐标系
        /// </summary>
        public bool convertRightHanded = false;
        /// <summary>
        /// 是否导出光照的阴影生成信息
        /// </summary>
        public bool mShadow = false;
        /// <summary>
        /// 是否使用模型名称创建导出文件夹 - 最终路径为 导出路径 + 模型名称/ + 模型名称.gltf
        /// </summary>
        public bool creatDirectoryByModelName = true;
        /// <summary>
        /// 是否使用选择的模型名称命名
        /// </summary>
        public bool useGLTFNameByModelName = true;
        public Preset presetAsset = null;
        public ExportOption()
        {
        }
    }
}
