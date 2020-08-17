using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEditor;
using Assets.ExporterGLTF20;

namespace Assets.ExporterGLTF20
{

    public class Value : GlTF_Writer
    {
    }

    public class BabylonFresnelParameters
    {
        public float[] leftColor;
        public float[] rightColor;
        public bool isEnabled;
        public float bias;
        public float power;
    }

    class BabylonMaterial
    {
        public string name;
        public string id;
        public bool backFaceCulling;
        public bool wireframe;
        public float alpha;
    }

    class BabylonPBRMaterial : BabylonMaterial
    {
        public BabylonFresnelParameters emissiveFresnelParameters;
        public BabylonFresnelParameters opacityFresnelParameters;

        public Value opacityTexture;
        public Value reflectionTexture;
        public Value emissiveTexture;
        public Value reflectivityTexture;
        public Value bumpTexture;
        public Value lightmapTexture;
        public Value refractionTexture;
        public Value ambientTexture;
        public Value albedoTexture;

        public bool useLightmapAsShadowmap;
        public float[] ambient;
        public float[] albedo;
        public float[] reflection;
        public float[] emissive;

        public bool useAlphaFromAlbedoTexture;
        public bool useEmissiveAsIllumination;
        public bool useMicroSurfaceFromReflectivityMapAlpha;
        public bool useSpecularOverAlpha;
        public bool useRadianceOverAlpha;
        public float indexOfRefraction;
        public bool invertRefractionY;
        public float[] reflectivity;

        public float overloadedMicroSurfaceIntensity;
        public string customType;
        public float directIntensity;
        public float emissiveIntensity;
        public float environmentIntensity;
        public float specularIntensity;
        public float cameraExposure;
        public float cameraContrast;
        public float microSurface;
        public float overloadedShadowIntensity;
        public float overloadedReflectionIntensity;
        public float overloadedShadeIntensity;
        public float overloadedAlbedoIntensity;
        public float overloadedReflectivityIntensity;
        public float overloadedEmissiveIntensity;
        public float[] overloadedAmbient;
        public float[] overloadedAlbedo;
        public float[] overloadedReflectivity;
        public float[] overloadedEmissive;
        public float[] overloadedReflection;
        public float overloadedMicroSurface;
        public float overloadedAmbientIntensity;
    }

    class GLTF_PBRMaterial
    {
        public void format(Material material, Renderer render)
        {

        }

        //private BabylonMaterial DumpPBRMaterial(Material material, Renderer renderer, bool metallic)
        //{
        //    //if (materialsDictionary.ContainsKey(material.name))
        //    //{
        //    //    return materialsDictionary[material.name];
        //    //}

        //    var babylonPbrMaterial = new BabylonPBRMaterial
        //    {
        //        name = material.name,
        //        id = Guid.NewGuid().ToString(),
        //        albedo = new float[4],
        //        useEmissiveAsIllumination = true,
        //        useSpecularOverAlpha = true,
        //        useRadianceOverAlpha = true,
        //    };

        //    babylonPbrMaterial.environmentIntensity = RenderSettings.ambientIntensity;

        //    // Albedo
        //    if (material.HasProperty("_Color"))
        //    {
        //        babylonPbrMaterial.albedo = material.color.ToFloat();
        //    }
        //    //babylonPbrMaterial.albedoTexture = DumpTextureFromMaterial(material, "_MainTex");

        //    // Transparency
        //    DumpTransparency(material, babylonPbrMaterial);

        //    // Glossiess/Reflectivity
        //    DumpGlossinessReflectivity(material, metallic, babylonPbrMaterial);

        //    // Occlusion
        //    //babylonPbrMaterial.ambientTexture = DumpTextureFromMaterial(material, "_OcclusionMap");
        //    //if (babylonPbrMaterial.ambientTexture != null && material.HasProperty("_OcclusionStrength"))
        //    //{
        //    //    babylonPbrMaterial.ambientTexture.level = material.GetFloat("_OcclusionStrength");
        //    //}

        //    // Emissive
        //    if (material.HasProperty("_EmissionColor"))
        //    {
        //        babylonPbrMaterial.emissive = material.GetColor("_EmissionColor").ToFloat();
        //    }
        //    //babylonPbrMaterial.emissiveTexture = DumpTextureFromMaterial(material, "_EmissionMap");

        //    // Normal
        //    //babylonPbrMaterial.bumpTexture = DumpTextureFromMaterial(material, "_BumpMap");
        //    //if (babylonPbrMaterial.bumpTexture != null && material.HasProperty("_BumpMapScale"))
        //    //{
        //    //    babylonPbrMaterial.bumpTexture.level = material.GetFloat("_BumpMapScale");
        //    //}

        //    // Reflection
        //    //babylonPbrMaterial.reflectionTexture = DumpReflectionTexture();

        //    //materialsDictionary.Add(babylonPbrMaterial.name, babylonPbrMaterial);

        //    return babylonPbrMaterial;
        //}

        //private void DumpGlossinessReflectivity(Material material, bool metallic, BabylonPBRMaterial babylonPbrMaterial)
        //{
        //    if (material.HasProperty("_Glossiness"))
        //    {
        //        babylonPbrMaterial.microSurface = material.GetFloat("_Glossiness");
        //    }

        //    if (metallic)
        //    {
        //        if (material.HasProperty("_Metallic"))
        //        {
        //            var metalness = material.GetFloat("_Metallic");
        //            babylonPbrMaterial.reflectivity = new float[] { metalness * babylonPbrMaterial.albedo[0],
        //                metalness * babylonPbrMaterial.albedo[1],
        //                metalness * babylonPbrMaterial.albedo[2] };

        //            if (babylonPbrMaterial.albedoTexture != null)
        //            {
        //                var albedoTexture = material.GetTexture("_MainTex") as Texture2D;
        //                if (albedoTexture != null)
        //                {
        //                    var albedoPixels = GetPixels(albedoTexture);
        //                    var reflectivityTexture = new Texture2D(albedoTexture.width, albedoTexture.height, TextureFormat.RGBA32, false);
        //                    reflectivityTexture.alphaIsTransparency = true;
        //                    babylonPbrMaterial.useMicroSurfaceFromReflectivityMapAlpha = true;

        //                    var metallicTexture = material.GetTexture("_MetallicGlossMap") as Texture2D;
        //                    if (metallicTexture == null)
        //                    {
        //                        for (var i = 0; i < albedoTexture.width; i++)
        //                        {
        //                            for (var j = 0; j < albedoTexture.height; j++)
        //                            {
        //                                albedoPixels[j * albedoTexture.width + i].r *= metalness;
        //                                albedoPixels[j * albedoTexture.width + i].g *= metalness;
        //                                albedoPixels[j * albedoTexture.width + i].b *= metalness;
        //                                albedoPixels[j * albedoTexture.width + i].a = babylonPbrMaterial.microSurface;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        var metallicPixels = GetPixels(metallicTexture);
        //                        for (var i = 0; i < albedoTexture.width; i++)
        //                        {
        //                            for (var j = 0; j < albedoTexture.height; j++)
        //                            {
        //                                var metallicPixel = metallicPixels[j * albedoTexture.width + i];
        //                                albedoPixels[j * albedoTexture.width + i].r *= metallicPixel.r;
        //                                albedoPixels[j * albedoTexture.width + i].g *= metallicPixel.r;
        //                                albedoPixels[j * albedoTexture.width + i].b *= metallicPixel.r;
        //                                albedoPixels[j * albedoTexture.width + i].a = metallicPixel.a;
        //                            }
        //                        }
        //                    }

        //                    reflectivityTexture.SetPixels(albedoPixels);
        //                    reflectivityTexture.Apply();

        //                    var textureName = albedoTexture.name + "_MetallicGlossMap.png";
        //                    var babylonTexture = new BabylonTexture { name = textureName };
        //                    var textureScale = material.GetTextureScale("_MainTex");
        //                    babylonTexture.uScale = textureScale.x;
        //                    babylonTexture.vScale = textureScale.y;

        //                    var textureOffset = material.GetTextureOffset("_MainTex");
        //                    babylonTexture.uOffset = textureOffset.x;
        //                    babylonTexture.vOffset = textureOffset.y;

        //                    var reflectivityTexturePath = Path.Combine(Path.GetTempPath(), textureName);
        //                    File.WriteAllBytes(reflectivityTexturePath, reflectivityTexture.EncodeToPNG());
        //                    babylonScene.AddTexture(reflectivityTexturePath);
        //                    if (File.Exists(reflectivityTexturePath))
        //                    {
        //                        File.Delete(reflectivityTexturePath);
        //                    }

        //                    babylonPbrMaterial.reflectivityTexture = babylonTexture;
        //                }
        //            }
        //            //else
        //            //{
        //            //      TODO. Manage Albedo Cube Texture.
        //            //}
        //        }
        //    }
        //    else
        //    {

        //        if (material.HasProperty("_SpecColor"))
        //        {
        //            babylonPbrMaterial.reflectivity = material.GetColor("_SpecColor").ToFloat();
        //        }
        //        babylonPbrMaterial.reflectivityTexture = DumpTextureFromMaterial(material, "_SpecGlossMap");
        //        if (babylonPbrMaterial.reflectivityTexture != null && babylonPbrMaterial.reflectivityTexture.hasAlpha)
        //        {
        //            babylonPbrMaterial.useMicroSurfaceFromReflectivityMapAlpha = true;
        //        }
        //    }
        //}
        //private static void DumpTransparency(Material material, BabylonPBRMaterial babylonPbrMaterial)
        //{
        //    if (material.HasProperty("_Mode"))
        //    {
        //        var mode = material.GetFloat("_Mode");
        //        if (mode >= 2.0f)
        //        {
        //            // Transparent Albedo
        //            if (babylonPbrMaterial.albedoTexture != null && babylonPbrMaterial.albedoTexture.hasAlpha)
        //            {
        //                babylonPbrMaterial.useAlphaFromAlbedoTexture = true;
        //            }
        //            // Material Alpha
        //            else
        //            {
        //                babylonPbrMaterial.alpha = babylonPbrMaterial.albedo[3];
        //            }
        //        }
        //        else if (mode == 1.0f)
        //        {
        //            // Cutout
        //            // Follow the texture hasAlpha property.
        //        }
        //        else
        //        {
        //            // Opaque
        //            if (babylonPbrMaterial.albedoTexture != null)
        //            {
        //                babylonPbrMaterial.albedoTexture.hasAlpha = false;
        //            }
        //            babylonPbrMaterial.alpha = 1.0f;
        //        }
        //    }
        //}
    }
}
