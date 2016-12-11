Shader "Custom/Appear" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Y("CurrentY", Float) = 0
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
		float y;
		float4 color : Color;
		float3 worldPos;
	};

	float4 _Color;
	float _Y;

	void surf(Input IN, inout SurfaceOutput o) {
		IN.color = _Color;
		IN.y = _Y;
		o.Albedo = IN.color;

		//Show if worldPos.y < y
		o.Alpha = step(IN.worldPos.y,IN.y)* IN.color.w;
	}

	ENDCG

	}
		FallBack "Diffuse"
}
