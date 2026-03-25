Shader "Custom/TransparentCircle"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Radius ("Radius", Range(0,1)) = 0.25
        _RevealCenter ("Reveal Center", Vector) = (0.5, 1.05, 0, 0)
        _RevealRadius ("Reveal Radius", Float) = 0
        _Feather ("Feather Width", Range(0, 0.5)) = 0.1
        _SpotFeather ("Spotlight Feather", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            float4 _Color;
            float2 _Center;
            float _Radius;
            float _Feather;
            float _SpotFeather;

            float4 _RevealCenter;
            float _RevealRadius;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float distSpot   = distance(i.uv, _Center);
                float distReveal = distance(i.uv, _RevealCenter.xy);

                // --- Spotlight with outward feather ---
                float spotInner = _Radius;                    // fully transparent inside this
                float spotOuter = _Radius + _SpotFeather;     // fades to black by here

                if (distSpot < spotInner)
                {
                    // Fully inside spotlight
                    return float4(0,0,0,0);
                }
                else if (distSpot < spotOuter)
                {
                    // Feather zone: fade from transparent to black
                    float t = (distSpot - spotInner) / (_SpotFeather);
                    float alpha = saturate(t); // 0 → 1
                    return float4(0,0,0,alpha);
                }

                // --- Reveal with feather (same idea as before) ---
                float revealInner = _RevealRadius - _Feather;
                float revealOuter = _RevealRadius;

                if (distReveal < revealInner)
                {
                    return float4(0,0,0,0);
                }
                else if (distReveal < revealOuter)
                {
                    float t = (distReveal - revealInner) / (_Feather);
                    float alpha = saturate(t);
                    return float4(0,0,0,alpha);
                }

                // Fully black outside both
                return float4(0,0,0,1);
            }

            ENDCG
        }
    }
}
