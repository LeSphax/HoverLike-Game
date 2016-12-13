Shader "Custom/Glow" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
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
		float3 viewDir;
	};

	float4 _Color;

	void surf(Input IN, inout SurfaceOutput o) {
		IN.color = _Color;
		o.Albedo = IN.color;

		half rim = saturate(dot(normalize(IN.viewDir), o.Normal));
		half tresh = step(0, rim) * step(rim, 0.1);
		o.Alpha = rim*rim*rim* IN.color.w;
	}

	ENDCG

	}
		FallBack "Diffuse"
}
