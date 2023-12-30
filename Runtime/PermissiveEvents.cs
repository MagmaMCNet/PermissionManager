using PermissionSystem;
using UdonSharp;
using UnityEngine;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class PermissiveEvents : PermissionManagerRef
{
    [Space(5)]
    [InspectorName("GameObjects")]
    [Tooltip("leave empty for self")]
    public UdonBehaviour[] AuthorizedReceivers;
    public string[] AuthorizedEvents;
    public UdonBehaviour[] DeniedReceivers;
    public string[] DeniedEvents;
    [Tooltip("If It Will Check Every Time The ")]
    public bool LoopCheck = false;
    public bool Networked = false;
    public NetworkEventTarget Targets;
    [Space(5)]
    public string[] AuthorizedPermissions = new string[0];

    public override void OnAwake()
    {
        if (LoopCheck)
            AddEventListener();
    }

    public void SendEvent(UdonBehaviour udonBehaviour, string eventName)
    {
        if (Networked)
            udonBehaviour.SendCustomNetworkEvent(Targets, eventName);
        else
            udonBehaviour.SendCustomEvent(eventName);
    }

    public override void OnDataUpdated()
    {
        bool Permission = HasPermissions(AuthorizedPermissions);
        if (Permission)
            for (int i = 0; i < AuthorizedEvents.Length; i++)
                SendEvent(AuthorizedReceivers[i], AuthorizedEvents[i]);
        else
            for (int i = 0; i < DeniedEvents.Length; i++)
                SendEvent(DeniedReceivers[i], DeniedEvents[i]);
    }

}
