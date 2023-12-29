using System;
using System.Text;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace PermissionSystem
{
    public class IStringDownloader: UdonSharpBehaviour
    {
        [Tooltip("Raw URL For Config It Is Recommended To Use GitHub.io To Host The Files")]
        [SerializeField] public VRCUrl DataUrl;
        [Tooltip("How Long It Will Wait To Send New Request To Download New Config From The Server")]
        [SerializeField] public byte DownloadDelay = 10;
        [Tooltip("Field for if the download will Start Again After Download Has Successfully Finished")]
        [SerializeField] public bool LoopDownload = true;
        [HideInInspector] public bool Ready = false;

        public override void OnStringLoadSuccess(IVRCStringDownload result)
        {
            Ready = true;
            OnStringDownloaded(result.Result);
            SendCustomEventDelayedSeconds(nameof(StartDownload), DownloadDelay);
        }

        public override void OnStringLoadError(IVRCStringDownload result) => StartDownload();


        public void Start()
        {
            OnAwake();
            StartDownload();
            OnStart();
        }


        public virtual void OnAwake() { }

        public virtual void OnStart() { }

        /// <summary>
        /// Function is called when the config string has finished downloading
        /// </summary>
        /// <param name="Data"></param>
        public virtual void OnStringDownloaded(string Data)
        {
            Debug.Log(Data, this);
        }

        public bool StartDownload()
        {
            if (Ready && !LoopDownload)
                return false;

            VRCStringDownloader.LoadUrl(DataUrl, (IUdonEventReceiver)this);
            return true;
        }

    }
    public static class ArrayExtensions
    {

        /// <summary>
        /// Concatenates the elements of a string array into a single string using the specified separator.
        /// </summary>
        /// <param name="array">The array of strings to join.</param>
        /// <param name="separator">The string to use as a separator between the elements. It can be an empty string or null.</param>
        /// <returns>A single string that consists of the elements of the array separated by the specified separator.</returns>
        public static string Join(this string[] array, string separator)
        {
            if (array == null || array.Length == 0)
                return "";

            string result = array[0];

            for (int i = 1; i < array.Length; i++)
            {
                result += separator + array[i];
            }

            return result;
        }


        /// <summary>
        /// Concatenates the elements of a string array into a single string using the specified separator.
        /// </summary>
        /// <param name="array">The array of strings to join.</param>
        /// <param name="separator">The character to use as a separator between the elements. It can be an empty string or null.</param>
        /// <returns>A single string that consists of the elements of the array separated by the specified separator.</returns>
        public static string Join(this string[] array, char separator) => Join(array, separator.ToString());

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
        public static bool ContainsAny<T>(this T[] sourceArray, params T[] targetArray)
        {
            foreach (T item in sourceArray)
                if (Array.IndexOf(sourceArray, item) != -1)
                    return true;

            return false;
        }

        /// <summary>
        /// Checks if the string contains any of the specified characters.
        /// </summary>
        public static bool ContainsAny(this string str, params string[] characters)
        {
            foreach (string c in characters)
                if (str.Contains(c))
                    return true;
            return false;
        }

        /// <summary>
        /// Checks if the string contains any of the specified characters.
        /// </summary>
        public static bool ContainsNumb(this string str)
        {
            foreach (int c in new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0 })
                if (str.Contains(c.ToString()))
                    return true;
            return false;
        }

        /// <summary>
        /// Removes duplicate elements from the array.
        /// </summary>
        public static T[] RemoveDuplicates<T>(this T[] array)
        {
            int uniqueCount = 0;

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
        public static T[] Add<T>(this T[] array, params T[] items)
        {
            T[] newArray = new T[array.Length + items.Length];
            Array.Copy(array, newArray, array.Length);

            for (int i = 0; i < items.Length; i++)
                newArray[array.Length + i] = items[i];

            return newArray;
        }

        /// <summary>
        /// Adds the specified item to the array.
        /// </summary>
        public static T[] Add<T>(this T[] array, T item)
        {
            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, newArray, array.Length);
            newArray[array.Length] = item;
            return newArray;
        }

        /// <summary>
        /// Converts all string elements in the array to lowercase.
        /// </summary>
        public static string[] ToLower(this string[] array)
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
        public static string[] ToUpper(this string[] array)
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

        /// <summary>
        /// Removes an element at the specified index from the array.
        /// </summary>
        /// <typeparam name="T">The type of elements in the array.</typeparam>
        /// <param name="array">The array to modify.</param>
        /// <param name="index">The index of the element to remove.</param>
        /// <returns>A new array with the element removed.</returns>
        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (index < 0 || index >= array.Length)
                return array;

            T[] newArray = new T[array.Length - 1];

            if (index > 0)
                Array.Copy(array, 0, newArray, 0, index);

            if (index < array.Length - 1)
                Array.Copy(array, index + 1, newArray, index, array.Length - index - 1);

            return newArray;
        }

    }
}