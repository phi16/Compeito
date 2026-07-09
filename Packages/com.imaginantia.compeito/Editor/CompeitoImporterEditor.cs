using System.IO;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace imaginantia.Compeito
{

[CustomEditor(typeof(CompeitoImporter))]
public class CompeitoImporterEditor : ScriptedImporterEditor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Bake .compeito into .shader"))
            Bake(((ScriptedImporter)target).assetPath);
        ApplyRevertGUI();
    }

    static void Bake(string filePath)
    {
        string name = Path.GetFileNameWithoutExtension(filePath);
        string shaderPath = EditorUtility.SaveFilePanelInProject("Bake .compeito into .shader", name, "shader", "", Path.GetDirectoryName(filePath));
        if (string.IsNullOrEmpty(shaderPath)) return;

        string src = CompeitoImporter.GenerateShaderSource(filePath, null);
        File.WriteAllText(shaderPath, src);
        AssetDatabase.ImportAsset(shaderPath);

        Shader shader = AssetDatabase.LoadAssetAtPath<Shader>(shaderPath);
        EditorGUIUtility.PingObject(shader);
    }
}

}
