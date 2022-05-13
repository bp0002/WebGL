# PBRLight

* 灯光逻辑流程
  * 灯光 A
    * 预计算 灯光信息
    * 计算灯光 辐照度
    * 计算表面粗糙度 受灯光辐照度的影响结果
    * 计算灯光对 漫反射的贡献 - diffuse
    * 计算灯光对 镜面反射的贡献 - specular
  * 灯光 B\C\D...
* 不同灯光类型处理逻辑不同
* 处理灯光信息数据结构
```glsl
struct preLightingInfo {
    // 起点: 像素世界坐标, 终点:灯光世界坐标
    vec3 lightOffset;
    // 灯光到像素点的平方距离 - dot()
    float lightDistanceSquared;
    // 灯光到像素点的距离
    float lightDistance;
    // 灯光在像素点的 衰减
    float attenuation;
    // 入射方向向量
    vec3 L;
    // 观察方向与入射方向夹角的角平分线方向
    vec3 H;
    // 观察方向与像素点法线方向夹角的角平分方向
    float NdotV;
    // 像素点法线方向与入射方向的点积
    float NdotLUnclamped;
    // 像素点法线方向与入射方向的点积 - 数值截断在 [0.0000001, 1.0] 范围
    float NdotL;
    // 观察方向 与 观察方向与入射方向夹角的角平分线方向 的点积 - 数值截断在 [0.0, 1.0] 范围
    float VdotH;
    // 像素点的表面粗糙度
    float roughness;
};
```

## 预处理全局

### 在光照之前计算表面信息

#### 方法准备

* Code
    ```glsl
    const vec3 LuminanceEncodeApprox = vec3(0.2126, 0.7152, 0.0722);

    #define MINIMUMVARIANCE 0.0005
    float convertRoughnessToAverageSlop(float roughness) {
        return square(roughness) + MINIMUMVARIANCE;
    }

    vec2 getAARoughnessFactors(vec3 normalVector) {
        return vec2(0., 0.);
    }

    vec3 getBRDFLookup(float NdotV, float perceptualRoughness) {
        vec2 UV         = vec2(NdotV, perceptualRoughness);
        vec4 brdfLookup = texture2D(environmentBrdfSampler, UV);

        return brdfLookup.rgb;
    }

    float getLuminance(vec3 color) {
        return clamp(dot(color, LuminanceEncodeApprox), 0., 1.);
    }
    ```

#### 主逻辑

* Code
    ```glsl
    vec4 metallicReflectanceFactors = vMetallicReflectanceFactors;
    // 计算 Albedo
    // AlbedoOpacityOutParams albedoOpacityOut;
    // albedoOpacityBlock(...)

    // 计算 AO
    // AmbientOcclusionOutParams aoOut;
    // ambientOcclusionBlock(...)

    // 计算 Reflectivity
    // ReflectivityOutParams reflectivityOut
    // reflectivityBlock(...)

    // 表面数据
    // float microSurface   = reflectivityOut.microSurface;
    // float roughness      = reflectivityOut.roughness;
    // vec3 surfaceAlbedo   = reflectivityOut.surfaceAlbedo;

    float NdotVUnclamped    = dot(normalW, viewDirectionW);
    float NdotV             = absEps(NdotVUnclamped);
    vec2 AARoughnessFactors = getAARoughnessFactors(normalW.xyz);

    // 
    float alphaG            = convertRoughnessToAverageSlope(roughness);
    vec3 environmentBrdf    = getBRDFLookup(NdotV, roughness);

    float ambientMonochrome = getLuminance(aoOut.ambientOcclusionColor);
    float seo               = environmentRadianceOcclusion(ambientchrome, NdotVUnclamped);

    float reflectance       = max(
        max(
            reflectivityOut.surfaceReflectivityColor.r,
            reflectivityOut.surfaceReflectivityColor.g,
        ),
        reflectivityOut.surfaceReflectivityColor.b,
    );

    vec3 specularEnvironmentR0  = reflectivityOut.surfaceReflectivityColor.rgb;
    vec3 specularEnvironmentR90 = vec3(metallicReflectanceFactors.a);

    ```

* Code
    ```glsl
    
    ```

#### Albedo

* 纹理内容为 gamma 空间数据,需转换到线性空间
* Code
    ```glsl
    struct AlbedoOpacityOutParams {
        vec3 surfaceAlbedo;
        float alpha;
    };
    void albedoOpacityBlock(
        vec3 albedoColor,
        float albedoColorAlpha,
        vec3 albedoTexture,
        float albedoTextureAlpha,
        float albedoTextureLevel,
        out AlbedoOpacityOutParams outParams
    ) {
        vec3 surfaceAlbedo  = albedoColor;
        float alpha         = albedoColorAlpha * albedoTextureAlpha;
        surfaceAlbedo       *= toLinearSpace(albedoTexture);
        surfaceAlbedo       *= alpha;

        outParams.surfaceAlbedo = surfaceAlbedo;
        outParams.alpha         = alpha;
    }
    ```

#### Reflectivity

* 处理微表面
* Code
    ```glsl
    struct ReflectivityOutParams {
        float microSurface;
        float roughness;
        vec3 surfaceReflectivityColor;
        vec3 surfaceAlbedo;
    };

    void reflectivityBlock(
        vec3 reflectivityColor,
        float reflectivityAlpha,
        vec3 surfaceAlbedoColor,
        float3 metallicReflectanceFactorsF0,
        out ReflectivityOutParams outParams
    ) {
        float microSurface = reflectivityAlpha;
        vec3 surfaceReflectivityColor = reflectivityColor;
        vec2 metallicRoughness = surfaceReflectivityColor.rg;

        microSurface        = 1.0 - metallicRoughness.g;
        microSurface        = saturate(microSurface);
        float metallic      = metallicRoughness.r;
        float roughness     = 1.0 - microSurface;

        vec3 baseColor      = surfaceAlbedoColor;
        vec3 metallicF0     = metallicReflectanceFactorsF0;

        surfaceReflectivityColor    = mix(metallicF0, baseColor, metallicRoughness.r);
        vec3 surfaceAlbedo          = mix(baseColor * (1.0 - metallicF0), vec3(0., 0., 0.), metallic));

        outParams.microSurface              = microSurface;
        outParams.roughness                 = roughness;
        outParams.surfaceReflectivityColor  = surfaceReflectivityColor;
        outParams.surfaceAlbedo             = surfaceAlbedo;
    }
    ```

#### AmbientOcclusion

* 环境遮蔽
* Code
    ```glsl
    struct AmbientOcclusionOutParams {
        vec3 ambientOcclusionColor;
    };

    void ambientOcclusionBlock(out AmbientOcclusionOutParams outParams) {
        vec3 ambientOcclusionColor = vec3(1., 1., 1.);

        outParams.ambientOcclusionColor = ambientOcclusionColor;
    }
    ```

#### Clear coat

* 透明涂层 - 微表面折射
* Code
    ```glsl
    struct ClearcoatOutParams {
        vec3 specularEnvironmentR0;
        float conservationFactor;
        vec3 clearCoatNormalW;
        vec2 clearCoatAARoughnessFactors;
        float clearCoatIntensity;
        float clearCoatRoughness;
        vec3 energyConservationFactorClearCoat;
    };

    void sub
    ```

#### EnvironmentReflectance

* 环境反射
* Code
    ```glsl

    ```


## 预计算 灯光信息

* 计算光照计算所需的几何数据

### DirectionalLight

* Input
  * vLightData
  * viewDirectionaW
  * normalW
* Output
  * preLightingInfo
* Code
    ```glsl
    preLightingInfo computeDirectionalPreLightingInfo(vec4 lightData, vec3 V, vec3 N) {
        preLightingInfo result;

        result.lightDistance    = length(-lightData.xyz);
        result.L                = normalize(-lightData.xyz);
        result.H                = normalize(V+result.L);
        result.VdotH            = saturate(dot(V, result.H));
        result.NdotLUnclamped   = dot(N, result.L);
        result.NdotL            = saturateEps(result.NdotLUnclamped);

        return result;
    }
    ```

### PointLight

* Input
  * vLightData
  * viewDirectionaW
  * normalW
* Output
  * preLightingInfo
* Code
    ```glsl
    preLightingInfo computePointAndSpotPreLightingInfo(vec4 lightData, vec3 V, vec3 N) {
        preLightingInfo result;

        result.lightOffset          = lightData.xyz-vPositionW;
        result.lightDistanceSquared = dot(result.lightOffset, result.lightOffset);
        result.lightDistance        = sqrt(result.lightDistanceSquared);
        result.L                    = normalize(result.lightOffset);
        result.H                    = normalize(V+result.L);
        result.VdotH                = saturate(dot(V, result.H));
        result.NdotLUnclamped       = dot(N, result.L);
        result.NdotL                = saturateEps(result.NdotLUnclamped);

        return result;
    }
    ```
  
### SpotlLight - 与 PointLight 相同

* Input
  * vLightData
  * viewDirectionaW
  * normalW
* Output
  * preLightingInfo
* Code
  * 略

### HemisphereLight

* Input
  * vLightData
  * viewDirectionaW
  * normalW
* Output
  * preLightingInfo
* Code
  ```glsl
    preLightingInfo computeHemisphericPreLightingInfo(vec4 lightData, vec3 V, vec3 N) {
        preLightingInfo result;

        result.L                = normalize(lightData.xyz);
        result.H                = normalize(V+result.L);
        result.VdotH            = saturate(dot(V, result.H));
        result.NdotL            = dot(N, lightData.xyz)*0.5+0.5;
        result.NdotL            = saturateEps(result.NdotL);
        result.NdotLUnclamped   = result.NdotL;

        return result;
    }
  ```

## 光照辐照度计算

* 辐照度随距离变化

### DirectionalLight

* 方向光无衰减
* Code
    ```glsl
    preInfo.attenuation = 1.0;
    ```

### PointLight , SpotLight , HemisphereLight

* 随平方距离的变化
* Code
    ```glsl
    float computeDistanceLightFalloff_GLTF(float lightDistanceSquared, float inverseSquaredRange) {
        float lightDistanceFalloff  = 1.0/maxEps(lightDistanceSquared);

        float factor                = lightDistanceSquared*inverseSquaredRange;
        float attenuation           = saturate(1.0-factor*factor);
        attenuation                 *= attenuation;
        lightDistanceFalloff        *= attenuation;

        return lightDistanceFalloff;
    }
    ```

## 计算表面粗糙度 受灯光辐照度的影响结果

### DirectionalLight、PointLight、SpotLight

* 表面粗糙度不受光照影响

## 计算灯光对 漫反射的贡献 - diffuse

### DirectionalLight、PointLight、SpotLight

* BRDF 应用
* Code
    ```glsl
    float diffuseBRDF_Burley(float NdotL, float NdotV, float VdotH, float roughness) {
        float diffuseFresnelNV = pow5(saturateEps(1.0-NdotL));
        float diffuseFresnelNL = pow5(saturateEps(1.0-NdotV));
        float diffuseFresnel90 = 0.5+2.0*VdotH*VdotH*roughness;
        float fresnel = (1.0+(diffuseFresnel90-1.0)*diffuseFresnelNL) *
        (1.0+(diffuseFresnel90-1.0)*diffuseFresnelNV);
        return fresnel/PI;
    }
    vec3 computeDiffuseLighting(preLightingInfo info, vec3 lightColor) {
        float diffuseTerm = diffuseBRDF_Burley(info.NdotL, info.NdotV, info.VdotH, info.roughness);
        return diffuseTerm * info.attenuation * info.NdotL * lightColor;
    }
    ```

## 计算灯光对镜面反射 的贡献 - Specular

### DirectionalLight、PointLight、SpotLight

* normal & Reflectance0 & Reflectance90 & roughness & specularColor
* Code
    ```glsl
    vec3 computeSpecularLighting(
        preLightingInfo info,
        vec3 N,
        vec3 reflectance0,
        vec3 reflectance90,
        float geometricRoughnessFactor,
        vec3 lightColor
    ) {
        float NdotH     = saturateEps(dot(N, info.H));
        float roughness = max(info.roughness, geometricRoughnessFactor);
        float alphaG    = convertRoughnessToAverageSlope(roughness);

        vec3 fresnel            = fresnelSchlickGGX(info.VdotH, reflectance0, reflectance90);
        float distribution      = normalDistributionFunction_TrowbridgeReitzGGX(NdotH, alphaG);
        float smithVisibility   = smithVisibility_GGXCorrelated(info.NdotL, info.NdotV, alphaG);
        vec3 specTerm           = fresnel * distribution * smithVisibility;
    
        return specTerm * info.attenuation * info.NdotL * lightColor;
    }
    ```
