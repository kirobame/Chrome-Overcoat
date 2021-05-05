Shader "Custom/SH_RetAim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ColorCenter("Color Center", Color) = (1,1,1,1)
        _ColorExt("Color Exterior", Color) = (1,1,1,1)

        _LerpDistance("Lerp Distance", Float) = 1
        _LerpAlpha("Lerp Alpha", Float) = 1

    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
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
                float4 screenPosition  : TEXCOORD0;
            };

            struct v2f
            {
                float4 screenPosition  : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _ColorCenter;
            float4 _ColorExt;

            float _LerpDistance;
            float _LerpAlpha;


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPosition = ComputeScreenPos(o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {

                float2 textureCoordinate = i.screenPosition.xy / i.screenPosition.w;
                textureCoordinate = textureCoordinate * 2 - 1;
                float textureCoordinateMul = length(textureCoordinate);
                // sample the texture
                //fixed4 col = tex2D(_MainTex, textureCoordinate);
                fixed4 col = lerp(_ColorCenter, _ColorExt, textureCoordinateMul * _LerpDistance);
                col.a = _LerpAlpha;

                return col;
            }
            ENDCG
        }
    }
            
    //Fallback "Diffuse"
}
