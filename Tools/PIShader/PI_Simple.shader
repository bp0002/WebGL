
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Pi/Simple" {
Properties {
	_TintColor ("Tint Color", Color) = (1,1,1,1)
	_MainTex ("Texture", 2D) = "white" {}


	_Hue ("Hue", Range(-0.5, 0.5)) = 0.0
	_Saturation ("Saturation", Range(-1.0, 1.0)) = 0.0
	_Brightness ("Brightness", Range(-1.0, 1.0)) = 0.0

	_UseAlphaFromDiffuseTexture("UseAlphaFromDiffuseTexture", Int) = 0
	_DiffuseTextureHasAlpha("DiffuseTextureHasAlpha", Int)	= 0

	// 自定义选择的光照图
	[HideInInspector] _CustomLightMapTex("Texture1", 2D) = "white" {}
	// 场景烘培的光照图
	_BakeLightMapTex("Texture2", 2D) = "white" {}
	// 光照图纹理强度
	_LightMapStrength("LightMapStrength", Range(-10.0, 10.0)) = 1.0
	// 场景烘培的光照图信息
	_UseBakeLightMap("UseBakeLightMap", Int)				= 0
	[HideInInspector] _BakeLightMapTexST0("BakeLightMapTexST0", Float) = 0.0
	[HideInInspector] _BakeLightMapTexST1("BakeLightMapTexST1", Float) = 0.0
	[HideInInspector] _BakeLightMapTexST2("BakeLightMapTexST2", Float) = 0.0
	[HideInInspector] _BakeLightMapTexST3("BakeLightMapTexST3", Float) = 0.0
	// 自定义的光照图信息
	_UseCustomLightMap("UseCustomLightMap", Int)			= 0
	// 光照图应用模式
	_UseLightmapAsShadowmap("UseLightmapAsShadowmap", Int)	= 0

	[HideInInspector] _IgnoreAreaLight("IgnoreAreaLight", Int)				= 1
	[HideInInspector] _IgnoreDirectionalLight("IgnoreDirectionalLight", Int)	= 1
	[HideInInspector] _IgnorePointLight("IgnorePointLight", Int)				= 1
	[HideInInspector] _IgnoreSpotLight("IgnoreSpotLight", Int)				= 1

	[Enum(UnityEngine.Rendering.CullMode)]_Cull("_Cull", Int) = 2
	[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend("SrcBlend", Int) = 5
	[Enum(UnityEngine.Rendering.BlendMode)] _DstBlend("DstBlend", Int) = 1	

    _Mode ("Mode", Float) = 0.0
}


    Category {
        Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
        Blend [_SrcBlend][_DstBlend]
        ColorMask RGB
        Cull [_Cull]
        
        SubShader {
            Pass {

                Tags{ "LightMode"="ForwardBase" }
                
                CGPROGRAM
                #pragma vertex vert alpha
                #pragma fragment frag alpha
                #pragma target 3.0
                #pragma multi_compile_fog

                #include "UnityCG.cginc"
                #include "Lighting.cginc"
                #include "ConvertColor.cginc"

                sampler2D _MainTex;
				sampler2D _CustomLightMapTex;
				sampler2D _BakeLightMapTex;

				float _BakeLightMapTexST0;
				float _BakeLightMapTexST1;
				float _BakeLightMapTexST2;
				float _BakeLightMapTexST3;

				float _LightMapStrength;

                fixed4 _TintColor;

                float _Hue;
                float _Saturation;
                float _Brightness;

				int _UseAlphaFromDiffuseTexture;
				int _DiffuseTextureHasAlpha;
				int _UseLightmapAsShadowmap;

				int _IgnoreAreaLight;
				int _IgnoreDirectionalLight;
				int _IgnorePointLight;
				int _IgnoreSpotLight;

				int _UseCustomLightMap;
				int _UseBakeLightMap;

                float4 _MainTex_ST; // for TRANSFORM_TEX
				float4 _CustomLightMapTex_ST; // for TRANSFORM_TEX
				float4 _BakeLightMapTex_ST; // for TRANSFORM_TEX
                float3 ShadeDirectionalLight(float4 vertex, float3 N);
				float4 diffuseFormat(float4 texColor, float2 texcoord);
				float4 lightmapFormat(float4 texColor, float2 lightmap_st_custom, float2 lightmap_st_bake);
                
                struct VertInput {
                    float4 vertex : POSITION;
					float4 vColor : COLOR;
                    float3 normal: NORMAL;
                    float2 texcoord : TEXCOORD0;
					float2 lightmap_st_custom : TEXCOORD1;
					float2 lightmap_st_bake : TEXCOORD2;
                };

                struct VertOutput {
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord : TEXCOORD0;
					float2 lightmap_st_custom : TEXCOORD1;
					float2 lightmap_st_bake : TEXCOORD2;
					UNITY_FOG_COORDS(3)
                };
                
                

                VertOutput vert (VertInput v)
                {
                    VertOutput o;
                    UNITY_SETUP_INSTANCE_ID(v);

                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);

					o.lightmap_st_custom	= TRANSFORM_TEX(v.lightmap_st_custom, _CustomLightMapTex);
					o.lightmap_st_bake		= TRANSFORM_TEX(v.lightmap_st_bake, _BakeLightMapTex);

                    // float3 color = UNITY_LIGHTMODEL_AMBIENT;
                    
                    // float3 N = UnityObjectToWorldNormal(v.normal);
                    // color += ShadeDirectionalLight(v.vertex, N);

                    //float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    
                    //color += Shade4PointLights(
                    //    unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                    //    unity_LightColor[0].rgb, unity_LightColor[1].rgb, 
                    //    unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                    //    unity_4LightAtten0, wpos, N);

					float3 color;

					color = float3(1.0, 1.0, 1.0);

                    o.color = float4(color, 1.0) * v.vColor;

					UNITY_TRANSFER_FOG(o, o.vertex);

                    return o;
                }

                fixed4 frag (VertOutput i) : SV_Target
                {
					float4 texColor = float4(0.0, 0.0, 0.0, 0.0);

					// 主纹理处理
					texColor = diffuseFormat(texColor, i.texcoord);

                    float3 hsl = rgb2hsl(texColor.rgb);
                    hsl += float3(_Hue, _Saturation, _Brightness); 
					texColor.rgb = hsl2rgb(hsl);

					// 光照图处理
					texColor = lightmapFormat(texColor, i.lightmap_st_custom, i.lightmap_st_bake);

					//texColor.rgb = texColor.rgb * texColor.a;

                    float4 col = _TintColor * texColor;
                    col = col * i.color;

                    //UNITY_APPLY_FOG_COLOR(i.fogCoord, col, fixed4(0,0,0,1));
					UNITY_APPLY_FOG(i.fogCoord, col);

                    return col;
                }

				float4 diffuseFormat(float4 texColor, float2 texcoord) {

					texColor = tex2D(_MainTex, texcoord);

					if (_DiffuseTextureHasAlpha == 1 && _UseAlphaFromDiffuseTexture == 1) {
						texColor = texColor;
					}
					else if (_UseAlphaFromDiffuseTexture == 1) {
						texColor = float4(texColor.rgb, texColor.a > 0.4 ? 1.0 : 0.0);
					}
					else {
						texColor = float4(texColor.rgb, 1.0);
					}

					return texColor;
				}

				float4 lightmapFormat(float4 texColor, float2 lightmap_st_custom, float2 lightmap_st_bake) {

					float4 lightmap = float4(1.0, 1.0, 1.0, 1.0);

					// lightmap
					if (_UseCustomLightMap == 1)
					{
						lightmap = tex2D(_CustomLightMapTex, lightmap_st_custom);

						// 非线性校正
						float af = lightmap.a;
						float rf = lightmap.r;
						float gf = lightmap.g;
						float bf = lightmap.b;

						float ur = pow(rf * af, 1.1) * 8.0;
						float ug = pow(gf * af, 1.1) * 8.0;
						float ub = pow(bf * af, 1.1) * 8.0;

						// 渲染时只处理 0 - _LightMapStrength 倍的亮度范围
						ur = clamp(ur / _LightMapStrength, 0, 1.0);
						ug = clamp(ug / _LightMapStrength, 0, 1.0);
						ub = clamp(ub / _LightMapStrength, 0, 1.0);

						lightmap.r = ur;
						lightmap.g = ug;
						lightmap.b = ub;

						lightmap.rgb = lightmap.rgb * _LightMapStrength;

						if (_UseLightmapAsShadowmap == 1) {
							texColor.rgb *= lightmap.rgb;
						}
						else {
							texColor.rgb += lightmap.rgb;
						}
					}

					if (_UseBakeLightMap == 1)
					{
						lightmap = tex2D(_BakeLightMapTex, lightmap_st_bake);

						// 非线性校正
						float af = lightmap.a;
						float rf = lightmap.r;
						float gf = lightmap.g;
						float bf = lightmap.b;
						
						float ur = pow(rf * af, 1.1) * 8.0;
						float ug = pow(gf * af, 1.1) * 8.0;
						float ub = pow(bf * af, 1.1) * 8.0;

						// 渲染时只处理 0 - _LightMapStrength 倍的亮度范围
						ur = clamp(ur / _LightMapStrength, 0, 1.0);
						ug = clamp(ug / _LightMapStrength, 0, 1.0);
						ub = clamp(ub / _LightMapStrength, 0, 1.0);
						
						lightmap.r = ur;
						lightmap.g = ug;
						lightmap.b = ub;
						
						lightmap.rgb  = lightmap.rgb * _LightMapStrength;

						if (_UseLightmapAsShadowmap == 1) {
							texColor.rgb *= lightmap.rgb;
						}
						else {
							texColor.rgb += lightmap.rgb;
						}
						//texColor.rgb = lightmap.rgb;
					}

					return texColor;
				}

                float3 ShadeDirectionalLight(float4 vertex, float3 N) {
                    
                    float3 L = normalize(_WorldSpaceLightPos0).xyz;
                    
                    float ndotl = saturate(dot(N, L));
                    
                    float4 col = _LightColor0 * ndotl;
                    
                    float3 V = normalize(WorldSpaceViewDir(vertex));

                    float3 R = 2 * dot(N, L) * N - L;
                    float3 H = normalize(V + L);
                    
                    float _Shin = 1.0;  // 待定
                    float _Spec = 1.0;  // 待定

                    float specScale = pow(saturate(dot(R, V)), _Shin);
                    
                    col += _Spec * specScale;
                    
                    return col.rgb;
                }
                ENDCG 
            }
        }	
    }

    CustomEditor "PI_SimpleGUI"
}
