using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace imaginantia.Compeito
{

[ScriptedImporter(1, "compeito")]
public class CompeitoImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        string src = File.ReadAllText(ctx.assetPath);
        string name = Path.GetFileNameWithoutExtension(ctx.assetPath);

        string templateDir = GetTemplateDirectory();
        string shaderTemplate = File.ReadAllText(Path.Combine(templateDir, "ShaderTemplate.hlsl"));
        string passTemplate = File.ReadAllText(Path.Combine(templateDir, "PassTemplate.hlsl"));

        var (kernels, line, body) = Parse(src, ctx);

        var sb = new StringBuilder();
        foreach (var kernel in kernels)
        {
            string pass = passTemplate
                .Replace("{{KERNEL}}", kernel)
                .Replace("{{PATH}}", ctx.assetPath)
                .Replace("{{LINE}}", (line + 1).ToString())
                .Replace("{{BODY}}", body);
            sb.Append(pass);
        }
        string shaderSrc = shaderTemplate
            .Replace("{{NAME}}", name)
            .Replace("{{PASSES}}", sb.ToString());

        Shader shaderAsset = ShaderUtil.CreateShaderAsset(ctx, shaderSrc, true);
        shaderAsset.name = name;

        Material material = new Material(shaderAsset);

        ctx.AddObjectToAsset("shader", shaderAsset);
        ctx.AddObjectToAsset("material", material);
        ctx.SetMainObject(material);
    }

    static readonly Regex IsPragmaKernel =
        new Regex(@"^#pragma\s+kernel\b");
    static readonly Regex KernelNameRx =
        new Regex(@"^#pragma\s+kernel\s+([a-zA-Z_]\w*)");

    static (List<string> kernels, int line, string body) Parse(string src, AssetImportContext ctx)
    {
        var kernels = new List<string>();
        var lines = src.Split('\n');
        int i = 0;
        for (; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            if (IsPragmaKernel.IsMatch(line))
            {
                var m = KernelNameRx.Match(line);
                if (m.Success)
                    kernels.Add(m.Groups[1].Value);
                else
                    ctx.LogImportError($"line {i + 1}: #pragma kernel requires a valid identifier");
            }
            else if (line.Length == 0 || line.StartsWith("//"))
                continue;
            else
                break;
        }
        string body = string.Join("\n", lines, i, lines.Length - i);
        return (kernels, i, body);
    }

    static string GetTemplateDirectory()
    {
        string[] guids = AssetDatabase.FindAssets($"{nameof(CompeitoImporter)} t:MonoScript");
        if (guids.Length == 0) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return path.Replace(Path.GetFileName(path), "");
    }
}

}