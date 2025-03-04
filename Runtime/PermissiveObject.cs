using PermissionSystem;
using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class PermissiveObject : PermissionManagerRef
{
    [Space(5)]
    [InspectorName("GameObjects")]
    [Tooltip("leave empty for self")]
    public GameObject[] GameObjects;
    [Tooltip("If the GameObject will get deleted")]
    public bool Destructive = false;
    [Tooltip("If It Will Check Every Time The ")]
    public bool LoopCheck = false;

    [Space(5)]
    public bool Reverse = false;
    public string[] AuthorizedPermissions = new string[0];
    public override void OnAwake()
    {
        if (GameObjects.Length == 0)
            GameObjects = new GameObject[1] { gameObject };
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
                foreach (var obj in GameObjects)
                    Destroy(obj);
                Destroy(this);
            }
        }
        else
            foreach (var obj in GameObjects)
                obj.SetActive(Permission);
    }

    public override void OnDataUpdated() => OnReady();

}
