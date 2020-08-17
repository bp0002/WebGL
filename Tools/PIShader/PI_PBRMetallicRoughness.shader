﻿// Unity built-in shader source. Babylon Edition. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Pi/PBRMetallicRoughness"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo", 2D) = "white" {}

		[HideInInspector] _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.4

		_Glossiness("Roughness", Range(0.0, 1.0)) = 0.5
		[HideInInspector] _SpecGlossMap("Roughness Map", 2D) = "white" {}

		[Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 1.0
		[HideInInspector] _MetallicGlossMap("Metallic", 2D) = "white" {}

		[HideInInspector] [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
		[HideInInspector] [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

		[HideInInspector] _BumpScale("Scale", Float) = 1.0
		[HideInInspector] _BumpMap("Normal Map", 2D) = "bump" {}

		[HideInInspector] _Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
		[HideInInspector] _ParallaxMap("Height Map", 2D) = "black" {}

		[HideInInspector] _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		[HideInInspector] _OcclusionMap("Occlusion", 2D) = "white" {}

		[HideInInspector] _EmissionColor("Color", Color) = (0,0,0)
		[HideInInspector] _EmissionMap("Emission", 2D) = "white" {}

		[HideInInspector] _DetailMask("Detail Mask", 2D) = "white" {}

		[HideInInspector] _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
		[HideInInspector] _DetailNormalMapScale("Scale", Float) = 1.0
		[HideInInspector] _DetailNormalMap("Normal Map", 2D) = "bump" {}

		// Babylon Custom Material Properties
		[HideInInspector] [ToggleOff] _Wireframe("Show Wireframe", Int) = 0
		[HideInInspector] _CameraContrast("Camera Contrast", Range(0.0, 10.0)) = 1.0
		[HideInInspector] _CameraExposure("Camera Exposure", Range(0.0, 10.0)) = 1.0
		[HideInInspector] _DirectIntensity("Direct Intensity", Range(0.0, 10.0)) = 1.0
		[HideInInspector] _EmissiveIntensity("Emissive Intensity", Range(0.0, 10.0)) = 1.0
		[HideInInspector] _SpecularIntensity("Specular Intensity", Range(0.0, 10.0)) = 1.0
		[HideInInspector] _EnvironmentIntensity("Environment Intensity", Range(0.0, 10.0)) = 1.0
		[HideInInspector] _MicroSurfaceScaling("Micro Surface Scaling", Range(0.0, 1.0)) = 1.0
		[HideInInspector] _HighlightIntensity("Highlighting Multiplier", Range(1.0, 25.0)) = 10.0
		[HideInInspector] _ReflectionColor("Reflection Color", Color) = (1,1,1,1)
		[HideInInspector] [ToggleOff] _BackFaceCulling("Back Face Culling", Int) = 1
		[HideInInspector] [ToggleOff] _TwoSidedLighting("Two Sided Lighting", Int) = 0
		[HideInInspector] [ToggleOff] _UseSkyboxRefraction("Set Skybox Refraction", Int) = 0
		[HideInInspector] _IndexOfRefraction("Index Of Refraction", Float) = 0.66
		[HideInInspector] [ToggleOff] _LinkRefractionWithTransparency("Link Refraction Trans", Int) = 0
		[HideInInspector] [ToggleOff] _DisableLighting("Disable Surface Lighting", Int) = 0
		[HideInInspector] _MaxSimultaneousLights("Max Simultaneous Lights", Int) = 4
		[HideInInspector] [ToggleOff] _UseSpecularOverAlpha("Use Specular Over Alpha", Int) = 0
		[HideInInspector] [ToggleOff] _UseRadianceOverAlpha("Use Radiance Over Alpha", Int) = 0
		[HideInInspector] [ToggleOff] _UsePhysicalLightFalloff("Use Physical Light Falloff", Int) = 0
		[HideInInspector] [ToggleOff] _UseEmissiveAsIllumination("Use Emissive Illumination", Int) = 0

		[HideInInspector] [Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Int) = 0

		// Blending state
		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}


		SubShader
		{
			Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False" }
			LOD 300


			// ------------------------------------------------------------------
			//  Base forward pass (directional light, emission, lightmaps, ...)
			Pass
			{
				Name "FORWARD"
				Tags { "LightMode" = "ForwardBase" }

				Blend[_SrcBlend][_DstBlend]
				ZWrite[_ZWrite]

				CGPROGRAM
				#pragma target 3.5

			// -------------------------------------
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog
			#pragma multi_compile_instancing

			#pragma vertex vertBase
			#pragma fragment fragBase
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
			// ------------------------------------------------------------------
			//  Additive forward pass (one light per pass)
			Pass
			{
				Name "FORWARD_DELTA"
				Tags { "LightMode" = "ForwardAdd" }
				Blend[_SrcBlend] One
				Fog { Color(0,0,0,0) } // in additive pass fog should be black
				ZWrite Off
				ZTest LEqual

				CGPROGRAM
				#pragma target 3.5

			// -------------------------------------

			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog

			#pragma vertex vertAdd
			#pragma fragment fragAdd
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
			// ------------------------------------------------------------------
			//  Shadow rendering pass
			Pass {
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }

				ZWrite On ZTest LEqual

				CGPROGRAM
				#pragma target 3.5

			// -------------------------------------

			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _PARALLAXMAP
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing

			#pragma vertex vertShadowCaster
			#pragma fragment fragShadowCaster

			#include "UnityStandardShadow.cginc"

			ENDCG
		}
			// ------------------------------------------------------------------
			//  Deferred pass
			Pass
			{
				Name "DEFERRED"
				Tags { "LightMode" = "Deferred" }

				CGPROGRAM
				#pragma target 3.0
				#pragma exclude_renderers nomrt


			// -------------------------------------
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICGLOSSMAP
			#pragma shader_feature _SPECGLOSSMAP
			#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature ___ _DETAIL_MULX2
			#pragma shader_feature _PARALLAXMAP

			#pragma multi_compile_prepassfinal
			#pragma multi_compile_instancing

			#pragma vertex vertDeferred
			#pragma fragment fragDeferred

			#include "UnityStandardCore.cginc"

			ENDCG
		}

			// ------------------------------------------------------------------
			// Extracts information for lightmapping, GI (emission, albedo, ...)
			// This pass it not used during regular rendering.
			Pass
			{
				Name "META"
				Tags { "LightMode" = "Meta" }

				Cull Off

				CGPROGRAM
				#pragma vertex vert_meta
				#pragma fragment frag_meta

				#pragma shader_feature _EMISSION
				#pragma shader_feature _METALLICGLOSSMAP
				#pragma shader_feature _SPECGLOSSMAP
				#pragma shader_feature ___ _DETAIL_MULX2
				#pragma shader_feature EDITOR_VISUALIZATION

				#include "UnityStandardMeta.cginc"
				ENDCG
			}
		}

			SubShader
		{
			Tags { "RenderType" = "Opaque" "PerformanceChecks" = "False" }
			LOD 150

			// ------------------------------------------------------------------
			//  Base forward pass (directional light, emission, lightmaps, ...)
			Pass
			{
				Name "FORWARD"
				Tags { "LightMode" = "ForwardBase" }

				Blend[_SrcBlend][_DstBlend]
				ZWrite[_ZWrite]

				CGPROGRAM
				#pragma target 2.0
				#pragma shader_feature _NORMALMAP
				#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature _EMISSION
				#pragma shader_feature _METALLICGLOSSMAP
				#pragma shader_feature _SPECGLOSSMAP
				#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
				#pragma shader_feature _ _GLOSSYREFLECTIONS_OFF
			// SM2.0: NOT SUPPORTED shader_feature ___ _DETAIL_MULX2
			// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP

			#pragma skip_variants SHADOWS_SOFT DIRLIGHTMAP_COMBINED

			#pragma multi_compile_fwdbase
			#pragma multi_compile_fog

			#pragma vertex vertBase
			#pragma fragment fragBase
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
			// ------------------------------------------------------------------
			//  Additive forward pass (one light per pass)
			Pass
			{
				Name "FORWARD_DELTA"
				Tags { "LightMode" = "ForwardAdd" }
				Blend[_SrcBlend] One
				Fog { Color(0,0,0,0) } // in additive pass fog should be black
				ZWrite Off
				ZTest LEqual

				CGPROGRAM
				#pragma target 2.0
				#pragma shader_feature _NORMALMAP
				#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature _METALLICGLOSSMAP
				#pragma shader_feature _SPECGLOSSMAP
				#pragma shader_feature _ _SPECULARHIGHLIGHTS_OFF
			// SM2.0: NOT SUPPORTED #pragma shader_feature ___ _DETAIL_MULX2
			// SM2.0: NOT SUPPORTED shader_feature _PARALLAXMAP
			#pragma skip_variants SHADOWS_SOFT

			#pragma multi_compile_fwdadd_fullshadows
			#pragma multi_compile_fog

			#pragma vertex vertAdd
			#pragma fragment fragAdd
			#include "UnityStandardCoreForward.cginc"

			ENDCG
		}
			// ------------------------------------------------------------------
			//  Shadow rendering pass
			Pass {
				Name "ShadowCaster"
				Tags { "LightMode" = "ShadowCaster" }

				ZWrite On ZTest LEqual

				CGPROGRAM
				#pragma target 2.0
				#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
				#pragma shader_feature _METALLICGLOSSMAP
				#pragma shader_feature _SPECGLOSSMAP
				#pragma skip_variants SHADOWS_SOFT
				#pragma multi_compile_shadowcaster

				#pragma vertex vertShadowCaster
				#pragma fragment fragShadowCaster

				#include "UnityStandardShadow.cginc"

				ENDCG
			}

			// ------------------------------------------------------------------
			// Extracts information for lightmapping, GI (emission, albedo, ...)
			// This pass it not used during regular rendering.
			Pass
			{
				Name "META"
				Tags { "LightMode" = "Meta" }

				Cull Off

				CGPROGRAM
				#pragma vertex vert_meta
				#pragma fragment frag_meta

				#pragma shader_feature _EMISSION
				#pragma shader_feature _METALLICGLOSSMAP
				#pragma shader_feature _SPECGLOSSMAP
				#pragma shader_feature ___ _DETAIL_MULX2
				#pragma shader_feature EDITOR_VISUALIZATION

				#include "UnityStandardMeta.cginc"
				ENDCG
			}
		}


		FallBack "VertexLit"
}
