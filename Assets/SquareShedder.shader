Shader "Custom/SquareBorderCenteredShader" {
    Properties {
        _MainTex ("Webcam Texture", 2D) = "white" {}
        _BorderColor ("Border Color", Color) = (1, 1, 1, 1)
        _BorderWidth ("Border Width", Float) = 1.0
        _SquareSize ("Square Size", Float) = 0.5
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _BorderColor;
            float _BorderWidth;
            float _SquareSize;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag (v2f i) : SV_Target {
                float4 color = tex2D(_MainTex, i.uv);
                float2 uv = i.uv - 0.5;

                if(uv.x < (_SquareSize - (_BorderWidth / 2)) || uv.x > (_SquareSize + (_BorderWidth / 2)))
                {
                    color = _BorderColor;
                }

                if(uv.y < (_SquareSize - (_BorderWidth / 2)) || uv.y > (_SquareSize + (_BorderWidth / 2)))
                {
                    color = _BorderColor;
                }

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}