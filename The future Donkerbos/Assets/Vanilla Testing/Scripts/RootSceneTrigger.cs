using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RootSceneTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    [Header("Transition Settings")]
    [SerializeField] private float delayBeforeFade = 2f;
    [SerializeField] private VRSceneFade sceneFadeSphere;

    private bool hasReceivedRoot = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasReceivedRoot) return;

        GameObject incomingObject =
            other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (incomingObject.CompareTag("Root"))
        {
            hasReceivedRoot = true;

            Debug.Log("The man received the root!");

            StartCoroutine(ChangeScene());
        }
    }

    private IEnumerator ChangeScene()
    {
        // Wait before fading
        yield return new WaitForSeconds(delayBeforeFade);

        // Fade to black
        if (sceneFadeSphere != null)
        {
            yield return StartCoroutine(sceneFadeSphere.FadeOut());
        }

        // Load next scene
        SceneManager.LoadScene(sceneToLoad);
    }
}