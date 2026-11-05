using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

// One-off helper: moves the Future scene's Unity-side objects (NPCs, triggers, props, camera rig)
// by the same amounts the rooms were moved in Blender (rooms pulled in to 70% of their distance
// from the centre; courtyard items scaled to 70%). Undo with Ctrl+Z. Safe to delete afterwards.
public static class DonkerbosCompactFuture
{
    const float S = 0.7f;
    const float Turn = 10f;   // degrees each side of the ring turns away from the entrance
    // Room centres in SkillsCentre_Environment local space (Unity x/z).
    static readonly Vector2[] Rooms = {
        new Vector2(-16.8f,-18.2f), new Vector2(-28.4f,0f), new Vector2(-14.9f,19.1f),
        new Vector2(14.9f,19.1f),   new Vector2(28.4f,0f),  new Vector2(16.8f,-18.2f) };
    static readonly HashSet<string> Skip = new HashSet<string> {
        "Future_SkillsCentre","SkillsCentre_Environment","Directional Light","Global Volume",
        "[BuildingBlock] OVRComprehensiveInteractionRig" };

    [MenuItem("Tools/Donkerbos/Compact Future Layout (match Blender)")]
    static void Run()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.name != "Future scene") { EditorUtility.DisplayDialog("Compact Future Layout", "Open the Future scene first.", "OK"); return; }
        GameObject env = GameObject.Find("SkillsCentre_Environment");
        if (env == null) { EditorUtility.DisplayDialog("Compact Future Layout", "Couldn't find SkillsCentre_Environment in this scene.", "OK"); return; }
        if (!EditorUtility.DisplayDialog("Compact Future Layout", "Move NPCs, triggers, props and the camera rig to match the tightened rooms?\n\nRun this ONCE. Ctrl+Z undoes it.", "Move them", "Cancel")) return;

        Transform frame = env.transform;
        var log = new System.Text.StringBuilder();
        foreach (var root in scene.GetRootGameObjects())
        {
            if (Skip.Contains(root.name)) continue;
            Transform t = root.transform;
            Vector3 local = frame.InverseTransformPoint(t.position);
            Vector2 q = new Vector2(local.x, local.z);
            Vector2 delta;
            int roomIndex = -1;
            if (root.name.Contains("Camera Rig")) delta = new Vector2(0f, -3.8f);   // stays behind the entrance map
            else
            {
                float a = Mathf.Atan2(q.y, q.x) * Mathf.Rad2Deg;
                int best = -1; float bestDiff = 999f;
                for (int i = 0; i < Rooms.Length; i++)
                {
                    float ra = Mathf.Atan2(Rooms[i].y, Rooms[i].x) * Mathf.Rad2Deg;
                    float d = Mathf.Abs(Mathf.DeltaAngle(a, ra));
                    if (d < bestDiff) { bestDiff = d; best = i; }
                }
                bool inRoomSector = bestDiff < 24f && q.magnitude > 7f;
                delta = inRoomSector ? Rooms[best] * (S - 1f) : q * (S - 1f);
                if (inRoomSector) roomIndex = best;
            }
            Vector2 moved = q + delta;
            float turn = 0f;
            if (roomIndex >= 0) turn = (Rooms[roomIndex].x < 0f) ? Turn : -Turn;   // widen the entrance gap
            if (turn != 0f)
            {
                float r = turn * Mathf.Deg2Rad, c = Mathf.Cos(r), s = Mathf.Sin(r);
                moved = new Vector2(moved.x * c - moved.y * s, moved.x * s + moved.y * c);
            }
            Vector3 newLocal = new Vector3(moved.x, local.y, moved.y);
            Undo.RecordObject(t, "Compact Future Layout");
            t.position = frame.TransformPoint(newLocal);
            if (turn != 0f) t.rotation = Quaternion.AngleAxis(-turn, frame.up) * t.rotation;
            log.AppendLine(root.name + "  moved " + delta.magnitude.ToString("0.0") + " m");
        }
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("Compact Future Layout done:\n" + log);
    }
}
