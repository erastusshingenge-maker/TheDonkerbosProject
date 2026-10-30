using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectSceneTrigger : MonoBehaviour
{
    [Header("Object Settings")]
    [SerializeField] private string requiredTag;
    [SerializeField] private bool disableObjectOnTrigger = true;

    [Header("Object to Enable (Optional)")]
    [SerializeField] private GameObject objectToEnable;

    [Header("Animation Settings")]
    [SerializeField] private Animator characterAnimator;
    [SerializeField] private string animationTriggerName;

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

            // Disable the incoming object
            if (disableObjectOnTrigger)
            {
                incomingObject.SetActive(false);
            }

            // Enable another object if assigned
            if (objectToEnable != null)
            {
                objectToEnable.SetActive(true);
            }

            // Trigger character animation
            if (characterAnimator != null &&
                !string.IsNullOrEmpty(animationTriggerName))
            {
                characterAnimator.SetTrigger(animationTriggerName);
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