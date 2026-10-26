// The children walk physically (hand tracking, no joystick), so synthetic locomotion
// and gravity are not needed. This switches on "Velocity Disabled" on every
// Interaction SDK FirstPersonLocomotor in the open scene, which stops the camera falling.
// Menu: Donkerbos > Stop Player Falling (Disable Locomotion Gravity). Supports Undo.
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DonkerbosNoFalling
{
    [MenuItem("Donkerbos/Stop Player Falling (Disable Locomotion Gravity)")]
    static void Apply()
    {
        int n = 0;
        foreach (var mb in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (mb == null || mb.GetType().Name != "FirstPersonLocomotor") continue;
            var so = new SerializedObject(mb);
            var p = so.FindProperty("_velocityDisabled");
            if (p == null) continue;
            Undo.RecordObject(mb, "Disable locomotion velocity");
            p.boolValue = true;
            so.ApplyModifiedProperties();
            PrefabUtility.RecordPrefabInstancePropertyModifications(mb);
            n++;
            Debug.Log("Velocity Disabled on " + mb.gameObject.name, mb.gameObject);
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.DisplayDialog("Stop Player Falling",
            n > 0 ? "Turned on Velocity Disabled on " + n + " locomotor(s).\nSave the scene (Cmd+S), then press Play."
                  : "No FirstPersonLocomotor found in this scene.", "OK");
    }
}
