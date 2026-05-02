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
                .Replace("{{KERNEL}}", kernel.Name)
                .Replace("{{RETURN_TYPE}}", kernel.ReturnType)
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

    struct KernelInfo
    {
        public string Name;
        public string ReturnType;
        public KernelInfo(string name, string returnType)
        {
            Name = name;
            ReturnType = returnType;
        }
    }

    static readonly Regex IdentifierRx = new Regex(@"^[a-zA-Z_]\w*$");

    static string StripComments(string line, ref bool inBlock)
    {
        var sb = new StringBuilder();
        int i = 0;
        while (i < line.Length)
        {
            if (inBlock)
            {
                int end = line.IndexOf("*/", i);
                if (end < 0) return sb.ToString().TrimEnd();
                inBlock = false;
                i = end + 2;
            }
            else
            {
                int block = line.IndexOf("/*", i);
                int lc    = line.IndexOf("//", i);
                if (lc >= 0 && (block < 0 || lc < block))
                    return sb.Append(line, i, lc - i).ToString().TrimEnd();
                if (block >= 0)
                {
                    sb.Append(line, i, block - i);
                    inBlock = true;
                    i = block + 2;
                }
                else
                {
                    sb.Append(line, i, line.Length - i);
                    break;
                }
            }
        }
        return sb.ToString().TrimEnd();
    }

    static (List<KernelInfo> kernels, int line, string body) Parse(string src, AssetImportContext ctx)
    {
        var kernels = new List<KernelInfo>();
        var lines = src.Split('\n');
        bool inBlock = false;
        int i = 0;
        for (; i < lines.Length; i++)
        {
            var line = StripComments(lines[i].Trim(), ref inBlock);
            if (line.Length == 0) continue;

            var tokens = line.Split(new char[] { ' ', '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
            int nameIdx = -1;
            if      (tokens.Length >= 2 && tokens[0] == "#pragma" && tokens[1] == "kernel") nameIdx = 2;
            else if (tokens.Length >= 3 && tokens[0] == "#" && tokens[1] == "pragma" && tokens[2] == "kernel") nameIdx = 3;
            if (nameIdx >= 0)
            {
                if (tokens.Length < nameIdx + 1 || !IdentifierRx.IsMatch(tokens[nameIdx]))
                    ctx.LogImportError($"line {i + 1}: #pragma kernel requires a valid identifier");
                else if (tokens.Length > nameIdx + 2)
                    ctx.LogImportError($"line {i + 1}: #pragma kernel has too many tokens");
                else
                {
                    string returnType = tokens.Length == nameIdx + 2 ? tokens[nameIdx + 1] : "float4";
                    kernels.Add(new KernelInfo(tokens[nameIdx], returnType));
                }
            }
            else
                break;
        }
        string body = string.Join("\n", lines, i, lines.Length - i);
        return (kernels, i, body);
    }

    public static string GetTemplateDirectory()
    {
        string[] guids = AssetDatabase.FindAssets($"{nameof(CompeitoImporter)} t:MonoScript");
        if (guids.Length == 0) return null;
        string path = AssetDatabase.GUIDToAssetPath(guids[0]);
        return path.Replace(Path.GetFileName(path), "");
    }
}

}