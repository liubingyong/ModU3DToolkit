using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GhostTrail : MonoBehaviour {

    #region public members

    [Range(2, 200)]
    [Tooltip("Number of Ghost frames in the object's trail")]
    public int trailSize = 20;

    [Tooltip("Spacing increments on a frame basis - n spacing setting results in waiting n frames before the next ghost will be set.")]
    [Range(0, 20)]
    public int spacing = 0;

    [Tooltip("Pick a color for the trail.  The Aplha of this color will be used to determine the transparancy fluctuation.")]
    public Color color = new Color(255, 255, 255, 100);

    [Tooltip("If the Ghost Material is not set, then the material from the GameObject will be used for the ghosts.")]
    [ContextMenuItem("Clear List", "DeleteMaterials")]
    public List<Material> ghostMaterial;

    [Tooltip("If checked, and a RigidBody is attached to this gameobject, then ghosts will only render when this gameobject is in motion.")]
    public bool renderOnMotion;

    [Tooltip("Set this to true to use the Aplha Fluctuation Override value.  See tooltip for Alpha Fluctuation Override.")]
    public bool colorAlphaOverride;

    [Tooltip("If the Color Alpha Override bool is set to true, this value (up to 255) will be used instead of the automagically set alpha fluctuation for the ghosts.")]
    [Range(0, 1)]
    public float alphaFluctuationOverride;

    [Range(0, 250)]
    public int alphaFluctuationDivisor;

    public Renderer targetRenderer;

    public Vector3 trailOffset;
    #endregion

    #region private members

    private int spacingCounter = 0;
    private List<Vector3> ghostPositions = new List<Vector3>();
    private List<GameObject> ghostList = new List<GameObject>();
    //private bool hasRigidBody;
    
    private float alpha;
    private bool killSwitch;
    private Vector3 lastFramePosition;

    #endregion

    // Use this for initialization
    void Start () {
        if (targetRenderer == null)
        {
            targetRenderer = gameObject.GetComponent<Renderer>();
            targetRenderer.material.renderQueue += 1;
        }

        ghostMaterial = TruncateMaterialList(ghostMaterial);

        if (ghostMaterial.Count == 0)
            ghostMaterial.Add(targetRenderer.sharedMaterial);

        //Vector3 position = gameObject.transform.position + trailOffset;

        //for (int i = 0; i < trailSize; i++)
        //    Populate(position, true);

        alpha = color.a;

        spacingCounter = spacing;

        if (spacing < 0)
            spacing = 0;

        //hasRigidBody = this.gameObject.GetComponent<Rigidbody>() != null ? true : false;

        ghostMaterial.Reverse();
    }

    void OnDisable()
    {
        ClearTrail();
    }

    // Update is called once per frame
    void Update () {
        if (killSwitch)
        {
            return;
        }

        if (ghostMaterial.Count == 0)
            ghostMaterial.Add(targetRenderer.sharedMaterial);

        if (trailSize < ghostList.Count)
        {
            for (int i = trailSize; i < ghostList.Count - 1; i++)
            {
                GameObject gone = ghostList[i];
                GameObject.Destroy(gone);
                ghostList.RemoveAt(i);
            }

        }
        
        if (spacingCounter < spacing)
        {
            spacingCounter++;
            return;
        }
        Vector3 position = gameObject.transform.position + trailOffset;

        if (ghostList.Count < trailSize)
        {

            Populate(position, false);
        }
        else
        {
            GameObject gone = ghostList[0];
            ghostList.RemoveAt(0);
            GameObject.Destroy(gone);
            Populate(position, false);
        }
        float temp;
        if (colorAlphaOverride)
            temp = alphaFluctuationOverride;
        else
        {
            temp = alpha;
        }
        int materialDivisor = (ghostList.Count - 1) / ghostMaterial.Count + 1;
        for (int i = ghostList.Count - 1; i >= 0; i--)
        {
            temp -= colorAlphaOverride && alphaFluctuationDivisor != 0 ?
                alphaFluctuationOverride / alphaFluctuationDivisor : alpha / ghostList.Count;
            color.a = temp;
            
            var renderer = ghostList[i].GetComponent<Renderer>();
            int subMat = (int)Mathf.Floor(i / materialDivisor);
            renderer.material = subMat <= 0 ? ghostMaterial[0] : ghostMaterial[subMat];
            renderer.material.SetColor("_Color", color);
        }
        spacingCounter = 0;

        lastFramePosition = this.transform.position;
    }

    #region public methods

    public void ClearTrail()
    {
        foreach (var s in ghostList)
            GameObject.Destroy(s);
        ghostList.Clear();
        ghostPositions.Clear();
    }

    private void KillSwitchEngage()
    {
        killSwitch = true;
        foreach (GameObject g in ghostList)
            GameObject.Destroy(g);
    }

    void OnDestroy()
    {
        killSwitch = true;
        KillSwitchEngage();
    }

    public void AddToMaterialList(Material material)
    {
        ghostMaterial.Add(material);
    }

#if UNITY_EDITOR
    public void RestoreDefaults()
    {
        Undo.RecordObject(gameObject.GetComponent<GhostTrail>(), "Restore Defaults");
        ghostMaterial.Clear();
        trailSize = 20;
        color = new Color(255, 255, 255, 50);
        spacing = 0;
        renderOnMotion = false;
        colorAlphaOverride = false;
        alphaFluctuationOverride = 0;
        alphaFluctuationDivisor = 1;

    }
#endif
    #endregion

    private void Populate(Vector3 position, bool allowPositionOverride)
    {
        if (renderOnMotion
           //&& hasRigidBody
           //&& gameObject.GetComponent<Rigidbody>().velocity == Vector3.zero
           && this.transform.position == lastFramePosition
           && !allowPositionOverride)
        {
            if (ghostList.Count > 0)
            {
                GameObject gone = ghostList[0];
                ghostList.RemoveAt(0);
                GameObject.Destroy(gone);
            }
            return;
        }

        SkinnedMeshRenderer convertedSkinnedRenderer;
        MeshRenderer convertedMeshRenderer;
        
        if ((convertedSkinnedRenderer = targetRenderer as SkinnedMeshRenderer) != null)
        {
            GameObject g = new GameObject();
            g.name = gameObject.name + " - GhostTrail";
            g.AddComponent<MeshFilter>();
            g.AddComponent<MeshRenderer>();
            g.transform.position = position;
            g.transform.localScale = targetRenderer.transform.localScale;
            g.transform.rotation = targetRenderer.transform.rotation;

            var mesh = new Mesh();
            convertedSkinnedRenderer.BakeMesh(mesh);
            g.GetComponent<MeshFilter>().mesh = mesh;
            g.GetComponent<MeshRenderer>().sharedMaterial = ghostMaterial[0];
            g.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", color);

            ghostList.Add(g);
        }
        else if ((convertedMeshRenderer = targetRenderer as MeshRenderer) != null)
        {
            GameObject g = new GameObject();
            g.name = gameObject.name + " - GhostTrail";
            g.AddComponent<MeshFilter>();
            g.AddComponent<MeshRenderer>();
            g.transform.position = position;
            g.transform.localScale = targetRenderer.transform.localScale;
            g.transform.rotation = targetRenderer.transform.rotation;
            g.GetComponent<MeshFilter>().mesh = convertedMeshRenderer.GetComponent<MeshFilter>().mesh;
            g.GetComponent<MeshRenderer>().sharedMaterial = ghostMaterial[0];
            g.GetComponent<MeshRenderer>().sharedMaterial.SetColor("_Color", color);

            ghostList.Add(g);
        }
    }

    private List<Material> TruncateMaterialList(List<Material> materialList)
    {
        List<Material> tempList = new List<Material>();
        foreach (Material material in materialList)
        {
            if (material)
                tempList.Add(material);
        }
        return tempList;
    }

    private void DeleteMaterials()
    {
        ghostMaterial.Clear();
    }
}
