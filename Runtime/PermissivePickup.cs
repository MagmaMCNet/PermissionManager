using PermissionSystem;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
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
        perm = HasPermissions(AuthorizedPermissions);
        if (Reverse)
            perm = !perm;
        if (Destructive)
        {
            if (perm)
            {
                foreach (var obj in Items)
                {
                    if (obj == null)
                        continue;
                    Destroy(obj);
                }
            }
        }
        else
            foreach (var obj in Items)
            {
                if (obj == null)
                    continue;
                obj.pickupable = perm;
            }
        UpdateObjects();
    }
    bool perm = false;
    public override void OnDataUpdated()
    {
        perm = HasPermissions(AuthorizedPermissions);
        if (Reverse)
            perm = !perm;
    }

    public void UpdateObjects()
    {
        foreach (var obj in Items)
            obj.pickupable = perm;
        SendCustomEventDelayedSeconds(nameof(UpdateObjects), 0.5f);
    }
}
