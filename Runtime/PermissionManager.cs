
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class PermissionManager : IStringDownloader
{
    public string Players;
    public string Permissions;


    public override void OnStringDownloaded(string Data)
    {
        
    }

    void Start()
    {
        
    }
}
