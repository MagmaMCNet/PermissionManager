using System;
using UdonSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRC.SDKBase;
using VRC.Udon;

namespace PermissionSystem
{
    public class PermissionManagerRef: UdonSharpBehaviour
    {
        public PermissionManager PermissionManager;
        [HideInInspector] public bool Ready = false;


        public bool HasPermissions(string PlayerName, params string[] Permissions)
        {
            int index = Array.IndexOf(PermissionManager.Players, PlayerName.ToLower());
            if (index == -1)
                return false;
            string[] _Permissions = PermissionManager.Players_Permissions[index].Split('+');
            return _Permissions.ContainsAny(Permissions);
        }
        public bool HasPermissions(VRCPlayerApi Player, params string[] Permissions) =>
            HasPermissions(Player.displayName, Permissions);
        public bool HasPermissions(int PlayerID, params string[] Permissions) =>
            HasPermissions(VRCPlayerApi.GetPlayerById(PlayerID), Permissions);
        public bool HasPermissions(params string[] Permissions) =>
            HasPermissions(Networking.LocalPlayer, Permissions);


        public virtual void OnDataUpdated() { }


        public void AddEventListener() =>
            PermissionManager.AddEventListener(this);

        public void RemoveEventListener() =>
            PermissionManager.RemoveEventListener(this);

        public void AddEventListener(UdonSharpBehaviour behaviour) =>
            PermissionManager.AddEventListener(behaviour);

        public void RemoveEventListener(UdonSharpBehaviour behaviour) =>
            PermissionManager.RemoveEventListener(behaviour);

        public void Start()
        {
            enabled = false;
            SendCustomEventDelayedFrames(nameof(CheckStatus), 1);
            OnStart();
        }

        public void CheckStatus()
        {
            if (PermissionManager.Ready)
            {
                OnReady();
                enabled = true;
                Ready = true;
            }
            else
                SendCustomEventDelayedSeconds(nameof(CheckStatus), 0.25f);
        }


        public void Awake()
        {
            if (PermissionManager == null)
                FindPermissionSystem();

            OnAwake();
        }

        private void FindPermissionSystem()
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
        public virtual void OnStart() { }
        public virtual void OnReady() { }
    }
}
