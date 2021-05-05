Shader "Unlit/SH_CRT"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CRTTex ("_CRTTex", 2D) = "white" {}
        [HDR]_Color("Color", Color) = (1,1,1,1)
        _CRTStrength("_CRTStrength", Float) = 1
        _CRTBrightness("_CRTBrightness", Float) = 1
        _CRTSaturation("_CRTSaturation", Float) = 1
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
            sampler2D _CRTTex;
            float4 _CRTTex_ST;
            float4 _Color;

            float _CRTStrength;
            float _CRTBrightness;
            float _CRTSaturation;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 crt = tex2D(_CRTTex, i.uv * _CRTTex_ST);
                crt.rgb *= _Color.rgb;

                float greyScaleValue = dot(crt.rgb, float3(0.2989, 0.5870, 0.1140));
                float4 crtGreyScale = float4(float3(greyScaleValue, greyScaleValue, greyScaleValue),1.0);
                crt = lerp(crtGreyScale, crt, _CRTSaturation);

                crt *= _CRTStrength;
                return col * (crt + _CRTBrightness);
            }
            ENDCG
        }
    }
}
