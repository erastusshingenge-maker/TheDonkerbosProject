// Places the first-aid kit (Dorm Assets/FirstAidKit/FirstAidKit.fbx) into the open scene.
// Menu: Donkerbos > Place First Aid Kit. Supports Undo (Cmd+Z).
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DonkerbosFirstAid
{
    const string FbxPath = "Assets/Dorm Assets/FirstAidKit/FirstAidKit.fbx";
    static readonly string[] Grabbable = { "Plaster", "Thermometer", "Scissors", "PillBottle" };

    static Type FindType(string name) =>
        AppDomain.CurrentDomain.GetAssemblies()
            .Select(a => a.GetTypes().FirstOrDefault(t => t.Name == name))
            .FirstOrDefault(t => t != null);

    [MenuItem("Donkerbos/Place First Aid Kit")]
    static void Place()
    {
        var model = AssetDatabase.LoadAssetAtPath<GameObject>(FbxPath);
        if (model == null) { EditorUtility.DisplayDialog("First Aid Kit", "Could not find " + FbxPath, "OK"); return; }

        var scene = EditorSceneManager.GetActiveScene();
        var kit = (GameObject)PrefabUtility.InstantiatePrefab(model, scene);
        kit.name = "FirstAidKit_Healer";
        Undo.RegisterCreatedObjectUndo(kit, "Place First Aid Kit");
        kit.transform.localScale = Vector3.one;

        // Put it 1 m in front of the XR Origin, on whatever surface is below.
        var xr = scene.GetRootGameObjects().FirstOrDefault(g =>
            g.name.Contains("XR Origin") || g.GetComponents<Component>().Any(c => c != null && c.GetType().Name == "XROrigin"));
        Vector3 pos = Vector3.zero;
        if (xr != null)
        {
            var fwd = xr.transform.forward; fwd.y = 0; fwd = fwd.sqrMagnitude > 0 ? fwd.normalized : Vector3.forward;
            pos = xr.transform.position + fwd;
            kit.transform.rotation = Quaternion.LookRotation(-fwd, Vector3.up);
        }
        Physics.SyncTransforms();
        if (Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out var hit, 10f)) pos.y = hit.point.y;
        kit.transform.position = pos;

        var grabType = FindType("XRGrabInteractable");
        foreach (Transform child in kit.GetComponentsInChildren<Transform>())
        {
            if (child == kit.transform) continue;
            if (child.GetComponent<MeshFilter>() == null) continue;
            if (child.GetComponent<Collider>() == null) Undo.AddComponent<BoxCollider>(child.gameObject);
            if (Grabbable.Contains(child.name) && grabType != null && child.GetComponent(grabType) == null)
            {
                var rb = child.GetComponent<Rigidbody>() ?? Undo.AddComponent<Rigidbody>(child.gameObject);
                rb.useGravity = false;
                rb.isKinematic = true;
                Undo.AddComponent(child.gameObject, grabType);
            }
        }
        Selection.activeGameObject = kit;
        EditorSceneManager.MarkSceneDirty(scene);
        Debug.Log("First aid kit placed. Plaster is the correct item. " + (grabType == null ? "XRGrabInteractable not found: items are not grabbable yet." : "Items are grabbable."));
    }
}
