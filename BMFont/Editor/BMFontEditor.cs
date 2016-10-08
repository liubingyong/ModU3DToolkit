using UnityEngine;
using UnityEditor;
using System.IO;

public class BMFontEditor : EditorWindow
{
    [MenuItem("ModU3DToolkit/Open/BMFont Maker")]
    static public void OpenBMFontMaker()
    {
        EditorWindow.GetWindow<BMFontEditor>(false, "BMFont Maker", true).Show();
    }

    [MenuItem("Assets/Create/Bitmap Font")]
    public static void GenerateBitmapFont()
    {
        Object[] textAssets = Selection.GetFiltered(typeof(TextAsset), SelectionMode.DeepAssets);
        Object[] textures = Selection.GetFiltered(typeof(Texture2D), SelectionMode.DeepAssets);

        if (textAssets.Length < 1)
        {
            Debug.LogError("BitmapFont Create Error -- Fnt Data File is not Selected.");
            return;
        }
        if (textures.Length < 1)
        {
            Debug.LogError("BitmapFont Create Error -- Texture File is not selected.");
            return;
        }

        string rootPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(textAssets[0]));
        string fullFileNameWithoutExtension = rootPath + "/" + Path.GetFileNameWithoutExtension(textAssets[0].name);

        var material = new Material(Shader.Find("UI/Default"));
        SaveAsset(material, fullFileNameWithoutExtension + ".mat");
        var font = new Font(textAssets[0].name);
        SaveAsset(font, fullFileNameWithoutExtension + ".fontsettings");

        Generate((TextAsset)textAssets[0], (Texture2D)textures[0], font, material);
    }

    [SerializeField]
    private Font targetFont;

    [SerializeField]
    private TextAsset fntData;

    [SerializeField]
    private Material fontMaterial;

    [SerializeField]
    private Texture2D fontTexture;

    private static BMFontEditor instance;

    public BMFontEditor()
    {
        instance = this;
    }

    void OnGUI()
    {
        fntData = EditorGUILayout.ObjectField("Fnt Data", fntData, typeof(TextAsset), false) as TextAsset;
        fontTexture = EditorGUILayout.ObjectField("Font Texture", fontTexture, typeof(Texture2D), false) as Texture2D;
        targetFont = EditorGUILayout.ObjectField("Target Font", targetFont, typeof(Font), false) as Font;
        fontMaterial = EditorGUILayout.ObjectField("Font Material", fontMaterial, typeof(Material), false) as Material;
        
        if (GUILayout.Button("Create BMFont"))
        {
            Generate(fntData, fontTexture, targetFont, fontMaterial);
        }
    }

    static void Generate(TextAsset fntData, Texture2D fontTexture, Font targetFont, Material fontMaterial)
    {
        BMFont bmFont = new BMFont();

        BMFontReader.Load(bmFont, fntData.name, fntData.bytes);
        CharacterInfo[] characterInfo = new CharacterInfo[bmFont.glyphs.Count];
        for (int i = 0; i < bmFont.glyphs.Count; i++)
        {
            BMGlyph bmInfo = bmFont.glyphs[i];
            CharacterInfo info = new CharacterInfo();
            info.index = bmInfo.index;
            info.uv.x = (float)bmInfo.x / (float)bmFont.texWidth;
            info.uv.y = 1 - (float)bmInfo.y / (float)bmFont.texHeight;
            info.uv.width = (float)bmInfo.width / (float)bmFont.texWidth;
            info.uv.height = -1f * (float)bmInfo.height / (float)bmFont.texHeight;
            info.minX = 0;
            info.minY = 0;
            info.maxX = bmInfo.width;
            info.maxY = -bmInfo.height;
            info.advance = bmInfo.advance;

            characterInfo[i] = info;
        }
        targetFont.characterInfo = characterInfo;
        if (fontMaterial)
        {
            fontMaterial.mainTexture = fontTexture;
        }
        targetFont.material = fontMaterial;
        fontMaterial.shader = Shader.Find("UI/Default");

        Debug.Log("Create Font <" + targetFont.name + "> Success");
        if (instance != null) { 
            instance.Close();
        }
    }

    static void SaveAsset(Object obj, string path)
    {
        var asset = AssetDatabase.LoadMainAssetAtPath(path);
        if (asset != null)
        {
            EditorUtility.CopySerialized(obj, asset);
            AssetDatabase.SaveAssets();
        }
        else
        {
            AssetDatabase.CreateAsset(obj, path);
        }
    }

    static T LoadAsset<T>(string path, T defaultAsset) where T : Object
    {
        T asset = AssetDatabase.LoadMainAssetAtPath(path) as T;
        if (asset == null)
        {
            asset = defaultAsset;
        }
        return asset;
    }
}