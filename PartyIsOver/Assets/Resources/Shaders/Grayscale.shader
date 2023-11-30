Shader "Custom/Grayscale"
{
	Properties
	{
		_LerpFactor("Lerp Factor", Range(0, 1)) = 0
	}

	SubShader
	{
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

			v2f vert(appdata v) 
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			sampler2D _MainTex;
			uniform float _LerpFactor;

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float lum = dot(col.rgb, float3(0.299, 0.587, 0.114));
				return lerp(col, float4(lum, lum, lum, 1.0), _LerpFactor);
			}
			ENDCG
		}
	}
}