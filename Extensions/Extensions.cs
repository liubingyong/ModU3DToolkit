using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;


public static class ExtensionsArray
{
    //
    // Static Methods
    //
    public static int ClampRange (this Array array, int index)
    {
        if (index < 0) {
            index = 0;
        } else {
            if (index >= array.Length) {
                index = array.Length - 1;
            }
        }
        return index;
    }

    public static bool InRange (this Array array, int index0)
    {
        return index0 >= 0 && index0 < array.Length;
    }

    public static void Swap<T> (T[] array, int index0, int index1) where T : UnityEngine.Object
    {
        T t = array [index0];
        array [index0] = array [index1];
        array [index1] = t;
    }
}

public static class ExtensionsBool
{
    //
    // Static Methods
    //
    public static float ToPlusMinusFloat (this bool me)
    {
        return (!me) ? -1 : 1;
    }

    public static int ToPlusMinusInt (this bool me)
    {
        return (!me) ? -1 : 1;
    }
}

public static class ExtensionsBounds
{
    //
    // Static Methods
    //
    public static Vector3 GetClampingVector (this Bounds me, Bounds other)
    {
        Vector3 zero = Vector3.zero;
        Bounds bounds = other;
        bounds.Encapsulate (me);
        zero.x = bounds.min.x - other.min.x + (bounds.max.x - other.max.x);
        zero.y = bounds.min.y - other.min.y + (bounds.max.y - other.max.y);
        zero.z = bounds.min.z - other.min.z + (bounds.max.z - other.max.z);
        return -zero;
    }

    public static Vector3 GetClampingVector (this Bounds me, Vector3 other)
    {
        Vector3 zero = Vector3.zero;
        if (other.x < me.min.x) {
            zero.x = me.min.x - other.x;
        } else {
            if (other.x > me.max.x) {
                zero.x = me.max.x - other.x;
            }
        }
        if (other.y < me.min.y) {
            zero.y = me.min.y - other.y;
        } else {
            if (other.y > me.max.y) {
                zero.y = me.max.y - other.y;
            }
        }
        if (other.z < me.min.z) {
            zero.z = me.min.z - other.z;
        } else {
            if (other.z > me.max.z) {
                zero.z = me.max.z - other.z;
            }
        }
        return zero;
    }
}

public static class ExtensionsColor
{
    /*
    public static Color LerpTo (this Color col, Color target, float amount)
    {
        return Color.Lerp (col, target, amount);
    }
        
    public static void Set (this Color col, float r, float g, float b)
    {
        col.r = r;
        col.g = g;
        col.b = b;
    }

    public static void Set (this Color col, float r, float g, float b, float a)
    {
        col.r = r;
        col.g = g;
        col.b = b;
        col.a = a;
    }

    public static void Set (this Color32 col, byte r, byte g, byte b)
    {
        col.r = r;
        col.g = g;
        col.b = b;
    }

    public static void Set (this Color32 col, byte r, byte g, byte b, byte a)
    {
        col.r = r;
        col.g = g;
        col.b = b;
        col.a = a;
    }

    public static void SetAlpha (this Color col, float a)
    {
        col.a = a;
    }

    public static void SetAlpha (this Color32 col, byte a)
    {
        col.a = a;
    }
    */
}

public static class ExtensionsContainers
{
    //
    // Static Methods
    //
    public static void Reverse_NoHeapAlloc<T> (this List<T> list)
    {
        int count = list.Count;
        for (int i = 0; i < count / 2; i++) {
            T value = list [i];
            list [i] = list [count - i - 1];
            list [count - i - 1] = value;
        }
    }

    public static void Reverse_NoHeapAlloc<T> (this T[] list)
    {
        int num = list.Length;
        for (int i = 0; i < num / 2; i++) {
            T t = list [i];
            list [i] = list [num - i - 1];
            list [num - i - 1] = t;
        }
    }
}

public enum ComparisonType
{
    NOT_EQUAL,
    EQUAL,
    GREATER,
    GREATER_EQUAL,
    LESS,
    LESS_EQUAL
}


public static class ExtensionsFloat
{
    //
    // Static Methods
    //
    public static float Abs (this float me)
    {
        return Mathf.Abs (me);
    }

    public static float Clamp (this float me, float min, float max)
    {
        ExtensionsFloat.Clamp (ref me, min, max);
        return me;
    }

    public static void Clamp (ref float me, float min, float max)
    {
        if (me > max) {
            me = max;
        } else {
            if (me < min) {
                me = min;
            }
        }
    }

    public static float ClampToCeiling (this float me, float ceiling)
    {
        if (me > ceiling) {
            return ceiling;
        }
        return me;
    }

    public static float ClampToFloor (this float me, float floor)
    {
        if (me < floor) {
            return floor;
        }
        return me;
    }

    public static bool Comparison (this float me, float other, ComparisonType comp)
    {
        switch (comp) {
        case ComparisonType.NOT_EQUAL:
            return me != other;
        case ComparisonType.EQUAL:
            return me == other;
        case ComparisonType.GREATER:
            return me > other;
        case ComparisonType.GREATER_EQUAL:
            return me >= other;
        case ComparisonType.LESS:
            return me < other;
        case ComparisonType.LESS_EQUAL:
            return me <= other;
        default:
            Debug.LogError ("Unknown Comparison " + comp.ToString ());
            return false;
        }
    }

    public static float DegreesShortestArcTo (this float me, float targetDegrees)
    {
        float num = targetDegrees - me;
        if (num > 180) {
            num -= 360;
        } else {
            if (num < -180) {
                num += 360;
            }
        }
        return num;
    }

    public static Vector2 DegreesToVector (this float me)
    {
        return UtilAngles.AngleVector (me);
    }

    public static float DegreesTurnToward (this float me, float targetDegrees, float turnAmount)
    {
        float f = me.DegreesShortestArcTo (targetDegrees);
        if (Mathf.Abs (f) < turnAmount) {
            return targetDegrees;
        }
        return me + turnAmount * Mathf.Sign (f);
    }

    public static float Lerp (this float me, float target, float progress)
    {
        return me + (target - me) * progress;
    }

    public static float LerpHalfCos (this float me, float target, float progress)
    {
        progress = 1f + Mathf.Cos (3.141593f + 3.141593f * progress) * 0.5f;
        return me + (target - me) * progress;
    }

    public static float RoundDownToDecimalPlace (this float me, int decimalPlace)
    {
        float num = Mathf.Pow (10, (float)decimalPlace);
        me = (float)((int)(me * num));
        return me / num;
    }

    public static float Sign (this float me)
    {
        return Mathf.Sign (me);
    }

    public static float ToDegrees (this float me)
    {
        return me * 57.29578f;
    }

    public static float ToRadians (this float me)
    {
        return me * 0.01745329f;
    }

    public static string MillisecondsToTime (this float time, bool printCents)
    {
        time = Mathf.Max (time, 0.0f);

        int cents = Mathf.FloorToInt (time * 100.0f) % 100,
        secs = Mathf.FloorToInt (time) % 60,
        mins = Mathf.FloorToInt (time / 60.0f);

        if (printCents)
            return string.Format (CultureInfo.InvariantCulture, "{0:D2}:{1:D2}:{2:D2}", mins, secs, cents);
        else
            return string.Format (CultureInfo.InvariantCulture, "{0:D2}:{1:D2}", mins, secs);
    }

    public static string MillisecondsToHours (this float time, bool printSeconds)
    {
        time = Mathf.Max (time, 0.0f);

        int secs = Mathf.FloorToInt (time) % 60,
        mins = Mathf.FloorToInt (time / 60.0f),
        hours = Mathf.FloorToInt (mins / 60.0f);
        mins = mins % 60;

        if (printSeconds)
            return string.Format (CultureInfo.InvariantCulture, "{0:D2}:{1:D2}:{2:D2}", hours, mins, secs);
        else
            return string.Format (CultureInfo.InvariantCulture, "{0:D2}:{1:D2}", hours, mins);
    }
}

public static class ExtensionsGameObject
{
    //
    // Static Methods
    //
    public static Bounds GetBoundsOfChildren<T> (this GameObject obj) where T : Renderer
    {
        Bounds result = new Bounds (obj.transform.localPosition, Vector3.zero);
        for (int i = 0; i < obj.transform.childCount; i++) {
            T component = obj.transform.GetChild (i).GetComponent<T> ();
            if (component != null) {
                result.Encapsulate (component.bounds);
            }
        }
        return result;
    }

    public static T GetComponentInChildrenSafe<T> (this GameObject obj) where T : Component
    {
        T componentInChildren = obj.GetComponentInChildren<T> ();
        if (componentInChildren == null) {
            Debug.LogError ("Expected to find component of type " + typeof(T) + " but found none", obj);
        }
        return componentInChildren;
    }

    public static T GetComponentSafe<T> (this GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T> ();
        if (component == null) {
            Debug.LogError ("Expected to find component of type " + typeof(T) + " but found none", obj);
        }
        return component;
    }

    public static int GetComponentsInChildrenNonAlloc<T> (this GameObject obj, T[] foundComponents) where T : Component
    {
        int num = 0;
        int num2 = 0;
        while (num2 < obj.transform.childCount && num < foundComponents.Length) {
            T component = obj.transform.GetChild (num2).gameObject.GetComponent<T> ();
            if (component != null) {
                foundComponents [num++] = component;
            }
            num2++;
        }
        return num;
    }

    public static Dictionary<int, GameObject> links = new Dictionary<int, GameObject> ();

    public static void SetLink (this GameObject obj, GameObject linkedGameObject)
    {
        links.Add (obj.GetInstanceID (), linkedGameObject);
    }

    public static GameObject GetLink (this GameObject obj)
    {
        var id = obj.GetInstanceID ();
        return links.ContainsKey (id) ? links [id] : null;
    }
}

public static class ExtensionsInt
{
    //
    // Static Methods
    //
    public static int Abs (this int me)
    {
        return Mathf.Abs (me);
    }

    public static void Clamp (ref int me, int min, int max)
    {
        if (me > max) {
            me = max;
        } else {
            if (me < min) {
                me = min;
            }
        }
    }

    public static int Clamp (this int me, int min, int max)
    {
        ExtensionsInt.Clamp (ref me, min, max);
        return me;
    }

    public static int ClampToCeiling (this int me, int ceiling)
    {
        if (me > ceiling) {
            return ceiling;
        }
        return me;
    }

    public static int ClampToFloor (this int me, int floor)
    {
        if (me < floor) {
            return floor;
        }
        return me;
    }

    public static bool Comparison (this int me, int other, ComparisonType comp)
    {
        switch (comp) {
        case ComparisonType.NOT_EQUAL:
            return me != other;
        case ComparisonType.EQUAL:
            return me == other;
        case ComparisonType.GREATER:
            return me > other;
        case ComparisonType.GREATER_EQUAL:
            return me >= other;
        case ComparisonType.LESS:
            return me < other;
        case ComparisonType.LESS_EQUAL:
            return me <= other;
        default:
            Debug.LogError ("Unknown Comparison " + comp.ToString ());
            return false;
        }
    }

    public static float Lerp (this int me, int target, float prog)
    {
        return (float)me + (float)(target - me) * prog;
    }

    public static int Sign (this int me)
    {
        return (me < 0) ? -1 : 1;
    }

    public static string FormatGroupSeparator (this int number, string separator)
    {
        NumberFormatInfo nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone ();
        nfi.NumberDecimalDigits = 0;
        nfi.NumberGroupSeparator = separator;
        return number.ToString ("n", nfi);
    }
}

public static class ExtensionsList
{
    //
    // Static Methods
    //
    public static int ClampRange<T> (this List<T> array, int index)
    {
        if (index < 0) {
            index = 0;
        } else {
            if (index >= array.Count) {
                index = array.Count - 1;
            }
        }
        return index;
    }

    public static bool InRange<T> (this List<T> array, int index0)
    {
        return index0 >= 0 && index0 < array.Count;
    }

    public static void Swap<T> (List<T> array, int index0, int index1) where T : UnityEngine.Object
    {
        T value = array [index0];
        array [index0] = array [index1];
        array [index1] = value;
    }
}

public static class ExtensionsObject
{
    //
    // Static Methods
    //
    public static bool IsNull<T> (this T obj) where T : class
    {
        return obj == null;
    }

    public static bool NotNull<T> (this T obj) where T : class
    {
        return obj != null;
    }
}

public static class ExtensionsParticleSystem
{
    //
    // Static Methods
    //
    public static void EnableEmission (this ParticleSystem me, bool enabled)
    {
        var emissionModule = me.emission;
        emissionModule.enabled = enabled;

        Transform transform = me.transform;
        for (int i = 0; i < transform.childCount; i++) {
            ParticleSystem component = transform.GetChild (i).GetComponent<ParticleSystem> ();
            if (component) {
                component.EnableEmission (enabled);
            }
        }
    }
}

public static class ExtensionsSprite
{
    //
    // Static Methods
    //
    public static void SetSpecialAlpha (this SpriteRenderer sprite, float a)
    {
        Color color = sprite.color;
        color.a = a;
        sprite.color = color;
    }

    public static void SetSpecialBrightness (this SpriteRenderer sprite, float brightness)
    {
        Color color = sprite.color;
        color.b = brightness;
        sprite.color = color;
    }

    public static void SetSpecialRamp (this SpriteRenderer sprite, float ramp)
    {
        Color color = sprite.color;
        color.r = ramp;
        sprite.color = color;
    }
}

public static class ExtensionsStringBuilder
{
    //
    // Static Methods
    //
    private static void AppendIntRecursive (System.Text.StringBuilder str, int num)
    {
        if (num == 0) {
            return;
        }
        int num2 = num / 10;
        ExtensionsStringBuilder.AppendIntRecursive (str, num2);
        str.Append ((char)(48 + (num - num2 * 10)));
    }

    public static void AppendPositiveInt (this System.Text.StringBuilder me, int num)
    {
        if (num > 0) {
            ExtensionsStringBuilder.AppendIntRecursive (me, num);
        } else {
            me.Append ('0');
        }
    }

    public static void Clear (this System.Text.StringBuilder me)
    {
        me.Length = 0;
    }
}

public static class ExtensionsTransform
{
    //
    // Static Methods
    //
    public static Transform FindChildRecursive (this Transform me, string child)
    {
        for (int i = 0; i < me.childCount; i++) {
            Transform child2 = me.GetChild (i);
            if (child2.name == child) {
                return child2;
            }
            Transform transform = child2.FindChildRecursive (child);
            if (transform != null) {
                return transform;
            }
        }
        return null;
    }

    public static void Clear (this Transform me)
    {
        for (int i = 0; i < me.childCount; i++) {  
            GameObject.Destroy (me.GetChild (i).gameObject);
        }
    }

    public static void SetAbsoluteRotX (this Transform me, float rotX)
    {
        Vector3 eulerAngles = me.localRotation.eulerAngles;
        me.rotation = Quaternion.Euler (rotX, eulerAngles.y, eulerAngles.z);
    }

    public static void SetAbsoluteRotY (this Transform me, float rotY)
    {
        Vector3 eulerAngles = me.localRotation.eulerAngles;
        me.rotation = Quaternion.Euler (eulerAngles.x, rotY, eulerAngles.z);
    }

    public static void SetAbsoluteRotZ (this Transform me, float rotZ)
    {
        Vector3 eulerAngles = me.localRotation.eulerAngles;
        me.rotation = Quaternion.Euler (eulerAngles.x, eulerAngles.y, rotZ);
    }

    public static void SetLocalPos2D (this Transform me, Vector2 pos)
    {
        me.localPosition = new Vector3 (pos.x, pos.y, me.localPosition.z);
    }

    public static void SetLocalPosX (this Transform me, float x)
    {
        me.localPosition = new Vector3 (x, me.localPosition.y, me.localPosition.z);
    }

    public static void SetLocalPosY (this Transform me, float y)
    {
        me.localPosition = new Vector3 (me.localPosition.x, y, me.localPosition.z);
    }

    public static void SetLocalPosZ (this Transform me, float z)
    {
        me.localPosition = new Vector3 (me.localPosition.x, me.localPosition.y, z);
    }

    public static void TranslateX (this Transform me, float x)
    {
        me.Translate (x, 0f, 0f);
//        me.localPosition = new Vector3 (me.localPosition.x + x, me.localPosition.y, me.localPosition.z);
    }

    public static void TranslateY (this Transform me, float y)
    {
        me.Translate (0f, y, 0f);
//        me.localPosition = new Vector3 (me.localPosition.x, me.localPosition.y + y, me.localPosition.z);
    }

    public static void TranslateZ (this Transform me, float z)
    {
        me.Translate (0f, 0f, z);
//        me.localPosition = new Vector3 (me.localPosition.x, me.localPosition.y, me.localPosition.z + z);
    }

    public static void SetLocalRotX (this Transform me, float rotX)
    {
        Vector3 eulerAngles = me.localRotation.eulerAngles;
        me.localRotation = Quaternion.Euler (rotX, eulerAngles.y, eulerAngles.z);
    }

    public static void SetLocalRotY (this Transform me, float rotY)
    {
        Vector3 eulerAngles = me.localRotation.eulerAngles;
        me.localRotation = Quaternion.Euler (eulerAngles.x, rotY, eulerAngles.z);
    }

    public static void SetLocalRotZ (this Transform me, float rotZ)
    {
        Vector3 eulerAngles = me.localRotation.eulerAngles;
        me.localRotation = Quaternion.Euler (eulerAngles.x, eulerAngles.y, rotZ);
    }

    public static void SetLocalScale (this Transform me, float allscale)
    {
        me.localScale = new Vector3 (allscale, allscale, allscale);
    }

    public static void SetLocalScale2D (this Transform me, Vector2 scale)
    {
        me.localScale = new Vector3 (scale.x, scale.y, me.localScale.z);
    }

    public static void SetLocalScaleX (this Transform me, float scaleX)
    {
        me.localScale = new Vector3 (scaleX, me.localScale.y, me.localScale.z);
    }

    public static void SetLocalScaleY (this Transform me, float scaleY)
    {
        me.localScale = new Vector3 (me.localScale.x, scaleY, me.localScale.z);
    }
}

public static class ExtensionsVector
{
    //
    // Static Methods
    //
    public static Vector2 Lerp (this Vector2 me, Vector2 target, float progress)
    {
        return me + (target - me) * progress;
    }

    //    public static Vector2 LerpHalfCos (this Vector2 me, Vector2 target, float progress)
    //    {
    //        return me + (target - me) * 0.LerpHalfCos (1, progress);
    //    }

    public static float ToDegrees (this Vector2 me)
    {
        return Mathf.Atan2 (me.y, me.x).ToDegrees ();
    }

    public static Vector2 ToVector2 (this Vector3 me)
    {
        return new Vector2 (me.x, me.y);
    }

    public static Vector3 ToVector3 (this Vector2 me)
    {
        return new Vector3 (me.x, me.y, 0);
    }

    public static Vector3 ToVector3 (this Vector2 me, float zVal)
    {
        return new Vector3 (me.x, me.y, zVal);
    }
}
