// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/RuleTrans" {
Properties {
	_MainTex ("Base", 2D) = "white" {}
	_FadeTex ("Fade", 2D) = "white" {}
	_RuleTex ("Rule", 2D) = "white" {}
	_Color ("Color", Color) = (1, 1, 1, 1)
	_Trans ("Trans", Range(0,1)) = 0
	_Rule  ("Rule", Range(0,1)) = 0
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha 
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _FadeTex;
			sampler2D _RuleTex;
			float4 _Color;
			float _Trans;
			float _Rule;
			
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 base = tex2D(_MainTex, i.texcoord);
				fixed4 fade = tex2D(_FadeTex, i.texcoord);
				fixed4 rule = tex2D(_RuleTex, i.texcoord);
				//最終的に全部黒くなるようにしてます
				rule.rgb = 1 - rule.rgb;
				if(_Rule<0.5){
					rule.rgb = (rule.rgb * _Rule*2);
				}else{
					rule.rgb = rule.rgb + (1-rule.rgb)*(_Rule-0.5)*2;
				}
				//滑らかに変化する様にする　説明が面倒臭い
				rule.rgb =  (int)(1.7 - rule.rgb)
							 - ((int)(0.7f + rule.rgb)*(int)(1.7 - rule.rgb))*(rule.rgb - 0.3)*2.5;
				
				//TransはクロスフェードRuleはルール画像を使用したトランジション
				if(_Trans != 0){
					base.rgb -= base.rgb*fade.w*(_Trans);
					fade.rgb -= fade.rgb*base.w*(1-_Trans);
					base.w = base.w * (1-_Trans);
					fade.w = fade.w * (_Trans);
				}else{
					base.rgb -= base.rgb*fade.w*(1-rule.x);
					fade.rgb -= fade.rgb*base.w*(rule.x);
					base.w = base.w * (rule.x);
					fade.w = fade.w * (1-rule.x);
				}
				return (base + fade) * (_Color);				
			}
		ENDCG
	}

}
	FallBack Off

}
