using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    [Header("Player Settings")]
    [SerializeField] private string playerTag = "Player";

    [Header("Transition Settings")]
    [SerializeField] private float delayBeforeFade = 2f;
    [SerializeField] private VRSceneFade sceneFade;

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;

        // Check if the player touched the trigger
        if (!other.CompareTag(playerTag))
            return;

        isTransitioning = true;

        Debug.Log("Player touched the door!");

        StartCoroutine(ChangeScene());
    }

    private IEnumerator ChangeScene()
    {
        // Wait 2 seconds
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