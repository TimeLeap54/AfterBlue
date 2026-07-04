Shader "AfterBlue/WaterSurfaceURP"
{
    Properties
    {
        _ShallowColor ("Shallow Cyan", Color) = (0.32, 0.82, 0.88, 0.78)
        _MidColor ("Mid Teal", Color) = (0.08, 0.52, 0.62, 0.78)
        _DeepColor ("Deep Blue Green", Color) = (0.03, 0.24, 0.30, 0.78)
        _HighlightColor ("Caustic Highlight", Color) = (0.82, 0.98, 1.0, 0.48)
        _Alpha ("Alpha", Range(0, 1)) = 0.78
        _WaveHeight ("Wave Height", Range(0, 0.25)) = 0.028
        _WaveScale ("Wave Scale", Range(0.1, 8)) = 0.36
        _WaveSpeed ("Wave Speed", Range(0, 4)) = 0.24
        _HighlightStrength ("Caustic Strength", Range(0, 1)) = 0.13
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

            float2 Hash22(float2 p)
            {
                float3 p3 = frac(float3(p.xyx) * 0.1031);
                p3 += dot(p3, p3.yzx + 33.33);
                return frac((p3.xx + p3.yz) * p3.zy);
            }

            float CellularEdge(float2 p, float time)
            {
                float2 cell = floor(p);
                float2 local = frac(p);
                float nearest = 8.0;
                float secondNearest = 8.0;

                [unroll]
                for (int y = -1; y <= 1; y++)
                {
                    [unroll]
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 offset = float2(x, y);
                        float2 randomPoint = Hash22(cell + offset);
                        randomPoint = 0.5 + 0.38 * sin(time + 6.2831 * randomPoint);
                        float2 delta = offset + randomPoint - local;
                        float distanceToPoint = dot(delta, delta);

                        if (distanceToPoint < nearest)
                        {
                            secondNearest = nearest;
                            nearest = distanceToPoint;
                        }
                        else if (distanceToPoint < secondNearest)
                        {
                            secondNearest = distanceToPoint;
                        }
                    }
                }

                float edge = secondNearest - nearest;
                return 1.0 - smoothstep(0.018, 0.095, edge);
            }

            Varyings vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float time = _Time.y * _WaveSpeed;

                float broadX = sin(positionWS.x * 0.55 + time);
                float broadZ = cos(positionWS.z * 0.48 + time * 1.27);
                float crossing = sin((positionWS.x + positionWS.z) * 0.32 + time * 0.72);
                float wave = broadX * 0.45 + broadZ * 0.38 + crossing * 0.17;

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

                float largeCells = CellularEdge(p * 0.54 + float2(time * 0.05, -time * 0.035), time);
                float smallCells = CellularEdge(p * 0.82 + float2(-time * 0.04, time * 0.055), time * 1.12);
                float caustic = saturate(largeCells * 0.52 + smallCells * 0.18);
                caustic = smoothstep(0.55, 1.0, caustic);

                float softVariation = sin(p.x * 0.37 + time) * 0.08 + cos(p.y * 0.41 - time * 0.7) * 0.08;
                float depthMix = saturate(0.58 + softVariation + input.uv.y * 0.08);
                half3 water = lerp(_DeepColor.rgb, _MidColor.rgb, depthMix);
                water = lerp(water, _ShallowColor.rgb, saturate(depthMix - 0.48) * 0.5);

                half3 highlighted = lerp(water, _HighlightColor.rgb, caustic * _HighlightStrength);

                return half4(highlighted, _Alpha);
            }
            ENDHLSL
        }
    }
}
