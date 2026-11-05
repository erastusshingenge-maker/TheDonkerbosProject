using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;
using System.IO;

// Tools > Donkerbos > Add Key Glow Disk
// Creates a soft glowing disk on the floor in front of the key station, pulsing gently,
// which fades away once the key is picked up.
public static class DonkerbosGlowTools
{
    const string Folder = "Assets/Donkerbos_Future/Glow";
    const string TexPath = Folder + "/GlowDisk_Soft.png";
    const string MatPath = Folder + "/GlowDisk.mat";

    [MenuItem("Tools/Donkerbos/Add Key Glow Disk")]
    static void AddKeyGlow()
    {
        GameObject key = GameObject.Find("RoomKey");
        if (key == null) { EditorUtility.DisplayDialog("Key Glow", "Couldn't find RoomKey in this scene. Open the Future scene first.", "OK"); return; }
        Material mat = GetOrMakeMaterial();

        GameObject old = GameObject.Find("Key Glow Disk");
        if (old != null) Undo.DestroyObjectImmediate(old);

        GameObject disk = GameObject.CreatePrimitive(PrimitiveType.Quad);
        disk.name = "Key Glow Disk";
        Object.DestroyImmediate(disk.GetComponent<Collider>());
        var r = disk.GetComponent<MeshRenderer>();
        r.sharedMaterial = mat;
        r.shadowCastingMode = ShadowCastingMode.Off;
        r.receiveShadows = false;

        // Find the floor straight below the key; stand the disk just in front of the board.
        // "In front" = towards where the player starts (the camera rig).
        GameObject rig = GameObject.Find("[BuildingBlock] Camera Rig");
        Vector3 forward = rig != null ? rig.transform.position - key.transform.position : Vector3.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.001f) forward = Vector3.forward;
        forward.Normalize();
        Vector3 start = key.transform.position + forward * 0.45f;
        float floorY = key.transform.position.y - 1.1f;
        foreach (var hit in Physics.RaycastAll(start + Vector3.up * 0.5f, Vector3.down, 4f))
        {
            if (hit.collider.transform.IsChildOf(key.transform)) continue;
            floorY = hit.point.y; break;
        }
        disk.transform.position = new Vector3(start.x, floorY + 0.01f, start.z);
        disk.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        disk.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

        var glow = disk.AddComponent<GlowMarker>();
        var so = new SerializedObject(glow);
        so.FindProperty("watchTarget").objectReferenceValue = key.transform;
        so.ApplyModifiedPropertiesWithoutUndo();

        Undo.RegisterCreatedObjectUndo(disk, "Add Key Glow Disk");
        Selection.activeGameObject = disk;
        EditorSceneManager.MarkSceneDirty(disk.scene);
        Debug.Log("Key Glow Disk added in front of the key station. Move it in the Scene view if you want it elsewhere.");
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
                float core = Mathf.Clamp01(1f - d); core = core * core;                 // soft centre
                float ring = Mathf.Exp(-Mathf.Pow((d - 0.78f) / 0.07f, 2f)) * 0.8f;    // faint rim
                float a = Mathf.Clamp01(core * 0.6f + ring) * Mathf.Clamp01((1f - d) * 6f);
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }
            File.WriteAllBytes(TexPath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(TexPath);
            var imp = (TextureImporter)AssetImporter.GetAtPath(TexPath);
            imp.alphaIsTransparency = true; imp.wrapMode = TextureWrapMode.Clamp; imp.mipmapEnabled = false; imp.SaveAndReimport();
        }
        var mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        if (mat == null)
        {
            Shader sh = Shader.Find("Universal Render Pipeline/Unlit");
            mat = new Material(sh);
            AssetDatabase.CreateAsset(mat, MatPath);
        }
        mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(TexPath));
        mat.SetColor("_BaseColor", new Color(1f, 0.78f, 0.35f, 0.25f));
        mat.SetFloat("_Surface", 1f);           // transparent
        mat.SetFloat("_Blend", 2f);             // additive
        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)BlendMode.One);
        mat.SetFloat("_ZWrite", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.renderQueue = (int)RenderQueue.Transparent;
        EditorUtility.SetDirty(mat); AssetDatabase.SaveAssets();
        return mat;
    }
}
