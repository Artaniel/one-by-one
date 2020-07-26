Shader "Custom/FriendlyEntity"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Center("Center", Vector) = (0.5, 0.5, 0, 0)
        _ColorMask("Color mask", Color) = (0.130, 0.317, 0.790, 1)
        _Period("Period", Float) = 20
        _AnimationSpeed("Animation Speed", Float) = 10
    }
        SubShader
    {
        Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
    float4 _Center;
    float4 _ColorMask;
    float _Period;
    float _AnimationSpeed;

    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        return o;
    }

    fixed4 frag(v2f i) : SV_Target
    {
        float4 center = _Center;
        // sample the texture
        fixed4 col = tex2D(_MainTex, i.uv);
        float2 uv = i.uv;

        float r2 = 0.1 * ((_AnimationSpeed * _Time) % (1 / _Period));

        float distanceToCenter = (uv.y - center.y) * (uv.y - center.y) + (uv.x - center.x) * (uv.x - center.x);
        fixed4 colorByPosition = _ColorMask * clamp(distanceToCenter, 0, 1.);

        if (distanceToCenter < r2 && distanceToCenter > r2 - (r2 / 2.)) {
            col *= _ColorMask;
        }

        return col;
    }
        ENDCG
    }
    }
}
