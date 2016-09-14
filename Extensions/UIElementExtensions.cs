using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public static class ExtensionsImage
{
    public static void SetWebImage (this Image image, string url)
    {
        image.StartCoroutine (SetSpriteByUrl (image, url));
    }

    private static IEnumerator SetSpriteByUrl (Image image, string url)
    {
        var www = new WWW (url);

        yield return www;

        var texture = www.texture;
        image.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 (0.5f, 0.5f));
    }
}
