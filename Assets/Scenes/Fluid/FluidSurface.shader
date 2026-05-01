Shader "Hidden/Fluid/FluidSurface"
{
    Properties
    {
        _TessFactor ("Tessellation Factor", Range(1, 64)) = 4
        _HeightIntensity ("Height Intensity", Range(0, 8)) = 1
        _NormalIntensity ("Normal Intensity", Range(0, 8)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma hull hull
            #pragma domain domain
            #pragma fragment frag
            #pragma target 5.0

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct ControlPoint
            {
                float4 vertex : INTERNALTESSPOS;
                float2 uv     : TEXCOORD0;
            };

            struct TessFactors
            {
                float edge[3] : SV_TessFactor;
                float inside  : SV_InsideTessFactor;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            ControlPoint vert(appdata v)
            {
                ControlPoint o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            float _TessFactor;

            TessFactors patchConstant(InputPatch<ControlPoint, 3> patch)
            {
                TessFactors f;
                f.edge[0] = _TessFactor;
                f.edge[1] = _TessFactor;
                f.edge[2] = _TessFactor;
                f.inside = _TessFactor;
                return f;
            }

            [domain("tri")]
            [partitioning("fractional_odd")]
            [outputtopology("triangle_cw")]
            [outputcontrolpoints(3)]
            [patchconstantfunc("patchConstant")]
            ControlPoint hull(InputPatch<ControlPoint, 3> patch, uint id : SV_OutputControlPointID)
            {
                return patch[id];
            }

            #include "Compute/Fluid.hlsl"

            float _HeightIntensity;
            float _NormalIntensity;

            [domain("tri")]
            v2f domain(TessFactors factors,
                       OutputPatch<ControlPoint, 3> patch,
                       float3 bary : SV_DomainLocation)
            {
                float4 pos = patch[0].vertex * bary.x
                           + patch[1].vertex * bary.y
                           + patch[2].vertex * bary.z;
                float2 uv  = patch[0].uv * bary.x
                           + patch[1].uv * bary.y
                           + patch[2].uv * bary.z;

                float3 s = sampleSurfaceHeight(uv * W);
                pos.xyz += float3(0, 0, - 1.0 / W) * s.z * _HeightIntensity;

                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv;
                return o;
            }

            sampler2D _ColorSampler;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 g = sampleSurfaceGrad(i.uv * W);
                float3 normal = normalize(float3(g.x, g.y, 1.0 / _HeightIntensity / _NormalIntensity));
                if(_HeightIntensity <= 0) normal = float3(0, 0, 1);
                float3 worldNormal = mul((float3x3)unity_ObjectToWorld, normal);
                worldNormal.xy *= -1;
                worldNormal = normalize(worldNormal);
                float3 col = tex2D(_ColorSampler, i.uv).rgb;
                col.rgb *= max(0, worldNormal.y);
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
