Shader "Custom/ConvergingField" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Width("Width", Float) = 0.01
		_Spacing("Spacing", Float) = 0.1
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
		float moduloY;
		float width;
		float spacing;
		float4 color : Color;
		float3 worldPos;
	};

	float4 _Color;
	float _Width;
	float _Spacing;
	float _Y;

	void surf(Input IN, inout SurfaceOutput o) {
		IN.width = _Width;
		IN.spacing = _Spacing;
		IN.color = _Color;
		IN.moduloY = abs(fmod(_Y,_Spacing));
		o.Albedo = IN.color;

		half moduledWorldPos = abs(fmod(IN.worldPos.y,IN.spacing));
		half isOpaque = step(IN.moduloY - IN.width, moduledWorldPos)*step(moduledWorldPos, IN.moduloY + IN.width);
		//Show if worldPos.y < y
		o.Alpha = (1 - isOpaque) * IN.color.w + isOpaque;
	}

	ENDCG

	}
		FallBack "Diffuse"
}
