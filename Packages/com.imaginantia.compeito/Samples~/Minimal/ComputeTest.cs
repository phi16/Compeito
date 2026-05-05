using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using imaginantia.Compeito;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ComputeTest : UdonSharpBehaviour
{
    public Material compute;
    public Material display;
    private RenderTexture output;

    int kernel;

    void Start() {
        kernel = compute.FindPass("Main");
        output = Compeito.CreateRT("ComputeTestOutput", 256, 256);
        display.SetTexture("_MainTex", output);
    }

    void Update() {
        Compeito.Dispatch(compute, kernel, output);
    }
}

