﻿using System;
using System.Collections.Generic;
using System.Linq;

public static class ListExtensions
{
    private static Random rng = new Random(); 
    
    public static T NextOf<T>(this IList<T> list, T item)
    {
        var itemIndex = list.IndexOf(item);
        return list[(itemIndex + 1) == list.Count ? 0 : itemIndex + 1];
    }

    public static T PrevOf<T>(this IList<T> list, T item)
    {
        var itemIndex = list.IndexOf(item);
        return list[(itemIndex == 0) ? list.Count - 1 : itemIndex - 1];
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T PickRandom<T>(this IEnumerable<T> source)
    {
        var ticks = DateTime.Now.Ticks;
        return source.ElementAt(new Random((int)(ticks & 0xffffffffL) | (int)(ticks >> 32)).Next(source.Count()));
    }
}
