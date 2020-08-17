using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.ExporterGLTF20
{
    class AlphaMode
    {

        /** Defines that alpha blending is disabled */
        public const int ALPHA_DISABLE = 0;
        /** Defines that alpha blending to SRC ALPHA * SRC + DEST */
        public const int ALPHA_ADD = 1;
        /** Defines that alpha blending to SRC ALPHA * SRC + (1 - SRC ALPHA) * DEST */
        public const int ALPHA_COMBINE = 2;
        /** Defines that alpha blending to DEST - SRC * DEST */
        public const int ALPHA_SUBTRACT = 3;
        /** Defines that alpha blending to SRC * DEST */
        public const int ALPHA_MULTIPLY = 4;
        /** Defines that alpha blending to SRC ALPHA * SRC + (1 - SRC) * DEST */
        public const int ALPHA_MAXIMIZED = 5;
        /** Defines that alpha blending to SRC + DEST */
        public const int ALPHA_ONEONE = 6;
        /** Defines that alpha blending to SRC + (1 - SRC ALPHA) * DEST */
        public const int ALPHA_PREMULTIPLIED = 7;
        /**
         * Defines that alpha blending to SRC + (1 - SRC ALPHA) * DEST
         * Alpha will be set to (1 - SRC ALPHA) * DEST ALPHA
         */
        public const int ALPHA_PREMULTIPLIED_PORTERDUFF = 8;
        /** Defines that alpha blending to CST * SRC + (1 - CST) * DEST */
        public const int ALPHA_INTERPOLATE = 9;
        /**
         * Defines that alpha blending to SRC + (1 - SRC) * DEST
         * Alpha will be set to SRC ALPHA + (1 - SRC ALPHA) * DEST ALPHA
         */
        public const int ALPHA_SCREENMODE = 10;

        // PI_BEIGN - 拓展混合模式
        public const int PI_ALPHA_PREMULTIPLIED = 11;
        // PI_END

        /**
         * Source color is added to the destination color without alpha affecting the result
         */
        public const int BLENDMODE_ONEONE = 0;
        /**
         * Blend current color and particle color using particle’s alpha
         */
        public const int BLENDMODE_STANDARD = 1;
        /**
         * Add current color and particle color multiplied by particle’s alpha
         */
        public const int BLENDMODE_ADD = 2;
        /**
         * Multiply current color with particle color
         */
        public const int BLENDMODE_MULTIPLY = 3;

        /**
         * Multiply current color with particle color then add current color and particle color multiplied by particle’s alpha
         */
        public const int BLENDMODE_MULTIPLYADD = 4;

        public static int format(int srcBlend, int dstBlend)
        {
            int resMode = ALPHA_COMBINE;
            switch (srcBlend)
            {
                case ((int)UnityEngine.Rendering.BlendMode.One): 
                    {
                        resMode = formatSrc_One(dstBlend);
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.SrcAlpha):
                    {
                        resMode = formatSrc_SrcAlpha(dstBlend);
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.SrcColor):
                    {
                        resMode = formatSrc_SrcColor(dstBlend);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return resMode;
        }
        public static int format_particle(int srcBlend, int dstBlend)
        {
            int resMode = format(srcBlend, dstBlend);
            switch (resMode)
            {
                case (ALPHA_ONEONE):
                    {
                        resMode = BLENDMODE_ONEONE;
                        break;
                    }
                case (ALPHA_MULTIPLY):
                    {
                        resMode = BLENDMODE_MULTIPLY;
                        break;
                    }
                case (ALPHA_ADD):
                    {
                        resMode = BLENDMODE_ADD;
                        break;
                    }
                default:
                    {
                        resMode = BLENDMODE_STANDARD;
                        break;
                    }
            }

            return resMode;
        }
        private static int formatSrc_One(int dstBlend)
        {
            int resMode = ALPHA_COMBINE;
            switch (dstBlend)
            {
                case ((int)UnityEngine.Rendering.BlendMode.Zero):
                    {
                        //resMode = ALPHA_ADD;
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.One):
                    {
                        resMode = ALPHA_ONEONE;
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha):
                    {
                        resMode = ALPHA_PREMULTIPLIED;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return resMode;
        }
        private static int formatSrc_SrcAlpha(int dstBlend)
        {
            int resMode = ALPHA_COMBINE;
            switch (dstBlend)
            {
                case ((int)UnityEngine.Rendering.BlendMode.Zero):
                    {
                        //resMode = ALPHA_ADD;
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.One):
                    {
                        resMode = ALPHA_ADD;
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha):
                    {
                        resMode = ALPHA_COMBINE;
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor):
                    {
                        resMode = ALPHA_MAXIMIZED;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return resMode;
        }
        private static int formatSrc_SrcColor(int dstBlend)
        {
            int resMode = ALPHA_COMBINE;
            switch (dstBlend)
            {
                case ((int)UnityEngine.Rendering.BlendMode.Zero):
                    {
                        //resMode = ALPHA_ADD;
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.One):
                    {
                        //resMode = ALPHA_ONEONE;
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha):
                    {
                        resMode = ALPHA_PREMULTIPLIED_PORTERDUFF;
                        break;
                    }
                case ((int)UnityEngine.Rendering.BlendMode.OneMinusSrcColor):
                    {
                        resMode = ALPHA_SCREENMODE;
                        break;
                    }
                default:
                    {
                        break;
                    }
            }

            return resMode;
        }
    }
}
