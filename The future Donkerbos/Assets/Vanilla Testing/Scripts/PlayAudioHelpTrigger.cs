
using UnityEngine;

public class PlayAudioHelpTrigger : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private GameObject playerRig;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private bool playOnlyOnce = true;

    private bool hasPlayed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (playOnlyOnce && hasPlayed) return;

        if (playerRig == null) return;

        // Check if the collider belongs to the player rig
        if (other.gameObject == playerRig ||
            other.transform.IsChildOf(playerRig.transform))
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
