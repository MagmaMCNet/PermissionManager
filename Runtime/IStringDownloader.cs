using System;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

public class IStringDownloader : UdonSharpBehaviour
{
    [Tooltip("Raw URL For Config It Is Recommended To Use GitHub.io To Host The Files")]
    [SerializeField] public VRCUrl DataUrl;
    [Tooltip("How Long It Will Wait To Send New Request To Download New Config From The Server")]
    [Range(4, byte.MaxValue)]
    [SerializeField] public byte DownloadDelay = 10;
    [HideInInspector] public bool Ready = false;

    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        Ready = true;
        OnStringDownloaded(result.Result);
        VRCStringDownloader.LoadUrl(DataUrl, (IUdonEventReceiver)this);
    }
    public override void OnStringLoadError(IVRCStringDownload result) =>
        VRCStringDownloader.LoadUrl(DataUrl, (IUdonEventReceiver)this);

    public void Start()
    {
        VRCStringDownloader.LoadUrl(DataUrl, (IUdonEventReceiver)this);
        OnStart();
    }


    public virtual void OnStart()
    {
    }

    /// <summary>
    /// Function is called when the config string has finished downloading
    /// </summary>
    /// <param name="Data"></param>
    public virtual void OnStringDownloaded(string Data)
    {
        Debug.Log(Data, this);
    }
}
public static class Extensions
{
    public static bool ArrayContains(string[] array, string value, bool CaptialSensitve = true, bool WeakCheck = false)
    {
        if (!CaptialSensitve)
            value = value.ToLower();
        foreach (string item in array)
        {
            string Item = !CaptialSensitve ? item.ToLower() : item;
            if (WeakCheck)
            {
                if (Item.Contains(value))
                    return true;
            }
            else
            {
                if (Item == value)
                    return true;
            }
        }
        return false;
    }
    public static T[] Resize<T>(this T[] array, int newSize)
    {
        if (array.Length != newSize)
        {
            T[] newArray = new T[newSize];
            Array.Copy(array, 0, newArray, 0, (array.Length > newSize) ? newSize : array.Length);
            return newArray;
        }
        return array;
    }
    public static bool ContainsAny<T>(this T[] sourceArray, T[] targetArray)
    {
        foreach (T item in sourceArray)
        {
            if (Array.IndexOf(targetArray, item) != -1)
            {
                return true;
            }
        }
        return false;
    }
    public static T[] RemoveDuplicates<T>(this T[] array)
    {
        int uniqueCount = 0;

        // Count unique elements
        for (int i = 0; i < array.Length; i++)
        {
            bool isUnique = true;

            for (int j = 0; j < uniqueCount; j++)
            {
                if (array[i].Equals(array[j]))
                {
                    isUnique = false;
                    break;
                }
            }

            if (isUnique)
            {
                array[uniqueCount] = array[i];
                uniqueCount++;
            }
        }

        T[] result = new T[uniqueCount];
        Array.Copy(array, result, uniqueCount);

        return result;
    }
    public static T[] Add<T>(this T[] array, params T[] items)
    {
        T[] newArray = new T[array.Length + items.Length];
        Array.Copy(array, newArray, array.Length);
        for (int i = 0; i < items.Length; i++)
            newArray[array.Length + i] = items[i];
        return newArray;
    }

    public static string[] ToLower(this string[] array)
    {
        string[] newArray = new string[array.Length];
        System.Array.Copy(array, newArray, array.Length);
        for (int i = 0; i < newArray.Length; i++)
            newArray[i] = newArray[i].ToLower();
        return newArray;
    }
    public static string[] ToUpper(this string[] array)
    {
        string[] newArray = new string[array.Length];
        System.Array.Copy(array, newArray, array.Length);
        for (int i = 0; i < newArray.Length; i++)
            newArray[i] = newArray[i].ToUpper();
        return newArray;
    }
    public static bool Contains<T>(this T[] array, T value)
    {
        foreach (T item in array)
            if (item.Equals(value))
                return true;
        return false;
    }

    /// <summary>
    /// Get Value Of Key From "Magma's Array Config Format"
    /// </summary>
    /// <param name="Key">The String Name Of The Variable Name To Locate Eg</param>
    /// <returns></returns>
    [Obsolete("Please Use The 2.0.0 MPC Format As The 1.0.0 MACF Format Has Been Deprecated", true)]
    public static string[] GetFromKey(this string data, string Key)
    {
        string[] ConfigData = data.Split('\n');
        string[] ReturnValue = new string[0];
        int StartIndex = 0;
        for (int i = 0; i < ConfigData.Length; i++)
        {
            if (ConfigData[i].Contains("--" + Key + "--"))
            {
                StartIndex = i + 1;
                break;
            }
        }
        for (int i = StartIndex; i < ConfigData.Length; i++)
        {
            if (ConfigData[i].StartsWith("--") || ConfigData[i].EndsWith("--"))
                break;
            if (!string.IsNullOrWhiteSpace(ConfigData[i]))
            {
                ReturnValue = ReturnValue.Add(ConfigData[i].Trim());
            }

        }
        return ReturnValue;
    }
}