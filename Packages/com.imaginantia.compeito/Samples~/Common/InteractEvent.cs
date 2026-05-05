
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class InteractEvent : UdonSharpBehaviour
{
    public UdonBehaviour behaviour;
    public string eventName;

    public override void Interact()
    {
        behaviour.SendCustomEvent(eventName);
    }
}
