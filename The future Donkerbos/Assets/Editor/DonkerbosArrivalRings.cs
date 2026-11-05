using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
using System.IO;

// Tools > Donkerbos > Add Room Arrival Rings
// Puts a soft, slowly pulsing amber ring on the floor just inside each of the six rooms,
// the same visual language as the key glow, so arriving in a room reads as "you made it".
// Purely visual: no triggers, no scripts changed.
public static class DonkerbosArrivalRings
{
    struct Spot { public string room; public Vector3 local; public float size;
        public Spot(string r, Vector3 l, float s) { room = r; local = l; size = s; } }

    // Positions are in SkillsCentre_Environment local space, picked clear of furniture.
    static readonly Spot[] Spots = {
        new Spot("Clinic", new Vector3(-5.832f, 0.150f, -9.566f), 1.3f),
        new Spot("Leather_Wood_Workshop", new Vector3(-14.317f, 0.150f, -2.525f), 1.3f),
        new Spot("Metal_Workshop", new Vector3(-9.129f, 0.150f, 6.264f), 1.3f),
        new Spot("ICT_Hub", new Vector3(8.348f, 0.150f, 7.177f), 1.0f),
        new Spot("Library", new Vector3(14.298f, 0.150f, -2.521f), 1.3f),
        new Spot("Agriculture_Shelter", new Vector3(5.831f, 0.150f, -9.564f), 1.3f),
    };

    const string Folder = "Assets/Donkerbos_Future/Glow";
    const string TexPath = Folder + "/ArrivalRing_Soft.png";
    const string MatPath = Folder + "/ArrivalRing.mat";

    [MenuItem("Tools/Donkerbos/Add Room Arrival Rings")]
    static void Add()
    {
        GameObject env = GameObject.Find("SkillsCentre_Environment");
        if (env == null) { EditorUtility.DisplayDialog("Arrival Rings", "Open the Future scene first (couldn't find SkillsCentre_Environment).", "OK"); return; }
        Material mat = GetOrMakeMaterial();
        GameObject parent = GameObject.Find("Room Arrival Rings");
        if (parent != null) Undo.DestroyObjectImmediate(parent);
        parent = new GameObject("Room Arrival Rings");
        Undo.RegisterCreatedObjectUndo(parent, "Add Room Arrival Rings");

        foreach (var s in Spots)
        {
            GameObject q = GameObject.CreatePrimitive(PrimitiveType.Quad);
            q.name = "Arrival Ring - " + s.room;
            Object.DestroyImmediate(q.GetComponent<Collider>());
            var r = q.GetComponent<MeshRenderer>();
            r.sharedMaterial = mat; r.shadowCastingMode = ShadowCastingMode.Off; r.receiveShadows = false;
            q.transform.SetParent(parent.transform, false);
            q.transform.position = env.transform.TransformPoint(s.local) + Vector3.up * 0.012f;
            q.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            q.transform.localScale = new Vector3(s.size, s.size, 1f);
            var g = q.AddComponent<GlowMarker>();
            var so = new SerializedObject(g);
            so.FindProperty("minAlpha").floatValue = 0.12f;
            so.FindProperty("maxAlpha").floatValue = 0.35f;
            so.FindProperty("pulseSeconds").floatValue = 3.2f;
            so.ApplyModifiedPropertiesWithoutUndo();
        }
        Selection.activeGameObject = parent;
        EditorSceneManager.MarkSceneDirty(parent.scene);
        Debug.Log("Added 6 arrival rings (one inside each room door). Move any of them in the Scene view if needed.");
    }

    static Material GetOrMakeMaterial()
    {
        if (!AssetDatabase.IsValidFolder(Folder)) AssetDatabase.CreateFolder("Assets/Donkerbos_Future", "Glow");
        if (!File.Exists(TexPath))
        {
            int n = 256; var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
            for (int y = 0; y < n; y++) for (int x = 0; x < n; x++)
            {
                float dx = (x + 0.5f) / n * 2f - 1f, dy = (y + 0.5f) / n * 2f - 1f;
                float d = Mathf.Sqrt(dx * dx + dy * dy);
                float ring  = Mathf.Exp(-Mathf.Pow((d - 0.80f) / 0.06f, 2f));          // main ring
                float inner = Mathf.Exp(-Mathf.Pow((d - 0.62f) / 0.03f, 2f)) * 0.35f;  // thin inner line
                float wash  = Mathf.Clamp01(1f - d) * 0.18f;                           // faint fill
                float a = Mathf.Clamp01(ring + inner + wash) * Mathf.Clamp01((1f - d) * 8f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
            File.WriteAllBytes(TexPath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(TexPath);
            var imp = (TextureImporter)AssetImporter.GetAtPath(TexPath);
            imp.alphaIsTransparency = true; imp.wrapMode = TextureWrapMode.Clamp; imp.mipmapEnabled = false; imp.SaveAndReimport();
        }
        var mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        if (mat == null) { mat = new Material(Shader.Find("Universal Render Pipeline/Unlit")); AssetDatabase.CreateAsset(mat, MatPath); }
        mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(TexPath));
        mat.SetColor("_BaseColor", new Color(1f, 0.78f, 0.35f, 0.3f));
        mat.SetFloat("_Surface", 1f); mat.SetFloat("_Blend", 2f);
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha); mat.SetFloat("_DstBlend", (float)BlendMode.One);
        mat.SetFloat("_ZWrite", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = (int)RenderQueue.Transparent;
        EditorUtility.SetDirty(mat); AssetDatabase.SaveAssets();
        return mat;
    }
}
