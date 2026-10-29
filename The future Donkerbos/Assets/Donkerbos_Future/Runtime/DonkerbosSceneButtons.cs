using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

// Two buttons (back < and next >) that appear in front of the player in every scene,
// just above head height. Touch one with an index fingertip to load the previous / next scene.
// Scene order = File > Build Profiles > Scene List.
//
// Nothing needs adding to any scene: this object creates itself when the app starts.
// It only READS fingertip positions from the hand objects the rig already has (it does not
// touch the hand tracking, interactors or any Meta/Interaction SDK setup).
// To remove the feature, delete this file.
public class DonkerbosSceneButtons : MonoBehaviour
{
    class Btn
    {
        public Transform root;
        public Renderer plate;
        public Color cold, hot;
        public int step;
        public bool wasInside;
    }

    static DonkerbosSceneButtons instance;

    static readonly Color Amber = new Color(0.77f, 0.63f, 0.32f);
    static readonly Color AmberHot = new Color(1f, 0.85f, 0.45f);
    static readonly Color Sage = new Color(0.30f, 0.48f, 0.35f);
    static readonly Color SageHot = new Color(0.50f, 0.78f, 0.56f);
    static Material baseMat;

    const float Reach = 0.5f;      // metres in front of the head
    const float Rise = 0.20f;      // metres ABOVE eye level (hovers just over the head, still easy to reach; lower = closer to eye level)
    const float Side = 0.12f;      // metres left / right of centre
    const float Half = 0.08f;      // half size of each button face

    readonly List<Btn> buttons = new List<Btn>();
    readonly List<Transform> tips = new List<Transform>();
    Coroutine spawning;
    float nextTipRefresh, lockUntil, farSince = -1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (instance != null) return;
        var go = new GameObject("DonkerbosSceneButtons");
        DontDestroyOnLoad(go);
        instance = go.AddComponent<DonkerbosSceneButtons>();
        instance.BeginSpawn();
    }

    void OnEnable() { SceneManager.sceneLoaded += OnLoaded; }
    void OnDisable() { SceneManager.sceneLoaded -= OnLoaded; }
    void OnLoaded(Scene s, LoadSceneMode m) { BeginSpawn(); }

    void BeginSpawn()
    {
        buttons.Clear();
        tips.Clear();
        nextTipRefresh = 0f;
        if (spawning != null) StopCoroutine(spawning);
        spawning = StartCoroutine(SpawnRoutine());
    }

    IEnumerator SpawnRoutine()
    {
        yield return null;
        if (SceneManager.sceneCountInBuildSettings < 2) yield break;   // nothing to switch to

        Camera cam = null;
        float waited = 0f;
        while (waited < 15f)
        {
            cam = FindCam();
            if (cam != null) break;
            waited += Time.unscaledDeltaTime;
            yield return null;
        }
        if (cam == null) yield break;

        yield return new WaitForSecondsRealtime(2f);   // let headset tracking settle
        Place();
    }

    static Camera FindCam()
    {
        Camera c = Camera.main;
        if (c != null && c.isActiveAndEnabled) return c;
        foreach (var x in Camera.allCameras)
            if (x.isActiveAndEnabled) return x;
        return null;
    }

    void Place()
    {
        var cam = FindCam();
        if (cam == null) return;

        foreach (var b in buttons)
            if (b.root != null) Destroy(b.root.gameObject);
        buttons.Clear();

        Vector3 f = cam.transform.forward; f.y = 0f;
        if (f.sqrMagnitude < 0.01f) f = Vector3.forward;
        f.Normalize();
        Vector3 right = Vector3.Cross(Vector3.up, f);

        Vector3 centre = cam.transform.position + f * Reach;
        centre.y = cam.transform.position.y + Rise;
        // Tilt the buttons so their faces turn down toward the player's eyes (local +x stays the player's right).
        Quaternion rot = Quaternion.LookRotation((centre - cam.transform.position).normalized, Vector3.up);

        buttons.Add(Build(centre - right * Side, rot, -1));
        buttons.Add(Build(centre + right * Side, rot, +1));
        lockUntil = Time.unscaledTime + 1f;
        farSince = -1f;
    }

    Btn Build(Vector3 pos, Quaternion rot, int step)
    {
        var root = new GameObject(step > 0 ? "SceneButton_Next" : "SceneButton_Back").transform;
        root.SetPositionAndRotation(pos, rot);

        // Next = amber square on the right. Previous = green square on the left.
        Color cold = step > 0 ? Amber : Sage;
        Color hot = step > 0 ? AmberHot : SageHot;
        var plate = Prim(root, Vector3.zero, Quaternion.identity,
                         new Vector3(Half * 2f, Half * 2f, 0.03f), cold);

        return new Btn { root = root, plate = plate, step = step, cold = cold, hot = hot };
    }

    static Material MakeMat(Color c)
    {
        if (baseMat == null)
        {
            var rp = GraphicsSettings.currentRenderPipeline;
            if (rp != null) baseMat = rp.defaultMaterial;   // the project's own URP material, so it is never pink
        }
        Material m = baseMat != null ? new Material(baseMat) : new Material(Shader.Find("Sprites/Default"));
        Tint(m, c);
        return m;
    }

    static void Tint(Material m, Color c)
    {
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (m.HasProperty("_Color")) m.SetColor("_Color", c);
    }

    static Renderer Prim(Transform parent, Vector3 localPos, Quaternion localRot, Vector3 scale, Color c)
    {
        var g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var col = g.GetComponent<Collider>();
        if (col != null) Destroy(col);           // visual only
        g.transform.SetParent(parent, false);
        g.transform.localPosition = localPos;
        g.transform.localRotation = localRot;
        g.transform.localScale = scale;
        var r = g.GetComponent<Renderer>();
        r.sharedMaterial = MakeMat(c);
        return r;
    }

    void Update()
    {
        if (buttons.Count == 0) return;

        if (Time.unscaledTime >= nextTipRefresh)
        {
            nextTipRefresh = Time.unscaledTime + 1f;
            tips.Clear();
            foreach (var t in FindObjectsByType<Transform>(FindObjectsSortMode.None))
            {
                if (!t.gameObject.activeInHierarchy) continue;
                string n = t.name.ToLowerInvariant();
                if (n.Contains("indextip") || n.Contains("index_tip")) tips.Add(t);
            }
        }

        foreach (var b in buttons)
        {
            if (b.root == null) continue;
            bool inside = false;
            foreach (var tip in tips)
            {
                if (tip == null) continue;
                Vector3 p = b.root.InverseTransformPoint(tip.position);
                if (Mathf.Abs(p.x) < Half + 0.02f && Mathf.Abs(p.y) < Half + 0.02f && p.z > -0.07f && p.z < 0.04f)
                { inside = true; break; }
            }

            Tint(b.plate.material, inside ? b.hot : b.cold);

            if (inside && !b.wasInside && Time.unscaledTime >= lockUntil)
            {
                b.wasInside = true;
                Go(b.step);
                return;
            }
            b.wasInside = inside;
        }

        // If the player walks away, bring the buttons back in front of them.
        var cam = FindCam();
        if (cam != null && buttons[0].root != null && buttons[1].root != null)
        {
            Vector3 mid = (buttons[0].root.position + buttons[1].root.position) * 0.5f;
            Vector3 d = cam.transform.position - mid; d.y = 0f;
            if (d.magnitude > 1.1f)
            {
                if (farSince < 0f) farSince = Time.unscaledTime;
                else if (Time.unscaledTime - farSince > 2f) Place();
            }
            else farSince = -1f;
        }
    }

    void Go(int step)
    {
        int count = SceneManager.sceneCountInBuildSettings;
        if (count < 2) return;
        int next = (SceneManager.GetActiveScene().buildIndex + step + count) % count;
        lockUntil = Time.unscaledTime + 1.5f;
        SceneManager.LoadScene(next);
    }
}
