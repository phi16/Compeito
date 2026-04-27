using System.IO;
using UnityEditor;
using UnityEngine;

namespace imaginantia.Compeito
{

public class CompeitoMenu : MonoBehaviour
{
    [MenuItem("Assets/Create/Compeito", false, 5)]
    static void CreateNewProgram()
    {
        string templateDir = CompeitoImporter.GetTemplateDirectory();
        string compeitoTemplate = File.ReadAllText(Path.Combine(templateDir, "CompeitoTemplate.hlsl"));
        ProjectWindowUtil.CreateAssetWithContent("New Compeito Program.compeito", compeitoTemplate);
    }
}

}
