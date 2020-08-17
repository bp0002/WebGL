// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Diffuse shader. Differences from regular Diffuse one:
// - no Main Color
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Pi/Mobile/Diffuse" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_TintColor("Tint Color", Color) = (1.0,1.0,1.0,1.0)
	}

	Fallback "Mobile/Diffuse"
}
