
using System.Collections;
using UnityEngine;

public class DonkerbosFinalDoor : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] private string requiredTag = "Key";
    [SerializeField] private bool disableKeyOnTrigger = true;

    [Header("Doors")]
    [SerializeField] private GameObject[] doorsToDisable;

    [Header("Choice Tracking")]
    [SerializeField] private string playerChoice = "Build";

    [Header("Final Audio")]
    [SerializeField] private AudioSource finalAudio;

    [Header("Ending Settings")]
    [SerializeField] private float delayBeforeFade = 2f;
    [SerializeField] private VRSceneFade sceneFade;

    private bool hasBeenTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasBeenTriggered) return;

        GameObject incomingObject =
            other.attachedRigidbody != null
            ? other.attachedRigidbody.gameObject
            : other.gameObject;

        if (!incomingObject.CompareTag(requiredTag))
            return;

        hasBeenTriggered = true;

        Debug.Log("Key inserted into door!");

        // Play final audio
        if (finalAudio != null)
        {
            finalAudio.loop = false;
            finalAudio.Play();
        }

        // Disable the doors
        foreach (GameObject door in doorsToDisable)
        {
            if (door != null)
            {
                door.SetActive(false);
            }
        }

        // Make the key disappear
        if (disableKeyOnTrigger)
        {
            incomingObject.SetActive(false);
        }

        // Record the final choice (Scene 3)
        if (DonkerbosChoiceTracker.Instance != null)
        {
            DonkerbosChoiceTracker.Instance.RecordChoice(
                3,
                playerChoice
            );
        }
        else
        {
            Debug.LogWarning(
                "DonkerbosChoiceTracker not found!"
            );
        }

        StartCoroutine(EndExperience());
    }

    private IEnumerator EndExperience()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        if (sceneFade != null)
        {
            yield return StartCoroutine(sceneFade.FadeOut());
        }

        Debug.Log("Donkerbos experience completed!");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
