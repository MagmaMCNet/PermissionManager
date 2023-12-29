using PermissionSystem;
using UnityEngine;

public class PermissiveObject : PermissionManagerRef
{
    [Space(5)]
    [InspectorName("GameObject")]
    [Tooltip("leave empty for self")]
    public GameObject gameobject;
    [Tooltip("If the GameObject will get deleted")]
    public bool Destructive = false;
    [Tooltip("If It Will Check Every Time The ")]
    public bool LoopCheck = false;

    [Space(5)]
    public bool Reverse = false;
    public string[] AuthorizedPermissions = new string[0];
    public override void OnAwake()
    {
        if (gameobject == null)
            gameobject = gameObject;
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
                Destroy(gameobject);
        }
        else
            gameObject.SetActive(HasPermissions(AuthorizedPermissions));

    }

    public override void OnDataUpdated()
    {
        bool Permission = HasPermissions(AuthorizedPermissions);
        if (Reverse)
            Permission = !Permission;
        gameObject.SetActive(Permission);
    }

}
