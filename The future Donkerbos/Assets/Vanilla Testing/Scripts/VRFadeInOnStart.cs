using System.Collections;
using UnityEngine;

public class VRFadeInOnStart : MonoBehaviour
{
    [SerializeField] private VRSceneFade sceneFadeSphere;

    private IEnumerator Start()
    {
        if (sceneFadeSphere != null)
        {
            yield return StartCoroutine(sceneFadeSphere.FadeIn());
        }
    }
}