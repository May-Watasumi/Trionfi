Shader "Utage/UI/RuleFade"
{
	Properties
	{
		_RuleTex("Rule", 2D) = "white" {}
		_Strength("Strength", Range(0.0, 1.0)) = 0.2
		_Vague("Vague", Range(0.001, 1.0)) = 0.2
		_SrcBlend("_src", Int) = 1
		_DstBlend("_dst", Int) = 0


		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend [_SrcBlend][_DstBlend]
		ColorMask [_ColorMask]

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			#pragma multi_compile __ PREMULTIPLIED_ALPHA

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1: TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				half2 texcoord1 : TEXCOORD1;
				float4 worldPosition : TEXCOORD2;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;

			sampler2D _MainTex;
			sampler2D _RuleTex;
			fixed _Strength;
			fixed _Vague;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				OUT.texcoord1 = IN.texcoord1;

				#ifdef UNITY_HALF_TEXEL_OFFSET
				OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
				#endif
				
				OUT.color = IN.color * _Color;
				return OUT;
			}


			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);

				half fade = (tex2D(_RuleTex, IN.texcoord1).a + _TextureSampleAdd.a);
				fade = clamp(-(1 / _Vague)*_Strength + (1 / _Vague - 1)*fade + 1, 0, 1);
#ifdef PREMULTIPLIED_ALPHA
				color.rgb *= IN.color.rgb * IN.color.a * fade;
				color.a = 1 - (1 - color.a)*IN.color.a * fade;
#else
				color.a *= fade;
#endif

				color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
#ifdef UNITY_UI_ALPHACLIP
				clip(color.a - 0.001);
#endif
				return color;
			}
		ENDCG
		}
	}
}
