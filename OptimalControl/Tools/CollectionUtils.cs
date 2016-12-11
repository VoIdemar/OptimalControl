using System;
using System.Collections.Generic;

namespace OptimalControl.Tools
{
    public static class CollectionUtils
    {
        public static string GetContentsString<T>(this IList<T> list)
        {
            string[] results = new string[list.Count];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = list[i].ToString();
            }
            return results.GetContentsString();
        }
        
        public static string GetContentsString<T>(this IList<T[]> list)
        {
            string[] results = new string[list.Count];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = list[i].GetContentsString();
            }
            return results.GetContentsString();
        }
    }
}
