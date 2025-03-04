using System;
using UdonSharp;
using UnityEngine;
using Varneon.VUdon.Logger.Abstract;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

namespace PermissionSystem
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    [DefaultExecutionOrder(-99999)]
    public class PermissionManager: UdonSharpBehaviour
    {

        [Tooltip("Enforces Scripts that use the key authorization to wait till the correct key is entered before functioning Is in beta and requires custom script")]
        [SerializeField] private bool KeypadExtension = false;
        [Tooltip("A Optional Way To Log Messages To UdonConsole To Display In Game")]
        public UdonLogger Logger;
        [Tooltip("Raw URL For Config It Is Recommended To Use GitHub.io To Host The Files")]
        [SerializeField] private VRCUrl DataUrl;

        [HideInInspector] public string[] Players;
        [HideInInspector] public string[] Players_Permissions;

        [HideInInspector] public string[] Groups;
        [HideInInspector] public string[] Groups_Permissions;

        [HideInInspector] public GameObject _Editor_Self;

        [HideInInspector] public UdonSharpBehaviour[] Events;

        [HideInInspector] public string RawData = "";

        [HideInInspector] public bool Authorized = false;
        [HideInInspector] private string AuthKey = "";
        [HideInInspector] public bool Ready = false;

        private byte DownloadDelay = 60;

        public void LogTrace(string Message)
        {
            if (Logger != null)
                Logger.Log($"<color=purple>[PermissionManager]</color> <i><color=blue>Trace</color></i>: {Message}");
            Debug.Log($"<color=purple>[PermissionManager]</color> <i><color=blue>Info</color></i>: {Message}");
        }
        public void LogInfo(string Message)
        {
            if (Logger != null)
                Logger.Log($"<color=purple>[PermissionManager]</color> <color=blue>Info</color>: {Message}");
                Debug.Log($"<color=purple>[PermissionManager]</color> <color=blue>Info</color>: {Message}");
        }
        public void LogWarning(string Message)
        {
            if (Logger != null)
                Logger.LogWarning($"<color=purple>[PermissionManager]</color> <color=yellow>Warning</color>: {Message}");
                Debug.LogWarning($"<color=purple>[PermissionManager]</color> <color=yellow>Warning</color>: {Message}");
        }
        public void LogError(string Message)
        {
            if (Logger != null)
                Logger.LogError($"<color=purple>[PermissionManager]</color> <color=red>Error</color>: {Message}");
                Debug.LogError($"<color=purple>[PermissionManager]</color> <color=red>Error</color>: {Message}");
        }

        public string GenerateHash(string input)
        {
            int hash = 0;
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                hash = (hash * 31 + c) % 1000;
            }
            return Mathf.Abs(hash).ToString("D3");
        }
        public bool ValidateHash(string hash, string input)
        {
            if (!KeypadExtension)
                return true;
            Authorized = GenerateHash(hash) == input;
            AuthKey = input;
            if (Authorized)
                LogWarning("Authorization Validated LocalPlayer");
            return Authorized;
        }
        public bool IsAuthed() =>
            AuthKey == GenerateHash(Networking.LocalPlayer.displayName) || !KeypadExtension;


        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            Ready = true;
            OnStringDownloaded(result.Result);
            SendCustomEventDelayedSeconds(nameof(StartDownload), DownloadDelay);
        }

        public override void OnStringLoadError(IVRCStringDownload result)
        {
            Debug.LogError($"Failed To Download`{DataUrl.Get()}`, Object: `{gameObject.name}`", this);
            SendCustomEventDelayedSeconds(nameof(StartDownload), 5);
        }

        public void Start()
        {
            Debug.LogWarning($"Downloading Text: `{DataUrl.Get()}`", this);
            StartDownload();
        }

        public virtual bool StartDownload()
        {
            VRCStringDownloader.LoadUrl(DataUrl, (IUdonEventReceiver)this);
            return true;
        }

        public void OnStringDownloaded(string Data)
        {
            RawData = Data;
            GetPermissions();
            foreach (var Behaviour in Events)
            {
                if (Behaviour != null)
                    Behaviour.SendCustomEvent("OnDataUpdated");
                else
                    Debug.LogWarning("Behaviour Is Null");
            }
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