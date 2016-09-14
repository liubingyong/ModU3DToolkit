using System;
using UnityEngine;

public class Utils
{
    //
    // Static Methods
    //
    public static void CheckIsNull (System.Object o)
    {
        if (o != null) {
            Debug.LogError ("GameObject should be Null");
        }
    }

    public static void CheckNotNull (System.Object o)
    {
        if (o == null) {
            Debug.LogError ("GameObject is Null");
        }
    }

    public static double GetSecondsSinceEpoch ()
    {
        DateTime d = new DateTime (1970, 1, 1, 8, 0, 0, DateTimeKind.Utc);
        return (DateTime.UtcNow - d).TotalSeconds;
    }

    public static void RandomIndices (ref int[] ints)
    {
        for (int i = 0; i < ints.Length; i++) {
            ints [i] = i;
        }
        for (int j = 0; j < ints.Length / 2; j++) {
            int num = UnityEngine.Random.Range (j, ints.Length);
            int num2 = ints [num];
            ints [num] = ints [j];
            ints [j] = num2;
        }
    }

    public static void SetVertexColours (GameObject obj, Color col)
    {
        MeshFilter componentInChildren = obj.GetComponentInChildren<MeshFilter> ();
        if (componentInChildren != null) {
            Mesh mesh = componentInChildren.mesh;
            Vector3[] vertices = mesh.vertices;
            Color[] array = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) {
                array [i] = col;
            }
            mesh.colors = array;
        } else {
            Debug.LogError ("SetVertexColours: No Mesh on " + obj);
        }
    }

    public static void SetVertexColoursRandom (GameObject obj)
    {
        MeshFilter componentInChildren = obj.GetComponentInChildren<MeshFilter> ();
        if (componentInChildren != null) {
            Mesh mesh = componentInChildren.mesh;
            Vector3[] vertices = mesh.vertices;
            Color[] array = new Color[vertices.Length];
            for (int i = 0; i < vertices.Length; i++) {
                array [i].r = UnityEngine.Random.Range (0, 1);
                array [i].g = UnityEngine.Random.Range (0, 1);
                array [i].b = UnityEngine.Random.Range (0, 1);
            }
            mesh.colors = array;
        } else {
            Debug.LogError ("SetVertexColours: No Mesh on " + obj);
        }
    }

    public static void ShuffleArray<T> (ref T[] toShuffle)
    {
        for (int i = 0; i < toShuffle.Length / 2; i++) {
            int num = UnityEngine.Random.Range (i, toShuffle.Length);
            T t = toShuffle [num];
            toShuffle [num] = toShuffle [i];
            toShuffle [i] = t;
        }
    }

    public static int StringToHash (string s)
    {
        return Animator.StringToHash (s);
    }
}


public static class UtilAngles
{
    //
    // Static Fields
    //
    public const float RAD_TO_DEG = 57.29578f;

    public const float DEG_TO_RAD = 0.01745329f;

    //
    // Static Methods
    //
    public static Vector2 AngleVector (float angleDegrees)
    {
        angleDegrees *= 0.01745329f;
        return new Vector2 (Mathf.Cos (angleDegrees), Mathf.Sin (angleDegrees));
    }
}

public static class UtilStrings
{
    //
    // Static Methods
    //
    //
    // Static Methods
    //
    public static string ReplaceUnsafeChars (string source)
    {
        source = source.Replace ("'", "&apos;");
        source = source.Replace ("\"", "&quot;");
        source = source.Replace ("&", "&amp;");
        return source;
    }

    public static string TrimSpace (string untrimmed)
    {
        if (string.IsNullOrEmpty (untrimmed)) {
            return string.Empty;
        }
        return untrimmed.Trim ();
    }
}