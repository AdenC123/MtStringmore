Shader "Custom/SkyGradient"
{
    Properties
    {
        _ColorTop ("Top Color", Color) = (1,1,0,1)
        _ColorBottom ("Bottom Color", Color) = (0,0,1,1)
        _OffsetY ("Offset Y", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            fixed4 _ColorTop;
            fixed4 _ColorBottom;
            float _OffsetY;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xy; // use world Y if needed
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float t = saturate((i.uv.y + _OffsetY) / 1.0); // adjust scale if needed
                return lerp(_ColorBottom, _ColorTop, t);
            }
            ENDCG
        }
    }
}
