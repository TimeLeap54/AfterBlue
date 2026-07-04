Shader "AfterBlue/WaterSurfaceURP"
{
    Properties
    {
        _ShallowColor ("Shallow Cyan", Color) = (0.36, 0.92, 0.98, 0.78)
        _MidColor ("Mid Teal", Color) = (0.10, 0.62, 0.72, 0.78)
        _DeepColor ("Deep Blue Green", Color) = (0.03, 0.26, 0.32, 0.78)
        _HighlightColor ("Wave Highlight", Color) = (0.86, 1.0, 1.0, 0.55)
        _Alpha ("Alpha", Range(0, 1)) = 0.78
        _WaveHeight ("Wave Height", Range(0, 0.25)) = 0.055
        _WaveScale ("Wave Scale", Range(0.1, 8)) = 1.45
        _WaveSpeed ("Wave Speed", Range(0, 4)) = 0.42
        _HighlightStrength ("Highlight Strength", Range(0, 1)) = 0.38
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "WaterSurface"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                half4 _ShallowColor;
                half4 _MidColor;
                half4 _DeepColor;
                half4 _HighlightColor;
                half _Alpha;
                half _WaveHeight;
                half _WaveScale;
                half _WaveSpeed;
                half _HighlightStrength;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float wave : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float time = _Time.y * _WaveSpeed;
                float2 p = positionWS.xz * _WaveScale;

                float broad = sin(p.x * 0.45 + p.y * 0.18 + time);
                float cross = cos(p.x * -0.22 + p.y * 0.52 + time * 1.35);
                float small = sin((p.x + p.y) * 1.35 + time * 2.1);
                float wave = broad * 0.55 + cross * 0.32 + small * 0.13;

                positionWS.y += wave * _WaveHeight;
                output.positionHCS = TransformWorldToHClip(positionWS);
                output.positionWS = positionWS;
                output.uv = input.uv;
                output.wave = wave;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float time = _Time.y * _WaveSpeed;
                float2 p = input.positionWS.xz * _WaveScale;

                float broadBand = sin(p.x * 0.58 + p.y * 0.16 + time * 1.2);
                float diagonalBand = sin(p.x * -0.32 + p.y * 0.72 + time * 1.55);
                float fineRipple = sin((p.x + p.y) * 2.2 + time * 2.6);
                float mixedWave = broadBand * 0.52 + diagonalBand * 0.34 + fineRipple * 0.14;

                float depthMix = saturate(0.56 + mixedWave * 0.22 + input.uv.y * 0.12);
                half3 water = lerp(_DeepColor.rgb, _MidColor.rgb, depthMix);
                water = lerp(water, _ShallowColor.rgb, saturate(depthMix - 0.45) * 0.62);

                float crest = smoothstep(0.62, 0.96, mixedWave);
                float longHighlight = smoothstep(0.72, 0.95, broadBand) * 0.55;
                half3 highlighted = lerp(water, _HighlightColor.rgb, saturate((crest + longHighlight) * _HighlightStrength));

                return half4(highlighted, _Alpha);
            }
            ENDHLSL
        }
    }
}
