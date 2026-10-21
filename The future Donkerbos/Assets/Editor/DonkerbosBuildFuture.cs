using UnityEditor;
using UnityEngine;

public static class DonkerbosBuildFuture
{
    [MenuItem("Donkerbos/Build Future Scene")]
    public static void Build()
    {
        const string root = "Assets/Donkerbos_Future";
        var parent = new GameObject("Future_SkillsCentre");

        var env = AssetDatabase.LoadAssetAtPath<GameObject>(root + "/SkillsCentre_Environment.fbx");
        if (env != null)
        {
            var e = (GameObject)PrefabUtility.InstantiatePrefab(env);
            e.transform.SetParent(parent.transform, false);
        }
        else Debug.LogWarning("SkillsCentre_Environment.fbx not found in " + root);

        int ok = 0, bad = 0;
        foreach (var guid in AssetDatabase.FindAssets("t:Model", new[] { root + "/Characters" }))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var imp = AssetImporter.GetAtPath(path) as ModelImporter;
            if (imp != null && imp.animationType != ModelImporterAnimationType.Human)
            {
                imp.animationType = ModelImporterAnimationType.Human;
                imp.SaveAndReimport();
            }
            foreach (var o in AssetDatabase.LoadAllAssetsAtPath(path))
                if (o is Avatar a) { if (a.isValid) ok++; else { bad++; Debug.LogWarning("Humanoid avatar invalid: " + path); } }

            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var inst = (GameObject)PrefabUtility.InstantiatePrefab(go);
            inst.transform.SetParent(parent.transform, false);
        }
        Selection.activeGameObject = parent;
        Debug.Log("Donkerbos: built Future scene. Humanoid avatars valid: " + ok + ", invalid: " + bad);
    }

    [MenuItem("Donkerbos/Apply Late-Afternoon Light")]
    public static void ApplyLight()
    {
        Light sun = RenderSettings.sun;
        if (sun == null) foreach (var l in Object.FindObjectsByType<Light>(FindObjectsSortMode.None)) if (l.type == LightType.Directional) { sun = l; break; }
        if (sun == null) { var g = new GameObject("Sun"); sun = g.AddComponent<Light>(); sun.type = LightType.Directional; }
        sun.transform.rotation = Quaternion.Euler(24f, 235f, 0f);
        sun.color = new Color(1f, 0.86f, 0.68f);
        sun.intensity = 1.5f;
        sun.shadows = LightShadows.Soft;
        sun.shadowStrength = 0.9f;
        RenderSettings.sun = sun;
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
        RenderSettings.ambientSkyColor = new Color(0.50f, 0.58f, 0.70f);
        RenderSettings.ambientEquatorColor = new Color(0.45f, 0.38f, 0.31f);
        RenderSettings.ambientGroundColor = new Color(0.22f, 0.17f, 0.13f);
        Debug.Log("Donkerbos: late-afternoon light applied (low warm sun, long hard shadows, dim ambient).");
    }
}
