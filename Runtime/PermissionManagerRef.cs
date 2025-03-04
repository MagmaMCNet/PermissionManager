using System;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace PermissionSystem
{
    public class PermissionManagerRef: UdonSharpBehaviour
    {
        public PermissionManager PermissionManager;
        [HideInInspector] public bool Ready = false;
        private bool Keypad = false;
        public bool Getkeypad() => Keypad;
        public void UseKeypad()
        {
            PermissionManager.LogWarning(gameObject.name + " - Enabled Keypad");
            Keypad = true;
        }

        public bool HasPermissions(string PlayerName, params string[] Permissions)
        {
            int index = Array.IndexOf(PermissionManager.Players, PlayerName.ToLower());
            if (index == -1)
                return false;
            string[] _Permissions = PermissionManager.Players_Permissions[index].Split('+');
            foreach(string item in Permissions)
            if (!_Permissions.Contains(item))
                return false;
            return true;
        }
        public bool HasPermissions(VRCPlayerApi Player, params string[] Permissions) =>
            HasPermissions(Player.displayName, Permissions);
        public bool HasPermissions(int PlayerID, params string[] Permissions) =>
            HasPermissions(VRCPlayerApi.GetPlayerById(PlayerID), Permissions);
        public bool HasPermissions(params string[] Permissions) =>
            HasPermissions(Networking.LocalPlayer, Permissions);


        public virtual void OnDataUpdated() { }
        public virtual void OnDataPreUpdated() { }


        public void AddEventListener() =>
            PermissionManager.AddEventListener(this);

        public void RemoveEventListener() =>
            PermissionManager.RemoveEventListener(this);

        public void AddEventListener(UdonSharpBehaviour behavior) =>
            PermissionManager.AddEventListener(behavior);

        public void RemoveEventListener(UdonSharpBehaviour behavior) =>
            PermissionManager.RemoveEventListener(behavior);

        public void Start()
        {
            enabled = false;
            OnPreStart();
            SendCustomEventDelayedFrames(nameof(_CheckStatus), 1);
            OnStart();
        }

        public void _CheckStatus()
        {
            if (PermissionManager == null)
                SendCustomEventDelayedSeconds(nameof(_CheckStatus), 0.25f);
            else if (PermissionManager.Ready)
            {
                OnReady();
                enabled = true;
                Ready = true;
                if (Keypad)
                    _CheckAuth();
            }
            else
                SendCustomEventDelayedSeconds(nameof(_CheckStatus), 0.25f);
        }

        public void _CheckAuth()
        {
            if (PermissionManager.Authorized)
            {
                PermissionManager.LogInfo(gameObject.name + " - Authorized");
                OnAuth();
                enabled = true;
                Ready = true;
            }
            else
                SendCustomEventDelayedSeconds(nameof(_CheckAuth), 0.25f);
        }


        public void Awake()
        {
            if (PermissionManager == null)
                _FindPermissionSystem();

            OnAwake();
        }

        private void _FindPermissionSystem()
        {
            PermissionManager = GetComponent<PermissionManager>();
            if (PermissionManager != null)
                return;

            PermissionManager = (GameObject.Find("PermissionSystem") ?? gameObject).GetComponent<PermissionManager>();
            if (PermissionManager != null)
                return;

            PermissionManager = (GameObject.Find("PermissionManager") ?? gameObject).GetComponent<PermissionManager>();
            if (PermissionManager != null)
                return;

            PermissionManager = (GameObject.Find("PlayerPermissions") ?? gameObject).GetComponent<PermissionManager>();
            if (PermissionManager != null)
                return;

            if (PermissionManager != null)
                return;

            PermissionManager = (GameObject.Find("VRCWorld") ?? gameObject).GetComponent<PermissionManager>();
            if (PermissionManager != null)
                return;

            if (PermissionManager == null)
            { 
                Debug.LogError("Failed To Find PermissionManager", this);
            }
        }

        public virtual void OnAwake() { }
        public virtual void OnPreStart() { }
        public virtual void OnStart() { }
        public virtual void OnReady() { }
        public virtual void OnAuth() { }
    }
}
