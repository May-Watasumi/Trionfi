Shader "GUI/Text-Outline"
{
   Properties {
       _MainTex ("Font Texture", 2D)                = "white" {}
       [HDR] _Color ("Text Color", Color)           = (1,1,1,1)
       [HDR] _OutlineColor ("Outline Color", Color) = (0,0,0,1)
       _OutlineSpread ("Outline Spread", Range(0.1, 10)) = 1
       [MaterialToggle(AUTO_OUTLINE_COLOR)] _AutoOutlineColor ("Auto Outline Color",float)=0
   }

   SubShader {

       Tags {
           "Queue"           = "Transparent"
           "IgnoreProjector" = "True"
           "RenderType"      = "Transparent"
           "PreviewType"     = "Plane"
       }
       Lighting Off Cull Off ZTest Always ZWrite Off
       Blend SrcAlpha OneMinusSrcAlpha

       Pass {
           CGPROGRAM
           #pragma vertex vert
           #pragma fragment frag
           #pragma shader_feature AUTO_OUTLINE_COLOR
           #include "UnityCG.cginc"

           struct appdata_t {
               float4 vertex : POSITION;
               half4  color  : COLOR;
               float2 uv     : TEXCOORD0;
           };

           struct v2f {
               float4 vertex : SV_POSITION;
               half4  color  : COLOR;
               float2 uv     : TEXCOORD0;
           };

           sampler2D _MainTex;
           float4 _MainTex_ST;
           float4 _MainTex_TexelSize;
           half4  _Color;
           half4  _OutlineColor;
           half   _OutlineSpread;

           v2f vert (appdata_t v)
           {
               v2f o;
               o.vertex = UnityObjectToClipPos(v.vertex);
               o.color = v.color * _Color;
               o.uv = TRANSFORM_TEX(v.uv,_MainTex);
               return o;
           }

           half4 frag (v2f i) : SV_Target
           {
               half4 col = i.color;
#ifdef AUTO_OUTLINE_COLOR
               half4 outc = abs(col - half4(1,1,1,0));
#else
               half4 outc = _OutlineColor;
#endif
               half a0 = tex2D(_MainTex, i.uv).a;
               col = lerp(outc, col, a0);

               float4 delta = float4(1, 1, 0,-1) * _MainTex_TexelSize.xyxy * _OutlineSpread;
               half a1 = max(max(tex2D(_MainTex, i.uv + delta.xz).a,
                                 tex2D(_MainTex, i.uv - delta.xz).a),
                             max(tex2D(_MainTex, i.uv + delta.zy).a,
                                 tex2D(_MainTex, i.uv - delta.zy).a));

               delta *= 0.7071;
               half a2 = max(max(tex2D(_MainTex, i.uv + delta.xy).a,
                                 tex2D(_MainTex, i.uv - delta.xy).a),
                             max(tex2D(_MainTex, i.uv + delta.xw).a,
                                 tex2D(_MainTex, i.uv - delta.xw).a));

               half aa = max(a0, max(a1, a2));
               col.a *= aa;

               return col;
           }
           ENDCG
       }
   }
}
