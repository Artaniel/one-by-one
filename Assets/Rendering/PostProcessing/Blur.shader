Shader "Hidden/Custom/Blur"
{
    HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

    TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
    float _BlurSize;

    float4 Frag(VaryingsDefault i) : SV_Target
    {
        //init color variable
        float4 col = 0;
        for (float index = 0; index<10; index++) {
            //get uv coordinate of sample
            float2 uv = i.texcoord + float2(0, (index / 9 - 0.5) * _BlurSize);
            //add color at position to color
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        }
        //divide the sum of values by the amount of samples
        col = col / 10;
        return col;
    }

    float4 Frag2(VaryingsDefault i) : SV_Target
    {
        //calculate aspect ratio
        float invAspect = _ScreenParams.y / _ScreenParams.x;
        //init color variable
        float4 col = 0;
        //iterate over blur samples
        for (float index = 0; index < 10; index++) {
            //get uv coordinate of sample
            float2 uv = i.texcoord + float2((index / 9 - 0.5) * _BlurSize * invAspect, 0);
            //add color at position to color
            col += SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);
        }
        //divide the sum of values by the amount of samples
        col = col / 10;
        return col;
    }

        ENDHLSL

        SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

#pragma vertex VertDefault
#pragma fragment Frag

            ENDHLSL
        }

        Pass
        {
            HLSLPROGRAM

#pragma vertex VertDefault
#pragma fragment Frag2

            ENDHLSL
        }
    }
}