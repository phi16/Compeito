Pass {
  Name "{{KERNEL}}"
  Cull Off
  HLSLPROGRAM
  #pragma vertex CompeitoGeneratedVert
  #pragma fragment CompeitoGeneratedFrag
  
  struct CompeitoGeneratedVertInput {
    float2 uv : TEXCOORD0;
  };
  struct CompeitoGeneratedVertOutput {
    float4 vertex : SV_POSITION;
  };

  #define CompeitoPass_{{KERNEL}}
  #line {{LINE}} "{{PATH}}"
  {{BODY}}

  #include "UnityCG.cginc"

  float4 _CompeitoOutputSize;

  CompeitoGeneratedVertOutput CompeitoGeneratedVert(CompeitoGeneratedVertInput input) {
    CompeitoGeneratedVertOutput output;
    output.vertex = float4(input.uv*2-1, 0, 1);
    return output;
  }

  float4 CompeitoGeneratedFrag(CompeitoGeneratedVertOutput input) : SV_Target {
    uint2 id = (uint2)input.vertex.xy;
    #if !UNITY_UV_STARTS_AT_TOP
    id.y = (uint)_CompeitoOutputSize.y - id.y - 1; 
    #endif
    return {{KERNEL}}(id);
  }

  ENDHLSL
}