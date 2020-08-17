using System;
using System.Linq;
using UnityEngine;

namespace Assets.Kanau.Editor.Utils
{
    public class ArrayHelpers
    {
        public static T[] ConcatArrays<T>(params T[][] arrays)
        {

            var result = new T[arrays.Sum(arr => arr.Length)];
            int offset = 0;
            for (int i = 0; i < arrays.Length; i++)
            {
                var arr = arrays[i];
                Buffer.BlockCopy(arr, 0, result, offset, arr.Length);
                offset += arr.Length;
            }
            return result;
        }

        public static T[] ConcatArrays<T>(T[] arr1, T[] arr2)
        {

            var result = new T[arr1.Length + arr2.Length];
            Buffer.BlockCopy(arr1, 0, result, 0, arr1.Length);
            Buffer.BlockCopy(arr2, 0, result, arr1.Length, arr2.Length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start, int length)
        {

            var result = new T[length];
            Buffer.BlockCopy(arr, start, result, 0, length);
            return result;
        }

        public static T[] SubArray<T>(T[] arr, int start)
        {
            return SubArray(arr, start, arr.Length - start);
        }

        public static bool isContain<T>(T elem, T[] arr)
        {
            if (elem == null)
                throw new Exception("elem为空！");

            if (arr == null || arr.Length == 0)
                return false;
            else {
                for (var i = 0; i < arr.Length; i++) {
                    if (arr[i].ToString() == elem.ToString())
                        return true;
                }
            }

            return false;
        }

        public static float[] Vector3toAray(Vector3 v)
        {
            float[] arr = new float[3];
            arr[0] = v.x;
            arr[1] = v.y;
            arr[2] = v.z;
            return arr;
        }
    }
}
