Shader "Hidden/UI/SDF/URP/Blur"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }

        SubShader
    {
        Tags { "RenderType" = "Opaque" }

        LOD 100
        ZTest Off
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma fragmentoption ARB_precision_hint_fastest
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
                float4 vertex : SV_POSITION;
                half2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            static const int samplingCount = 10;

            sampler2D _MainTex;

            float _Blur;
            half4 _Offsets;
            half _Weights[samplingCount];

            half4 frag(v2f i) : SV_Target
            {
                float blur = _Blur;
                blur = max(1, blur);

                fixed4 col = fixed4(0, 0, 0, 0);

                int j;

                [unroll]
                for (j = samplingCount - 1; j > 0; j--)
                {
                    col += tex2D(_MainTex, i.uv - (_Offsets.xy * j)) * _Weights[j];
                }

                [unroll]
                for (j = 0; j < samplingCount; j++)
                {
                    col += tex2D(_MainTex, i.uv + (_Offsets.xy * j)) * _Weights[j];
                }

                return col;
            }
            ENDCG
        }
    }
}