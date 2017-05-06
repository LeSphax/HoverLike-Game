Shader "Custom/Circle" {

	//!!!! This shader only works on quads with a (90,0,0) rotation !!!!
	Properties{
		_Color("Color", Color) = (1,0,0,0)
		_Thickness("Thickness", Range(0.0,0.5)) = 0.05
		_Radius("Radius", Range(0.0, 0.5)) = 0.4
		_Dropoff("Dropoff", Range(0.01, 4)) = 0.1
		_Scale("Scale", Float) = 0.1
	}
		SubShader{
			Tags{ "Queue" = "Transparent"
			"RenderType" = "Transparent" }

			CGPROGRAM

		//#pragma vertex vert
		//#pragma fragment frag
		#pragma surface surf NoLighting alpha noforwardadd

			//#include "UnityCG.cginc"


			fixed4 _Color; // low precision type is usually enough for colors
			float _Thickness;
			float _Radius;
			float _Dropoff;
			float _Scale;

			struct Input {
				float thickness;
				float radius;
				float dropoff;
				float4 color : Color;
				float3 worldPos;
			};

			half4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten) { 
			half4 c;
           c.rgb = s.Albedo;
           c.a = s.Alpha;
           return c;
		   }

			// r = radius
			// d = distance
			// t = thickness
			// p = % thickness used for dropoff
			float antialias(float r, float d, float t, float p) {
				if (d < (r - 0.5*t))
					return  -pow(d - r + 0.5*t, 2) / pow(p*t, 2) + 1.0;
				else if (d > (r + 0.5*t))
					return  -pow(d - r - 0.5*t, 2) / pow(p*t, 2) + 1.0;
				else
					return 1.0;
			}

			void surf(Input IN, inout SurfaceOutput o) {
				IN.color = _Color;
				IN.dropoff = _Dropoff;
				IN.thickness = _Thickness * _Scale;
				IN.radius = _Radius * _Scale;

				float3 localPos = IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz;
				float distance = sqrt(pow(localPos.x, 2) + pow(localPos.z, 2));
				o.Albedo = IN.color;
				o.Alpha = antialias(IN.radius, distance, IN.thickness, IN.dropoff) * IN.color.w;
			}
			//fragmentInput vert(appdata_base v)
			//{
			//	fragmentInput o;

			//	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			//	o.uv = v.texcoord.xy - fixed2(0.5,0.5);

			//	return o;
			//}

			//

			//fixed4 frag(fragmentInput i) : SV_Target{
			//	float distance = sqrt(pow(i.uv.x, 2) + pow(i.uv.y,2));

			//	return fixed4(_Color.r, _Color.g, _Color.b, _Color.a*antialias(_Radius, distance, _Thickness, _Dropoff));
			//}


		ENDCG

	}

}