#pragma kernel Main

float4 Main(uint2 id) {
  return float4((id.x ^ id.y) % 256 / 255.0, 1, saturate(id.x & id.y), 1);
}