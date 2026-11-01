using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;
    [SerializeField] private bool endExperience = false;

    [Header("Player Settings")]
    [SerializeField] private string playerTag = "Player";

    [Header("Transition Settings")]
    [SerializeField] private float delayBeforeFade = 2f;
    [SerializeField] private VRSceneFade sceneFade;

    [Header("Choice Tracking")]
    [SerializeField] private int sceneNumber = 1;
    [SerializeField] private string playerChoice = "Find";

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;

        if (!other.CompareTag(playerTag))
            return;

        isTransitioning = true;

        Debug.Log("Player entered trigger!");

        // Record the player's choice
        if (DonkerbosChoiceTracker.Instance != null)
        {
            DonkerbosChoiceTracker.Instance.RecordChoice(
                sceneNumber,
                playerChoice
            );
        }
        else
        {
            Debug.LogWarning("DonkerbosChoiceTracker not found!");
        }

        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        // Wait before fading
        yield return new WaitForSeconds(delayBeforeFade);

        // Fade to black
        if (sceneFade != null)
        {
            yield return StartCoroutine(sceneFade.FadeOut());
        }

        // End the experience if selected
        if (endExperience)
        {
            Debug.Log("Donkerbos experience completed!");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            // Load the next scene normally
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}