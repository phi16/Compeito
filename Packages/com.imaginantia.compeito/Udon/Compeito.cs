using UnityEngine;
#if UDONSHARP
using UdonSharp;
using VRC.SDKBase;
#endif

namespace imaginantia.Compeito
{

using Kernel = System.Int32;
using RT = UnityEngine.RenderTexture;

#if UDONSHARP
public class Compeito : UdonSharpBehaviour
#else
public class Compeito : MonoBehaviour
#endif
{
    // Utility functions

    public static RT CreateRT(string name, int width, int height, RenderTextureFormat format = RenderTextureFormat.ARGBFloat, bool autoGenerateMips = false) {
        RT rt = new RT(width, height, 0, format);
        rt.name = name;
        rt.filterMode = FilterMode.Point;
        rt.useMipMap = autoGenerateMips;
        rt.autoGenerateMips = autoGenerateMips;
        rt.Create();
        return rt;
    }

    public static void Dispatch(Material material, Kernel kernel, RT dest) {
        if(material == null) {
            Debug.LogError("Compeito.Dispatch: material is null");
            return;
        }
        if(kernel == -1) {
            Debug.LogError("Compeito.Dispatch: kernel is -1");
            return;
        }
        if(dest == null) {
            Debug.LogError("Compeito.Dispatch: dest is null");
            return;
        }
        material.SetVector("_CompeitoOutputSize", new Vector4(dest.width, dest.height, 0, 0));
#if UDONSHARP
        VRCGraphics.Blit(null, dest, material, kernel);
#else
        Graphics.Blit(null, dest, material, kernel);
#endif
    }
}

}