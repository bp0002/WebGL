using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ModelTextureDetail
{
    class Base
    {
        static bool getPixelsFromTexture(ref Texture2D texture, out Color[] pixels)
        {
            string path = AssetDatabase.GetAssetPath(texture);
            //Make texture readable
            TextureImporter im = AssetImporter.GetAtPath(path) as TextureImporter;
            if (!im)
            {
                pixels = new Color[1];
                return false;
            }
            bool readable = im.isReadable;
#if UNITY_5_4
		TextureImporterFormat format = im.textureFormat;
#else
            TextureImporterCompression format = im.textureCompression;
#endif
            TextureImporterType type = im.textureType;
            bool isConvertedBump = im.convertToNormalmap;

            if (!readable)
                im.isReadable = true;
#if UNITY_5_4
		if (type != TextureImporterType.Image)
			im.textureType = TextureImporterType.Image;
		im.textureFormat = TextureImporterFormat.ARGB32;
#else
            if (type != TextureImporterType.Default)
                im.textureType = TextureImporterType.Default;

            im.textureCompression = TextureImporterCompression.Uncompressed;
#endif
            im.SaveAndReimport();

            pixels = texture.GetPixels();

            if (!readable)
                im.isReadable = false;
#if UNITY_5_4
		if (type != TextureImporterType.Image)
			im.textureType = type;
#else
            if (type != TextureImporterType.Default)
                im.textureType = type;
#endif
            if (isConvertedBump)
                im.convertToNormalmap = true;

#if UNITY_5_4
		im.textureFormat = format;
#else
            im.textureCompression = format;
#endif

            im.SaveAndReimport();

            return true;
        }
    }
}
