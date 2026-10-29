using UnityEditor;
using UnityEngine;

// The Future characters were exported with their position inside the Future hub baked in, so when you
// drag one into another scene the body stands several metres away from the object you dropped.
// Select the character in the Hierarchy and run Donkerbos > Stand Selected Character On Its Pivot:
// it moves the character so its feet sit exactly where the object's own position is.
// Then drag it (W tool) to wherever you want it. Nothing else in the project is touched.
public static class DonkerbosCharacterTools
{
    [MenuItem("Donkerbos/Stand Selected Character On Its Pivot")]
    public static void StandOnPivot()
    {
        int done = 0;
        foreach (var go in Selection.gameObjects)
        {
            var rs = go.GetComponentsInChildren<Renderer>();
            if (rs.Length == 0) { Debug.LogWarning("Donkerbos: " + go.name + " has no visible parts."); continue; }
            Bounds b = rs[0].bounds;
            foreach (var r in rs) b.Encapsulate(r.bounds);
            Vector3 feet = new Vector3(b.center.x, b.min.y, b.center.z);
            Undo.RecordObject(go.transform, "Stand character on pivot");
            go.transform.position += go.transform.position - feet;
            done++;
        }
        Debug.Log("Donkerbos: moved " + done + " character(s) onto their pivot.");
    }
}
