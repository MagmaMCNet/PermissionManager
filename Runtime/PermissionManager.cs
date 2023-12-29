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

        string[] _Groups = new string[0];
        string[] _Groups_Permissions = new string[0];

        bool InGroup = false;
        string CurrentGroup = "";

        foreach(string line in TrimData(RawData))
        {
            if (IsGroup(line))
            {
                InGroup = true;
                CurrentGroup = line.Split('>')[1].TrimEnd(line.Split('>')[2].ToCharArray());

            }
        }


    }

    public bool IsGroup(string RawLine) => RawLine.StartsWith(">>") && RawLine.ContainsNumb();

    public string GetGroupName(string rawLine)
    {
        int startIdx = rawLine.IndexOf(">> ") + 3;
        int endIdx = rawLine.IndexOf(" >");

        if (startIdx != -1 && endIdx != -1 && startIdx < endIdx)
        {
            return rawLine.Substring(startIdx, endIdx - startIdx);
        }

        return null;
    }

    public string[] GetGroupPermissions(string rawLine)
    {
        int startIdx = rawLine.IndexOf(" > ") + 3;

        if (startIdx != -1 && startIdx < rawLine.Length)
        {
            string permissionsSubstring = rawLine.Substring(startIdx);
            return permissionsSubstring.Split('+');
        }

        return null;
    }


    public string[] TrimData(string Raw)
    {
        string[] _strings = Raw.Split('\n');

        for (int i = 0; i < _strings.Length; i++)
            _strings[i] = _strings[i].Trim();

        return _strings;
    }

}
