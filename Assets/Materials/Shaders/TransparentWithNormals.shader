Shader "Custom/TransparentWithNormals" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader{
		Tags{ "Queue" = "Transparent"
		"RenderType" = "Transparent" }
		LOD 200

		CGPROGRAM
		#pragma surface surf NoLighting alpha
			
			struct Input {
				float4 color : Color;
				float3 viewDir;
			};
						
			half4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) { 
			   half4 c;
			   c.rgb = s.Albedo;
			   c.a = s.Alpha;
			   return c;
			}

			float4 _Color;

			void surf(Input IN, inout SurfaceOutput o) {
				IN.color = _Color;
				o.Albedo = IN.color.rgb;

				//Show if worldPos.y < y
				half rim = saturate(dot(normalize(IN.viewDir), o.Normal));
				o.Alpha = IN.color.w *(1- rim);
				o.Alpha = max(0.2,o.Alpha);

			}

		ENDCG

	}
		FallBack "Diffuse"
}
