using System;

namespace OptimalControl.Tools
{
    public static class ArrayExtensions
    {
        public static bool EqualsElementwise<T>(this T[] arr1, T[] arr2) 
        {
            if (arr1 == null && arr2 == null) 
            {
                return true;
            }
            if (arr1 != null && arr2 != null) 
            {
                if (arr1.Length != arr2.Length) 
                {
                    return false;
                }
                for (int i = 0; i < arr1.Length; i++) 
                {
                    if (!arr1[i].Equals(arr2[i])) 
                    {
                        return false;
                    }
                }
                return true;
            } 
            else
            {
                return false;
            }
        }        
        
        public static bool EqualsElementwiseWithPrecision(this double[] arr1, double[] arr2, double eps)
        {
            if (arr1 == null && arr2 == null) 
            {
                return true;
            }
            if (arr1 != null && arr2 != null) 
            {
                if (arr1.Length != arr2.Length) 
                {
                    return false;
                }
                for (int i = 0; i < arr1.Length; i++) 
                {
                    if (Math.Abs(arr1[i] - arr2[i]) > eps)
                    {
                        return false;
                    }
                }
                return true;
            } 
            else
            {
                return false;
            }
        }     
        
        public static void Fill<T>(this T[] arr, T val)
        {
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    arr[i] = val;
                }
            }
        }
        
        public static T[] Slice<T>(this T[] arr, int start, int end)
        {
            if (start >= arr.Length) throw new ArgumentException("Index 'start' is out of bounds");
            if (end - start < 0) throw new ArgumentException("Negative slice length is not allowed");
            int sliceLength = ((end > arr.Length) ? arr.Length : end) - start;
            T[] result = new T[sliceLength];
            for (int i = 0; i < sliceLength; i++)
            {
                result[i] = arr[start + i];
            }
            return result;
        }
        
        public static T[] Append<T>(this T[] currentArr, T[] arr)
        {
            int currentArrLength = currentArr.Length, arrLength = arr.Length;
            T[] result = new T[currentArrLength + arrLength];
            for (int i = 0; i < currentArrLength; i++)
            {
                result[i] = currentArr[i];
            }
            for (int i = 0; i < arrLength; i++)
            {
                result[i + currentArrLength] = arr[i];
            }
            return result;
        }
        
        public static string GetContentsString<T>(this T[] arr)
        {
            int rowsCount = (int)Math.Ceiling(arr.Length/10.0);
            string[] rows = new string[rowsCount];
            rows[0] = string.Join(", ", arr.Slice(0, 10));
            for (int i = 1; i < rowsCount; i++)
            {
                rows[i] = "\n" + string.Join(", ", arr.Slice(10*i, 10*(i+1)));
            }
            return string.Format("[{0}]", string.Join(", ", rows));
        }
        
        public static string DeepGetContentsString<T>(this T[][] arr)
        {
            string[] result = new string[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                result[i] = arr[i].GetContentsString() + "\n";
            }
            return result.GetContentsString();
        }
    }
}
