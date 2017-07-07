Shader "Demondrian/MatchAll"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_BorderColor("Border Color", Color) = (0, 0, 0, 1)
		_Scale("Scale", float) = 1
		_Border("Border Width", float) = 0.05
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
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _Color;
			half4 _BorderColor;
			float _Scale;
			float _Border;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : SV_POSITION) : SV_Target
			{
				float lineMult = ((screenPos.y + _Color.r + _Color.g + screenPos.x * _Scale) % 8) * 0.25 + 0.7;

				float limit = _Border / _Scale;
				if (i.uv.x < limit || i.uv.x >(1 - limit) ||
					i.uv.y < limit || i.uv.y >(1 - limit)) {
					return fixed4(_Color.rgb * (1 - _BorderColor.a) + _BorderColor.rgb * _BorderColor.a, 1);
				}
				return _Color * lineMult;
			}
			ENDCG
		}
	}
}
