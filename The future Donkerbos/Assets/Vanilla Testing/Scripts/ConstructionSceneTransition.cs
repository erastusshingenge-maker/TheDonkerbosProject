using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConstructionSceneTransition : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    [Header("Transition Settings")]
    [SerializeField] private float delayBeforeFade = 2f;
    [SerializeField] private VRSceneFade sceneFade;

    [Header("Choice Tracking")]
    [SerializeField] private int sceneNumber = 1;
    [SerializeField] private string playerChoice = "Build";

    private bool transitionStarted = false;

    public void StartSceneTransition()
    {
        if (transitionStarted) return;

        transitionStarted = true;

        Debug.Log("Construction completed! Starting scene transition.");

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
        // Wait before transitioning
        yield return new WaitForSeconds(delayBeforeFade);

        // Fade to black
        if (sceneFade != null)
        {
            yield return StartCoroutine(sceneFade.FadeOut());
        }

        // Load the next scene
        SceneManager.LoadScene(sceneToLoad);
    }
}