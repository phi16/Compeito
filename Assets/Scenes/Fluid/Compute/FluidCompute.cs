
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using imaginantia.Compeito;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FluidCompute : UdonSharpBehaviour
{
    public Material compute;
    private int addForce;
    private int advect;
    private int pressureInit;
    private int projectInit;
    private int projectStep;
    private int finalize;
    private int updateColor;
    private int updateSurface;

    private RenderTexture velocity0, velocity1;
    private RenderTexture pressure0, pressure1;
    private RenderTexture color0, color1;
    private RenderTexture pressureCopy;
    private RenderTexture divergence;
    private RenderTexture surface;

    public RenderTexture touchVelocity, touchColor;

    const int W = 256;
    private bool initialized = false;

    public int projectIterations = 4;

    public Material displayVelocity;
    public Material displayPressure;
    public Material displayColor;
    public Material fluidSurface;

    void Start()
    {
        addForce = compute.FindPass("AddForce");
        advect = compute.FindPass("Advect");
        pressureInit = compute.FindPass("PressureInit");
        projectInit = compute.FindPass("ProjectInit");
        projectStep = compute.FindPass("ProjectStep");
        finalize = compute.FindPass("Finalize");
        updateColor = compute.FindPass("UpdateColor");
        updateSurface = compute.FindPass("UpdateSurface");

        velocity0 = Compeito.CreateRT("Velocity0", W, W, RenderTextureFormat.RGFloat);
        velocity1 = Compeito.CreateRT("Velocity1", W, W, RenderTextureFormat.RGFloat);
        pressure0 = Compeito.CreateRT("Pressure0", W, W, RenderTextureFormat.RFloat);
        pressure1 = Compeito.CreateRT("Pressure1", W, W, RenderTextureFormat.RFloat);
        pressureCopy = Compeito.CreateRT("PressureCopy", W, W, RenderTextureFormat.RFloat, true);
        color0 = Compeito.CreateRT("Color0", W, W, RenderTextureFormat.ARGBFloat, true);
        color1 = Compeito.CreateRT("Color1", W, W);
        divergence = Compeito.CreateRT("Divergence", W, W, RenderTextureFormat.RFloat);
        surface = Compeito.CreateRT("Surface", W, W, RenderTextureFormat.ARGBFloat, true);
    }

    void Update()
    {
        compute.SetFloat("_Init", initialized ? 0 : 1);
        initialized = true;

        compute.SetTexture("_Velocity", velocity0);
        compute.SetTexture("_TouchVelocity", touchVelocity);
        compute.SetTexture("_TouchColor", touchColor);
        Compeito.Dispatch(compute, addForce, velocity1);

        compute.SetTexture("_Velocity", velocity1);
        Compeito.Dispatch(compute, advect, velocity0);

        Compeito.Copy(pressure0, pressureCopy);
        compute.SetTexture("_Pressure", pressureCopy);
        Compeito.Dispatch(compute, pressureInit, pressure0);

        compute.SetTexture("_Velocity", velocity0);
        Compeito.Dispatch(compute, projectInit, divergence);

        compute.SetTexture("_Divergence", divergence);
        for(int i=0;i<projectIterations;i++) {
            compute.SetTexture("_Pressure", pressure0);
            Compeito.Dispatch(compute, projectStep, pressure1);
            compute.SetTexture("_Pressure", pressure1);
            Compeito.Dispatch(compute, projectStep, pressure0);
        }

        compute.SetTexture("_Pressure", pressure0);
        Compeito.Dispatch(compute, finalize, velocity1);

        compute.SetTexture("_Color", color0);
        compute.SetTexture("_Velocity", velocity1);
        Compeito.Dispatch(compute, updateColor, color1);
        Compeito.Dispatch(compute, updateSurface, surface);
        Compeito.Copy(velocity1, velocity0);
        Compeito.Copy(color1, color0);

        displayVelocity.SetTexture("_MainTex", velocity0);
        displayPressure.SetTexture("_MainTex", pressure0);
        displayColor.SetTexture("_MainTex", color0);
        fluidSurface.SetTexture("_ColorSampler", color0);
        fluidSurface.SetTexture("_Surface", surface);
    }

    public void Restart() {
        initialized = false;
    }
}
