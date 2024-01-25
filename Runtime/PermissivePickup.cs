using PermissionSystem;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class PermissivePickup: PermissionManagerRef
{
    [Space(5)]
    [InspectorName("Pickups")]
    [Tooltip("leave empty for self")]
    public VRCPickup[] Items;
    [Tooltip("If the GameObject will get deleted")]
    public bool Destructive = false;
    [Tooltip("If It Will Check Every Time The ")]
    public bool LoopCheck = false;

    [Space(5)]
    public bool Reverse = false;
    public string[] AuthorizedPermissions = new string[0];
    public override void OnAwake()
    {
        if (Destructive)
            LoopCheck = false;
        if (LoopCheck)
            AddEventListener();
    }

    public override void OnReady()
    {
        bool Permission = HasPermissions(AuthorizedPermissions);
        if (Reverse)
            Permission = !Permission;
        if (Destructive)
        {
            if (Permission)
            {
                foreach (var obj in Items)
                    Destroy(obj);
                Destroy(this);
            }
        }
        else
            foreach (var obj in Items)
                obj.pickupable = Permission;
    }

    public override void OnDataUpdated()
    {
        bool Permission = HasPermissions(AuthorizedPermissions);
        if (Reverse)
            Permission = !Permission;
        foreach (var obj in Items)
            obj.pickupable = Permission;
    }

}
