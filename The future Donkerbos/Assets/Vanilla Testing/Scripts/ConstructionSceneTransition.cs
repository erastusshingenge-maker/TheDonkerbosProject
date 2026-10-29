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

    private bool transitionStarted = false;

    public void StartSceneTransition()
    {
        if (transitionStarted) return;

        transitionStarted = true;

        Debug.Log("Chair completed! Starting scene transition.");

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
