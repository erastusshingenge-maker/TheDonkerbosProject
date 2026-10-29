using System.Collections;
using UnityEngine;

public class VRSceneFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 1.5f;

    private Material fadeMaterial;

    private void Awake()
    {
        Renderer sphereRenderer = GetComponent<Renderer>();

        fadeMaterial = sphereRenderer.material;

        SetFade(1f);
    }

    public IEnumerator FadeOut()
    {
        yield return Fade(0f, 1f);
    }

    public IEnumerator FadeIn()
    {
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(
                startAlpha,
                endAlpha,
                elapsed / fadeDuration
            );

            SetFade(alpha);

            yield return null;
        }

        SetFade(endAlpha);
    }

    private void SetFade(float alpha)
    {
        Color color = fadeMaterial.color;
        color.a = alpha;
        fadeMaterial.color = color;
    }
}