using UnityEngine;

// Soft, slowly pulsing glow on a flat disk. Hides itself once the watched object
// (e.g. the key) has been picked up and moved away, or switched off.
[RequireComponent(typeof(Renderer))]
public class GlowMarker : MonoBehaviour
{
    [Header("Look")]
    [SerializeField] private Color glowColor = new Color(1f, 0.78f, 0.35f, 1f);
    [Range(0f, 1f)] [SerializeField] private float minAlpha = 0.10f;
    [Range(0f, 1f)] [SerializeField] private float maxAlpha = 0.30f;
    [SerializeField] private float pulseSeconds = 2.6f;

    [Header("Hide when this is picked up")]
    [SerializeField] private Transform watchTarget;
    [SerializeField] private float hideAfterMovedMetres = 0.15f;
    [SerializeField] private float fadeOutSeconds = 0.6f;

    private Renderer rend;
    private MaterialPropertyBlock block;
    private Vector3 targetStart;
    private float fade = 1f;
    private bool hiding;
    private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        if (watchTarget != null) targetStart = watchTarget.position;
    }

    private void Update()
    {
        if (!hiding && watchTarget != null &&
            (!watchTarget.gameObject.activeInHierarchy ||
             (watchTarget.position - targetStart).sqrMagnitude > hideAfterMovedMetres * hideAfterMovedMetres))
            hiding = true;

        if (hiding)
        {
            fade -= Time.deltaTime / Mathf.Max(0.01f, fadeOutSeconds);
            if (fade <= 0f) { gameObject.SetActive(false); return; }
        }

        float t = (Mathf.Sin(Time.time * Mathf.PI * 2f / Mathf.Max(0.1f, pulseSeconds)) + 1f) * 0.5f;
        Color c = glowColor;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t) * fade;
        rend.GetPropertyBlock(block);
        block.SetColor(BaseColor, c);
        rend.SetPropertyBlock(block);
    }
}
