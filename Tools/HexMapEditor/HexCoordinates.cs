using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HexMapEditor
{
    public class HexCoordinates
    {
        public int hx = 0;
        public int hy = 0;
        public int hz = 0;

        public float fx = 0;
        public float fy = 0;
        public float fz = 0;

        //public int ix;
        //public int iy;
        //public int iz;

        public int col = 0;
        public int row = 0;

        /// <summary>
        /// false: 尖角向上
        /// </summary>
        public readonly Boolean isRotate;

        public readonly Boolean isHex;

        public HexCoordinates(Boolean _isHex, Boolean _isRotate)
        {
            this.isHex = _isHex;
            this.isRotate = _isRotate;
        }

        public List<HexCoordinates> SelectByDistance(int distance, float size, Boolean isRotate)
        {
            var list = new List<HexCoordinates>();

            if (Parame.distanceTypeIndex != 2)
            {
                list.Add(this);
            }

            if (distance < 1)
            {
            }
            else
            {
                int start = 0;

                switch (Parame.distanceTypeIndex)
                {
                    case (0):
                        {
                            start = distance + 1;
                            break;
                        }
                    case (1):
                        {
                            start = distance;
                            break;
                        }
                    case (2):
                        {
                            start = distance;
                            break;
                        }
                    case (3):
                        {
                            start = 1;
                            break;
                        }
                }
                for (int t = start; t <= distance; t++)
                {
                    for (int i = 0; i <= t; i++)
                    {
                        list.Add(HexCoordinates.FromIntPostionHex(
                            this.hx + t,
                            this.hy + (-i),
                            this.hz + (-(t - i)),
                            size, isRotate)
                            );
                        list.Add(HexCoordinates.FromIntPostionHex(
                            this.hx + t * -1,
                            this.hy + (-(t - i)) * -1,
                            this.hz + (-i) * -1,
                            size, isRotate)
                            );
                    }
                    for (int i = 0; i <= t; i++)
                    {
                        list.Add(HexCoordinates.FromIntPostionHex(
                            this.hx + (-i),
                            this.hy + t,
                            this.hz + (-(t - i)),
                            size, isRotate)
                            );
                        list.Add(HexCoordinates.FromIntPostionHex(
                            this.hx + (-(t - i)) * -1,
                            this.hy + t * -1,
                            this.hz + (-i) * -1,
                            size, isRotate)
                            );
                    }
                    for (int i = 0; i <= t; i++)
                    {
                        list.Add(HexCoordinates.FromIntPostionHex(
                            this.hx + (-(t - i)),
                            this.hy + (-i),
                            this.hz + t,
                            size, isRotate)
                            );
                        list.Add(HexCoordinates.FromIntPostionHex(
                            this.hx + (-i) * -1,
                            this.hy + (-(t - i)) * -1,
                            this.hz + t * -1,
                            size, isRotate)
                            );
                    }
                }

            }


            return list;
        }

        public static HexCoordinates FromFloatCenterPositionHex(float centerX, float centerY, float centerZ, float size, Boolean isRotate)
        {
            return HexCoordinates.FromPixelPositionHex(centerX, centerY, centerZ, size, isRotate);
        }

        public static HexCoordinates FromRowColHex(int col, int row, float size, Boolean isRotate)
        {
            size = size / 2.0f;

            var res = new HexCoordinates(true, isRotate);

            res.col = col;
            res.row = row;

            if (!isRotate)
            {
                res.hx = col - (row - (row & 1)) / 2;
                res.hz = row;
                res.hy = -res.hx - res.hz;

                int iX = res.hx;
                int iY = res.hy;
                int iZ = res.hz;

                res.fx = size * (Mathf.Sqrt(3) * iX + Mathf.Sqrt(3) / 2 * iZ);
                res.fz = size * (3.0f / 2 * iZ);
            }
            else
            {
                res.hx = col;
                res.hz = row - (col - (col & 1)) / 2;
                res.hy = -res.hx - res.hz;

                int iX = res.hx;
                int iY = res.hy;
                int iZ = res.hz;

                res.fx = size * (3.0f / 2 * iX);
                res.fz = size * (Mathf.Sqrt(3) / 2 * iX + Mathf.Sqrt(3) * iZ);
            }

            return res;
        }

        public static HexCoordinates FromIntPostionHex(int iX, int iY, int iZ, float size, Boolean isRotate)
        {
            size = size / 2.0f;

            var res = new HexCoordinates(true, isRotate);

            if (!isRotate)
            {

                res.hx = iX;
                res.hy = iY;
                res.hz = iZ;
                res.fx = size * (Mathf.Sqrt(3) * iX + Mathf.Sqrt(3) / 2 * iZ);
                res.fz = size * (3.0f / 2 * iZ);

                res.col = res.hx + (res.hz - (res.hz % 1)) / 2;
                res.row = res.hz;
            } 
            else
            {
                res.hx = iX;
                res.hy = iY;
                res.hz = iZ;
                res.fx = size * (3.0f / 2 * iX);
                res.fz = size * (Mathf.Sqrt(3) / 2 * iX + Mathf.Sqrt(3) * iZ);

                res.col = res.hx;
                res.row = res.hz + (res.hx - (res.hx % 1)) / 2;
            }

            return res;
        }

        public static HexCoordinates FromPixelPositionHex(float x, float y, float z, float size, Boolean isRotate)
        {
            size = size / 2.0f;

            var res = new HexCoordinates(true, isRotate);

            float fx, fy, fz;
            //int hx, hy, hz;

            fx = x;
            fy = y;
            fz = z;

            // 尖角向上
            if (!isRotate)
            {
                float q = (Mathf.Sqrt(3) / 3 * x - 1.0f / 3 * z) / size;
                float r = (2.0f / 3 * z) / size;

                float fhx = q;
                float fhy = -q - r;
                float fhz = r;

                int iX = Mathf.RoundToInt(fhx);
                int iY = Mathf.RoundToInt(fhy);
                int iZ = Mathf.RoundToInt(fhz);

                if (iX + iY + iZ != 0)
                {
                    float dx = Mathf.Abs(fhx - iX);
                    float dy = Mathf.Abs(fhy - iY);
                    float dz = Mathf.Abs(-fhx - fhy - iZ);

                    if (dx > dy && dx > dz)
                    {
                        iX = -iY - iZ;
                    }
                    else
                    {
                        iZ = -iX - iY;
                    }
                }

                res.hx = iX;
                res.hy = iY;
                res.hz = iZ;
                res.fx = size * (Mathf.Sqrt(3) * iX + Mathf.Sqrt(3) / 2 * iZ);
                res.fz = size * (3.0f / 2 * iZ);
                    
                res.col = res.hx + (res.hz - (res.hz % 1)) / 2;
                res.row = res.hz;
            } else
            {
                float q = (2.0f / 3 * x) / size;
                float r = (-1.0f / 3 * x + Mathf.Sqrt(3) / 3 * z) / size;

                float fhx = q;
                float fhy = -q - r;
                float fhz = r;

                int iX = Mathf.RoundToInt(fhx);
                int iY = Mathf.RoundToInt(fhy);
                int iZ = Mathf.RoundToInt(fhz);

                if (iX + iY + iZ != 0)
                {
                    float dx = Mathf.Abs(fhx - iX);
                    float dy = Mathf.Abs(fhy - iY);
                    float dz = Mathf.Abs(-fhx - fhy - iZ);

                    if (dx > dy && dx > dz)
                    {
                        iX = -iY - iZ;
                    }
                    else
                    {
                        iZ = -iX - iY;
                    }
                }


                res.hx = iX;
                res.hy = iY;
                res.hz = iZ;
                res.fx = size * (3.0f / 2 * iX);
                res.fz = size * (Mathf.Sqrt(3) / 2 * iX + Mathf.Sqrt(3) * iZ);

                res.col = res.hx;
                res.row = res.hz + (res.hx - (res.hx % 1)) / 2;
            }

            
            res.fy = fy;


            return res;
        }

        public static List<HexCoordinates> FromPixelPositionHex(float x, float y, float z, float size, Boolean isRotate, int distance)
        {
            var simple = HexCoordinates.FromPixelPositionHex(x, y, z, size, isRotate);

            return simple.SelectByDistance(distance, size, isRotate);
        }

        public static HexCoordinates FromPositionHex(Vector3 vec, Boolean isRotate, float outR, float innerR)
        {
            return HexCoordinates.FromPixelPositionHex(vec.x, vec.y, vec.z, outR, isRotate);
        }

        ///////////////////////////////// 正方形

        
        public List<HexCoordinates> SelectByDistanceSquare(int distance, float size, Boolean isRotate)
        {
            var list = new List<HexCoordinates>();

            if (Parame.distanceTypeIndex != 2)
            {
                list.Add(this);
            }

            if (distance < 1)
            {
            }
            else
            {
                int start = 0;

                switch (Parame.distanceTypeIndex)
                {
                    case (0):
                        {
                            start = distance + 1;
                            break;
                        }
                    case (1):
                        {
                            start = distance;
                            break;
                        }
                    case (2):
                        {
                            start = distance;
                            break;
                        }
                    case (3):
                        {
                            start = 1;
                            break;
                        }
                }

                for (int t = start; t <= distance; t++)
                {
                    int temp = t * 2 + 1;

                    for (int i = 0; i < temp; i++)
                    {
                        if (isRotate)
                        {
                            list.Add(HexCoordinates.FromIntPostionSquare(
                                    this.hx + t,
                                    0,
                                    this.hz + t - i,
                                    size,
                                    isRotate
                                    )
                                );
                            list.Add(HexCoordinates.FromIntPostionSquare(
                                    this.hx + t * -1,
                                    0,
                                    this.hz + t - i,
                                    size,
                                    isRotate
                                    )
                                );
                            list.Add(HexCoordinates.FromIntPostionSquare(
                                    this.hx + t - i,
                                    0,
                                    this.hz + t,
                                    size,
                                    isRotate
                                    )
                                );
                            list.Add(HexCoordinates.FromIntPostionSquare(
                                    this.hx + t - i,
                                    0,
                                    this.hz - t,
                                    size,
                                    isRotate
                                    )
                                );
                        } else
                        {
                            list.Add(HexCoordinates.FromIntPostionSquare(
                                    this.hx + t,
                                    0,
                                    this.hz - i,
                                    size,
                                    isRotate
                                    )
                                );
                            list.Add(HexCoordinates.FromIntPostionSquare(
                                    this.hx + t * -1,
                                    0,
                                    this.hz + i,
                                    size,
                                    isRotate
                                    )
                                );
                            list.Add(HexCoordinates.FromIntPostionSquare(
                                    this.hx + t - i,
                                    0,
                                    this.hz + i,
                                    size,
                                    isRotate
                                    )
                                );
                            list.Add(HexCoordinates.FromIntPostionSquare(
                                    this.hx - t + i,
                                    0,
                                    this.hz - i,
                                    size,
                                    isRotate
                                    )
                                );
                        }
                    }
                }
            }

            return list;
        }

        public static HexCoordinates FromFloatCenterPositionSquare(float centerX, float centerY, float centerZ, float size, Boolean isRotate)
        {
            return HexCoordinates.FromPixelPositionSquare(centerX, centerY, centerZ, size, isRotate);
        }

        public static HexCoordinates FromRowColSquare(int col, int row, float size, Boolean isRotate)
        {
            var res = new HexCoordinates(false, isRotate);

            return res;
        }

        public static HexCoordinates FromIntPostionSquare(int iX, int iY, int iZ, float size, Boolean isRotate)
        {
            //size = size * 0.5f;

            var res = new HexCoordinates(false, isRotate);

            if (!isRotate)
            {

                res.hx = iX;
                res.hy = iY;
                res.hz = iZ;
                res.fx = size * (HexMetrics.squrt2 * iX + HexMetrics.squrt2 / 2 * iZ);
                res.fz = size * (HexMetrics.squrt2 / 2 * iZ);

                res.col = res.hx + (res.hz - (res.hz % 1)) / 2;
                res.row = res.hz;
            }
            else
            {
                res.hx = iX;
                res.hy = iY;
                res.hz = iZ;
                res.fx = size * (iX);
                res.fz = size * (iZ);

                res.col = res.hx;
                res.row = res.hz;
            }

            return res;
        }

        public static HexCoordinates FromPixelPositionSquare(float x, float y, float z, float size, Boolean isRotate)
        {
            //size = size / 2.0f;

            var res = new HexCoordinates(true, isRotate);

            float fx, fy, fz;
            //int hx, hy, hz;

            fx = x;
            fy = y;
            fz = z;

            // 尖角向上
            if (!isRotate)
            {
                float q = (HexMetrics.squrt2 / 2 * x - HexMetrics.squrt2 / 2 * z) / size;
                float r = (HexMetrics.squrt2 * z) / size;

                float fhx = q;
                float fhy = -q - r;
                float fhz = r;

                int iX = Mathf.RoundToInt(fhx);
                int iY = 0;
                int iZ = Mathf.RoundToInt(fhz);

                res.hx = iX;
                res.hy = iY;
                res.hz = iZ;
                res.fx = size * (HexMetrics.squrt2 * iX + HexMetrics.squrt2 / 2 * iZ);
                res.fz = size * (HexMetrics.squrt2 / 2 * iZ);

                res.col = res.hx + (res.hz - (res.hz % 1)) / 2;
                res.row = res.hz;
            }
            else
            {
                float q = (x) / size;
                float r = (z) / size;

                float fhx = q;
                float fhy = -q - r;
                float fhz = r;

                int iX = Mathf.RoundToInt(fhx);
                int iY = 0;
                int iZ = Mathf.RoundToInt(fhz);

                //if (iX + iY + iZ != 0)
                //{
                //    float dx = Mathf.Abs(fhx - iX);
                //    float dy = Mathf.Abs(fhy - iY);
                //    float dz = Mathf.Abs(-fhx - fhy - iZ);

                //    if (dx > dy && dx > dz)
                //    {
                //        iX = -iY - iZ;
                //    }
                //    else
                //    {
                //        iZ = -iX - iY;
                //    }
                //}


                res.hx = iX;
                res.hy = iY;
                res.hz = iZ;
                res.fx = size * (iX);
                res.fz = size * (iZ);

                res.col = res.hx;
                res.row = res.hz;
            }


            res.fy = fy;

            return res;
        }

        public static List<HexCoordinates> FromPixelPositionSquare(float x, float y, float z, float size, Boolean isRotate, int distance)
        {
            var simple = HexCoordinates.FromPixelPositionSquare(x, y, z, size, isRotate);

            return simple.SelectByDistanceSquare(distance, size, isRotate);

        }

        public static HexCoordinates FromPositionSquare(Vector3 vec, Boolean isRotate, float outR)
        {
            return HexCoordinates.FromPixelPositionSquare(vec.x, vec.y, vec.z, outR, isRotate);
        }

        public static int ComputeDistance(HexCoordinates a, HexCoordinates b)
        {
            if (!a.isHex && !a.isRotate)
            {
                if ((a.hx - b.hx) * (a.hz - b.hz) > 0)
                {
                    return Math.Abs(a.hx - b.hx) + (Math.Abs(a.hy - b.hy) + Math.Abs(a.hz - b.hz));
                } else
                {
                    return Math.Max(Math.Abs(a.hx - b.hx), Math.Max(Math.Abs(a.hy - b.hy), Math.Abs(a.hz - b.hz)));
                }
            }
            else
            {
                return Math.Max(Math.Abs(a.hx - b.hx), Math.Max(Math.Abs(a.hy - b.hy), Math.Abs(a.hz - b.hz)));
            }
        }
    }
}
