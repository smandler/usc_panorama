Shader "VPaint/Unlit/VertexColorsA"
{
	SubShader {
		Tags { "RenderType"="Opaque" }
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct v2f {
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;
			};
			
			struct appdata {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f o) : COLOR
			{
				return (fixed4)o.color.a;
			}
			ENDCG
		}
	} 
}
