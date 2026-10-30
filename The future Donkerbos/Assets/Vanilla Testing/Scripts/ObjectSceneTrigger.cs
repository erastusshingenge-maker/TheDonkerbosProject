using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSceneTrigger : MonoBehaviour
{
    [Header("Object Settings")]
    [SerializeField] private string requiredTag;
    [SerializeField] private bool disableObjectOnTrigger = true;

    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    [Header("Transition Settings")]
    [SerializeField] private float delayBeforeFade = 2f;
    [SerializeField] private VRSceneFade sceneFadeSphere;

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return;

        GameObject incomingObject =
            other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (incomingObject.CompareTag(requiredTag))
        {
            hasBeenTriggered = true;

            Debug.Log("Object received: " + requiredTag);

            // Disable the object if enabled
            if (disableObjectOnTrigger)
            {
                incomingObject.SetActive(false);
            }

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