using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class PermissionManager : IStringDownloader
{
    public string[] Players;
    public string[] Players_Permissions;
    
    public string[] Groups;
    public string[] Groups_Permissions;


    [HideInInspector] public override byte DownloadDelay { get; set; } = 30;


    private string RawData = "";


    public override void OnStringDownloaded(string Data)
    {
        RawData = Data;
    }

    void Start()
    {
        
    }

    public string[] GetGroups() { }

    public string[] GetGroup_Permissions(string Group) { }
    public string[] GetGroup_Players(string Group) { }
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns>{ "Player1": {"Permission1", "Permission2"}, "Player2": {"Permission1", "Permission2"}  }</returns>
    public string[][] GetPermissions()
    {
        string[] _Players = new string[0];
        string[] _Permissions = new string[0];

        bool InGroup = false;
        

        foreach(string line in RawData.Split(')


    }


}
