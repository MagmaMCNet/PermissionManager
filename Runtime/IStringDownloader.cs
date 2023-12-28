﻿using System;
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
    [SerializeField] public virtual byte DownloadDelay { get; set; } = 10;
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
    /// <summary>
    /// Checks if the array contains a specified string value.
    /// </summary>
    /// <param name="array">The array of strings to search.</param>
    /// <param name="value">The string value to search for.</param>
    /// <param name="capitalSensitive">Indicates whether the search is case-sensitive. Default is true.</param>
    /// <param name="weakCheck">Indicates whether to perform a weak (substring) check instead of an exact match. Default is false.</param>
    /// <returns>True if the array contains the specified value; otherwise, false.</returns>
    public static bool ArrayContains(this string[] array, string value, bool capitalSensitive = true, bool weakCheck = false)
    {
        if (!capitalSensitive)
            value = value.ToLower();

        foreach (string item in array)
        {
            string itemToCheck = !capitalSensitive ? item.ToLower() : item;

            if (weakCheck)
            {
                if (itemToCheck.Contains(value))
                    return true;
            }
            else
            {
                if (itemToCheck == value)
                    return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Resizes the array to the specified size.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    /// <param name="array">The array to resize.</param>
    /// <param name="newSize">The new size of the array.</param>
    /// <returns>The resized array.</returns>
    public static T[] Resize<T>(ref T[] array, int newSize)
    {
        if (array.Length != newSize)
        {
            T[] newArray = new T[newSize];
            Array.Copy(array, newArray, Math.Min(array.Length, newSize));
            array = newArray;
        }
        return array;
    }

    /// <summary>
    /// Checks if any elements in the source array are present in the target array.
    /// </summary>
    public static bool ContainsAny<T>(ref T[] sourceArray, T[] targetArray)
    {
        foreach (T item in sourceArray)
            if (Array.IndexOf(targetArray, item) != -1)
                return true;

        return false;
    }

    /// <summary>
    /// Removes duplicate elements from the array.
    /// </summary>
    public static T[] RemoveDuplicates<T>(ref T[] array)
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

    /// <summary>
    /// Adds the specified items to the array.
    /// </summary>
    public static T[] Add<T>(ref T[] array, params T[] items)
    {
        T[] newArray = new T[array.Length + items.Length];
        Array.Copy(array, newArray, array.Length);

        for (int i = 0; i < items.Length; i++)
            newArray[array.Length + i] = items[i];

        return newArray;
    }

    /// <summary>
    /// Converts all string elements in the array to lowercase.
    /// </summary>
    public static string[] ToLower(ref string[] array)
    {
        string[] newArray = new string[array.Length];
        Array.Copy(array, newArray, array.Length);

        for (int i = 0; i < newArray.Length; i++)
            newArray[i] = newArray[i].ToLower();

        return newArray;
    }

    /// <summary>
    /// Converts all string elements in the array to uppercase.
    /// </summary>
    public static string[] ToUpper(ref string[] array)
    {
        string[] newArray = new string[array.Length];
        Array.Copy(array, newArray, array.Length);

        for (int i = 0; i < newArray.Length; i++)
            newArray[i] = newArray[i].ToUpper();

        array = newArray;
        return newArray;
    }

    /// <summary>
    /// Checks if the array contains the specified value.
    /// </summary>
    public static bool Contains<T>(this T[] array, T value)
    {
        foreach (T item in array)
            if (item.Equals(value))
                return true;

        return false;
    }
}