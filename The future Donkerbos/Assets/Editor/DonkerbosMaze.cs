// Places the health-office maze (The present/Wayfinder/Maze_HealthOffice) into the open scene,
// textured with the dorm's own textures so both feel like one building.
// Menu: Donkerbos > Place Health Office Maze. Supports Undo (Cmd+Z).
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DonkerbosMaze
{
    const string Fbx = "Assets/The present/Wayfinder/Maze_HealthOffice/Maze_HealthOffice.fbx";
    const string MatDir = "Assets/The present/Wayfinder/Maze_HealthOffice/Materials";
    const string Tex = "Assets/The present/Setting/abandoned_room_interior_style_5/textures/";

    // material name in the model -> dorm texture, tiling
    static readonly Dictionary<string, string> Map = new Dictionary<string, string> {
        { "Maze_WallUpper", "WallPaint01_baseColor.jpeg" },
        { "Maze_WallLower", "WallPaint02_baseColor.jpeg" },
        { "Maze_Floor",     "Floor_baseColor.jpeg" },
        { "Maze_Ceiling",   "Ceiling_baseColor.jpeg" },
    };

    static Material MakeMat(string name, string texFile)
    {
        Directory.CreateDirectory(MatDir);
        string path = MatDir + "/" + name + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            var sh = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            m = new Material(sh);
            AssetDatabase.CreateAsset(m, path);
        }
        var t = AssetDatabase.LoadAssetAtPath<Texture2D>(Tex + texFile);
        if (t != null)
        {
            if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", t);
            if (m.HasProperty("_MainTex")) m.SetTexture("_MainTex", t);
        }
        if (m.HasProperty("_Smoothness")) m.SetFloat("_Smoothness", 0.15f);
        EditorUtility.SetDirty(m);
        return m;
    }

    [MenuItem("Donkerbos/Place Health Office Maze")]
    static void Place()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(Fbx);
        if (model == null) { EditorUtility.DisplayDialog("Maze", "Could not find " + Fbx, "OK"); return; }
        var mats = new Dictionary<string, Material>();
        foreach (var kv in Map) mats[kv.Key] = MakeMat(kv.Key, kv.Value);
        AssetDatabase.SaveAssets();

        var scene = EditorSceneManager.GetActiveScene();
        var maze = (GameObject)PrefabUtility.InstantiatePrefab(model, scene);
        maze.name = "Maze_HealthOffice";
        Undo.RegisterCreatedObjectUndo(maze, "Place Maze");
        maze.transform.position = Vector3.zero;
        maze.transform.localScale = Vector3.one;

        foreach (var r in maze.GetComponentsInChildren<MeshRenderer>())
        {
            var list = r.sharedMaterials;
            for (int i = 0; i < list.Length; i++)
            {
                if (list[i] == null) continue;
                string n = list[i].name.Replace(" (Instance)", "");
                if (mats.TryGetValue(n, out var m)) list[i] = m;
            }
            r.sharedMaterials = list;
            var go = r.gameObject;
            if (go.name.Contains("Walls") || go.name.Contains("Floor"))
            {
                if (go.GetComponent<MeshCollider>() == null) Undo.AddComponent<MeshCollider>(go);
            }
            else if (go.name.Contains("NurseDoor") && go.GetComponent<BoxCollider>() == null)
            {
                Undo.AddComponent<BoxCollider>(go);
            }
        }
        Selection.activeGameObject = maze;
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Maze placed at the scene origin. The entrance is the only gap in the outer wall (the old S block). Move the whole Maze_HealthOffice object so the entrance meets the dorm doorway.");
    }
}
