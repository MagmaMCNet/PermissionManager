using System;
using System.Reflection;
using UdonSharp;
using UnityEngine;
using VRC.Udon;

namespace PermissionSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class PermissionManager: IStringDownloader
    {
        [HideInInspector] public string[] Players;
        [HideInInspector] public string[] Players_Permissions;

        [HideInInspector] public string[] Groups;
        [HideInInspector] public string[] Groups_Permissions;

        [HideInInspector] public GameObject _Editor_Self;

        public UdonSharpBehaviour[] Events;

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
            foreach (var Behaviour in Events)
                Behaviour.SendCustomEvent("OnDataUpdated");
        }

        public bool AddEventListener(UdonSharpBehaviour Behaviour)
        {
            if (Array.IndexOf(Events, Behaviour) == -1)
            {
                Events = Events.Add(Behaviour);
                return true;
            }
            return false;
        }

        public bool RemoveEventListener(UdonSharpBehaviour Behaviour)
        {
            int index = Array.IndexOf(Events, Behaviour);

            if (index != -1)
            {
                Events = Events.RemoveAt(index);
                return true;
            }
            return false;
        }



        public string[] GetGroups()
        {
            string[] Groups = new string[0];
            foreach (string line in TrimData(RawData))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;

                if (IsGroup(line))
                    Groups = Groups.Add(GetGroupName(line));
                else
                    continue;
            }
            return Groups;
        }

        public string[] GetGroup_Permissions(string Group)
        {
            foreach (string line in TrimData(RawData))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;

                if (IsGroup(line))
                    if (GetGroupName(line).ToLower() == Group.ToLower())
                        return GetGroupPermissions(line);

                    else
                        continue;
            }
            return null;

        }
        public string[] GetGroup_Players(string Group)
        {
            bool InGroup = false;
            string[] Players = new string[0];
            foreach (string line in TrimData(RawData))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;

                if (IsGroup(line))
                {
                    if (GetGroupName(line).ToLower() == Group.ToLower())
                        InGroup = true;
                    else
                        InGroup = false;
                    continue;
                }

                if (!InGroup)
                    continue;

                Players = Players.Add(line.ToLower());
            }
            return Players;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>{ "Player1": {"Permission1+Permission2"}, "Player2": {"Permission1+Permission2"}  }</returns>
        public void GetPermissions()
        {

            bool InGroup = false;
            string[] Current_Groups_Permissions = new string[0];

            foreach (string line in TrimData(RawData))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//")) continue;

                if (IsGroup(line))
                {
                    InGroup = true;
                    Groups = Groups.Add(GetGroupName(line));

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

        public bool IsGroup(string RawLine) => RawLine.StartsWith(">>");

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
#if UNITY_EDITOR
        public void OnDrawGizmos()
        {

            Gizmos.DrawIcon(transform.position, "..\\..\\Packages\\dev.magmamc.permissionmanager\\Gizmos\\ManagerIcon.tiff", true);
        }
#endif
    }
}