using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string sceneToLoad;

    [Header("Player Settings")]
    [SerializeField] private string playerTag = "Player";

    private bool isTransitioning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isTransitioning) return;

        // Check if the player touched the trigger
        if (!other.CompareTag(playerTag))
            return;

        isTransitioning = true;

        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);
    }
}