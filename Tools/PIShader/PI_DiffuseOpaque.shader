Shader "Pi/DiffuseOpaque" {
	Properties{
		_TintColor("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGBA)", 2D) = "white" {}

		[HideInInspector] _UseAlphaFromDiffuseTexture("UseAlphaFromDiffuseTexture", Int) = 1
		[HideInInspector] _DiffuseTextureHasAlpha("DiffuseTextureHasAlpha", Int) = 1
	}
		SubShader{
			Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" "ForceNoShadowCasting" = "True" }
			Cull back
			LOD 200

			Pass
			{

				CGPROGRAM
				// Physically based Standard lighting model, and enable shadows on all light types

				// Use shader model 3.0 target, to get nicer looking lighting
				#pragma target 3.0
				#pragma vertex vert alpha
				#pragma fragment frag alpha

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				fixed4 _TintColor;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;
					return col;
				}

				ENDCG
			}
		}
}
