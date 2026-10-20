// Donkerbos scene tools — one-click real-world scaling for VR.
// Menu: Tools > Donkerbos > ...   All actions support Undo (Cmd+Z).
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DonkerbosSceneTools
{
    const string EnvName = "Environment";
    const string RefName = "HeightReference_1.7m (delete me)";
    const float DefaultScale = 0.07f;

    static bool IsXROrigin(GameObject go) =>
        go.name.Contains("XR Origin") ||
        go.GetComponents<Component>().Any(c => c != null && c.GetType().Name == "XROrigin");

    static GameObject FindXROrigin() =>
        EditorSceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(IsXROrigin);

    [MenuItem("Tools/Donkerbos/1. Group and Scale Environment to Real Size")]
    static void GroupAndScale()
    {
        var scene = EditorSceneManager.GetActiveScene();
        Undo.IncrementCurrentGroup();
        int group = Undo.GetCurrentGroup();
        Undo.SetCurrentGroupName("Scale Environment");

        var env = scene.GetRootGameObjects().FirstOrDefault(g => g.name == EnvName);
        if (env == null)
        {
            env = new GameObject(EnvName);
            Undo.RegisterCreatedObjectUndo(env, "Create Environment");
            env.transform.position = Vector3.zero;
            env.transform.rotation = Quaternion.identity;
            env.transform.localScale = Vector3.one;
        }

        var xr = FindXROrigin();
        Vector3 oldXrPos = xr != null ? xr.transform.position : Vector3.zero;

        int moved = 0;
        foreach (var root in scene.GetRootGameObjects())
        {
            if (root == env || IsXROrigin(root) || root.name == RefName) continue;
            if (root.GetComponent<Light>() != null && root.GetComponent<MeshRenderer>() == null) continue;
            Undo.SetTransformParent(root.transform, env.transform, "Parent to Environment");
            moved++;
        }

        Undo.RecordObject(env.transform, "Scale Environment");
        env.transform.localScale = Vector3.one * DefaultScale;

        if (xr != null)
        {
            Undo.RecordObject(xr.transform, "Move XR Origin");
            Vector3 p = oldXrPos * DefaultScale;
            Physics.SyncTransforms();
            if (Physics.Raycast(p + Vector3.up * 50f, Vector3.down, out var hit, 200f))
                p.y = hit.point.y;
            xr.transform.position = p;
            Selection.activeGameObject = xr;
        }

        Undo.CollapseUndoOperations(group);
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log($"[Donkerbos] Grouped {moved} objects under '{EnvName}' and scaled to {DefaultScale}. " +
                  "Check a door next to the 1.7 m reference (Tools > Donkerbos > 2). Adjust Environment scale if needed.");
    }

    [MenuItem("Tools/Donkerbos/2. Add 1.7m Person Reference at XR Origin")]
    static void AddReference()
    {
        var xr = FindXROrigin();
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = RefName;
        Undo.RegisterCreatedObjectUndo(cube, "Add Height Reference");
        Vector3 basePos = xr != null ? xr.transform.position : Vector3.zero;
        cube.transform.localScale = new Vector3(0.5f, 1.7f, 0.3f);
        cube.transform.position = basePos + new Vector3(1f, 0.85f, 0f);
        Object.DestroyImmediate(cube.GetComponent<Collider>());
        Selection.activeGameObject = cube;
        SceneView.FrameLastActiveSceneView();
    }
}
