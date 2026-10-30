using UnityEngine;
using UnityEngine.Playables;

public class TimelineTrigger : MonoBehaviour
{
    [Header("Timeline Settings")]
    [SerializeField] private PlayableDirector timeline;

    [Header("Trigger Settings")]
    [SerializeField] private string playerTag = "Player";

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag(playerTag) ||
            other.transform.root.CompareTag(playerTag))
        {
            hasTriggered = true;

            Debug.Log("Player entered trigger! Playing Timeline.");

            timeline.Play();
        }
    }
}