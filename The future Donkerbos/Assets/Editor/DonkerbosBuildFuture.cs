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
}
