using UnityEngine;

public class PlayAudioHelpTrigger : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool playOnlyOnce = true;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (playOnlyOnce && hasPlayed) return;

        // Only trigger when the player enters
        if (other.CompareTag("Player") ||
            other.transform.root.CompareTag("Player"))
        {
            if (audioSource != null)
            {
                audioSource.Play();
                hasPlayed = true;

                Debug.Log("Player triggered audio!");
            }
        }
    }
}