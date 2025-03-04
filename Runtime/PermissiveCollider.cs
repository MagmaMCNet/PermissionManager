using PermissionSystem;
using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PermissiveCollider: PermissionManagerRef
{
    [Space(5)]
    public Collider[] Colliders;
    [Tooltip("If the Colliders will get deleted")]
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
                foreach (var obj in Colliders)
                    Destroy(obj);
                Destroy(this);
            }
        }
        else
            foreach (var item in Colliders)
                item.enabled = Permission;
    }

    public override void OnDataUpdated()
    {
        bool Permission = HasPermissions(AuthorizedPermissions);
        if (Reverse)
            Permission = !Permission;
        foreach (var item in Colliders)
            item.enabled = Permission;
    }

}
