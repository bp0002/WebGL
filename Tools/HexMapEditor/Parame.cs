﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HexMapEditor
{
    public static class Parame
    {
        //public static List<Color> colorList = new List<Color>();
        //public static List<string> colorStringList = new List<string>();

        public static string hexMapName = "HexMap";
        public static string hexMapNameDesc = "设置一个网格地图名称,然后点击按钮，创建出网格地图根节点。";

        public static string exportPath;
        public static string exportFileName;
        //public static int selectColorIndex = 0;

        public static Boolean exportHexGridDynamicTerrain = false;
        public static string exportHexGridDynamicTerrainDesc = "是否导出 HexGridDynamic 的 Terrain 数据";

        public static Boolean exportCellWorldPosition   = true;
        public static string exportCellWorldPositionDesc = "导出 单元格坐标数据时，是否导出 绝对坐标,‘否’则导出相对坐标";

        public static Boolean exportCellWorldScale      = false;

        public static Boolean exportSimpleCellData      = false;
        public static string exportSimpleCellDataDesc = "导出 HexGridDynamic 编辑的网格数据时，是否仅导出 单元格信息;仅将编辑器用于地块定位时会用到.导出的简单数据便于Excel操作.";

        public static Boolean activeDown = true;
        public static Boolean activeDrag = true;
        public static Boolean activeUp = false;
        public static Boolean activeEditor = true;
        public static string activeDesc = "场景中选中模板节点时, 按下并拖动鼠标 可进行快速编辑。";

        /// <summary>
        /// 生成 动态大小 正方形 单元格时 锚点位置
        /// </summary>
        public static string[] planOptions = { "中心", "右上", "左下" };

        /// <summary>
        /// 生成 动态大小 菱形 单元格时 锚点位置
        /// </summary>
        public static string[] diamondOptions = { "中心", "正上方", "正下方" };
        public static string OptionsDesc = "编辑的单元格比背景单元格大时,用于确定单元格的生成位置。";

        /// <summary>
        /// 生成 动态大小 正方形 单元格时 锚点位置
        /// </summary>
        public static int planIndex = 1;

        /// <summary>
        /// 生成 动态大小 菱形 单元格时 锚点位置
        /// </summary>
        public static int diamondIndex = 1;

        /// <summary>
        /// 距离场参数说明
        /// </summary>
        public static string distanceDesc = "distance(距离场距离) - 生成 多个 固定大小<模板单元尺寸需与背景尺寸相同> 时要批量生成的单元格与命中单元格的距离<单元格距离>,\n为 0 时,则仅生成命中点处单元格";

        /// <summary>
        /// 生成 多个 固定大小<背景单元格大小> 时要批量生成的单元格与命中单元格的距离<单元格距离>
        /// 为 0 时,则生成命中点处单元格
        /// 模板尺寸 与 背景尺寸相同时才生效
        /// </summary>
        public static int distance = 0;

        /// <summary>
        /// 生成 动态大小 菱形 单元格时 锚点位置
        /// </summary>
        public static string[] distanceTypes = { "仅中心单元", "中心单元 与 最远单元组成的环", "仅最远单元组成的环", "所有满足条件的单元" };

        public static int distanceTypeIndex = 0;

        public static int realTimeDistance = 0;

        public static string edgeWaring = "BackgroundGrid 启用Edge时,模板cellSize需要与BackgroundGrid的celllSize相等!";

        public static Boolean hitInfoFlag = false;
        public static RaycastHit hitInfo = new RaycastHit();

        public static float GridCellEdgeWidth = 0.0f;
        public static Boolean EditEdge = false;
        public static Boolean EditPoint = false;

        public const byte RSquareType = 1;
        public const byte SquareType = 2;
        public const byte RHexType = 3;
        public const byte HexType = 4;
                     
        public const byte RSquareEdgeType1 = 11;
        public const byte RSquareEdgeType2 = 12;
        public const byte RSquareEdgeType3 = 13;
        public const byte RSquareEdgeType4 = 14;
                     
        public const byte RSquarePointType1 = 21;
        public const byte RSquarePointType2 = 22;
        public const byte RSquarePointType3 = 23;
        public const byte RSquarePointType4 = 24;
                     
        public const byte SquareEdgeType1 = 31;
        public const byte SquareEdgeType2 = 32;
        public const byte SquareEdgeType3 = 33;
        public const byte SquareEdgeType4 = 34;
                     
        public const byte SquarePointType1 = 41;
        public const byte SquarePointType2 = 42;
        public const byte SquarePointType3 = 43;
        public const byte SquarePointType4 = 44;
                     
        public const byte RHexEdgeType1 = 51;
        public const byte RHexEdgeType2 = 52;
        public const byte RHexEdgeType3 = 53;
        public const byte RHexEdgeType4 = 54;
        public const byte RHexEdgeType5 = 55;
        public const byte RHexEdgeType6 = 56;
                     
        public const byte RHexPointType1 = 61;
        public const byte RHexPointType2 = 62;
        public const byte RHexPointType3 = 63;
        public const byte RHexPointType4 = 64;
        public const byte RHexPointType5 = 65;
        public const byte RHexPointType6 = 66;
                     
        public const byte HexEdgeType1 = 71;
        public const byte HexEdgeType2 = 72;
        public const byte HexEdgeType3 = 73;
        public const byte HexEdgeType4 = 74;
        public const byte HexEdgeType5 = 75;
        public const byte HexEdgeType6 = 76;
                     
        public const byte HexPointType1 = 81;
        public const byte HexPointType2 = 82;
        public const byte HexPointType3 = 83;
        public const byte HexPointType4 = 84;
        public const byte HexPointType5 = 85;
        public const byte HexPointType6 = 86;

        public static byte[] tempTerrainData = null;
    }
}
