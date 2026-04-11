Shader "Custom/Gradation"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _Color ("Tint Color", Color) = (1,1,1,1)

        _Start ("Gradient Start", Range(0,1)) = 0.3
        _End ("Gradient End", Range(0,1)) = 0.7

        _Direction ("Direction (0=X, 1=Y)", Float) = 0
        _Invert ("Invert", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _Color;

            float _Start;
            float _End;
            float _Direction;
            float _Invert;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float GetGradient(float uvCoord)
            {
                float t = saturate((uvCoord - _Start) / (_End - _Start));

                if (_Invert > 0.5)
                    t = 1.0 - t;

                return t;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;

                float coord = (_Direction < 0.5) ? i.uv.x : i.uv.y;
                float gradient = GetGradient(coord);
                col.a *= gradient;

                return col;
            }
            ENDCG
        }
    }
}