Shader "Hidden/Fluid/Touch/MotionVectorOverlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        LOD 100
        ZWrite Off
        ZTest Off

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

            v2f vert (appdata v)
            {
                float3 center = mul(UNITY_MATRIX_M, float4(0, 0, 0, 1)).xyz;
                if(distance(center, _WorldSpaceCameraPos) > 0.01) { // not the target camera
                    return (v2f) 0;
                }
                if(unity_OrthoParams.w < 0.5) { // perspective
                    return (v2f) 0;
                }

                v2f o;
                o.vertex = float4(v.uv*2-1, 0, 1);
                o.vertex.y *= -1;
                o.uv = v.uv;
                return o;
            }

            sampler2D _CameraMotionVectorsTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_CameraMotionVectorsTexture, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
