using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GhostTrail : MonoBehaviour {
	public Material ghostMaterial;
	public string colorLabelName;
	public Color color = new Color(255, 255, 255, 100);

	public float maxTrails;
	public float spawnRate;
	public float lifeTime;
	public float fadeTime;
	public bool renderOnMotion;

	private MeshFilter[] meshFilters = null;  
	private MeshRenderer[] meshRenderers = null;  
	private SkinnedMeshRenderer[] skinnedMeshRenderers = null;  

	public bool autoSpawn;
	private float spawnInterval;
	private float lastSpawnTime;

	private Vector3 lastFramePosition;

	public class GhostTrailSettings  
	{
		public GameObject go;  
		public float fadeTime;  
		public float lifeTime;  

		public List<Material> materials = new List<Material> ();

		public GhostTrailSettings(GameObject go, Material material, float lifeTime, float fadeTime)  
		{  
			this.go = go;  
			this.lifeTime = lifeTime;  
			this.fadeTime = fadeTime;  

			foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>())  
			{  
				if (renderer as MeshRenderer || renderer as SkinnedMeshRenderer)  
				{  
					renderer.sharedMaterial = material;
					materials.Add(renderer.material);
				}  
				else  
				{
					renderer.enabled = false;  
				}  
			}
		}
	}

	private float updateInterval = 0.05f;
	private float lastUpdateTime;

	private List<GhostTrailSettings> trails;

	public void SetTrail() {
		SetTrail (lifeTime, fadeTime);
	}

	public void SetTrail(float newlifeTime) {
		SetTrail (newlifeTime, fadeTime);
	}

	public void SetTrail(float newlifeTime, float newfadeTime) {
		for (int i = 0; skinnedMeshRenderers != null && i < skinnedMeshRenderers.Length; ++i) {  
			Mesh mesh = new Mesh ();  
			skinnedMeshRenderers [i].BakeMesh (mesh);  

			GameObject go = new GameObject ();  
			//				go.hideFlags = HideFlags.HideAndDontSave;
			go.name = gameObject.name + " - GhostTrail";
			go.transform.position = skinnedMeshRenderers [i].transform.position;
			go.transform.rotation = skinnedMeshRenderers [i].transform.rotation;
			go.transform.localScale = skinnedMeshRenderers [i].transform.localScale;

			MeshFilter meshFilter = go.AddComponent<MeshFilter> ();  
			meshFilter.mesh = mesh;  

			MeshRenderer meshRenderer = go.AddComponent<MeshRenderer> ();  

			trails.Add (new GhostTrailSettings(go, ghostMaterial, newlifeTime, newfadeTime));
		}  

		for (int i = 0; meshRenderers != null && i < meshRenderers.Length; ++i) {  
			GameObject go = new GameObject ();  
			//				go.hideFlags = HideFlags.HideAndDontSave; 
			go.name = gameObject.name + " - GhostTrail";
			go.transform.position = meshRenderers [i].transform.position;
			go.transform.rotation = meshRenderers [i].transform.rotation;
			go.transform.localScale = meshRenderers [i].transform.localScale;

			MeshFilter meshFilter = go.AddComponent<MeshFilter> ();  
			meshFilter.mesh = meshRenderers[i].GetComponent<MeshFilter>().mesh;  

			MeshRenderer meshRenderer = go.AddComponent<MeshRenderer> ();

			trails.Add (new GhostTrailSettings(go, ghostMaterial, newlifeTime, newfadeTime));
		}
	}

	// Use this for initialization
	void OnEnable () {
		meshFilters = this.gameObject.GetComponentsInChildren<MeshFilter> ();  
		meshRenderers = this.gameObject.GetComponentsInChildren<MeshRenderer> ();
		skinnedMeshRenderers = this.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
		spawnInterval = 1 / spawnRate;

		for (int i = 0; i < meshRenderers.Length; i++) {
			meshRenderers[i].material.renderQueue += 1;
		}

		for (int i = 0; i < skinnedMeshRenderers.Length; i++) {
			skinnedMeshRenderers[i].material.renderQueue += 1;
		}

		trails = new List<GhostTrailSettings> ();

		ghostMaterial.SetColor(colorLabelName, color);  
	}

	// Update is called once per frame
	void Update () {
		if (Time.time - lastUpdateTime > updateInterval) {
			for (int i = trails.Count - 1; i >= 0; --i)  
			{  
				trails[i].lifeTime -= (Time.time - lastUpdateTime);  

				if (trails[i].lifeTime <= 0)  
				{  
					GameObject.Destroy(trails[i].go);
					trails.RemoveAt(i);
					continue;  
				}  

				if (trails[i].lifeTime < trails[i].fadeTime && !string.IsNullOrEmpty(colorLabelName))  
				{  
					float alpha = trails[i].lifeTime / trails[i].fadeTime;  

					foreach (Material material in trails[i].materials)  
					{  
						if (material.HasProperty(colorLabelName))  
						{  
							Color color = material.GetColor(colorLabelName);  
							color.a = alpha;
							color.r = color.r;
							color.g = color.g;
							color.b = color.b;
							material.SetColor(colorLabelName, color);  
						}  
					}  
				}               
			}

			lastUpdateTime = Time.time;
		}

		if (autoSpawn && Time.time - lastSpawnTime > spawnInterval && trails.Count < maxTrails * (meshFilters.Length + skinnedMeshRenderers.Length)) {
			if (renderOnMotion && this.transform.position == lastFramePosition) {
				return;
			}

			SetTrail ();

			lastSpawnTime = Time.time;
		}

		lastFramePosition = this.transform.position;
	}
}
