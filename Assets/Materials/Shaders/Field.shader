Shader "Custom/Field" {
	Properties{
		_ColorM("ColorMesh", Color) = (1,1,1,1)
		_ColorS("Color Stripes", Color) = (1,1,1,1)
		_Width("Width", Float) = 0.01
		_Spacing("Spacing", Float) = 0.1
		_Y("CurrentY", Float) = 0
	}
	SubShader{
		Tags{ "Queue" = "Geometry"
		"RenderType" = "Transparent" }
		LOD 200
	Pass{
		ZWrite On
		ColorMask 0
	}
	CGPROGRAM
	#pragma surface surf Lambert alpha

		struct Input {
			float4 colorM : Color;
		};

		float4 _ColorM;

		void surf(Input IN, inout SurfaceOutput o) {
			IN.colorM = _ColorM;
			o.Albedo = IN.colorM;
			o.Alpha = IN.colorM.w;
		}

	ENDCG

	CGPROGRAM
	#pragma surface surf Lambert alpha
		struct Input {
			float y;
			float width;
			float spacing;
			float4 colorS : Color;
			float3 worldPos;
		};

		float4 _ColorS;
		float _Width;
		float _Spacing;
		float _Y;

		void surf(Input IN, inout SurfaceOutput o) {
			IN.width = _Width / 2;
			IN.spacing = _Spacing;
			IN.colorS = _ColorS;
			IN.y = _Y;

			half moduledWorldPos = abs(fmod(IN.worldPos.y - IN.y, IN.spacing));

			half opacity = 1.0
				- step(IN.width, moduledWorldPos)*step(moduledWorldPos, IN.spacing / 2)* pow(moduledWorldPos - IN.width, 2) / pow(IN.width, 2)
				- step(IN.spacing / 2,moduledWorldPos)*step(moduledWorldPos,IN.spacing - IN.width)* pow(moduledWorldPos - IN.spacing + IN.width, 2) / pow(IN.width, 2);

			o.Albedo = IN.colorS;
			o.Alpha = opacity* IN.colorS.w;
		}

	ENDCG

	}
	FallBack "Diffuse"
}
