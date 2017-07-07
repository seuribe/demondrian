Shader "Demondrian/Piece"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_BorderColor("Border Color", Color) = (0, 0, 0, 1)
		_Scale("Scale", float) = 1
		_Border("Border Width", float) = 0.05
		_OnlyTexture("Render only texture", int) = 0
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
				float4 vertex : POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half4 _Color;
			half4 _BorderColor;
			float _Scale;
			float _Border;
			int _OnlyTexture;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			inline bool inBorder(float2 uv, float borderWidth) {
			    return uv.x < borderWidth || uv.x > (1 - borderWidth) || uv.y < borderWidth || uv.y > (1 - borderWidth);
			}

			inline bool showTexture(half3 texColor) {
			    return _OnlyTexture == 1 || texColor.r != 1 || texColor.g != 1 || texColor.b != 1;
			}

			fixed4 frag(v2f i, UNITY_VPOS_TYPE screenPos : POSITION) : SV_Target
			{
                half3 texColor = tex2D(_MainTex, i.uv).rgb;
				if (showTexture(texColor))
					return half4(texColor, _Color.a);

				float limit = _Border / _Scale;
				if (inBorder(i.uv, limit))
					return fixed4(_Color.rgb * (1 - _BorderColor.a) + _BorderColor.rgb * _BorderColor.a, 1);

				return _Color;
			}
			ENDCG
		}
	}
}
