# Compeito

Automatic shader/material generation from `.compeito` snippets,
for writing GPGPU procedures for VRChat in a ComputeShader-like style.

![Thumbnail](.github/images/thumbnail.png)

## Usage

Install from VPM: https://phi16.github.io/VRC_Packages/

0. Create a new `.compeito` file from the context menu, or from scratch.
1. Write your shader function (e.g. `float4 Main(uint2 id) { ... }`) in HLSL as usual.
  - Input: Texel location (`uint2`) of the output texture
  - Output: The value (typically `float4`) to output
2. Declare `#pragma kernel Main` to create a shader pass.
  - You can declare multiple passes.
  - You must declare all kernels at the top of the `.compeito` file.
  - You can also declare the return type, like `#pragma kernel Main float4`.
3. You've got the generated shader and material! 🎉
4. `passIndex = material.FindPass("Main");` to get the pass index.
5. `Compeito.Dispatch(material, passIndex, outputTexture);` to dispatch the kernel.

## Repository contents

- Examples (highly recommended to check out)
  - Minimal
    - [ComputeTest.compeito](https://github.com/phi16/Compeito/blob/main/Assets/Scenes/Minimal/ComputeTest.compeito)
    - [ComputeTest.cs](https://github.com/phi16/Compeito/blob/main/Assets/Scenes/Minimal/ComputeTest.cs)
  - Fluid
    - [FluidCompute.compeito](https://github.com/phi16/Compeito/blob/main/Assets/Scenes/Fluid/Compute/FluidCompute.compeito)
    - [FluidCompute.cs](https://github.com/phi16/Compeito/blob/main/Assets/Scenes/Fluid/Compute/FluidCompute.cs)
- Main implementation
  [CompeitoImporter.cs](https://github.com/phi16/Compeito/blob/main/Packages/com.imaginantia.compeito/Editor/CompeitoImporter.cs)
- Udon utility
  [Compeito.cs](https://github.com/phi16/Compeito/blob/main/Packages/com.imaginantia.compeito/Udon/Compeito.cs)

## Origin of the name

こんぺいとう (Konpeito) is a traditional Japanese sugar candy, small and cute.

## Syntax Highlighting for VSCode

Add the following to your `.vscode/settings.json`.

```
{
  "files.associations": {
    "*.compeito": "hlsl"
  }
}
```

## License

MIT
