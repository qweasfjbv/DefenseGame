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

        _NoiseStrength ("Noise Strength", Range(0,0.2)) = 0.03
        _NoiseScale ("Noise Scale", Float) = 10
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

            float _NoiseStrength;
            float _NoiseScale;

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

            float Noise(float2 uv)
            {
                float n = sin(uv.x * _NoiseScale)
                        + sin(uv.y * _NoiseScale * 1.37)
                        + sin((uv.x + uv.y) * _NoiseScale * 0.7);

                return n / 3.0; // 대략 -1 ~ 1
            }

            float GetGradient(float uvCoord, float2 uv)
            {
                float noiseStart = Noise(uv * 1.1) * _NoiseStrength;
                float noiseEnd   = Noise(uv * 0.9 + 10.0) * _NoiseStrength;

                float noisyStart = _Start + noiseStart;
                float noisyEnd   = _End + noiseEnd;

                noisyEnd = max(noisyEnd, noisyStart + 0.001);

                float t = saturate((uvCoord - noisyStart) / (noisyEnd - noisyStart));

                if (_Invert > 0.5)
                    t = 1.0 - t;

                return t;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col *= _Color;

                float coord = (_Direction < 0.5) ? i.uv.x : i.uv.y;
                float gradient = GetGradient(coord, i.uv);
                col.a *= gradient;

                return col;
            }
            ENDCG
        }
    }
}