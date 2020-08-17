// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Pi/TransparentUseLight" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TintColor("Tint Color", Color) = (1.0,1.0,1.0,1.0)
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 150

		CGPROGRAM
		#pragma surface surf Lambert noforwardadd alpha:fade


		fixed4 _TintColor;

		sampler2D _MainTex;
		sampler2D _BumpMap;
		half _Shininess;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = tex.rgb * _TintColor;
			o.Alpha = tex.a;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
		}
		ENDCG
	}

	Fallback "Legacy Shaders/Transparent/Diffuse"
}
