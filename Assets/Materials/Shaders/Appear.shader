Shader "Custom/Appear" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_Y("CurrentY", Float) = 0
		_Size("Size", Float) = 25
	}
	SubShader{
		Tags{ "Queue" = "Transparent"
		"RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
#pragma surface surf NoLighting alpha

			struct Input {
				float y;
				float size;
				float4 color : Color;
				float3 worldPos;
			};

			
			half4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) { 
			half4 c;
           c.rgb = s.Albedo;
           c.a = s.Alpha;
           return c;
		   }

			float4 _Color;
			float _Y;
			float _Size;

			void surf(Input IN, inout SurfaceOutput o) {
				IN.color = _Color;
				IN.y = _Y;
				IN.size = _Size;
				o.Albedo = IN.color* IN.worldPos.y/(IN.size/2);

				//Show if worldPos.y < y
				o.Alpha = step(IN.worldPos.y,IN.y)* IN.color.w;
			}

		ENDCG

	}
		FallBack "Diffuse"
}
