Shader "Custom/Glow" {
	Properties{
		_ColorTint("Color Tint", Color) = (1,1,1,1)
	}
		SubShader{
		Tags{ "Queue" = "Transparent"
		"RenderType" = "Transparent" }
		LOD 200
		Pass{
		ZWrite On
		ColorMask 0
	}
		CGPROGRAM
#pragma surface surf Lambert alpha

		struct Input {
		float4 color : Color;
		float2 uv_MainTex;
		float2 uv_BumpMap;
		float3 viewDir;
	};

	float4 _ColorTint;

	void surf(Input IN, inout SurfaceOutput o) {
		IN.color = _ColorTint;
		o.Albedo = IN.color;

		half rim = saturate(dot(normalize(IN.viewDir), o.Normal));
		half tresh = step(0, rim) * step(rim, 0.1);
		o.Alpha = rim*rim*rim;
	}

	ENDCG

	}
		FallBack "Diffuse"
}
