// Reconnects Mesh Colliders that lost their mesh (e.g. after a model was re-imported),
// so the player stops falling through the floor.
// Menu: Donkerbos > Fix Missing Mesh Colliders. Supports Undo (Cmd+Z).
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DonkerbosFixColliders
{
    [MenuItem("Donkerbos/Fix Missing Mesh Colliders")]
    static void Fix()
    {
        int fixedCount = 0, total = 0;
        foreach (var mc in Object.FindObjectsByType<MeshCollider>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            total++;
            var mf = mc.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null) continue;
            if (mc.sharedMesh != mf.sharedMesh)
            {
                Undo.RecordObject(mc, "Fix Mesh Collider");
                mc.sharedMesh = null;          // force Unity to re-cook it
                mc.sharedMesh = mf.sharedMesh;
                EditorUtility.SetDirty(mc);
                fixedCount++;
                Debug.Log("Reconnected collider on " + mc.gameObject.name, mc.gameObject);
            }
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("Mesh Colliders", "Checked " + total + " Mesh Colliders, reconnected " + fixedCount + ".\nSave the scene (Cmd+S) to keep the fix.", "OK");
    }
}
