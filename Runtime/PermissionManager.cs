using System;
using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
public class PermissionManager : IStringDownloader
{
    public string[] Players;
    public string[] Players_Permissions;
    
    public string[] Groups;
    public string[] Groups_Permissions;


    private string RawData = "";

    public override void OnAwake()
    {
        DownloadDelay = 30;
        LoopDownload = true;
    }


    public override void OnStringDownloaded(string Data)
    {
        RawData = Data;
        GetPermissions();
    }

    /*
    public string[] GetGroups()
    {

    }

    public string[] GetGroup_Permissions(string Group) { }
    public string[] GetGroup_Players(string Group) { }
    */

    /// <summary>
    /// 
    /// </summary>
    /// <returns>{ "Player1": {"Permission1+Permission2"}, "Player2": {"Permission1+Permission2"}  }</returns>
    public void GetPermissions()
    {

        bool InGroup = false;
        string CurrentGroup = "";
        string[] Current_Groups_Permissions = new string[0];

        foreach (string line in TrimData(RawData))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;
            
            if (IsGroup(line))
            {
                InGroup = true;
                CurrentGroup = GetGroupName(line);
                Groups = Groups.Add(CurrentGroup);

                Current_Groups_Permissions = GetGroupPermissions(line);
                Groups_Permissions = Groups_Permissions.Add(Current_Groups_Permissions.Join('+'));

                continue;
            }

            if (!InGroup)
                continue;

            string PlayerName = line.ToLower();
            string[] PlayerPermissions = Current_Groups_Permissions;
            int index = Array.IndexOf(Players, PlayerName);
            if (index != -1)
            {
                PlayerPermissions = PlayerPermissions.Add(Players_Permissions[index].Split('+'));
                Players_Permissions[index] = PlayerPermissions.Join('+');
            }
            else
            {
                Players = Players.Add(PlayerName);
                Players_Permissions = Players_Permissions.Add(PlayerPermissions.Join('+'));
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
