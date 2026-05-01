Shader "Hidden/Fluid/Debug/DisplayU"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Intensity ("Intensity", Float) = 1
        _Offset ("Offset", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float _Intensity;
            float _Offset;

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float2 u = tex2D(_MainTex, i.uv).rg;
                float4 col = float4(u * _Intensity, 0, 1);
                col.rgb += _Offset;
                return col;
            }
            ENDCG
        }
    }
}
