#pragma once

#define W 256
#define LogW 8

Texture2D<float2> _Velocity; // 1-form, u
Texture2D<float> _Pressure; // 0-form, ρ
Texture2D<float4> _Color; // 0-form, c

Texture2D<float4> _AdvectColor; // 0-form, Backtrace[u](c)
Texture2D<float> _Divergence; // 0-form, D = div u
Texture2D<float4> _Surface; // 1-form g, 0-form h

//
//   ^
//   |
// x >  ρ,c
//   |    
//   +---^--->
//       y

float2 pickVelocity(uint2 id) {
  return _Velocity.Load(int3(id, 0)).xy;
}
float pickPressure(uint2 id) {
  return _Pressure.Load(int3(id, 0)).x;
}
float4 pickColor(uint2 id) {
  return _Color.Load(int3(id, 0));
}
float4 pickAdvectColor(uint2 id) {
  return _AdvectColor.Load(int3(id, 0));
}
float pickDivergence(uint2 id) {
  return _Divergence.Load(int3(id, 0)).x;
}
float pickPressureAverage(uint2 id) {
  return _Pressure.Load(int3(0, 0, LogW)).x;
}
float2 pickPressureBound(uint2 id) {
  if(id.x < 0 || id.x >= W || id.y < 0 || id.y >= W) return 0;
  return float2(pickPressure(id), 1);
}
float3 pickPressureGrad(uint2 id) {
  float rho = pickPressure(id);
  float2 g = float2(
    id.x == 0 ? 0 : rho - pickPressure(id - uint2(1, 0)),
    id.y == 0 ? 0 : rho - pickPressure(id - uint2(0, 1))
  );
  return float3(g, rho);
}

SamplerState sampler_linear_clamp;

float2 sampleVelocityElement(float2 loc) {
  float2 d = saturate(loc - (W - 1));
  loc = min(loc, W - 1);
  float2 u = _Velocity.SampleLevel(sampler_linear_clamp, (loc + 0.5) / W, 0).xy;
  u = lerp(u, 0, d);
  return u;
}
float2 sampleVelocity(float2 loc) {
  float x = sampleVelocityElement(loc - float2(0, 0.5)).x;
  float y = sampleVelocityElement(loc - float2(0.5, 0)).y;
  return float2(x, y); 
}
float4 sampleColor(float2 loc) {
  return _Color.SampleLevel(sampler_linear_clamp, loc / W, 0);
}
float4 sampleAdvectColor(float2 loc) {
  return _AdvectColor.SampleLevel(sampler_linear_clamp, loc / W, 0);
}
float3 sampleSurfaceHeight(float2 loc) {
  return _Surface.SampleLevel(sampler_linear_clamp, loc / W, 0).z;
}
float2 sampleSurfaceGradElement(float2 loc) {
  float2 d = saturate(loc - (W - 1));
  loc = min(loc, W - 1);
  float2 u = _Surface.SampleLevel(sampler_linear_clamp, (loc + 0.5) / W, 0).xy;
  u = lerp(u, 0, d);
  return u;
}
float2 sampleSurfaceGrad(float2 loc) {
  float x = sampleSurfaceGradElement(loc - float2(0, 0.5)).x;
  float y = sampleSurfaceGradElement(loc - float2(0.5, 0)).y;
  return float2(x, y); 
}