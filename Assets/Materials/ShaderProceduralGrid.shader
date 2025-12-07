Shader "Custom/URPCheckerboard"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1,1,1,1)
        _Color2 ("Color 2", Color) = (0,0,0,1)
        _GridSize ("Grid Size", Float) = 10
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            Name "CheckerPass"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color1;
            float4 _Color2;
            float _GridSize;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv * _GridSize;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 grid = floor(IN.uv);
                float checker = fmod(grid.x + grid.y, 2.0);
                return checker < 1.0 ? _Color1 : _Color2;
            }
            ENDHLSL
        }
    }
}