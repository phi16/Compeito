Shader "Hidden/Fluid/FluidSurface"
{
    Properties
    {
        _HeightIntensity ("Height Intensity", Range(0, 8)) = 1
        _NormalIntensity ("Normal Intensity", Range(0, 8)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "DisableBatching"="True" }
        LOD 100

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

            #include "Compute/Fluid.hlsl"

            float _HeightIntensity;
            float _NormalIntensity;

            v2f vert(appdata v)
            {
                float3 s = sampleSurfaceHeight(v.uv * W);
                v.vertex.xyz += float3(0, 0, - 1.0 / W) * s.z * _HeightIntensity;

                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 g = sampleSurfaceGrad(i.uv * W);
                float3 normal = normalize(float3(g.x, g.y, 1.0 / _HeightIntensity / _NormalIntensity));
                if(_HeightIntensity <= 0) normal = float3(0, 0, 1);
                float3 worldNormal = mul((float3x3)unity_ObjectToWorld, normal);
                worldNormal.xy *= -1;
                worldNormal = normalize(worldNormal);
                float3 col = _Color.Sample(sampler_linear_clamp, i.uv).rgb;
                col.rgb *= max(0, worldNormal.y);
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
