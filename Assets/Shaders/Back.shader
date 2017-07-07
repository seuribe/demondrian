Shader "Demondrian/Back"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1, 1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : POSITION;
			};


			half4 _Color;

			float rand(float3 co)
			{
				return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
			}

			float noise(float3 input) {
				return (frac(sin(input.x * input.y) * cos(input.y + input.z) * 43758.5453) - 0.5).x;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : POSITION) : SV_Target
			{
				float lineMult = clamp(rand(i.vertex.xyz + float3(screenPos.xy, 1)), 0, 0.25) * 0.1 + 0.9;
				half4 ret = _Color * lineMult;
				ret.r += noise(i.vertex) * 0.025;
				ret.b += noise(screenPos) * 0.05;
//				ret.g += noise(screenPos) * 0.01;
				return ret;
			}


			ENDCG
		}
	}
}
