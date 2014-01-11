Shader "Custom/Sprite/AlphaMask"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_MaskTex ("Mask (Sprite)", 2D) = "white" {}
		_Cutoff("Alpha cutoff", Range(0,1)) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue"="AlphaTest"
			"IgnoreProjector"="True"
			"RenderType"="TransparentCutout"
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
	
		Pass
		{		
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON
			#include "UnityCG.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord0 : TEXCOORD0;
				half2 texcoord1 : TEXCOORD1;
			};
			
			float4 _MainTex_ST;
			float4 _MaskTex_ST;			
			
			v2f vert (appdata_base IN) {
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
			    OUT.texcoord0 = TRANSFORM_TEX(IN.texcoord, _MainTex);
				OUT.texcoord1 = TRANSFORM_TEX(IN.texcoord, _MaskTex);
				return OUT;
			}
			
			sampler2D _MainTex;
			sampler2D _MaskTex;
			fixed _Cutoff;
			
			half4 frag (v2f IN) : COLOR {
				fixed4 base = tex2D(_MainTex, IN.texcoord0);
				fixed4 mask = tex2D(_MaskTex, IN.texcoord1);
				clip(mask.a - _Cutoff);
				//clip(mask.r - _Cutoff);		// r=g=b
				return base;
			}
			
			ENDCG
		}
	}
	FallBack "VertexLit"
}
