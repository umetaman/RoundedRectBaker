Shader "RoundedRectBaker/RoundedRect"
{
    Properties
    {
        _BorderRadius("Border Radius", Range(0, 0.5)) = 0.05
        _Width("Width", Float) = 0.025
        _Margin("Margin", Range(0, 1.0)) = 0.1
        _Softness("Softness", Range(0, 1.0)) = 0.01
        _Color("Color", Color) = (1, 1, 1, 1)
        _Ratio("Ratio", Vector) = (1, 1, 0, 0)
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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

            float _BorderRadius;
            float _Width;
            float _Margin;
            float _Softness;
            float4 _Color;
            float2 _Ratio;

            float2 UVToCenterPosition(float2 uv) {
                return uv - (_Ratio / 2.0) + (_Ratio * 0.5 + float2(-0.5, -0.5));
            }

            float2 MarginToRectSize() {
                float x = clamp(_Margin, _Width * 1.5, 1.0);
                return (_Ratio * 0.5) - float2(x, x);
            }

            float RoundedRectDistance(float2 centerPosition, float2 rectSize, float radius)
            {
                return length(max(abs(centerPosition) - rectSize + radius, 0.0)) - radius;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float d = RoundedRectDistance(UVToCenterPosition(i.uv), MarginToRectSize(), _BorderRadius);
                float distance = smoothstep(_Width, _Width + _Softness, d);
                return float4(_Color.rgb, 1.0 - distance);
            }
            ENDCG
        }
    }
}
