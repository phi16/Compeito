# Compeito

Automatic shader/material generation from `.compeito` snippet,
for writing GPGPU procedures for VRChat in a ComputeShader-like style.

## Usage

Install from VPM: https://phi16.github.io/VRC_Packages/

Example: [ComputeTest.compeito](https://github.com/phi16/Compeito/blob/main/Assets/Scenes/Compeito/ComputeTest.compeito), [ComputeTest.cs](https://github.com/phi16/Compeito/blob/main/Assets/Scenes/Compeito/ComputeTest.cs)

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
